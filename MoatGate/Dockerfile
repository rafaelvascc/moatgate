FROM mcr.microsoft.com/dotnet/core/sdk:2.2.203-alpine3.9 AS build
WORKDIR /src
COPY MoatGate.csproj .
RUN dotnet restore "MoatGate.csproj"
COPY . .
RUN dotnet build "MoatGate.csproj" -c Release -o /app
RUN dotnet publish "MoatGate.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2.4-alpine3.9 AS base
WORKDIR /app
EXPOSE 5000
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "MoatGate.dll"]