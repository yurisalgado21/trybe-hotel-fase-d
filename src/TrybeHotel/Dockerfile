FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "/home/yuri/projects-trybe/csharp-033-csharp-projeto-trybe-hotel-fase-d/src/TrybeHotel/obj/Debug/net6.0/TrybeHotel.dll" ]