docker compose up --build

# Configuration

In best case there will be a way to deploy the services preconfigured, nevertheless I want to document the manual steps I currently take to configure those services:

1. Keycloak
- login to admin panel with default credentials (docker-compose.yml)
- create new realm "pro-softworks"
- create realm-roles "admin" and "employee"
- create users
- create client for web api
  -> Client ID = public-client
  -> Valid redirect URIs = http://localhost:8080/*
  -> Web origins = http://localhost:8080
  -> Implicit Flow = true