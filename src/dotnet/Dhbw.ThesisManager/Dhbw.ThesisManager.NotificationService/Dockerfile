FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Dhbw.ThesisManager/Dhbw.ThesisManager.NotificationService/Dhbw.ThesisManager.NotificationService.csproj", "Dhbw.ThesisManager.NotificationService/"]
COPY ["Dhbw.ThesisManager/Dhbw.ThesisManager.Domain/Dhbw.ThesisManager.Domain.csproj", "Dhbw.ThesisManager.Domain/"]
RUN dotnet restore "Dhbw.ThesisManager.NotificationService/Dhbw.ThesisManager.NotificationService.csproj"
COPY . .
RUN dotnet build "Dhbw.ThesisManager.NotificationService/Dhbw.ThesisManager.NotificationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dhbw.ThesisManager.NotificationService/Dhbw.ThesisManager.NotificationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dhbw.ThesisManager.NotificationService.dll"]
