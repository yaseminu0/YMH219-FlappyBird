# YMH219 – Neon Flappy Bird

C# ve Windows Forms kullanılarak geliştirilen, Flappy Bird oyunundan esinlenen 2D bir masaüstü arcade oyunudur. Proje, Fırat Üniversitesi Yazılım Mühendisliği bölümü **YMH219 – Nesne Tabanlı Programlama** dersi kapsamında hazırlanmıştır.

Oyuncu, farklı renklerdeki kuşlardan birini seçerek neon ışıklı bir gece şehrinde engellerin arasından geçmeye, puan toplamaya ve uçuş sırasında altın coin'leri yakalamaya çalışır. Oyun ilerledikçe engellerin hızı ve aralarındaki mesafe değişerek zorluk artar.

## İçindekiler

- [Proje Özellikleri](#proje-özellikleri)
- [Ekran Görüntüleri](#ekran-görüntüleri)
- [Kullanılan Teknolojiler](#kullanılan-teknolojiler)
- [Kurulum ve Çalıştırma](#kurulum-ve-çalıştırma)
- [Kontroller](#kontroller)
- [Oyun Mekanikleri](#oyun-mekanikleri)
- [Proje Mimarisi ve Sınıflar](#proje-mimarisi-ve-sınıflar)
- [Oyuncu Verilerinin Saklanması](#oyuncu-verilerinin-saklanması)
- [Proje Yapısı](#proje-yapısı)
- [Geliştirme Notları](#geliştirme-notları)
- [Geliştirici](#geliştirici)

## Proje Özellikleri

- **Neon Night teması:** Yıldızlar, ay, neon ışıklı binalar ve gece atmosferiyle özel çizilmiş oyun arka planı.
- **Animasyonlu kuş:** Kanat animasyonu, uçuş yönüne göre dönüş ve zıplama/dalış hareketleri.
- **Dört kuş seçeneği:** Sarı, mavi, kırmızı ve yeşil kuş türleri.
- **Hareketli engeller:** Rastgele açıklık konumlarına sahip, neon detaylarla çizilmiş boru/engel çiftleri.
- **Çarpışma kontrolü:** Kuşun engellere, tavana veya zemine temas etmesi durumunda oyun sona erer.
- **Puan sistemi:** Her engel çiftinin başarıyla geçilmesi puan kazandırır.
- **Coin sistemi:** Oyun sırasında toplanabilen animasyonlu altın coin'ler.
- **Seviye ve zorluk sistemi:** Puan arttıkça engel hızı, engeller arası mesafe ve geçiş açıklığı kademeli olarak değişir.
- **Oyun durumları:** Ana menü, oyun, duraklatma ve Game Over ekranları.
- **Ayarlar ve talimatlar:** Oyun içi talimat ekranı ve efekt ayarı.
- **Kalıcı oyuncu verileri:** En yüksek puan ve toplam coin bilgileri sonraki oturumlar için yerel olarak saklanır.

## Ekran Görüntüleri

Aşağıdaki alanlara oyunun gerçek ekran görüntüleri eklenecektir.

| Ana Menü | Oyun Ekranı | Game Over |
| :---: | :---: | :---: |
| *Ekran görüntüsü eklenecek* | *Ekran görüntüsü eklenecek* | *Ekran görüntüsü eklenecek* |

## Kullanılan Teknolojiler

| Teknoloji | Kullanım Amacı |
| --- | --- |
| C# | Oyun mantığı ve nesne yönelimli programlama |
| .NET 10 | Uygulamanın hedef çalışma platformu |
| Windows Forms | Masaüstü kullanıcı arayüzü ve pencere yönetimi |
| GDI+ (System.Drawing) | Oyun nesnelerinin ve animasyonların çizimi |
| Visual Studio | Geliştirme, derleme ve hata ayıklama |
| Git & GitHub | Sürüm kontrolü ve kaynak kod yönetimi |

## Kurulum ve Çalıştırma

### Gereksinimler

- Windows 10 veya Windows 11
- .NET 10 SDK
- Visual Studio 2026 (Windows Forms ve .NET masaüstü geliştirme bileşenleri)
- Git (depoyu klonlamak için)

### Depoyu klonlama

PowerShell veya Terminal üzerinden:

```powershell
git clone https://github.com/yaseminu0/YMH219-FlappyBird.git
cd YMH219-FlappyBird
```

### Visual Studio ile çalıştırma

1. Depoyu bilgisayarınıza klonlayın veya ZIP olarak indirin.
2. Proje klasöründeki solution (.sln) dosyasını Visual Studio ile açın.
3. Gerekli .NET SDK bileşenlerinin yüklü olduğunu kontrol edin.
4. **Build > Build Solution** seçeneğiyle projeyi derleyin.
5. **Start** düğmesine veya **F5** tuşuna basarak oyunu başlatın.

## Kontroller

| Tuş / İşlem | Açıklama |
| --- | --- |
| Space veya ↑ | Kuşu yukarı zıplatır. |
| ↓ | Oyun sırasında kuşun daha hızlı aşağı inmesini sağlar (dive). |
| P | Oyunu duraklatır veya devam ettirir. |
| Escape | Açık olan talimat/seçim/ayar ekranından geri döner; oyun duraklatılmışsa devam ettirir. |
| Enter | Game Over ekranında yeni oyunu başlatır. |
| Fare tıklaması | Menü seçeneklerini ve oyun içi butonları kullanır; oyun sırasında kuşu zıplatır. |

## Oyun Mekanikleri

### Uçuş ve hareket

Kuşun hareketi yerçekimi, zıplama kuvveti ve maksimum düşüş hızı kullanılarak hesaplanır. Kanat animasyonu ve kuşun açısı uçuş hareketine göre güncellenir.

### Engeller ve puan

Engeller ekran boyunca hareket eder. Kuş bir engel çiftini geçtikten sonra puan kazanır. Engellere, tavana veya zemine çarpılması oyunu bitirir.

### Seviye sistemi

Zorluk, mevcut puana göre belirlenir. Seviye yükseldikçe engel hızı artırılır, engeller arasındaki yatay mesafe ve geçiş boşluğu azaltılır. Zorluk adları **EASY**, **NORMAL**, **HARD** ve **EXPERT** olarak gösterilir.

### Coin sistemi

Coin'ler engellerin geçiş boşluğunda yer alır. Kuş coin ile temas ettiğinde coin toplanır ve toplam coin sayısı güncellenir.

## Proje Mimarisi ve Sınıflar

Proje, oyun sorumluluklarını farklı sınıflara ayıran nesne yönelimli bir yapıyla geliştirilmiştir.

| Sınıf | Sorumluluk |
| --- | --- |
| `Program` | Uygulamanın başlangıç noktasıdır ve ana Windows Forms penceresini açar. |
| `Form1` | Ana pencereyi, menüleri, kullanıcı girişlerini, oyun döngüsünü ve çizim akışını yönetir. |
| `Bird` | Kuşun türünü, hareketini, zıplamasını, dalışını, animasyonunu ve çizimini yönetir. |
| `Pipe` | Engellerin konumunu, geçiş boşluğunu, hareketini, puan kontrolünü, çarpışmasını ve çizimini yönetir. |
| `Coin` | Coin konumunu, animasyonunu, toplanma kontrolünü ve çizimini yönetir. |
| `GameManager` | Oyun durumunu, puanı, seviyeyi, zorluğu, çarpışmaları ve oyun nesneleri arasındaki koordinasyonu yönetir. |
| `PlayerData` | En yüksek puan ve toplam coin verilerini yerel dosyaya kaydeder ve yükler. |
| `BirdType` | Kuş seçeneklerini `Yellow`, `Blue`, `Red` ve `Green` olarak tanımlar. |

## Oyuncu Verilerinin Saklanması

En yüksek puan ve toplam coin sayısı, Windows kullanıcısının yerel uygulama verileri klasöründe saklanır:

```text
%LOCALAPPDATA%\YMH219_FlappyBird\playerdata.txt
```

Dosyada ilk satır en yüksek puanı, ikinci satır toplam coin sayısını tutar. Oyun ilk kez açıldığında dosyanın henüz bulunmaması normaldir; kayıt gerektiğinde klasör ve dosya oluşturulur. Dosya erişimiyle ilgili bir hata oluşursa oyun çalışmaya devam edecek şekilde tasarlanmıştır.

## Proje Yapısı

Depodaki temel kaynak dosyaları:

```text
YMH219-FlappyBird/
├── Program.cs
├── Form1.cs
├── Form1.Designer.cs
├── Bird.cs
├── Pipe.cs
├── Coin.cs
├── GameManager.cs
├── PlayerData.cs
├── .gitignore
└── README.md
```

## Geliştirme Notları

- Oyun alanı 800 × 550 piksel boyutlarında sabit bir Windows Forms penceresi olarak tasarlanmıştır.
- Oyun çizimleri harici görsel dosyalar yerine C# ve GDI+ kullanılarak oluşturulur.
- Oyun güncelleme döngüsü Windows Forms Timer üzerinden yürütülür.
- Derleme çıktıları (`bin/`, `obj/`) ve Visual Studio kullanıcı dosyaları Git tarafından takip edilmez.

## Geliştirici

**Yasemin Ubeyd**

GitHub: [@yaseminu0](https://github.com/yaseminu0)

Repository: [YMH219-FlappyBird](https://github.com/yaseminu0/YMH219-FlappyBird)

---

Bu proje, YMH219 Nesne Tabanlı Programlama dersi kapsamında eğitim amacıyla geliştirilmiştir.
