using System.Collections.Concurrent;

namespace AdService.Services
{
    public class AdStorage
    {
        // Ключ = локация (например "/ru/svrd/revda")
        // Значение = список площадок, действующих в этой локации
        private readonly ConcurrentDictionary<string, HashSet<string>> _ads =
            new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Полностью очищает и перезаписывает данные.
        /// </summary>
        public void ReplaceAll(Dictionary<string, List<string>> newData)
        {
            _ads.Clear();
            foreach (var kvp in newData)
            {
                _ads[kvp.Key] = new HashSet<string>(kvp.Value);
            }
        }

        /// <summary>
        /// Возвращает площадки для конкретной локации.
        /// </summary>
        public IEnumerable<string> GetForLocation(string location)
        {
            var result = new HashSet<string>();

            // идём снизу вверх по дереву локаций
            var current = location;
            while (!string.IsNullOrEmpty(current))
            {
                if (_ads.TryGetValue(current, out var platforms))
                {
                    foreach (var p in platforms)
                        result.Add(p);
                }

                // поднимаемся на уровень выше
                var lastSlash = current.LastIndexOf('/');
                if (lastSlash <= 0) break; // дошли до корня
                current = current[..lastSlash];
            }

            return result;
        }
    }
}
