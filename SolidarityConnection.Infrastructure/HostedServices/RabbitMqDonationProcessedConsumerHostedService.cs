using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SolidarityConnection.Infrastructure.Adapters.Events.Consumers;
using SolidarityConnection.Infrastructure.Messaging.Events;
using SolidarityConnection.Infrastructure.Options;
using System.Text;
using System.Text.Json;

namespace SolidarityConnection.Infrastructure.HostedServices;

public sealed class RabbitMqDonationProcessedConsumerHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RabbitMqDonationProcessedConsumerHostedService> _logger;
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private IConnection? _connection;
    private IModel? _channel;
    private AsyncEventingBasicConsumer? _consumer;
    private string? _consumerTag;

    public RabbitMqDonationProcessedConsumerHostedService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<RabbitMqDonationProcessedConsumerHostedService> logger,
        IOptions<RabbitMqOptions> rabbitMqOptions)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _rabbitMqOptions = rabbitMqOptions.Value;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        InitializeRabbitMq();
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ArgumentNullException.ThrowIfNull(_channel);

        _channel.QueueDeclare(
            queue: _rabbitMqOptions.DonationProcessedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        _consumer = new AsyncEventingBasicConsumer(_channel);
        _consumer.Received += OnDonationProcessedAsync;
        _consumerTag = _channel.BasicConsume(
            queue: _rabbitMqOptions.DonationProcessedQueue,
            autoAck: false,
            consumer: _consumer);

        _logger.LogInformation(
            "Worker escutando a fila {QueueName}.",
            _rabbitMqOptions.DonationProcessedQueue);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Encerrando consumidor de status de doações.");
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null && _channel.IsOpen && !string.IsNullOrWhiteSpace(_consumerTag))
        {
            _channel.BasicCancel(_consumerTag);
        }

        return base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }

    private async Task OnDonationProcessedAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(_channel);

        try
        {
            var payload = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var donationEvent = JsonSerializer.Deserialize<DonationProcessedEvent>(
                payload,
                _jsonSerializerOptions);

            if (donationEvent is null)
            {
                _logger.LogWarning(
                    "Mensagem recebida na fila {QueueName} não pode ser desserializada.",
                    _rabbitMqOptions.DonationProcessedQueue);

                _channel.BasicReject(eventArgs.DeliveryTag, requeue: false);
                return;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<DonationProcessedConsumer>();

            await consumer.ConsumeAsync(donationEvent, CancellationToken.None);

            _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (JsonException ex)
        {
            _logger.LogError(
                ex,
                "Mensagem inválida recebida na fila {QueueName}. A mensagem será descartada.",
                _rabbitMqOptions.DonationProcessedQueue);

            _channel.BasicReject(eventArgs.DeliveryTag, requeue: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao processar mensagem da fila {QueueName}. A mensagem será reenfileirada.",
                _rabbitMqOptions.DonationProcessedQueue);

            _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
        }
    }

    private void InitializeRabbitMq()
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.Host,
            Port = _rabbitMqOptions.Port,
            UserName = _rabbitMqOptions.Username,
            Password = _rabbitMqOptions.Password,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }
}
