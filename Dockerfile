# ═══════════════════════════════════════════════════════════════════════════════
# DARKLYN TECH STORE API - Dockerfile
# .NET 10 | Multi-stage Build | Otimizado para Produção
# ═══════════════════════════════════════════════════════════════════════════════

# ───────────────────────────────────────────────────────────────────────────────
# STAGE 1: BUILD
# ───────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia apenas os arquivos .csproj primeiro (otimiza cache de restore)
COPY ["src/Api/Api.csproj", "src/Api/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]

# Restore de dependências (cached se .csproj não mudar)
RUN dotnet restore "src/Api/Api.csproj"

# Copia todo o código fonte
COPY . .

# Build em Release
WORKDIR "/src/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build --no-restore

# ───────────────────────────────────────────────────────────────────────────────
# STAGE 2: PUBLISH
# ───────────────────────────────────────────────────────────────────────────────
FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish --no-restore \
    /p:UseAppHost=false \
    /p:PublishTrimmed=false

# ───────────────────────────────────────────────────────────────────────────────
# STAGE 3: RUNTIME (Imagem Final)
# ───────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Configurações de segurança
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Expõe porta HTTP
EXPOSE 8080

# Copia arquivos publicados
COPY --from=publish /app/publish .


# Roda como usuário não-root
USER app

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entrypoint
ENTRYPOINT ["dotnet", "Api.dll"]