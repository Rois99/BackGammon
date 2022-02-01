FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Backgammon.Services.Identity.Api/Backgammon.Services.Identity.Api.csproj", "Backgammon.Services.Identity.Api/"]
COPY ["Backgammon.Services.Identity.Application/Backgammon.Services.Identity.Application.csproj", "Backgammon.Services.Identity.Application/"]
COPY ["Backgammon.Services.Identity.Domain/Backgammon.Services.Identity.Domain.csproj", "Backgammon.Services.Identity.Domain/"]
COPY ["Backgammon.Services.Identity.Infra.Data/Backgammon.Services.Identity.Infra.Data.csproj", "Backgammon.Services.Identity.Infra.Data/"]
COPY ["Backgammon.Services.Identity.Infra.Ioc/Backgammon.Services.Identity.Infra.Ioc.csproj", "Backgammon.Services.Identity.Infra.Ioc/"]
COPY ["Backgammon.Services.Identity.Infra.Secrets/Backgammon.Services.Identity.Infra.Secrets.csproj", "Backgammon.Services.Identity.Infra.Secrets/"]
RUN dotnet restore "Backgammon.Services.Identity.Api/Backgammon.Services.Identity.Api.csproj"
COPY . .
WORKDIR "/src/Backgammon.Services.Identity.Api"
RUN dotnet build "Backgammon.Services.Identity.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Backgammon.Services.Identity.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir /data
ENTRYPOINT ["dotnet", "Backgammon.Services.Identity.Api.dll"]