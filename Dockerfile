FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["PushNotifications.sln", "./"]
COPY ["Notifications/Notification.csproj", "Notifications/"]
COPY ["Notifications.Services.Abstractions/Notifications.Services.Abstractions.csproj", "Notifications.Services.Abstractions/"]
COPY ["Notifications.Services.PushService/Notifications.Services.PushService.csproj", "Notifications.Services.PushService/"]
COPY ["Notifications.Services.Sqlite/Notifications.Services.Sqlite.csproj", "Notifications.Services.Sqlite/"]
COPY ["Notifications.Services/PushNotifications.Services.csproj", "Notifications.Services/"]
RUN dotnet restore "Notifications/Notification.csproj"
COPY . .
WORKDIR "/src/Notifications"
RUN dotnet build "Notification.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notification.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification.dll"]