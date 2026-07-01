FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["UserAPI.API/UserAPI.API.csproj", "UserAPI.API/"]
COPY ["UserAPI.Application/UserAPI.Application.csproj", "UserAPI.Application/"]
COPY ["UserAPI.Domain/UserAPI.Domain.csproj", "UserAPI.Domain/"]
COPY ["UserAPI.Infrastructure/UserAPI.Infrastructure.csproj", "UserAPI.Infrastructure/"]
RUN dotnet restore "UserAPI.API/UserAPI.API.csproj"
COPY . .
WORKDIR "/src/UserAPI.API"
RUN dotnet build "UserAPI.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserAPI.API.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserAPI.API.dll"]
