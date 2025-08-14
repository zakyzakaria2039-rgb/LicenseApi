# Gunakan image .NET SDK untuk build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj dan restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy semua file dan build
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Gunakan image .NET runtime untuk menjalankan
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "LicenseApi.dll"]
