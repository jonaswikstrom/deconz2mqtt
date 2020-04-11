FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Deconz2Mqtt/*.csproj ./Deconz2Mqtt/
RUN dotnet restore

# copy everything else and build app
COPY Deconz2Mqtt/. ./Deconz2Mqtt/
WORKDIR /app/Deconz2Mqtt
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY --from=build /app/Deconz2Mqtt/out ./
ENTRYPOINT ["dotnet", "Deconz2Mqtt.dll"]