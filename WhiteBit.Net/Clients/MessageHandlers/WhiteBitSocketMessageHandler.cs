using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Linq;
using System.Text.Json;

namespace WhiteBit.Net.Clients.MessageHandlers
{
    internal class WhiteBitSocketMessageHandler : JsonSocketMessageHandler
    {
        public override JsonSerializerOptions Options { get; } = WhiteBitExchange._serializerContext;

        public WhiteBitSocketMessageHandler()
        {
        }

        protected override MessageEvaluator[] TypeEvaluators { get; } = [ 
            new MessageEvaluator {
                Priority = 1,
                Fields = [
                    new PropertyFieldReference("code"),
                ],
                IdentifyMessageCallback = x => x.FieldValue("code")!
            },

        ];
    }
}
