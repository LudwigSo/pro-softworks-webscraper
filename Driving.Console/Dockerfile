FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
USER root
ARG configuration=Release
WORKDIR /src
COPY . .
WORKDIR "/src/Driving.Console"
RUN dotnet build "Driving.Console.csproj" -c $configuration -o /app/build
RUN dotnet publish "Driving.Console.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Driving.Console.dll"]
