FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# create cert for Dev SSL @see https://github.com/dotnet/dotnet-docker/blob/main/samples/run-aspnetcore-https-development.md#linux
RUN dotnet dev-certs https -ep /source/TodoApi.pfx -p 12345
RUN dotnet user-secrets -p /source/TodoApi.csproj init
RUN dotnet user-secrets -p /source/TodoApi.csproj set "Kestrel:Certificates:Development:Password" "12345"

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
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "/app/TodoApi.dll"]