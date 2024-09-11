FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
USER root
ARG configuration=Release
WORKDIR /src
COPY . .
WORKDIR "/src/Driving.Service"
RUN dotnet build "Driving.Service.csproj" -c $configuration -o /app/build
RUN dotnet publish "Driving.Service.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Driving.Service.dll"]
