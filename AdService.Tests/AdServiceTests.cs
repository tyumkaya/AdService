using AdService.Services;
using NUnit.Framework.Legacy;
using System.Text;

namespace AdService.Tests;

[TestFixture]
public class AdServiceTests
{
    private AdStorage _storage;
    private Services.AdService _service;

    [SetUp]
    public void Setup()
    {
        _storage = new AdStorage();
        _service = new Services.AdService(_storage);
    }

    [Test]
    public void Search_ReturnsCorrectPlatforms_ForNestedLocations()
    {
        _storage.ReplaceAll(new Dictionary<string, List<string>>
        {
            ["/ru"] = new List<string> { "Яндекс.Директ" },
            ["/ru/svrd"] = new List<string> { "Крутая реклама" },
            ["/ru/svrd/revda"] = new List<string> { "Ревдинский рабочий" }
        });

        var result = _service.Search("/ru/svrd/revda").ToList();
        CollectionAssert.AreEquivalent(
            new[] { "Яндекс.Директ", "Крутая реклама", "Ревдинский рабочий" },
            result
        );
    }

    [Test]
    public void LoadFromFile_IgnoresEmptyAndInvalidLines()
    {
        string data = @"
Пустая строка

Неправильная строка без двоеточия
Площадка1:/ru/svrd
";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        _service.LoadFromFile(stream);

        var result = _service.Search("/ru/svrd").ToList();
        CollectionAssert.AreEquivalent(new[] { "Площадка1" }, result);
    }

    [Test]
    public void LoadFromFile_DoesNotDuplicatePlatforms()
    {
        string data = "Площадка1:/ru/svrd,/ru/svrd";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        _service.LoadFromFile(stream);

        var result = _service.Search("/ru/svrd").ToList();
        CollectionAssert.AreEquivalent(new[] { "Площадка1" }, result);
    }

    [Test]
    public void Search_ReturnsOnlyRelevantPlatforms_ForParentLocation()
    {
        _storage.ReplaceAll(new Dictionary<string, List<string>>
        {
            ["/ru"] = new List<string> { "Яндекс.Директ" },
            ["/ru/svrd"] = new List<string> { "Крутая реклама" }
        });

        var result = _service.Search("/ru").ToList();
        CollectionAssert.AreEquivalent(new[] { "Яндекс.Директ" }, result);
    }
}
