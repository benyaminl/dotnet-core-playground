# .NET Core 6.0 playground with SQL Server and S3
[![wakatime](https://wakatime.com/badge/user/61ea0986-ad92-4f11-a269-8a63ad48b2cc/project/49f0cfe9-2ee8-4737-a9a8-129f4f163b7c.svg)](https://wakatime.com/badge/user/61ea0986-ad92-4f11-a269-8a63ad48b2cc/project/49f0cfe9-2ee8-4737-a9a8-129f4f163b7c)
This is a simple .NET Core 6.0 playground with SQL Server trusted connection/WinAuth, and swagger UI and some improvement on the swagger generator. 

I don't provide any other readme as this is just a playground for myself. kindly follow https://blog.benyamin.xyz to see more notes regarding this project. 

## Run and build project

Restore using `dotnet restore` [restore](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-restore), then `dotnet run` [run](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-run). Then you are good to go.


### Minio/S3 Compatible dependencies

This project use 3rd party plugin/package from [Min.io](https://www.nuget.org/packages/Minio/) and [Minio.AspNetCore](https://github.com/appany/Minio.AspNetCore)
For more, you can use Podman/Docker as Server emulator, see [here](https://min.io/download#/docker)

