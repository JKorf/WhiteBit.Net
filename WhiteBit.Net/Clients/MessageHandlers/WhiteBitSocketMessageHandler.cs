using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using CryptoExchange.Net.Converters.SystemTextJson;
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

        protected override MessageEvaluator[] TypeEvaluators { get; } = [ 
            new MessageEvaluator {
                Priority = 1,
                Fields = [
                    new PropertyFieldReference("id") { Constraint = x => x != null },
                ],
                IdentifyMessageCallback = x => x.FieldValue("id")!
            },

            new MessageEvaluator {
                Priority = 2,
                Fields = [
                    new PropertyFieldReference("method"),
                ],
                IdentifyMessageCallback = x => x.FieldValue("method")!
            },

        ];
    }
}
