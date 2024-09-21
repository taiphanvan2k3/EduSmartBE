FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép file csproj và restore các package
COPY UserService.csproj ./
RUN dotnet restore "./UserService.csproj"

# Sao chép toàn bộ mã nguồn và build ứng dụng
COPY . .
RUN dotnet publish "UserService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "UserService.dll"]
