FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Mystique/Mystique.csproj", "Mystique/"]
COPY ["Mystique.Core/Mystique.Core.csproj", "Mystique.Core/"]
COPY ["Mystique.Core.Mvc/Mystique.Core.Mvc.csproj", "Mystique.Core.Mvc/"]
RUN dotnet restore "Mystique/Mystique.csproj"
COPY . .
WORKDIR "/src/Mystique"
RUN dotnet build "Mystique.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Mystique.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Mystique.dll"]