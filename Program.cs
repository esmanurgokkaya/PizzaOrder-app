// Program.cs

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PizzaOrderApp.Services; // Servislerinizin bulunduğu ad alanı (Bunu eklemeyi unutmayın!)
using System;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// ... (Mevcut kodlar ve diğer varsayılan servis kayıtları) ...

// HttpClient'ı yapılandırma (Statik JSON dosyalarını okumak için gereklidir)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// 1. PizzaService Kaydı (Singleton - Veri bir kez yüklenir ve uygulama boyunca aynı kalır)
builder.Services.AddSingleton<PizzaService>();

// 2. OrderService Kaydı (Scoped - Durum yönetimi için gereklidir. Oturum boyunca aynı Order nesnesi kullanılır.)
builder.Services.AddScoped<OrderService>();


await builder.Build().RunAsync();