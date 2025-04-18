using System.Text.Json;

namespace LabProject.Utilities;

public class JsonExportUtil
{
    private static JsonExportUtil? _instance;
    private static readonly object _lock = new object();

    private JsonExportUtil() {}

    public static JsonExportUtil Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new JsonExportUtil();
                }
            }
            return _instance;
        }
    }

    public string ExportToJson<T>(T data, string[]? selectedColumns = null)
    {
        if (data == null) return "{}";

        if (selectedColumns == null || selectedColumns.Length == 0)
        {
            return JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        // Handle collection of objects
        if (data is IEnumerable<object> collection)
        {
            var filteredData = collection.Select(item =>
            {
                var type = item.GetType();
                var properties = type.GetProperties()
                    .Where(p => selectedColumns.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
                    .ToDictionary(p => p.Name, p => p.GetValue(item));
                return properties;
            });

            return JsonSerializer.Serialize(filteredData, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        // Handle single object
        var obj = data.GetType().GetProperties()
            .Where(p => selectedColumns.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
            .ToDictionary(p => p.Name, p => p.GetValue(data));

        return JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}