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
    // URL'lerin k���k harfle olu�turulmas�n� sa�lar
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

app.UseStaticFiles(); /*wwwroot dosyalar� kullan�labilir olsun*/
app.UseHttpsRedirection();

//RequestLocalization middleware'i
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    // E�er URL yaln�zca dil kodundan olu�uyorsa ve sonunda `/` varsa, do�rudan bu yola y�nlendir (Home eklenmeden)
    if (new Regex(@"^\/(en|tr|ar)\/?$").IsMatch(path))
    {
        // Yol zaten uygun formattaysa (sonunda / varsa) do�rudan devam et
        if (!path.EndsWith("/"))
        {
            // Sonunda / yoksa ekleyerek y�nlendir
            var newPath = $"{path}/";
            context.Response.Redirect(newPath);
            return;
        }
    }
    // E�er path bo�sa veya sadece `/` ise, varsay�lan dile y�nlendir
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

    // About sayfas� i�in kural
    if (Regex.IsMatch(path, @"^\/(en|tr|ar)\/Home\/About\/?$", RegexOptions.IgnoreCase))
    {
        if (!path.EndsWith("/"))
        {
            var newPath = $"{path}/"; // Yeni URL olu�tur
            context.Response.Redirect(newPath, permanent: false);
            return; // Bu noktada i�lemi sonland�r
        }
    }
    // Service controller'�n�n Index sayfas� i�in kural
    else if (Regex.IsMatch(path, @"^\/(en|tr|ar)\/Service\/MyService\/?$", RegexOptions.IgnoreCase))
    {
        if (!path.EndsWith("/"))
        {
            var newPath = $"{path}/"; // Yeni URL olu�tur
            context.Response.Redirect(newPath, permanent: false);
            return; // Bu noktada i�lemi sonland�r
        }
    }

    // E�er �stteki ko�ullar�n hi�biri sa�lanmazsa, sonraki middleware'e ge�
    await next();
});


app.UseRouting();
// �zel durum kodu sayfalar�n� etkinle�tir
app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{culture=ar}/{controller=Home}/{action=Index}/{id?}");

app.Run();
