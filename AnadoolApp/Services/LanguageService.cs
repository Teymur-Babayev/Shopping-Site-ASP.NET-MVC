using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace AnadoolApp.Services
{
    public class LanguageService
    {
        private readonly IStringLocalizer _localizer;

        public LanguageService(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("SharedResource", assemblyName.Name);
        }

        public LocalizedString Getkey(string key)
        {
            return _localizer[key];
        }

        // Yeni eklenen metod: .txt dosyasından uzun metin okuma
        public string GetTextFromFile(string fileName, CultureInfo cultureInfo)
        {
            // Dosya yolu örneği: Resources/PrivacyPolicy.en.txt
            var filePath = $"Resources/{fileName}.{cultureInfo.Name}.txt";
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }

            return $"File not found: {filePath}";
        }
    }
}


