FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["MoatGate.csproj", "MoatGate/"]
RUN dotnet restore "MoatGate/MoatGate.csproj"
WORKDIR "/src/MoatGate"
COPY . .
RUN dotnet build "MoatGate.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MoatGate.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MoatGate.dll"]