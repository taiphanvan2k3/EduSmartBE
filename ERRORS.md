# Nhật Ký Lỗi & Giải Pháp (ERRORS.md)

Tài liệu này ghi nhận các lỗi hệ thống gặp phải trong quá trình phát triển, cấu hình và triển khai hệ thống EduSmartBE Microservices cùng giải pháp khắc phục chi tiết.

---

## 1. Lỗi `413 Content Too Large` / CORS khi Upload File lớn hơn 100 MB
* **Ngày ghi nhận**: 07/07/2026

### 🛑 Triệu chứng:
* Trình duyệt báo lỗi CORS khi thực hiện yêu cầu `POST` tải lên tệp tin video dung lượng ~160 MB đến cổng dịch vụ.
* Trong Network Tab, mã trạng thái HTTP trả về là **`413 Content Too Large`** (hoặc `413 Request Entity Too Large`), header trả về ghi nhận `server: cloudflare`.

### 🔍 Nguyên nhân:
* Hệ thống sử dụng Cloudflare proxy cho domain `edusmart.taiphanvan.id.vn`. 
* Gói dịch vụ Cloudflare Free/Pro/Business giới hạn dung lượng upload tối đa qua proxy là **`100 MB`**. Mọi yêu cầu vượt quá giới hạn này sẽ bị Cloudflare chặn và trả về lỗi `413` ngay tại Edge mà không gửi tới server backend. Do trang lỗi của Cloudflare không chứa các header cấu hình CORS của ứng dụng nên trình duyệt hiểu lầm thành lỗi CORS.

### 💡 Giải pháp khắc phục:
1. **Giải pháp ngắn hạn (Development/Testing)**: Trỏ trực tiếp Front-End gọi qua IP server và cổng của API Gateway (ví dụ: `http://<IP_SERVER>:7155/...`) để bypass qua proxy Cloudflare.
2. **Giải pháp dài hạn (Production - Khuyên dùng)**: Sử dụng cơ chế **Presigned URL** của MinIO:
   * Front-end xin một link upload từ backend.
   * Backend sinh một đường dẫn được ký sẵn (Presigned URL) có thời hạn từ MinIO S3.
   * Front-end thực hiện đẩy file trực tiếp từ trình duyệt lên MinIO bằng đường dẫn đó (hoàn toàn bỏ qua API Gateway và Backend).

---

## 2. Lỗi `404 Not Found` sang sai Service sau khi Rebuild/Recreate Backend
* **Ngày ghi nhận**: 07/07/2026

### 🛑 Triệu chứng:
* Sau khi chỉnh sửa code hoặc cấu hình CORS của các microservice (`authservice`, `userservice`...) và build lại, tất cả các request gửi đến `/course-service/...` đều nhận về mã lỗi **`404 Not Found`**.
* Kiểm tra log của container `userservice` thì thấy các request của `/course-service/...` đang chạy nhầm vào đây và bị từ chối với mã 404.

### 🔍 Nguyên nhân:
* Khi chạy `docker compose up -d` sau khi rebuild, các container backend bị hủy và tạo lại, dẫn đến **địa chỉ IP nội bộ của chúng trong mạng ảo Docker thay đổi**.
* Container Nginx API Gateway (`api_gateway`) không có thay đổi cấu hình nên không khởi động lại. Nginx theo cơ chế mặc định chỉ phân giải tên miền (DNS) của các upstream (`courseservice`, `userservice`...) **một lần duy nhất lúc khởi động và cache lại**.
* Nginx tiếp tục gửi request của Course Service sang IP cũ (mà lúc này Docker đã cấp lại cho User Service), gây ra hiện tượng định tuyến sai mục tiêu.

### 💡 Giải pháp khắc phục:
  ```bash
  docker compose restart api-gateway
  ```

---

## 3. Lỗi `400 Bad Request` / Trắng Trang (Failed to load response data) khi tạo khóa học với mảng tagIds[] rỗng
* **Ngày ghi nhận**: 07/07/2026

### 🛑 Triệu chứng:
* Yêu cầu `POST /course-service/api/teacher-course-management/courses` bị lỗi **`400 Bad Request`**.
* Trình duyệt (Chrome/Edge) xoay tròn rất lâu rồi báo lỗi trắng trơn trong tab Response: `Failed to load response data. No data found for resource with given identifier` kèm cảnh báo `Provisional headers are shown`.

