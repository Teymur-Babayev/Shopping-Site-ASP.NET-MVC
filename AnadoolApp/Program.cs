using AnadoolApp.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region Localization
//Step 1
builder.Services.AddSingleton<LanguageService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
            return factory.Create("SharedResource", assemblyName.Name);
        };
    });

builder.Services.Configure<RequestLocalizationOptions>(
    options =>
    {
        var supportedCultures = new List<CultureInfo>
            {
                            new CultureInfo("ar"),
                            new CultureInfo("en"),
                            new CultureInfo("tr")
            };

        options.DefaultRequestCulture = new RequestCulture(culture: "ar", uiCulture: "ar");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;

        options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
    });
#endregion

builder.Services.AddControllersWithViews();

builder.Services.AddRouting(options =>
{
    // URL'lerin küçük harfle oluþturulmasýný saðlar
    options.LowercaseUrls = true;
});

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles(); /*wwwroot dosyalarý kullanýlabilir olsun*/
app.UseHttpsRedirection();

//RequestLocalization middleware'i
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    // Eðer URL yalnýzca dil kodundan oluþuyorsa ve sonunda `/` varsa, doðrudan bu yola yönlendir (Home eklenmeden)
    if (new Regex(@"^\/(en|tr|ar)\/?$").IsMatch(path))
    {
        // Yol zaten uygun formattaysa (sonunda / varsa) doðrudan devam et
        if (!path.EndsWith("/"))
        {
            // Sonunda / yoksa ekleyerek yönlendir
            var newPath = $"{path}/";
            context.Response.Redirect(newPath);
            return;
        }
    }
    // Eðer path boþsa veya sadece `/` ise, varsayýlan dile yönlendir
    else if (string.IsNullOrWhiteSpace(path) || path == "/")
    {
        context.Response.Redirect("/ar/");
        return;
    }

    await next();
});

// RequestLocalization middleware'ini kullan
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    // About sayfasý için kural
    if (Regex.IsMatch(path, @"^\/(en|tr|ar)\/Home\/About\/?$", RegexOptions.IgnoreCase))
    {
        if (!path.EndsWith("/"))
        {
            var newPath = $"{path}/"; // Yeni URL oluþtur
            context.Response.Redirect(newPath, permanent: false);
            return; // Bu noktada iþlemi sonlandýr
        }
    }
    // Service controller'ýnýn Index sayfasý için kural
    else if (Regex.IsMatch(path, @"^\/(en|tr|ar)\/Service\/MyService\/?$", RegexOptions.IgnoreCase))
    {
        if (!path.EndsWith("/"))
        {
            var newPath = $"{path}/"; // Yeni URL oluþtur
            context.Response.Redirect(newPath, permanent: false);
            return; // Bu noktada iþlemi sonlandýr
        }
    }

    // Eðer üstteki koþullarýn hiçbiri saðlanmazsa, sonraki middleware'e geç
    await next();
});


app.UseRouting();
// Özel durum kodu sayfalarýný etkinleþtir
app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{culture=ar}/{controller=Home}/{action=Index}/{id?}");

app.Run();
