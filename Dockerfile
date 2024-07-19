# Use the official .NET runtime image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ServerYourWorldMMORPG/ServerYourWorldMMORPG.csproj", "ServerYourWorldMMORPG/"]
RUN dotnet restore "ServerYourWorldMMORPG/ServerYourWorldMMORPG.csproj"
COPY . .
WORKDIR "/src/ServerYourWorldMMORPG"
RUN dotnet build "ServerYourWorldMMORPG.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServerYourWorldMMORPG.csproj" -c Release -o /app/publish

# Copy the build app to the runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServerYourWorldMMORPG.dll"]
