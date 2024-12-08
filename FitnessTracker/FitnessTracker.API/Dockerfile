# Базовый образ для рантайма ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Образ для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


# Копируем проекты
COPY ["FitnessTracker.API/", "FitnessTracker.API/"]
COPY ["FitnessTracker.Core/", "FitnessTracker.Core/"]
COPY ["FitnessTracker.Application/", "FitnessTracker.Application/"]

# Восстанавливаем зависимости для решения
RUN dotnet restore "FitnessTracker.API/FitnessTracker.API.csproj"

# Копируем весь код проекта, включая папки всех проектов и решения
#COPY . .

# Сборка приложения
WORKDIR /src/FitnessTracker.API
RUN dotnet build -c Release -o /app/build

# Публикация приложения
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Образ для запуска приложения
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FitnessTracker.API.dll"]