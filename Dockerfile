FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY TaskManager.slnx ./
COPY src/TaskManager.Core/TaskManager.Core.csproj src/TaskManager.Core/
COPY src/TaskManager.Api/TaskManager.Api.csproj src/TaskManager.Api/

RUN dotnet restore src/TaskManager.Api/TaskManager.Api.csproj

COPY src/ src/

RUN dotnet publish src/TaskManager.Api/TaskManager.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

RUN mkdir -p data/

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT [ "dotnet", "TaskManager.Api.dll" ]