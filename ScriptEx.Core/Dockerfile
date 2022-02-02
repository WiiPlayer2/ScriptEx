#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG TARGETOS
ARG TARGETARCH
WORKDIR /src
COPY . .
RUN dotnet restore "ScriptEx.Core/ScriptEx.Core.csproj"
WORKDIR "/src/ScriptEx.Core"
RUN dotnet build "ScriptEx.Core.csproj" -c Release -o /app/build -r ${TARGETOS}-$(if [ "${TARGETARCH}" = "amd64" ]; then echo "x64"; else echo "${TARGETARCH}"; fi)

FROM build AS publish
RUN dotnet publish "ScriptEx.Core.csproj" -c Release -o /app/publish -r ${TARGETOS}-$(if [ "${TARGETARCH}" = "amd64" ]; then echo "x64"; else echo "${TARGETARCH}"; fi)

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScriptEx.Core.dll"]
