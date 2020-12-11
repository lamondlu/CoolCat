FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY Mystique ./Mystique/

WORKDIR /app/Mystique
RUN dotnet restore

RUN dotnet publish -c Release -o out

# ±‡“ÎDockeræµœÒ
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app/Mystique
COPY --from=build-env /app/Mystique/out .
ENV ASPNETCORE_URLS http://0.0.0.0:5000
ENTRYPOINT ["dotnet", "Mystqiue.dll"]