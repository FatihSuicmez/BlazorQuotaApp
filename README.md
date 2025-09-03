# Blazor Quota App - Sorgu Limiti Uygulaması

Bu proje, bir teknik değerlendirme kapsamında geliştirilmiş, Blazor Server tabanlı tek sayfalık bir web uygulamasıdır (Single Page Application - SPA). Uygulama, kullanıcıların sisteme kaydolup giriş yaparak, belirli kurallar çerçevesinde (günlük 5, aylık 20) veri sorgulamasına olanak tanır. Limitler aşıldığında, arayüz kullanıcıyı bilgilendirir ve API katmanı standart HTTP 429 hata kodunu döndürür.

[Türkçe](#türkçe) | [English](#english)

---

<a name="türkçe"></a>
##  Türkçe Açıklama

<details>
<summary>Detayları görmek için tıklayın</summary>

### 🤖 Örnek Kullanım / Demo

![Blazor Quota App Demo](images/blazor.gif)

---

### 🚀 Proje Hakkında

Bu sistem, ASP.NET Core Identity altyapısını kullanarak güvenli bir kullanıcı kimlik doğrulama sistemi sunar. Giriş yapan kullanıcılar, tek bir arayüz üzerinden sorgulama yapabilirler. Her sorgu, kullanıcının kotasından bir hak düşürür. `QuotaService` adında merkezi bir servis, tüm iş mantığını (zaman dilimi yönetimi, limit hesaplama, veritabanı işlemleri) yönetir. Mimari, hem Blazor Server arayüzünün doğrudan bu servisi kullanmasına olanak tanır, hem de dış sistemlerin entegrasyonu için standart HTTP API uç noktaları sunar.

### 🏛️ Mimarî Şeması

Proje, sorumlulukların ayrılması (Separation of Concerns) ilkesine uygun olarak katmanlı bir yapıda tasarlanmıştır.

- **Sunum Katmanı (UI):** Blazor Server (`Home.razor`)
- **API Katmanı:** Minimal API (`SearchApiEndpoints.cs`)
- **İş Mantığı Katmanı:** `QuotaService`
- **Veri Erişim Katmanı:** Entity Framework Core (`ApplicationDbContext`)
- **Veritabanı:** SQLite (`app.db`)

Bu yapı, projenin test edilebilirliğini ve bakımını kolaylaştırır.

### ✨ Temel Özellikler

* **Gerçek Zamanlı Arayüz:** Blazor Server ve SignalR sayesinde, yapılan her sorgu sonrası sayaçlar ve ilerleme çubukları **sayfa yenilenmeden** anında güncellenir.
* **Güvenli Kimlik Doğrulama:** Endüstri standardı olan ASP.NET Core Identity ile kullanıcı kaydı ve girişi sağlanır. API uç noktaları yetkisiz erişime karşı korunmaktadır.
* **Dinamik Limit Kontrolü:** Kullanım limitleri, İstanbul (UTC+3) yerel saat dilimine göre anlık olarak hesaplanır. Günlük ve aylık pencereler, her sorguda dinamik olarak belirlenir.
* **Atomik Veritabanı İşlemleri:** "Yarış Koşulu" (Race Condition) riskini ortadan kaldırmak için, her sorgu hakkı tüketimi veritabanı **Transaction**'ları içinde güvenli bir şekilde gerçekleştirilir.
* **Proaktif Kullanıcı Bilgilendirme:** Limit dolduğunda, kullanıcıya anında görsel bir uyarı gösterilir ve "Sorgula" butonu, gereksiz sunucu isteği yapılmasını önlemek için otomatik olarak devre dışı bırakılır.
* **Standart API Uç Noktaları:** Harici sistemlerin entegrasyonu için belgelenmiş, standart `GET /api/usage` ve `POST /api/search` uç noktaları mevcuttur. Limit aşımında standart `HTTP 429` kodu ve önerilen `X-RateLimit-*` başlıkları ile yanıt verilir.

### 🛠️ Kullanılan Teknolojiler

* **Backend Framework:** .NET 9
* **Dil:** C#
* **Arayüz Teknolojisi:** Blazor Server
* **API Teknolojisi:** ASP.NET Core Minimal API
* **Veri Erişimi (ORM):** Entity Framework Core
* **Veritabanı:** SQLite
* **Kimlik Doğrulama:** ASP.NET Core Identity

### 📂 Proje Yapısı

```
QuotaApp/
│
├── Components/
│   └── Pages/
│       └── Home.razor           # Ana kullanıcı arayüzü
│
├── Data/
│   ├── ApplicationDbContext.cs  # EF Core veritabanı bağlamı
│   ├── ApplicationUser.cs     # Identity kullanıcı modeli
│   └── QueryLog.cs            # Sorgu kayıtları modeli
│
├── Endpoints/
│   └── SearchApiEndpoints.cs    # HTTP API uç noktaları
│
├── Migrations/
│   └── ...                      # Veritabanı şema geçmişi
│
├── Models/
│   └── ApiContractModels.cs     # Veri taşıma nesneleri (DTOs)
│
├── Services/
│   ├── IQuotaService.cs         # Servis arayüzü
│   └── QuotaService.cs          # Limit kontrolü iş mantığı
│
├── wwwroot/                     # Statik dosyalar (CSS, JS)
├── .gitignore                   # Git tarafından takip edilmeyecek dosyalar
├── app.db                       # SQLite veritabanı dosyası
├── Program.cs                   # Uygulama başlangıç ve yapılandırma dosyası
└── README.md                    # Bu dosya
```

### 🏁 Kurulum ve Çalıştırma

#### Adım 1: Projeyi ve Bağımlılıkları Kurma
```bash
# Projeyi klonlayın
git clone https://github.com/FatihSuicmez/BlazorQuotaApp.git
cd BlazorQuotaApp

# Gerekli .NET paketlerini yükleyin
dotnet restore
```

#### Adım 2: Veritabanını Oluşturma

Proje, Entity Framework Core "Code-First" yaklaşımını kullanır. Veritabanını ve tabloları oluşturmak için aşağıdaki komutu çalıştırmanız yeterlidir.

```bash
# Bu komut, proje ana dizininde app.db adında bir SQLite veritabanı dosyası oluşturacaktır.
dotnet ef database update
```


#### Adım 3: Uygulamayı Çalıştırma

Uygulamayı başlatmak için aşağıdaki komutu kullanın:

```bash
dotnet run
```

Terminalde belirtilen **`http://localhost:xxxx`** adresini bir web tarayıcısında açın.


#### Adım 4: Kullanım

* Sitenin sağ üst köşesindeki Register linki ile yeni bir kullanıcı hesabı oluşturun.

* Login linki ile sisteme giriş yapın.

* Ana sayfadaki sorgulama arayüzünü kullanarak limitler dahilinde testlerinizi yapabilirsiniz.

</details>

---

<a name="english"></a>
## English Description

<details>
<summary>Click to see details</summary>

### 🤖 Sample Usage / Demo

![Blazor Quota App Demo](images/blazor.gif)

---

### 🚀 About The Project

This system provides a secure user authentication system using the ASP.NET Core Identity framework. Logged-in users can perform queries through a single interface. Each query consumes one credit from the user's quota. A central service named `QuotaService` manages all business logic (timezone management, limit calculation, database operations). The architecture allows both the Blazor Server UI to use this service directly and also provides standard HTTP API endpoints for external system integrations.

### 🏛️ Architectural Diagram

The project is designed with a layered structure in accordance with the Separation of Concerns principle.

- **Presentation Layer (UI):** Blazor Server (`Home.razor`)
- **API Layer:** Minimal API (`SearchApiEndpoints.cs`)
- **Business Logic Layer:** `QuotaService`
- **Data Access Layer:** Entity Framework Core (`ApplicationDbContext`)
- **Database:** SQLite (`app.db`)

This structure facilitates the project's testability and maintainability.

### ✨ Key Features

* **Real-Time UI:** Thanks to Blazor Server and SignalR, counters and progress bars are updated instantly **without a page refresh** after each query.
* **Secure Authentication:** User registration and login are handled with the industry-standard ASP.NET Core Identity. API endpoints are protected against unauthorized access.
* **Dynamic Limit Control:** Usage limits are calculated on-the-fly based on the Istanbul (UTC+3) local timezone. Daily and monthly windows are determined dynamically with each query.
* **Atomic Database Operations:** To eliminate the risk of "Race Conditions," each query credit consumption is securely performed within database **Transactions**.
* **Proactive User Feedback:** When a limit is reached, a visual alert is instantly shown to the user, and the "Query" button is automatically disabled to prevent unnecessary server requests.
* **Standard API Endpoints:** Documented, standard `GET /api/usage` and `POST /api/search` endpoints are available for external system integration. In case of a limit breach, the system responds with a standard `HTTP 429` code and the recommended `X-RateLimit-*` headers.

### 🛠️ Technologies Used

- **Backend Framework:** .NET 9
- **Language:** C#
- **UI Technology:** Blazor Server
- **API Technology:** ASP.NET Core Minimal API
- **Data Access (ORM):** Entity Framework Core
- **Database:** SQLite
- **Authentication:** ASP.NET Core Identity

### 📂 Project Structure

```
QuotaApp/
│
├── Components/
│   └── Pages/
│       └── Home.razor           # Main user interface
│
├── Data/
│   ├── ApplicationDbContext.cs  # EF Core database context
│   ├── ApplicationUser.cs     # Identity user model
│   └── QueryLog.cs            # Query logs model
│
├── Endpoints/
│   └── SearchApiEndpoints.cs    # HTTP API endpoints
│
├── Migrations/
│   └── ...                    # Database schema history
│
├── Models/
│   └── ApiContractModels.cs     # Data Transfer Objects (DTOs)
│
├── Services/
│   ├── IQuotaService.cs         # Service interface
│   └── QuotaService.cs          # Limit control business logic
│
├── wwwroot/                     # Static files (CSS, JS)
├── .gitignore                   # Files to be ignored by Git
├── app.db                       # SQLite database file
├── Program.cs                   # Application startup and configuration file
└── README.md                    # This file
```

### 🏁 Setup and Running

#### Step 1: Clone the Project and Install Dependencies

```bash
# Clone the project
git clone https://github.com/FatihSuicmez/BlazorQuotaApp.git
cd BlazorQuotaApp

# Install the necessary .NET packages
dotnet restore
```

#### Step 2: Create the Database

The project uses the Entity Framework Core "Code-First" approach. You only need to run the following command to create the database and its tables.

```bash
# This command will create a SQLite database file named app.db in the project's root directory.
dotnet ef database update
```

#### Step 3: Run the Application

Use the following command to start the application:

```bash
dotnet run
```
Open the **`http://localhost:xxxx`** address shown in the terminal in a web browser.

#### Step 4: Usage

* Create a new user account using the `Register` link in the top-right corner of the site.

* Log in to the system with the `Login` link.

* You can perform your tests within the defined limits using the query interface on the main page.
