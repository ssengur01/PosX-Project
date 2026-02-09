# PosX API Gateway

API Gateway uygulaması YARP (Yet Another Reverse Proxy) kullanarak tüm mikroservislere tek giriş noktası sağlar.

## Özellikler

- **Reverse Proxy**: YARP kullanarak tüm mikroservislere yönlendirme
- **JWT Authentication**: Merkezi authentication doğrulama
- **CORS**: Cross-Origin Resource Sharing desteği
- **Rate Limiting**: Dakikada 100 istek limiti
- **Health Checks**: `/health` endpoint'i ile sağlık kontrolü
- **Authorization Policies**: Route bazlı yetkilendirme

## Port Konfigürasyonu

API Gateway: **http://localhost:5000**

### Mikroservisler
- Identity API: http://localhost:5101
- Products API: http://localhost:5102
- Sales API: http://localhost:5103
- Inventory API: http://localhost:5104
- Customers API: http://localhost:5105
- Employees API: http://localhost:5106
- Reports API: http://localhost:5107

## Route Yapısı

| Route Pattern | Mikroservis | Authentication |
|--------------|-------------|----------------|
| `/api/v1/identity/**` | Identity.API | ❌ Public |
| `/api/v1/products/**` | Products.API | ✅ Required |
| `/api/v1/sales/**` | Sales.API | ✅ Required |
| `/api/v1/inventory/**` | Inventory.API | ✅ Required |
| `/api/v1/customers/**` | Customers.API | ✅ Required |
| `/api/v1/employees/**` | Employees.API | ✅ Required |
| `/api/v1/reports/**` | Reports.API | ✅ Required |
| `/api/v1/dashboard/**` | Reports.API | ✅ Required |

## JWT Konfigürasyonu

JWT ayarları `appsettings.json` dosyasında yapılandırılmıştır:

```json
{
  "Jwt": {
    "Key": "YourSecretKey",
    "Issuer": "PosX.Identity",
    "Audience": "PosX.Clients"
  }
}
```

**Önemli:** Production ortamında JWT Key'i Azure Key Vault veya benzeri güvenli bir yerde saklanmalıdır.

## Rate Limiting

- **Limit**: Dakikada 100 istek
- **Window**: 1 dakika (Fixed Window)
- **Strategy**: IP bazlı rate limiting

## Kullanım

### Uygulama Başlatma

```bash
cd src/ApiGateway/PosX.ApiGateway
dotnet run
```

### Health Check

```bash
curl http://localhost:5000/health
```

### Authentication Gerektiren Endpoint'ler

JWT token ile istek:

```bash
curl -H "Authorization: Bearer YOUR_JWT_TOKEN" http://localhost:5000/api/v1/products
```

### Login (Public Endpoint)

```bash
curl -X POST http://localhost:5000/api/v1/identity/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password"}'
```

## Desktop App Entegrasyonu

Desktop app tüm mikroservislere API Gateway üzerinden bağlanır:

```csharp
// Program.cs
var apiGatewayUrl = "http://localhost:5000";

builder.Services.AddHttpClient<AuthApiClient>(client =>
{
    client.BaseAddress = new Uri(apiGatewayUrl);
});
```

## Güvenlik

- ✅ JWT token validation
- ✅ CORS policy
- ✅ Rate limiting
- ✅ HTTPS redirection (production)
- ⚠️ Authentication bypass for Identity endpoints (login/register)

## Monitoring

Health check endpoint'i kullanarak servis sağlığını kontrol edebilirsiniz:

```
GET /health
```

## Geliştirme Notları

- YARP konfigürasyonu `appsettings.json` dosyasındadır
- Route değişiklikleri için uygulama yeniden başlatılmalıdır
- JWT secret production'da mutlaka değiştirilmelidir
