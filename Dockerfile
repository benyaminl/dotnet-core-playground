FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# create cert for Dev SSL @see https://github.com/dotnet/dotnet-docker/blob/main/samples/run-aspnetcore-https-development.md#linux
RUN dotnet dev-certs https -ep /source/TodoApi.pfx -p 12345
RUN dotnet user-secrets -p /source/TodoApi.csproj init
RUN dotnet user-secrets -p /source/TodoApi.csproj set "Kestrel:Certificates:Development:Password" "12345"

# Disable Cache on Docker when build 
# ALWAYS USE --build-arg CACHE_DATE=$(date +%Y-%m-%d:%H:%M:%S)
# If you need to drop cache before this, use --build --no-cache flag
# @see https://stackoverflow.com/a/38261124/4906348
ARG CACHE_DATE=not_a_date
# copy and publish app and libraries
COPY . .
RUN dotnet build -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
# Copy the Source file @see https://github.com/dotnet/dotnet-docker/blob/main/samples/run-aspnetcore-https-development.md#linux
COPY --from=build /source/TodoApi.pfx /root/.aspnet/https/TodoApi.pfx
COPY --from=build /root/.microsoft/usersecrets /root/.microsoft/usersecrets
# Supporting SQL Server 2008 R2 TLS 1.0 @see https://programmer.ink/think/net-5-error-accessing-mssql-in-docker.html
# @see https://docs.microsoft.com/en-us/sql/connect/ado-net/sqlclient-troubleshooting-guide?view=sql-server-ver15#possible-reasons-and-solutions
RUN sed -i 's/TLSv1.2/TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "/app/TodoApi.dll"]