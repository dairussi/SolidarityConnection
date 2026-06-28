FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["SolidarityConnection.Presentation/SolidarityConnection.Presentation.csproj", "SolidarityConnection.Presentation/"]
COPY ["SolidarityConnection.Application/SolidarityConnection.Application.csproj", "SolidarityConnection.Application/"]
COPY ["SolidarityConnection.Domain/SolidarityConnection.Domain.csproj", "SolidarityConnection.Domain/"]
COPY ["SolidarityConnection.Infrastructure/SolidarityConnection.Infrastructure.csproj", "SolidarityConnection.Infrastructure/"]
RUN dotnet restore "SolidarityConnection.Presentation/SolidarityConnection.Presentation.csproj"

COPY . .
RUN dotnet build "SolidarityConnection.Presentation/SolidarityConnection.Presentation.csproj" -c $BUILD_CONFIGURATION --no-restore
RUN dotnet publish "SolidarityConnection.Presentation/SolidarityConnection.Presentation.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-build \
    /p:UseAppHost=false

RUN dotnet tool install --global dotnet-ef --version 8.0.8
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet ef migrations bundle \
    --project SolidarityConnection.Infrastructure/SolidarityConnection.Infrastructure.csproj \
    --startup-project SolidarityConnection.Presentation/SolidarityConnection.Presentation.csproj \
    --configuration Release \
    --no-build \
    --self-contained \
    -o /app/efbundle

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SolidarityConnection.Presentation.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS migrations
WORKDIR /app
COPY --from=build /app/efbundle .
COPY --from=build /app/publish .
RUN chmod +x ./efbundle
ENTRYPOINT ["./efbundle"]