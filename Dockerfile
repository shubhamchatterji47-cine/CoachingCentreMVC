# ═══════════════════════════════════════
# Stage 1 — BUILD
# ═══════════════════════════════════════
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CoachingMVC.csproj ./
RUN dotnet restore "CoachingMVC.csproj"

COPY . .
RUN dotnet publish "CoachingMVC.csproj" \
    -c Release \
    -o /app/publish

# ═══════════════════════════════════════
# Stage 2 — RUNTIME
# ═══════════════════════════════════════
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 10000
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "CoachingMVC.dll"]
