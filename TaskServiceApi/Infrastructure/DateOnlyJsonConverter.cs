using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskServiceApi.Infrastructure
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string Format = "yyyy-MM-dd"; // ISO-like

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (string.IsNullOrEmpty(s))
                return DateOnly.FromDateTime(DateTime.Today);

            return DateOnly.ParseExact(s, Format, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
        }

    }
}
