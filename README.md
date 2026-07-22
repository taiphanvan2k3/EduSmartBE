# EduSmartBE - Back-End Microservices Architecture

This project is the back-end system for the **EduSmart** smart education platform, designed with an event-driven **Microservices** architecture using .NET 8.0, Node.js, RabbitMQ, PostgreSQL, Redis, and file storage via MinIO S3 Object Storage.

---

## 🏗️ System Architecture & Service Ports

The system consists of member services running isolated via Docker and connected through the internal virtual network `microservices-network`:

| Service Name | Technology | Host Port | Direct URL | API Gateway URL (Port 7155) |
| :--- | :--- | :--- | :--- | :--- |
| **Auth Service** | .NET 8 | `7150` | [http://localhost:7150/auth-service/swagger/index.html](http://localhost:7150/auth-service/swagger/index.html) | [http://localhost:7155/auth-service/swagger/index.html](http://localhost:7155/auth-service/swagger/index.html) |
| **User Service** | .NET 8 | `7151` | [http://localhost:7151/user-service/swagger/index.html](http://localhost:7151/user-service/swagger/index.html) | [http://localhost:7155/user-service/swagger/index.html](http://localhost:7155/user-service/swagger/index.html) |
| **Course Service** | .NET 8 | `7152` | [http://localhost:7152/course-service/swagger/index.html](http://localhost:7152/course-service/swagger/index.html) | [http://localhost:7155/course-service/swagger/index.html](http://localhost:7155/course-service/swagger/index.html) |
| **Payment Service** | .NET 8 | `7153` | [http://localhost:7153/payment-service/swagger/index.html](http://localhost:7153/payment-service/swagger/index.html) | [http://localhost:7155/payment-service/swagger/index.html](http://localhost:7155/payment-service/swagger/index.html) |
| **Media Service** | Node.js | `7154` | Background image processing & FFmpeg service | - |
| **API Gateway (Nginx)** | Reverse Proxy | `7155` | Front-End unified entry point: [http://localhost:7155](http://localhost:7155) | - |
| **RabbitMQ** | Message Bus | `15672` | Management Console: [http://localhost:15672](http://localhost:15672) (guest/guest) | - |
| **PostgreSQL** | Database | `5432` | Primary system database port | - |
| **MinIO** | S3 Storage | - | - | - |
| **Redis** | Caching | - | - | - |

> [!NOTE]  
> Thanks to the **API Gateway**, the Front-End (FE) only needs to set the `NEXT_PUBLIC_API_URL` environment variable to **`http://localhost:7155`**. Requests matching `http://localhost:7155/auth-service/...` or `http://localhost:7155/user-service/...` will be automatically routed by Nginx to the corresponding service.

> [!IMPORTANT]  
> All Swagger API documentation interfaces (routes starting with `/*/swagger`) are protected via Nginx using **HTTP Basic Authentication** for security.
> *   **Default Login Credentials**:
>     *   **Username**: `admin`
>     *   **Password**: `edusmart_admin`
> *   Direct API calls from Front-end (e.g., `/api/...` endpoints) are **unaffected** and remain publicly accessible.

---

## 🛠️ Prerequisites

To develop or run the project locally, your machine needs:
1. **Docker** & **Docker Compose**
2. **.NET SDK 8.0** (If building or running services directly without Docker)

### 📦 Guide to Install .NET SDK 8.0 on Ubuntu 22.04 LTS

Run the following commands in your terminal to install officially:

```bash
# 1. Register Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# 2. Update package list and install .NET 8 SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

# 3. Verify installation
dotnet --version
```

---

## 🚀 Running with Docker Compose

### Step 1: Build Services
Compile and build Docker images for all services:
```bash
docker compose build
```

### Step 2: Start Services
Run containers in background (detached mode):
```bash
docker compose up -d
```

### Step 3: Check Container Status
```bash
docker compose ps
```

### Step 4: Stop System
```bash
docker compose down
```

### Step 5: Reload Nginx Configuration (API Gateway)
If you modify the `nginx/nginx.conf` file, you can apply changes immediately without restarting the container:
```bash
docker exec api_gateway nginx -s reload
```

---

## 💻 Local Development (Without Docker)

To edit and debug code directly using an IDE (VS Code, Visual Studio, Rider):

1. **Start Infrastructure Services (Database & Message Bus)**:
   Launch only `postgres` and `rabbitmq` containers:
   ```bash
   docker compose up -d postgres rabbitmq
   ```

2. **Run Individual Services**:
   Navigate to the desired service directory and execute:
   ```bash
   dotnet restore
   dotnet run
   ```

3. **Configure Environment Variables**:
   Services support overriding environment settings via `appsettings.Development.json` or environment variables set in terminal.

---

## 🔒 Working Remotely via SSH / Cloud Server

If deploying to a cloud server and connecting via **VS Code SSH Remote**:
* Host ports (`7150`, `7151`, `7152`, `7153`) must be **Port Forwarded** in VS Code (under the *Ports* tab next to *Terminal*).
* Once forwarded, access `http://localhost:<PORT>/...` directly in your local web browser.

---

## 💾 Database Backup & Restore Guide (PostgreSQL)

Backup files are saved in the `./backups/` directory.

### 1. Database Backup

* **Backup All Databases (Full `.bak` / `.sql` file):**
  ```bash
  mkdir -p ./backups
  docker exec postgres pg_dumpall -U pbl6duter > ./backups/postgres_full_backup_$(date +%Y%m%d_%H%M%S).bak
  ```

* **Backup Individual Service Databases (Custom Format `.bak`):**
  ```bash
  docker exec postgres pg_dump -U pbl6duter -F c -d "EduSmart.AuthService" > ./backups/EduSmart.AuthService.bak
  docker exec postgres pg_dump -U pbl6duter -F c -d "EduSmart.CourseManagementService" > ./backups/EduSmart.CourseManagementService.bak
  docker exec postgres pg_dump -U pbl6duter -F c -d "EduSmart.PaymentManagementService" > ./backups/EduSmart.PaymentManagementService.bak
  docker exec postgres pg_dump -U pbl6duter -F c -d "EduSmart.UserService" > ./backups/EduSmart.UserService.bak
  ```

### 2. Database Restore

* **Restore Full Backup File:**
  ```bash
  docker exec -i postgres psql -U pbl6duter < ./backups/postgres_full_backup_<TIMESTAMP>.bak
  ```

* **Restore Individual Service Database (Custom Format `.bak`):**
  ```bash
  docker exec -i postgres pg_restore -U pbl6duter -d "EduSmart.AuthService" --clean < ./backups/EduSmart.AuthService.bak
  ```
