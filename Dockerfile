# ── Stage 1: Build ───────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["CoachingCentre/CoachingMVC/CoachingMVC.csproj", "CoachingCentre/CoachingMVC/"]

# Restore dependencies
RUN dotnet restore "CoachingCentre/CoachingMVC/CoachingMVC.csproj"

# Copy all source code
COPY . .

# Build and publish
WORKDIR "/src/CoachingCentre/CoachingMVC"
RUN dotnet publish "CoachingMVC.csproj" -c Release -o /app/publish

# ── Stage 2: Runtime ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Render provides PORT env variable
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE 10000

ENTRYPOINT ["dotnet", "CoachingMVC.dll"]
