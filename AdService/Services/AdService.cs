using System.Text;

namespace AdService.Services
{
    public interface IAdService
    {
        void LoadFromFile(Stream fileStream);
        List<string> Search(string location);
    }

    public class AdService : IAdService
    {
        private readonly AdStorage _storage;

        public AdService(AdStorage storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Загружает рекламные площадки из текстового файла.
        /// Каждая строка формата: Название:локации
        /// </summary>
        public void LoadFromFile(Stream fileStream)
        {
            var newData = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            using var reader = new StreamReader(fileStream, Encoding.UTF8, leaveOpen: true);

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(':', 2);
                if (parts.Length != 2) continue; // некорректная строка

                var platform = parts[0].Trim();
                var locations = parts[1]
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var loc in locations)
                {
                    if (!newData.ContainsKey(loc))
                        newData[loc] = new List<string>();

                    newData[loc].Add(platform);
                }
            }

            _storage.ReplaceAll(newData);
        }

        /// <summary>
        /// Возвращает список площадок для указанной локации.
        /// </summary>
        public List<string> Search(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return new List<string>();

            return _storage.GetForLocation(location).ToList();
        }
    }
}
