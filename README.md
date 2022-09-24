# .NET YARP İLE Microservice Gateway

Selamlar, Bu proje de YARP(Reverse Proxy) Teknolojisi ile örnek bir Api Gateway projesi oluşturacağız.

![gateway](https://raw.githubusercontent.com/mehmetsafabenli/YarpSample/master/YARP.Gateway/Resources/reverse-proxy.png)


# Projeler ve Kullanım Amaçları
![Projeler](https://raw.githubusercontent.com/mehmetsafabenli/YarpSample/master/YARP.Gateway/Resources/servisler.png)


## YARP.Gateway Katmanı

Projenin doğrudan Api Gateway Kısmıdır. Bu katmanda Gateway üzerine gelen HTTP İstekleri Reverse Proxy İle Map edilerek ilgili servislere yönlendirilir.

#### Gateway Katman İçeriği
![gatewayinfo](https://raw.githubusercontent.com/mehmetsafabenli/YarpSample/master/YARP.Gateway/Resources/gatewayinfo.png)

 - Extensions : Gateway için gerekli Extensionları yazacağımız yer.
 - yarp.json : YARP için Cluster ve Routes ayarlarını yazacağımız appSettings dosyası.

#### **yarp.json Settings Ayarlaması**
``` c#
{  
  "ReverseProxy": {  
    "Routes": {  
      "Color Service": {  
        "ClusterId": "colorCluster",  
        "Match": {  
          "Path": "/color/{**everything}"  
  }  
      },  
      "Number Service": {  
        "ClusterId": "numberCluster",  
        "Match": {  
          "Path": "/number/{**everything}"  
  }  
      }  
    },  
    "Clusters": {  
      "colorCluster": {  
        "Destinations": {  
          "destination1": {  
            "Address": "https://localhost:5000"  
  }  
        }  
      },  
      "numberCluster": {  
        "Destinations": {  
          "destination1": {  
            "Address": "https://localhost:7000"  
  }  
        }  
      }  
    }  
  }  
}

```



#### **Extension  Class'lar**

#### *YARP için yarp.json Konfigurasyonunun Projeye Eklenmesi*

``` c#
namespace YARP.Gateway.Extensions;  
  
public static class YarpExtensions  
{  
    private const string AppYarpJsonPath = "yarp.json";  
  
    public static IHostBuilder AddYarpJson(  
        this IHostBuilder hostBuilder,  
        bool optional = true,  
        bool reloadOnChange = true,  
        string path = AppYarpJsonPath)  
    {  
        return hostBuilder.ConfigureAppConfiguration((_, builder) =>  
        {  
            builder.AddJsonFile(  
                    path: AppYarpJsonPath,  
                    optional: optional,  
                    reloadOnChange: reloadOnChange  
  )  
                .AddEnvironmentVariables();  
        });  
    }  
}
```
#### *Seri Log un Ayağa Kaldırılması*
``` c#
using Serilog;  
using Serilog.Events;  
  
namespace YARP.Gateway.Extensions;  
  
public static class SerilogConfigurationHelper  
{  
    public static void Configure(string applicationName)  
    {  
        Log.Logger = new LoggerConfiguration()  
#if DEBUG  
            .MinimumLevel.Debug()  
#else  
  .MinimumLevel.Information()  
#endif  
  .MinimumLevel.Override("Microsoft", LogEventLevel.Information)  
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)  
            .Enrich.FromLogContext()  
            .Enrich.WithProperty("Application", $"{applicationName}")  
            .WriteTo.Async(c => c.File("Logs/logs.txt"))  
            .WriteTo.Async(c => c.Console())  
            .CreateLogger();  
    }  
}
```

   

## Program.cs

``` c#
using Serilog;  
using YARP.Gateway.Extensions;  

var builder = WebApplication.CreateBuilder(args);  
  
builder.Host  
  .AddYarpJson()  
    .UseSerilog();  
  
var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;  
  
if (assemblyName != null) SerilogConfigurationHelper.Configure(assemblyName);  
  
builder.Services.AddControllers();  
builder.Services.AddEndpointsApiExplorer();  
builder.Services.AddSwaggerGen();  
builder.Services.AddReverseProxy()  
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));  
  
var app = builder.Build();  
if (app.Environment.IsDevelopment())  
{  
    app.UseSwagger();  
    app.UseSwaggerUI();  
}  
  
app.UseHttpsRedirection();  
app.UseRouting();  
app.UseEndpoints(endpoints => { endpoints.MapReverseProxy(); });  
  
await app.RunAsync();
```

## Services Katmanı
![services](https://raw.githubusercontent.com/mehmetsafabenli/YarpSample/master/YARP.Gateway/Resources/services.png)

 2 Adet örnek servisimiz mevcut. Color servisi ile rastgele bir renk ve ya renkler numara servisi ile rastgele sayı veya sayılar alıyoruz. Örnek olarak Color Servisi yazalım.

**IColorService İnterface'i yazılması.**
``` c#
public interface IColorService  
{  
    Task<ColorEntity> GenerateRandomColor();  
    Task<IEnumerable<ColorEntity>> GenerateRandomColors(int count = 3);  
}
```
**ColorService yazılması ve IColorService interface üzerinden kalıtım alınması.**
``` c#
public class ColorService : IColorService  
{  
    public async Task<ColorEntity> GenerateRandomColor()  
    {  
        var random = new Random();  
        var color = new ColorEntity  
  {  
            Red = random.Next(0, 255),  
            Green = random.Next(0, 255),  
            Blue = random.Next(0, 255),  
            Id = Guid.NewGuid()  
        };  
  
        return color;  
    }  
  
    public async Task<IEnumerable<ColorEntity>> GenerateRandomColors(int count = 3)  
    {  
        var colors = new List<ColorEntity>();  
        for (var i = 0; i < count; i++)  
        {  
            colors.Add(await GenerateRandomColor());  
        }  
  
        return colors;  
    }  
}
```



**Color Microservis Projemizi 5001 Portu üzerinden kaldırılması için gerekli ayarlamanın yapılması.**

 -  Burada ki port ayarlaması tamamen size kalmış bir durum.

``` c#
"profiles": {  
  "Color.Api": {  
    "commandName": "Project",  
    "dotnetRunMessages": true,  
    "launchBrowser": true,  
    "launchUrl": "swagger",  
    "applicationUrl": "https://localhost:5000;http://localhost:5001",  
    "environmentVariables": {  
      "ASPNETCORE_ENVIRONMENT": "Development"  
  }  
```
**Son olarak Controller Sınıfının yazılıp Api Projesinin ayağa kaldırılabilir hale getirilmesi.**


``` c#
[ApiController]  
[Route("color/[controller]")]  
public class HomeController : ControllerBase  
{  
    private readonly IColorService _colorService;  
  
    public HomeController(IColorService colorService)  
    {  
        _colorService = colorService;  
    }  
  
    [HttpGet]  
    [Route("get-random-color")]  
    [ProducesResponseType((int) HttpStatusCode.OK)]  
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]  
    public async Task<ActionResult> GenerateColor()  
    {  
        return Ok(await _colorService.GenerateRandomColor());  
    }  
  
    [HttpGet]  
    [Route("get-random-colors")]  
    [ProducesResponseType((int) HttpStatusCode.OK)]  
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]  
    public async Task<ActionResult> GenerateColors(int count)  
    {  
        return Ok(await _colorService.GenerateRandomColors(count));  
    }  
}

```

## Reverse Proxy Routing

##### 5000 Portundan ayağa kaldırdığımız Color Servisine HTTP GET isteği atarak 5000 Portu üzerinden yanıt alıyoruz.
![colorswagger](https://raw.githubusercontent.com/mehmetsafabenli/YarpSample/master/YARP.Gateway/Resources/colorswagger.png)

Şimdi Proxy Routing aşamasına geçelim.

### Api Gateway Servisimizi Ayağa Kaldıralım.

![swagger](https://raw.githubusercontent.com/mehmetsafabenli/YarpSample/master/YARP.Gateway/Resources/yarpswagger.png)

### Şimdi Aynı isteği 7083 Portu üzerinden ayağa kalkan Gateway Servisimize atalım.

![getchrome](https://raw.githubusercontent.com/mehmetsafabenli/YarpSample/master/YARP.Gateway/Resources/gatewayproxy.png)


![routingconsole](https://raw.githubusercontent.com/mehmetsafabenli/YarpSample/master/YARP.Gateway/Resources/routingconsole.png)

 

> Gördüğünüz Gibi 7083 Portu üzerine gelen istek verdiğimiz konfigurasyonlar neticesinde 5000 portuna yani Color Micro Servisimize Yönlendiriliyor.

