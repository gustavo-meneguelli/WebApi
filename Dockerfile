FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

COPY src/Api/Api.csproj src/Api/
COPY src/Application/Application.csproj src/Application/
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/

RUN dotnet restore src/Api/Api.csproj

COPY src/ src/

WORKDIR /app/src/Api
RUN dotnet publish -c Release -o /app/publish

# -------------------------------------------------------


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p Data

EXPOSE 8080

ENTRYPOINT ["dotnet", "Api.dll"]