using System;
using System.Collections.Generic;
using System.Linq;
using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using Microsoft.Extensions.Logging;
using WhiteBit.Net.Objects.Internal;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Objects.Sockets.Subscriptions
{
    /// <inheritdoc />
    internal class WhiteBitOpenOrderSubscription : Subscription
    {
        private readonly SocketApiClient _client;
        private readonly MessagePath _methodPath = MessagePath.Get().Property("method");

        private readonly Action<DataEvent<WhiteBitOrderUpdate>> _handler;
        private readonly Action<DataEvent<WhiteBitOtoOrderUpdate>>? _otoHandler;

        private string[] _symbols;

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitOpenOrderSubscription(ILogger logger, SocketApiClient client, IEnumerable<string> symbols, Action<DataEvent<WhiteBitOrderUpdate>> handler, Action<DataEvent<WhiteBitOtoOrderUpdate>>? otoHandler) : base(logger, true)
        {
            _client = client;
            _handler = handler;
            _otoHandler = otoHandler;
            _symbols = symbols.ToArray();
            Topic = "OpenOrder";

            var checkers = new List<MessageHandlerLink>();
            var routes = new List<MessageRoute>();
            foreach (var symbol in symbols)
            {
                checkers.Add(new MessageHandlerLink<WhiteBitSocketUpdate<WhiteBitOrderUpdate>>(MessageLinkType.Full, "ordersPending_update." + symbol, DoHandleMessage));
                checkers.Add(new MessageHandlerLink<WhiteBitSocketUpdate<WhiteBitOtoOrderUpdate>>(MessageLinkType.Full, "otoOrdersPending_update." + symbol, DoHandleMessage));

                routes.Add(MessageRoute<WhiteBitSocketUpdate<WhiteBitOrderUpdate>>.CreateWithTopicFilter("ordersPending_update", symbol, DoHandleMessage));
                routes.Add(MessageRoute<WhiteBitSocketUpdate<WhiteBitOtoOrderUpdate>>.CreateWithTopicFilter("otoOrdersPending_update", symbol, DoHandleMessage));
            }

            MessageMatcher = MessageMatcher.Create(checkers.ToArray());
            MessageRouter = MessageRouter.Create(routes.ToArray());
        }

        /// <inheritdoc />
        protected override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "ordersPending_subscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        protected override Query? GetUnsubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "ordersPending_unsubscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, WhiteBitSocketUpdate<WhiteBitOtoOrderUpdate> message)
        {
            _otoHandler?.Invoke(
                    new DataEvent<WhiteBitOtoOrderUpdate>(message.Data!, receiveTime, originalData)
                        .WithStreamId(message.Method)
                        .WithSymbol(message.Data!.Order.TriggerOrder?.Symbol)
                        .WithUpdateType(SocketUpdateType.Update)
                );
            return CallResult.SuccessResult;
        }

        /// <inheritdoc />
        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, WhiteBitSocketUpdate<WhiteBitOrderUpdate> message)
        {
            _handler?.Invoke(
                    new DataEvent<WhiteBitOrderUpdate>(message.Data!, receiveTime, originalData)
                        .WithStreamId(message.Method)
                        .WithSymbol(message.Data!.Order.Symbol)
                        .WithUpdateType(SocketUpdateType.Update)
                );
            return CallResult.SuccessResult;
        }
    }
}
