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
