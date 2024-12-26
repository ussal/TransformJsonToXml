using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
namespace WinFormsApp1.Helpers
{
    public class SOAPHelper
    {
        public static string TransformJsonToXml(string xmlPath, string jsonPath)
        {
            // XML şablonunu dosyadan yükle
            string xmlTemplate = File.ReadAllText(xmlPath);
            XDocument xdoc = XDocument.Parse(xmlTemplate);

            // JSON verisini dosyadan yükle
            string jsonFileText = File.ReadAllText(jsonPath);
            JObject jsonObject = JObject.Parse(jsonFileText); // JSON'u bir JObject olarak parse et

            // XML'deki tüm yer tutucuları bul (örneğin <title>{{.books[].title}}</title>)
            var placeholders = xdoc.Descendants()
                                   .Where(e => e.HasElements == false && e.Value.Contains("{{.") && e.Value.Contains("}}"))
                                   .ToList();
            List<string> complatedArrays = [];
            foreach (var placeholder in placeholders)
            {
                // Yer tutucudaki anahtarı regex ile ayıklıyoruz: {{.books[].title}} => books[].title
                string jsonKey = ExtractJsonKey(placeholder.Value);

                //Bir Array mi? ve Daha önce tamamlanan dizi mi?
                if (jsonKey.Contains("[]"))
                {

                    //Array Key'i Ayır
                    string arrayKey = jsonKey.Split("[]")[0];

                    //Daha önce tamamlanan Array ise devam et
                    if(complatedArrays.Contains(arrayKey))
                        continue;

                    // Array işlemi için fonksiyonu çağır
                    ProcessArrayKey(arrayKey, jsonObject, placeholder);

                    //Tamamlanan Dizilere Ekle
                    complatedArrays.Add(arrayKey);
                }
                else
                {
                    JToken jsonValue = jsonObject.SelectToken(jsonKey);
                    placeholder.Value = jsonValue.ToString() ?? string.Empty;
                }
            }

            // Güncellenmiş XML'i string olarak döndür
            return xdoc.ToString();
        }

        // Yer tutucudaki JSON anahtarını ayıklamak için regex kullanan metot
        private static string ExtractJsonKey(string placeholderValue)
        {
            // Regex ile [[.books.title]] formatından books.title çıkarmak için
            var match = Regex.Match(placeholderValue, @"\{\{\.(.*?)\}\}");
            if (match.Success)
            {
                return match.Groups[1].Value; // "books.title" gibi döndürür
            }
            throw new InvalidOperationException("Geçersiz yer tutucu formatı.");
        }

        // Yer tutucuları JSON verisiyle değiştiren metot
        private static void ReplacePlaceholder(XElement element, string tagName, string value)
        {
            // XML şablonundaki yer tutucuya göre değiştir
            var placeholder = element.Descendants().Where(e => e.HasElements == false && e.Value.Contains(tagName)).FirstOrDefault();
            if (placeholder != null)
            {
                placeholder.Value = value ?? string.Empty;
            }
        }

        private static void ProcessArrayKey(string arrayKey, JObject jsonObject, XElement placeholder)
        {
            // JSON içinde bu anahtara karşılık gelen bir değer varsa işleme alalım
            JToken arrayJson = jsonObject.SelectToken(arrayKey);

            // JSON'da karşılık gelen değer bir dizi ise işlem yapalım
            if (arrayJson != null && arrayJson is JArray jsonArrayObj)
            {
                XElement itemTemplate = new XElement(placeholder.Parent);

                // Array elemanlarını döngü ile işleyelim
                foreach (var jsonItem in jsonArrayObj)
                {
                    // Yeni öğe oluştur
                    XElement newItem = new XElement(itemTemplate);

                    // JSON öğesindeki her bir property için yer tutucuyu güncelle
                    JObject itemObject = jsonItem as JObject;

                    if (itemObject != null)
                    {
                        foreach (var property in itemObject)
                        {
                            // JSON property adını tagName olarak al
                            string tagName = property.Key;
                            // JSON property değerini al
                            string value = property.Value.ToString();

                            // Yer tutucuyu JSON verisi ile değiştir
                            ReplacePlaceholder(newItem, tagName, value);
                        }
                    }

                    // Yeni öğeyi XML'e ekle
                    placeholder.Parent.Parent.Add(newItem);
                }

                // İlk yer tutucuyu kaldır (array yerine yeni öğeler ekledik)
                placeholder.Parent.Remove();
            }
        }
    }
}