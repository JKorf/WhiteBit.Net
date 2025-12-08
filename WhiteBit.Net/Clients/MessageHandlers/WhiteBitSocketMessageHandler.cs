using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using CryptoExchange.Net.Converters.SystemTextJson.MessageHandlers;
using System.Linq;
using System.Text.Json;
using WhiteBit.Net.Objects.Internal;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Clients.MessageHandlers
{
    internal class WhiteBitSocketMessageHandler : JsonSocketMessageHandler
    {
        public override JsonSerializerOptions Options { get; } = WhiteBitExchange._serializerContext;

        public WhiteBitSocketMessageHandler()
        {
            AddTopicMapping<WhiteBitSocketUpdate<WhiteBitTradeUpdate>>(x => x.Data!.Symbol);
            AddTopicMapping<WhiteBitSocketUpdate<WhiteBitLastPriceUpdate>>(x => x.Data!.Symbol);
            AddTopicMapping<WhiteBitSocketUpdate<WhiteBitTickerUpdate>>(x => x.Data!.Symbol);
            AddTopicMapping<WhiteBitSocketUpdate<WhiteBitBookTickerUpdate[]>>(x => x.Data!.First().Symbol);
            AddTopicMapping<WhiteBitSocketUpdate<WhiteBitBookUpdate>>(x => x.Data!.Symbol);
            AddTopicMapping<WhiteBitSocketUpdate<WhiteBitClosedOrder[]>>(x => x.Data!.First().Symbol);
            AddTopicMapping<WhiteBitSocketUpdate<WhiteBitKlineUpdate[]>>(x => x.Data!.First().Symbol);
            AddTopicMapping<WhiteBitSocketUpdate<WhiteBitOrderUpdate>>(x => x.Data!.Order.Symbol);
            AddTopicMapping<WhiteBitSocketUpdate<WhiteBitUserTradeUpdate>>(x => x.Data!.Symbol);
        }

        protected override MessageTypeDefinition[] TypeEvaluators { get; } = [ 
            new MessageTypeDefinition {
                Fields = [
                    new PropertyFieldReference("id").WithNotNullContstraint(),
                ],
                TypeIdentifierCallback = x => x.FieldValue("id")!
            },

            new MessageTypeDefinition {
                Fields = [
                    new PropertyFieldReference("method"),
                ],
                TypeIdentifierCallback = x => x.FieldValue("method")!
            },

        ];
    }
}
