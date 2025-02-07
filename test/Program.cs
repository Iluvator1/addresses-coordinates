using System.Text.Json;

// Читаем API-ключ из файла ".env" (формат значений в файле: YANDEX_API_KEY=YUOR_KEY)
string apiKey = File.ReadAllLines(".env")[0].Split('=')[1];

// Запрашиваем у пользователя адрес для получения координат
Console.WriteLine("Введите адрес для получения координат:");
string address = Console.ReadLine();

// Формируем URL запроса к API Яндекс.Карт
string apiUrl = $"https://geocode-maps.yandex.ru/1.x/?apikey={apiKey}&geocode={address.Replace(" ", "+")}&format=json";

// Создаем экземпляр HttpClient для отправки HTTP-запроса
using (HttpClient client = new HttpClient())
{
    try
    {
        // Отправляем GET-запрос к API Яндекс.Карт
        HttpResponseMessage response = await client.GetAsync(apiUrl);

        // Если статус ответа не успешный, генерируется исключение
        response.EnsureSuccessStatusCode();

        // Читаем тело ответа в виде строки
        string responseBody = await response.Content.ReadAsStringAsync();

        // Парсим JSON
        var jsonDoc = JsonDocument.Parse(responseBody);
        var root = jsonDoc.RootElement;

        // Извлекаем координаты из JSON-ответа
        var point = root
            .GetProperty("response")
            .GetProperty("GeoObjectCollection")
            .GetProperty("featureMember")[0]
            .GetProperty("GeoObject")
            .GetProperty("Point")
            .GetProperty("pos")
            .GetString();

        //Выводим в консоль найденные коордлинаты или сообщение об ошибке
        if (point != null)
        {
            string[] coords = point.Split(' ');
            Console.WriteLine($"Координаты: Долгота {coords[0]}, Широта {coords[1]}");
        }
        else
        {
            Console.WriteLine("Координаты не найдены");
        }
    }
    catch (HttpRequestException e)
    {
        // Обрабатываем исключения, связанные с HTTP-запросом
        Console.WriteLine($"Ошибка HTTP запроса: {e.Message}");
    }
    catch (Exception e)
    {
        // Обрабатываем другие исключения
        Console.WriteLine($"Ошибка обработки данных: {e.Message}");
    }
}
