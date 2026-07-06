FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SigmaTrack.WebApi/SigmaTrack.WebApi.csproj", "SigmaTrack.WebApi/"]
COPY ["SigmaTrack.Application/SigmaTrack.Application.csproj", "SigmaTrack.Application/"]
COPY ["SigmaTrack.Domain/SigmaTrack.Domain.csproj", "SigmaTrack.Domain/"]
COPY ["SigmaTrack.Infrastructure/SigmaTrack.Infrastructure.csproj", "SigmaTrack.Infrastructure/"]

RUN dotnet restore "SigmaTrack.WebApi/SigmaTrack.WebApi.csproj"

COPY . .
WORKDIR "/src/SigmaTrack.WebApi"
RUN dotnet build "SigmaTrack.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SigmaTrack.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5069
ENV ASPNETCORE_URLS=http://+:5069

ENTRYPOINT ["dotnet", "SigmaTrack.WebApi.dll"]