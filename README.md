# EduSmartBE - Hệ thống Microservices Phía Back-End

Dự án này là hệ thống back-end của nền tảng giáo dục thông minh **EduSmart**, được thiết kế theo kiến trúc **Microservices** hướng sự kiện (Event-Driven) sử dụng .NET 8.0, Node.js, RabbitMQ, PostgreSQL, Redis và lưu trữ tệp tin thông qua MinIO S3 Object Storage.

---

## 🏗️ Kiến Trúc Hệ Thống & Cổng Dịch Vụ

Hệ thống bao gồm các dịch vụ thành viên chạy cô lập qua Docker và kết nối qua mạng nội bộ ảo `microservices-network`:

| Tên Service | Công Nghệ | Cổng Host | Cổng Container | Đường Dẫn Swagger UI / Quản Trị |
| :--- | :--- | :--- | :--- | :--- |
| **Auth Service** | .NET 8 | `7150` | `80`, `10000` (gRPC) | [http://localhost:7150/auth-service/swagger/index.html](http://localhost:7150/auth-service/swagger/index.html) |
| **User Service** | .NET 8 | `7151` | `80`, `10001` (gRPC) | [http://localhost:7151/user-service/swagger/index.html](http://localhost:7151/user-service/swagger/index.html) |
| **Course Service** | .NET 8 | `7152` | `80` | [http://localhost:7152/course-service/swagger/index.html](http://localhost:7152/course-service/swagger/index.html) |
| **Payment Service** | .NET 8 | `7153` | `80` | [http://localhost:7153/payment-service/swagger/index.html](http://localhost:7153/payment-service/swagger/index.html) |
| **Media Service** | Node.js | `7154` | `7154` | Dịch vụ xử lý ảnh & FFmpeg ngầm |
| **RabbitMQ** | Message Bus | `5672`, `15672` | `5672`, `15672` | [http://localhost:15672](http://localhost:15672) (User/Pass: `guest` / `guest`) |
| **PostgreSQL** | Database | `5432` | `5432` | Cơ sở dữ liệu chính của hệ thống |
| **MinIO** | S3 Storage | - | - |  |
| **Redis** | Caching | - | - |  |

---

## 🛠️ Yêu Cầu Hệ Thống

Để phát triển hoặc khởi chạy dự án, máy tính cần cài sẵn:
1. **Docker** & **Docker Compose**
2. **.NET SDK 8.0** (Nếu muốn build hoặc chạy trực tiếp không qua Docker)

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
