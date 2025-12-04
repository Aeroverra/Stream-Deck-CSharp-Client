using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;

namespace Aeroverra.StreamDeck.Client.Feedback
{
    [JsonConverter(typeof(FeedbackValueConverter))]
    public abstract class FeedbackValue
    {
    }

    public class FeedbackStringValue : FeedbackValue
    {
        public string Value { get; set; }
    }

    public class FeedbackNumberValue : FeedbackValue
    {
        public double Value { get; set; }
    }

    public class FeedbackRange
    {
        [JsonProperty("max")]
        public double Max { get; set; }

        [JsonProperty("min")]
        public double Min { get; set; }
    }

    public class FeedbackBarValue : FeedbackValue
    {
        [JsonProperty("background")]
        public string? Background { get; set; }

        [JsonProperty("bar_bg_c")]
        public string? BarBgC { get; set; }

        [JsonProperty("bar_border_c")]
        public string? BarBorderC { get; set; }

        [JsonProperty("bar_fill_c")]
        public string? BarFillC { get; set; }

        [JsonProperty("bar_h")]
        public double? BarH { get; set; }

        [JsonProperty("border_w")]
        public double? BorderW { get; set; }

        [JsonProperty("enabled")]
        public bool? Enabled { get; set; }

        [JsonProperty("opacity")]
        public double? Opacity { get; set; }

        [JsonProperty("range")]
        public FeedbackRange? Range { get; set; }

        [JsonProperty("subtype")]
        public int? Subtype { get; set; }

        [JsonProperty("value")]
        public double? Value { get; set; }

        [JsonProperty("zOrder")]
        public int? ZOrder { get; set; }
    }

    public class FeedbackBackgroundValue : FeedbackValue
    {
        [JsonProperty("background")]
        public string? Background { get; set; }

        [JsonProperty("enabled")]
        public bool? Enabled { get; set; }

        [JsonProperty("opacity")]
        public double? Opacity { get; set; }

        [JsonProperty("value")]
        public string? Value { get; set; }

        [JsonProperty("zOrder")]
        public int? ZOrder { get; set; }
    }

    public class FeedbackFont
    {
        [JsonProperty("size")]
        public double? Size { get; set; }

        [JsonProperty("weight")]
        public double? Weight { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FeedbackTextAlignment
    {
        center,
        left,
        right
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FeedbackTextOverflow
    {
        clip,
        ellipsis,
        fade
    }

    public class FeedbackTextBlockValue : FeedbackValue
    {
        [JsonProperty("alignment")]
        public FeedbackTextAlignment? Alignment { get; set; }

        [JsonProperty("background")]
        public string? Background { get; set; }

        [JsonProperty("color")]
        public string? Color { get; set; }

        [JsonProperty("enabled")]
        public bool? Enabled { get; set; }

        [JsonProperty("font")]
        public FeedbackFont? Font { get; set; }

        [JsonProperty("opacity")]
        public double? Opacity { get; set; }

        [JsonProperty("text-overflow")]
        public FeedbackTextOverflow? TextOverflow { get; set; }

        [JsonProperty("value")]
        public string? Value { get; set; }

        [JsonProperty("zOrder")]
        public int? ZOrder { get; set; }
    }

    internal class FeedbackValueConverter : JsonConverter<FeedbackValue>
    {
        public override void WriteJson(JsonWriter writer, FeedbackValue? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            switch (value)
            {
                case FeedbackStringValue s:
                    writer.WriteValue(s.Value);
                    return;
                case FeedbackNumberValue n:
                    writer.WriteValue(n.Value);
                    return;
                default:
                    JToken.FromObject(value, serializer).WriteTo(writer);
                    return;
            }
        }

        public override FeedbackValue? ReadJson(JsonReader reader, Type objectType, FeedbackValue? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException("Deserialization of FeedbackValue is not supported.");
        }
    }
}
