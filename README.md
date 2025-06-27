# Todo List Dapper

Bir yapılacaklar (Todo) uygulaması. ASP.NET Core MVC, Identity, JWT authentication, Dapper, Entity Framework Core ve drag-and-drop arayüzlü.

## Özellikler
- JWT tabanlı kimlik doğrulama (güvenli giriş/kayıt)
- Kategorilere ayırma ve renkli kategori desteği
- Sürükle-bırak ile görev ve kategori sıralama
- Modal tabanlı düzenleme ve silme
- Batch sıralama güncelleme (drag-drop sonrası toplu güncelleme)
- Tüm veritabanı işlemleri (CRUD + sıralama) **Stored Procedure** ile
- Migration desteği: Veritabanı ve SP'ler otomatik oluşur

## Teknolojiler
- ASP.NET Core 6 MVC
- Entity Framework Core + Dapper
- SQL Server (Migration + Stored Procedure)
- Bootstrap 5, Bootstrap Icons
- JWT Authentication, ASP.NET Identity
- Modern JS (drag.js)

## Kurulum

1. **Projeyi Klonla**
   ```sh
   git clone https://github.com/eniscanyildiz/ToDoListDapper.git
   cd ToDoListDapper
   ```
   Bağımlılıkları Yükle
   ```
   dotnet restore
   ```
   Veritabanı Bağlantını Ayarla
   - `appsettings.json` veya `appsettings.Development.json` dosyasında `DefaultConnection`'ı kendi SQL Server bağlantı stringinle değiştir.

   Migration'ları Uygula
   ```
   dotnet ef database update
   ```

5. **Giriş/Kayıt**
   - Uygulama ilk açıldığında kayıt olabilirsin.

## Kullanım Senaryosu
- Kayıt ol.
- Kategoriler oluştur, renk seç.
- Görevlerini ekle, kategorilere ata.
- Görev ve kategorileri sürükle-bırak ile sırala.
- Görev satırına tıklayarak düzenle veya sil.
- Çıkış yapmak için sağ üstteki butonu kullan.


<img width="2240" alt="Ekran Resmi 2025-06-26 13 16 17" src="https://github.com/user-attachments/assets/69cd8b3e-ba32-47f6-96c2-346a8521e538" />
<img width="1062" alt="Ekran Resmi 2025-06-26 13 16 53" src="https://github.com/user-attachments/assets/129693dd-a8ee-471d-b1da-8539c809e2af" />
<img width="1062" alt="Ekran Resmi 2025-06-26 13 17 00" src="https://github.com/user-attachments/assets/31a7dd20-c2be-4674-89b3-f4ca6e30d947" />
<img width="2240" alt="Ekran Resmi 2025-06-26 13 17 18" src="https://github.com/user-attachments/assets/c2cde210-edfd-47e0-9acd-0f4202e6edf9" />

