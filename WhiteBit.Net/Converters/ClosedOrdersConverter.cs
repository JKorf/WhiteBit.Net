using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using WhiteBit.Net.Objects.Models;
using CryptoExchange.Net.Converters.SystemTextJson;

namespace WhiteBit.Net.Converters
{
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.

    internal class ClosedOrdersConverter : JsonConverter<Dictionary<string, WhiteBitClosedOrder[]>>
    {
        private static readonly JsonSerializerOptions _deserializeOptions = SerializerOptions.WithConverters(new WhiteBitSourceGenerationContext());

        /// <inheritdoc />
        public override Dictionary<string, WhiteBitClosedOrder[]>? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartArray:
                    _ = JsonSerializer.Deserialize<object[]>(ref reader, options);
                    return default;
                case JsonTokenType.StartObject:
                    return JsonSerializer.Deserialize<Dictionary<string, WhiteBitClosedOrder[]>>(ref reader, _deserializeOptions);
            }
            ;

            return default;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Dictionary<string, WhiteBitClosedOrder[]> value, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, (object?)value, options);
    }
}
