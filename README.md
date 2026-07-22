# EduSmartBE - Hệ thống Microservices Phía Back-End

Dự án này là hệ thống back-end của nền tảng giáo dục thông minh **EduSmart**, được thiết kế theo kiến trúc **Microservices** hướng sự kiện (Event-Driven) sử dụng .NET 8.0, Node.js, RabbitMQ, PostgreSQL, Redis và lưu trữ tệp tin thông qua MinIO S3 Object Storage.

---

## 🏗️ Kiến Trúc Hệ Thống & Cổng Dịch Vụ

Hệ thống bao gồm các dịch vụ thành viên chạy cô lập qua Docker và kết nối qua mạng nội bộ ảo `microservices-network`:

| Tên Service | Công Nghệ | Cổng Host | Đường Dẫn Trực Tiếp | Đường Dẫn Qua API Gateway (Cổng 7155) |
| :--- | :--- | :--- | :--- | :--- |
| **Auth Service** | .NET 8 | `7150` | [http://localhost:7150/auth-service/swagger/index.html](http://localhost:7150/auth-service/swagger/index.html) | [http://localhost:7155/auth-service/swagger/index.html](http://localhost:7155/auth-service/swagger/index.html) |
| **User Service** | .NET 8 | `7151` | [http://localhost:7151/user-service/swagger/index.html](http://localhost:7151/user-service/swagger/index.html) | [http://localhost:7155/user-service/swagger/index.html](http://localhost:7155/user-service/swagger/index.html) |
| **Course Service** | .NET 8 | `7152` | [http://localhost:7152/course-service/swagger/index.html](http://localhost:7152/course-service/swagger/index.html) | [http://localhost:7155/course-service/swagger/index.html](http://localhost:7155/course-service/swagger/index.html) |
| **Payment Service** | .NET 8 | `7153` | [http://localhost:7153/payment-service/swagger/index.html](http://localhost:7153/payment-service/swagger/index.html) | [http://localhost:7155/payment-service/swagger/index.html](http://localhost:7155/payment-service/swagger/index.html) |
| **Media Service** | Node.js | `7154` | Dịch vụ xử lý ảnh & FFmpeg ngầm | - |
| **API Gateway (Nginx)** | Reverse Proxy | `7155` | Điểm truy cập chung của FE: [http://localhost:7155](http://localhost:7155) | - |
| **RabbitMQ** | Message Bus | `15672` | Giao diện quản trị: [http://localhost:15672](http://localhost:15672) (guest/guest) | - |
| **PostgreSQL** | Database | `5432` | Cổng CSDL chính của hệ thống | - |
| **MinIO** | S3 Storage | - | - | - |
| **Redis** | Caching | - | - | - |

> [!NOTE]  
> Nhờ có **API Gateway**, phía Front-End (FE) chỉ cần trỏ biến môi trường `NEXT_PUBLIC_API_URL` về duy nhất địa chỉ **`http://localhost:7155`**. Các request có dạng `http://localhost:7155/auth-service/...` hay `http://localhost:7155/user-service/...` sẽ được Nginx tự động định tuyến đến đúng service tương ứng.

> [!IMPORTANT]  
> Toàn bộ giao diện tài liệu API Swagger (các đường dẫn bắt đầu bằng `/*/swagger`) đều được bảo vệ bằng **HTTP Basic Authentication** qua Nginx để bảo mật thông tin.
> *   **Tài khoản đăng nhập mặc định**:
>     *   **Username**: `admin`
>     *   **Password**: `edusmart_admin`
> *   Các cuộc gọi API trực tiếp từ Front-end (ví dụ qua đầu dẫn `/api/...`) hoàn toàn **không bị ảnh hưởng** và truy cập công khai bình thường.

---

## 🛠️ Yêu Cầu Hệ Thống

Để phát triển hoặc khởi chạy dự án, máy tính cần cài sẵn:
1. **Docker** & **Docker Compose**
2. **.NET SDK 8.0** (Nếu muốn build hoặc chạy trực tiếp không qua Docker)

### 📦 Hướng dẫn cài đặt .NET SDK 8.0 trên Ubuntu 22.04 LTS

Chạy chuỗi lệnh sau trực tiếp trên Terminal của hệ điều hành để cài đặt chính thức:

```bash
# 1. Đăng ký kho gói lưu trữ của Microsoft
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# 2. Cập nhật danh sách gói và cài đặt .NET 8 SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

# 3. Kiểm tra xem đã cài thành công chưa
dotnet --version
```

---

## 🚀 Hướng Dẫn Chạy Bằng Docker Compose

### Bước 1: Khởi tạo và Build các Service
Chạy lệnh biên dịch và đóng gói Docker image cho tất cả các dịch vụ:
```bash
docker compose build
```

### Bước 2: Khởi chạy hệ thống
Chạy các container ở chế độ nền (detached mode):
```bash
docker compose up -d
```

### Bước 3: Kiểm tra trạng thái hoạt động
```bash
docker compose ps
```

### Bước 4: Tắt hệ thống
```bash
docker compose down
```

### Bước 5: Reload cấu hình Nginx (API Gateway)
Nếu bạn cập nhật hoặc chỉnh sửa tệp tin cấu hình `nginx/nginx.conf`, bạn có thể yêu cầu Nginx áp dụng cấu hình mới ngay lập tức mà không cần khởi động lại container bằng cách chạy lệnh:
```bash
docker exec api_gateway nginx -s reload
```

---

## 💻 Hướng Dẫn Phát Triển Local (Không Qua Docker)

Nếu muốn phát triển và sửa code nhanh trực tiếp bằng IDE (VS Code, Visual Studio, Rider):

1. **Khởi chạy tài nguyên nền (Database & Message Bus)**:
   Bạn có thể chỉ chạy `postgres` và `rabbitmq` bằng Docker:
   ```bash
   docker compose up -d postgres rabbitmq
   ```

2. **Chạy từng Service**:
   Trỏ vào thư mục của service mong muốn và chạy lệnh:
   ```bash
   dotnet restore
   dotnet run
   ```

3. **Cấu hình biến môi trường**:
   Các dịch vụ hỗ trợ ghi đè biến môi trường thông qua tệp `appsettings.Development.json` hoặc truyền trực tiếp trên terminal trước khi chạy.

---

## 🔒 Lưu Ý Khi Làm Việc Qua SSH Remote / Cloud Server

Nếu bạn deploy dự án trên server đám mây và kết nối qua **VS Code SSH Remote**:
* Các cổng trên host (như `7150`, `7151`, `7152`, `7153`) cần phải được **Port Forwarding** trong cửa sổ VS Code (Tab *Ports* cạnh Tab *Terminal*).
* Sau khi forward port, bạn có thể truy cập `http://localhost:<PORT>/...` bình thường ngay trên trình duyệt máy cá nhân của mình.

---

## 💾 Hướng Dẫn Sao Lưu & Khôi Phục Dữ Liệu (PostgreSQL Backup & Restore)

Các file backup của hệ thống được lưu trữ trong thư mục `./backups/`.

### 1. Sao Lưu Dữ Liệu (Backup)

* **Sao lưu toàn bộ CSDL (File tổng `.bak` / `.sql`):**
  ```bash
  mkdir -p ./backups
  docker exec postgres pg_dumpall -U pbl6duter > ./backups/postgres_full_backup_$(date +%Y%m%d_%H%M%S).bak
  ```

* **Sao lưu từng CSDL dịch vụ dạng Custom Format (`.bak`):**
  ```bash
  docker exec postgres pg_dump -U pbl6duter -F c -d "EduSmart.AuthService" > ./backups/EduSmart.AuthService.bak
  docker exec postgres pg_dump -U pbl6duter -F c -d "EduSmart.CourseManagementService" > ./backups/EduSmart.CourseManagementService.bak
  docker exec postgres pg_dump -U pbl6duter -F c -d "EduSmart.PaymentManagementService" > ./backups/EduSmart.PaymentManagementService.bak
  docker exec postgres pg_dump -U pbl6duter -F c -d "EduSmart.UserService" > ./backups/EduSmart.UserService.bak
  ```

### 2. Khôi Phục Dữ Liệu (Restore)

* **Khôi phục toàn bộ từ file backup tổng:**
  ```bash
  docker exec -i postgres psql -U pbl6duter < ./backups/postgres_full_backup_<TIMESTAMP>.bak
  ```

* **Khôi phục từng CSDL dịch vụ từ file `.bak` custom format:**
  ```bash
  docker exec -i postgres pg_restore -U pbl6duter -d "EduSmart.AuthService" --clean < ./backups/EduSmart.AuthService.bak
  ```

