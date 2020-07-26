FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY Mystique ./Mystique/
COPY Mystique.Core ./Mystique.Core/
COPY Mystique.Core.Mvc ./Mystique.Core.Mvc/
COPY Mystique.Core.Repository.MySql ./Mystique.Core.Repository.MySql/

WORKDIR /app/Mystique
RUN dotnet restore

RUN dotnet publish -c Release -o out

# ±‡“ÎDockeræµœÒ
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app/Mystique
COPY --from=build-env /app/Mystique/out .
ENV ASPNETCORE_URLS http://0.0.0.0:5000
ENTRYPOINT ["dotnet", "Mystique.dll"]