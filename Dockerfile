# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["NetworkTopologyVisitingCard.csproj", "./"]
RUN dotnet restore "NetworkTopologyVisitingCard.csproj"

# Copy source code and build
COPY . .
RUN dotnet build "NetworkTopologyVisitingCard.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "NetworkTopologyVisitingCard.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install Entity Framework Core tools
RUN apt-get update && \
    apt-get install -y postgresql-client && \
    rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80

ENTRYPOINT ["dotnet", "NetworkTopologyVisitingCard.dll"]
