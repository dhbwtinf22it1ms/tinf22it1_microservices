FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Dhbw.ThesisManager/Setup/Setup.csproj", "Setup/"]
COPY ["Dhbw.ThesisManager/Dhbw.ThesisManager.Api/Dhbw.ThesisManager.Api.csproj", "Dhbw.ThesisManager.Api/"]
RUN dotnet restore "Setup/Setup.csproj"
COPY . .
RUN dotnet build "Setup/Setup.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Setup/Setup.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Setup.dll"]
