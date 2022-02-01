FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Backgammon.Services.Chat.Api/Backgammon.Services.Chat.Api.csproj", "Backgammon.Services.Chat.Api/"]
COPY ["Backgammon.Services.Chat.Application/Backgammon.Services.Chat.Application.csproj", "Backgammon.Services.Chat.Application/"]
COPY ["Backgammon.Services.Chat.Domain/Backgammon.Services.Chat.Domain.csproj", "Backgammon.Services.Chat.Domain/"]
COPY ["Backgammon.Services.Chat.Infra.Data/Backgammon.Services.Chat.Infra.Data.csproj", "Backgammon.Services.Chat.Infra.Data/"]
COPY ["Backgammon.Services.Chat.Infra.Ioc/Backgammon.Services.Chat.Infra.Ioc.csproj", "Backgammon.Services.Chat.Infra.Ioc/"]
COPY ["Backgammon.Services.Chat.Infra.Secrets/Backgammon.Services.Chat.Infra.Secrets.csproj", "Backgammon.Services.Chat.Infra.Secrets/"]
RUN dotnet restore "Backgammon.Services.Chat.Api/Backgammon.Services.Chat.Api.csproj"
COPY . .
WORKDIR "/src/Backgammon.Services.Chat.Api"
RUN dotnet build "Backgammon.Services.Chat.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Backgammon.Services.Chat.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir /data
ENTRYPOINT ["dotnet", "Backgammon.Services.Chat.Api.dll"]