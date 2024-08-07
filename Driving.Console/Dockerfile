FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base

WORKDIR /app

USER root
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Driving.Console/Driving.Console.csproj", "Driving.Console/"]
RUN dotnet restore "Driving.Console/Driving.Console.csproj"
COPY . .
WORKDIR "/src/Driving.Console"
RUN dotnet build "Driving.Console.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Driving.Console.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Driving.Console.dll"]
