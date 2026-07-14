using System.Net.Http.Headers;
using Microsoft.JSInterop;
using SolidarityConnection.Frontend.Models.Auth;

namespace SolidarityConnection.Frontend.Services;

public sealed class AppSessionService(IJSRuntime jsRuntime, HttpClient httpClient)
{
    private const string StorageKey = "solidarity-connection.session";
    private Task? _initializeTask;

    public event Action? Changed;

    public UserSession CurrentUser { get; private set; } = UserSession.Guest;

    public bool IsAuthenticated => CurrentUser.IsAuthenticated;

    public bool IsAdmin => CurrentUser.IsAdmin;

    public Task EnsureInitializedAsync()
    {
        _initializeTask ??= InitializeCoreAsync();
        return _initializeTask;
    }

    public async Task SignInAsync(LoginResponse response)
    {
        var session = new UserSession
        {
            Token = response.Token,
            Name = response.Name,
            Role = response.Role
        };

        CurrentUser = session;
        ApplyAuthorizationHeader(session.Token);
        await jsRuntime.InvokeVoidAsync("solidaritySession.set", StorageKey, session);
        Changed?.Invoke();
    }

    public async Task SignOutAsync()
    {
        CurrentUser = UserSession.Guest;
        ApplyAuthorizationHeader(null);
        await jsRuntime.InvokeVoidAsync("solidaritySession.clear", StorageKey);
        Changed?.Invoke();
    }

    private async Task InitializeCoreAsync()
    {
        try
        {
            var session = await jsRuntime.InvokeAsync<UserSession?>("solidaritySession.get", StorageKey);
            CurrentUser = session ?? UserSession.Guest;
            ApplyAuthorizationHeader(CurrentUser.Token);
        }
        catch
        {
            CurrentUser = UserSession.Guest;
            ApplyAuthorizationHeader(null);
        }
        finally
        {
            Changed?.Invoke();
        }
    }

    private void ApplyAuthorizationHeader(string? token)
    {
        httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }
}