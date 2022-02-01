FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Backgammon.Services.Game.Api/Backgammon.Services.Game.Api.csproj", "Backgammon.Services.Game.Api/"]
COPY ["Backgammon.Services.Game.App/Backgammon.Services.Game.App.csproj", "Backgammon.Services.Game.App/"]
COPY ["Backgammon.Services.Game.Domain/Backgammon.Services.Game.Domain.csproj", "Backgammon.Services.Game.Domain/"]
COPY ["Backgammon.Services.Game.Infra/Backgammon.Services.Game.Infra.csproj", "Backgammon.Services.Game.Infra/"]
RUN dotnet restore "Backgammon.Services.Game.Api/Backgammon.Services.Game.Api.csproj"
COPY . .
WORKDIR "/src/Backgammon.Services.Game.Api"
RUN dotnet build "Backgammon.Services.Game.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Backgammon.Services.Game.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir /data
ENTRYPOINT ["dotnet", "Backgammon.Services.Game.Api.dll"]