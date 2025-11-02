FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5223


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["8BallPool.WebAPI/8BallPool.WebAPI.csproj", "8BallPool.WebAPI/"]
COPY ["8BallPool.Business/8BallPool.Business.csproj", "8BallPool.Business/"]
COPY ["8BallPool.Data/8BallPool.Data.csproj", "8BallPool.Data/"]
RUN dotnet restore "8BallPool.WebAPI/8BallPool.WebAPI.csproj"
COPY . .
WORKDIR "/src/8BallPool.WebAPI"
RUN dotnet build "8BallPool.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "8BallPool.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "8BallPool.WebAPI.dll"]