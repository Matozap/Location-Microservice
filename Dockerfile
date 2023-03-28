FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR "/src"
RUN dotnet dev-certs https

# Copy everything
COPY ./ /src
RUN ls -l

WORKDIR /src/LocationService.API
# Restore as distinct layers
RUN dotnet restore "LocationService.API.csproj"
RUN dotnet build "LocationService.API.csproj" -c Release -o /app/build
# Build and publish a release
RUN dotnet publish "LocationService.API.csproj" -c Release -o /app/publish

# Build runtime image
#FROM mcr.microsoft.com/dotnet/aspnet:7.0.2-alpine3.17-amd64 as final
FROM mcr.microsoft.com/dotnet/aspnet:7.0.4-jammy as final

ENV ASPNETCORE_URLS="https://*:5001;http://*:5000"
ENV DOTNET_URLS="https://+:5001;http://+:5000"
ENV DOTNET_GENERATE_ASPNET_CERTIFICATE=true

# Added only when using small images - consider using for runtiome FROM mcr.microsoft.com/dotnet/aspnet
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
# RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib

COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/

WORKDIR /app
COPY --from=build /app/publish /app
ENTRYPOINT ["dotnet", "LocationService.API.dll"]