### 🔍 Nguyên nhân:
1. **Lỗi xác thực dữ liệu (C# Model Binder)**: Phía Front-End gửi dữ liệu dạng Form-Data nhưng lại append khóa `tagIds[]` với giá trị rỗng `""` khi người dùng không chọn tag nào. Phía backend C# định nghĩa `List<int> TagIds`, nên Model Binder cố gắng ép kiểu chuỗi rỗng `""` sang `int`, gây lỗi `The value '' is invalid` trong `ModelState`.
2. **Lỗi trắng trang (DevTools)**: Do payload gửi lên có dung lượng lớn (chứa file ảnh thumbnail/video). Khi C# gặp lỗi xác thực ở luồng đầu vào, Kestrel lập tức ngắt kết nối (close connection) và trả về mã 400. Trình duyệt lúc này vẫn đang mải upload file lớn, gặp sự cố kết nối bị đứt đột ngột sẽ hiển thị lỗi Connection Reset/Aborted, khiến DevTools không kịp đọc dữ liệu JSON 400 trả về.

### 💡 Giải pháp khắc phục:
* **Phía Front-End**: Chỉ thực hiện append `tagIds[]` vào `FormData` khi thực sự có ít nhất một nhãn tag được chọn. Nếu không chọn tag nào, **bỏ hoàn toàn (không append)** trường này khỏi `FormData`.
  ```javascript
  if (tags && tags.length > 0) {
      tags.forEach(tagId => formData.append("tagIds[]", tagId));
  }
  ```

---

## 4. Lỗi thiếu `ffmpeg` trong container `courseservice` khi phân tích video bài giảng
* **Ngày ghi nhận**: 07/07/2026

### 🛑 Triệu chứng:
* Việc upload video bài giảng chạy ngầm (background job) báo lỗi đỏ log: 
  `[VideoLessonDetailService] [UpdateLessonVideoUrl->GetDurationOfVideo] Cannot find FFmpeg in PATH. This package needs installed FFmpeg...`
* Bài học được tạo nhưng thời lượng video (`DurationInSeconds`) không được tính toán đúng (mặc định bằng 0).

### 🔍 Nguyên nhân:
* Ứng dụng chạy bên trong container Docker dựa trên base image `mcr.microsoft.com/dotnet/aspnet:8.0` tối giản, không đi kèm sẵn công cụ dòng lệnh xử lý đa phương tiện `ffmpeg`.

### 💡 Giải pháp khắc phục:
* Cập nhật file `Dockerfile` của `CourseManagementService`, thêm lệnh cài đặt `ffmpeg` vào tầng **`runtime`** (đặt trước câu lệnh `COPY --from=build` để Docker cache lại layer cài đặt, tránh cài lại khi thay đổi code C#):
  ```dockerfile
  # Stage 2: Tạo image runtime
  FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
  WORKDIR /app

  # Cài đặt FFmpeg trong môi trường runtime để phân tích video
  RUN apt-get update && \
      apt-get install -y ffmpeg && \
      rm -rf /var/lib/apt/lists/*

  # Copy output từ stage build
  COPY --from=build /app/publish .
  ```

---

## 5. Lỗi CORS khi gọi API sang `/media-service/...`

* **Ngày ghi nhận**: 07/07/2026

### 🛑 Triệu chứng:
* Yêu cầu gọi API từ Front-End đến đường dẫn `https://edusmart.taiphanvan.id.vn/media-service/api/templates` bị trình duyệt chặn CORS với lỗi: `No 'Access-Control-Allow-Origin' header is present on the requested resource`.

### 🔍 Nguyên nhân:
* Thiếu cấu hình định tuyến cho `media-service` trong API Gateway (`nginx/nginx.conf`).
* Khi Front-End gửi yêu cầu preflight `OPTIONS` đến đường dẫn không tồn tại trên Nginx, Nginx sẽ trực tiếp trả về lỗi `404 Not Found` (hoặc `405 Method Not Allowed`) mà không kèm theo các header CORS, dẫn đến việc trình duyệt báo lỗi CORS.

### 💡 Giải pháp khắc phục:
1. Thêm cấu hình upstream cho `media-service` trỏ đến cổng `7154` của container:
   ```nginx
   upstream media_service {
       server mediaservice:7154;
   }
   ```
2. Thêm các location blocks chuyển tiếp yêu cầu API và bảo vệ Swagger của `media-service` bằng Basic Auth:
   ```nginx
   location ~ ^/media-service/swagger {
       auth_basic "Restricted area - Swagger Docs";
       auth_basic_user_file /etc/nginx/.htpasswd;
       proxy_pass http://media_service;
   }

   location /media-service {
       proxy_pass http://media_service;
   }
   ```
3. Chạy reload lại Nginx mà không cần restart container:
   ```bash
   docker exec api_gateway nginx -s reload
   ```

---

## 6. Lỗi `The server does not support SSL connections` khi kết nối Database từ `media-service`

* **Ngày ghi nhận**: 07/07/2026

### 🛑 Triệu chứng:
* Gọi API của `media-service` bị lỗi `500 Internal Server Error`.
* Trong log container hiển thị lỗi: `ERROR: [getAllAchievementTemplates] Error: The server does not support SSL connections`.

### 🔍 Nguyên nhân:
* Trong tệp cấu hình kết nối [database.js](file:///home/taipv/workspace/EduSmartBE/MediaService/src/configs/database.js), thuộc tính `ssl` luôn được định nghĩa (dưới dạng object chứa `rejectUnauthorized`).
* Thư viện `pg` (Node Postgres) khi thấy thuộc tính `ssl` được định nghĩa sẽ mặc định cố gắng đàm phán một kết nối SSL bảo mật với cơ sở dữ liệu. Tuy nhiên, container `postgres` chạy local không được bật cấu hình mã hóa SSL, dẫn đến việc Postgres từ chối và ngắt kết nối.

### 💡 Giải pháp khắc phục:
1. Cấu hình lại [database.js](file:///home/taipv/workspace/EduSmartBE/MediaService/src/configs/database.js) chỉ kích hoạt `ssl` khi biến môi trường `DB_SSL` được đặt bằng `"true"`:
   ```javascript
   ssl: process.env.DB_SSL === "true"
       ? { rejectUnauthorized: process.env.DB_TRUST_CERT !== "true" }
       : false
   ```
2. Thêm biến môi trường `DB_SSL=false` vào tệp cấu hình [MediaService/.env](file:///home/taipv/workspace/EduSmartBE/MediaService/.env#L12) để tắt kết nối SSL khi chạy môi trường local/docker-compose.
3. Rebuild và khởi động lại container:
   ```bash
   docker compose build mediaservice && docker compose up -d mediaservice && docker compose restart api-gateway
   ```
