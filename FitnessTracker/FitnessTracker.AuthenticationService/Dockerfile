# Базовый образ для рантайма ASP.NET Core
ARG D_RUNTIME=mcr.microsoft.com/dotnet/aspnet:8.0
ARG D_SDK=mcr.microsoft.com/dotnet/sdk:8.0

FROM ${D_RUNTIME} AS base
WORKDIR /app

# Образ для сборки
FROM ${D_SDK} AS build
WORKDIR /src


# Копируем проекты
COPY ["FitnessTracker.AuthenticationService/", "FitnessTracker.AuthenticationService/"]
COPY ["FitnessTracker.Core/", "FitnessTracker.Core/"]
COPY ["FitnessTracker.DataAccess/", "FitnessTracker.DataAccess/"]
COPY ["FitnessTracker.Application/", "FitnessTracker.Application/"]

# Восстанавливаем зависимости для решения
RUN dotnet restore "FitnessTracker.AuthenticationService/FitnessTracker.AuthenticationService.csproj"


# Копируем весь код проекта, включая папки всех проектов и решения
#COPY . .

# Сборка приложения
WORKDIR /src/FitnessTracker.AuthenticationService
RUN dotnet build -c Release -o /app/build

# Публикация приложения
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish


# Образ для запуска приложения
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FitnessTracker.AuthenticationService.dll"]