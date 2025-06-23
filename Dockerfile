# Etapa base (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE 80

# Etapa build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copia apenas o .csproj e restaura
COPY *.csproj ./
RUN dotnet restore

# Copia o restante e publica
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Etapa final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Ajuste o nome do DLL abaixo, conforme seu .csproj real
ENTRYPOINT ["dotnet", "ProjetoTCC.dll"]
