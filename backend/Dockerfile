FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/GymManagement.API/GymManagement.API.csproj", "src/GymManagement.API/"]
COPY ["src/GymManagement.Application/GymManagement.Application.csproj", "src/GymManagement.Application/"]
COPY ["src/GymManagement.Domain/GymManagement.Domain.csproj", "src/GymManagement.Domain/"]
COPY ["src/GymManagement.Infrastructure/GymManagement.Infrastructure.csproj", "src/GymManagement.Infrastructure/"]
RUN dotnet restore "src/GymManagement.API/GymManagement.API.csproj"
COPY . .
WORKDIR "/src/src/GymManagement.API"
RUN dotnet build "GymManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GymManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GymManagement.API.dll"]
