FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Casefile.sln ./
COPY src/Casefile.Domain/Casefile.Domain.csproj src/Casefile.Domain/
COPY src/Casefile.Api/Casefile.Api.csproj src/Casefile.Api/
RUN dotnet restore src/Casefile.Api/Casefile.Api.csproj
COPY . .
RUN dotnet publish src/Casefile.Api/Casefile.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN mkdir -p /app/data
ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__Casefile="Data Source=/app/data/casefile.db"
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Casefile.Api.dll"]
