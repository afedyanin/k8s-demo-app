FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base
WORKDIR /app
EXPOSE 80
ENV DOTNET_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["KuberDemo.csproj", "."]
RUN dotnet restore "./KuberDemo.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "KuberDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KuberDemo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KuberDemo.dll"]
