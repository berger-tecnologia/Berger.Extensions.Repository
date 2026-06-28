using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Berger.Extensions.Repository;

public sealed class JsonValueConverter<T> : ValueConverter<T?, string?>
{
    public JsonValueConverter(JsonSerializerOptions? options = null)
        : base(
            value => value == null ? null : JsonSerializer.Serialize(value, options),
            value => string.IsNullOrWhiteSpace(value) ? default : JsonSerializer.Deserialize<T>(value, options))
    { }
}

public sealed class StringListConverter : ValueConverter<List<string>, string>
{
    public StringListConverter()
        : base(
            value => string.Join(",", value),
            value => value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList())
    { }
}
