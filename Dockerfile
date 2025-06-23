# Etapa base (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

# Define a porta que será usada (Render define via PORT)
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE 80

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copia e restaura as dependências
COPY ProjetoTCC/*.csproj ./ProjetoTCC/
RUN dotnet restore ./ProjetoTCC/ProjetoTCC.csproj

# Copia todo o projeto e compila em Release
COPY ProjetoTCC/ ./ProjetoTCC/
WORKDIR /src/ProjetoTCC
RUN dotnet publish -c Release -o /app/publish

# Etapa final (runtime)
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Inicia a aplicação
ENTRYPOINT ["dotnet", "ProjetoTCC.dll"]
