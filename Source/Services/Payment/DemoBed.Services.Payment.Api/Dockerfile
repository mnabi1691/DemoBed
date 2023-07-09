#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Source/Services/Payment/DemoBed.Services.Payment.Api/DemoBed.Services.Payment.Api.csproj", "Source/Services/Payment/DemoBed.Services.Payment.Api/"]
COPY ["Source/Base/ExeptionHandling/DemoBed.Base.ExceptionHandling,Middleware/DemoBed.Base.ExceptionHandling,Middleware.csproj", "Source/Base/ExeptionHandling/DemoBed.Base.ExceptionHandling,Middleware/"]
COPY ["Source/Services/Payment/DemoBed.Services.Payment.ApplicationService/DemoBed.Services.Payment.ApplicationService.csproj", "Source/Services/Payment/DemoBed.Services.Payment.ApplicationService/"]
COPY ["Source/Base/Data/DemoBed.Base.Data/DemoBed.Base.Data.csproj", "Source/Base/Data/DemoBed.Base.Data/"]
COPY ["Source/Base/Models/DemoBed.Base.Models/DemoBed.Base.Models.csproj", "Source/Base/Models/DemoBed.Base.Models/"]
COPY ["Source/Services/Payment/DemoBed.Services.Payment.Data/DemoBed.Services.Payment.Data.csproj", "Source/Services/Payment/DemoBed.Services.Payment.Data/"]
COPY ["Source/Base/Services/DemoBed.Base.Services/DemoBed.Base.Services.csproj", "Source/Base/Services/DemoBed.Base.Services/"]
COPY ["Source/Base/Entities/DemoBed.Base.Entities/DemoBed.Base.Entities.csproj", "Source/Base/Entities/DemoBed.Base.Entities/"]
RUN dotnet restore "Source/Services/Payment/DemoBed.Services.Payment.Api/DemoBed.Services.Payment.Api.csproj"
COPY . .
WORKDIR "/src/Source/Services/Payment/DemoBed.Services.Payment.Api"
RUN dotnet build "DemoBed.Services.Payment.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DemoBed.Services.Payment.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoBed.Services.Payment.Api.dll"]