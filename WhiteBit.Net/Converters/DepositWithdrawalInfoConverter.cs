using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Converters
{
    internal class DepositWithdrawalInfoConverter : JsonConverter<WhiteBitDepositWithdraw>
    {
#if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL3050:RequiresUnreferencedCode", Justification = "JsonSerializerOptions provided here has TypeInfoResolver set")]
        [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2026:RequiresUnreferencedCode", Justification = "JsonSerializerOptions provided here has TypeInfoResolver set")]
#endif
        public override WhiteBitDepositWithdraw? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var cryptoInfo = new List<WhiteBitDepositWithdrawalCrypto>();
            var fiatInfo = new List<WhiteBitDepositWithdrawalFiat>();

            var doc = JsonDocument.ParseValue(ref reader);

            foreach(var item in doc.RootElement.EnumerateObject())
            {
                if (item.Value.GetProperty("providers").GetArrayLength() > 0)
                    fiatInfo.Add(item.Value.Deserialize<WhiteBitDepositWithdrawalFiat>(options)!);
                else
                    cryptoInfo.Add(item.Value.Deserialize<WhiteBitDepositWithdrawalCrypto>(options)!);
            }

            return new WhiteBitDepositWithdraw
            {
                CryptoInfo = cryptoInfo.ToArray(),
                FiatInfo = fiatInfo.ToArray(),
            };
        }

        public override void Write(Utf8JsonWriter writer, WhiteBitDepositWithdraw value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
