FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép file csproj và restore các package
COPY CourseManagementService.csproj ./
RUN dotnet restore "./CourseManagementService.csproj"

# Sao chép toàn bộ mã nguồn và build ứng dụng
COPY . .
RUN dotnet publish "CourseManagementService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Cài đặt FFmpeg trên Debian
RUN apt-get update && \
    apt-get install -y ffmpeg && \
    rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "CourseManagementService.dll"]
