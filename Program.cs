using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PizzaOrderApp; // App component namespace
using PizzaOrderApp.Services; // Ad alanını kontrol edin
using System;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Root components: mount the app into #app and enable HeadOutlet
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient kaydı
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Servis Kayıtları
// PizzaService depends on HttpClient (scoped), so it must not be registered as a singleton.
builder.Services.AddScoped<PizzaService>(); // Veri tek sefer yüklenecek (scoped to allow HttpClient injection)
builder.Services.AddScoped<OrderService>(); // Sipariş durumu oturum boyunca saklanacak

await builder.Build().RunAsync();