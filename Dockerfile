Etapa base (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

Etapa build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

Copia o arquivo de projeto
COPY ProjetoTCC/*.csproj ./ProjetoTCC/
RUN dotnet restore ./ProjetoTCC/ProjetoTCC.csproj

Copia o restante do projeto
COPY ProjetoTCC/ ./ProjetoTCC/
WORKDIR /src/ProjetoTCC

Publica a aplicação em modo Release
RUN dotnet publish -c Release -o /app/publish

Etapa final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProjetoTCC.dll"]