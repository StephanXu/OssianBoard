FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 443
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["Logger/Logger.csproj", "Logger/"]
RUN dotnet restore "Logger/Logger.csproj"
COPY . .
WORKDIR "/src/Logger"
RUN dotnet build "Logger.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Logger.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Logger.dll"]
