using System;
using System.Collections.Generic;
using System.Linq;
using CryptoExchange.Net;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using WhiteBit.Net.Objects.Internal;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Objects.Sockets.Subscriptions
{
    /// <inheritdoc />
    internal class WhiteBitOpenOrderSubscription : Subscription<WhiteBitSocketResponse<WhiteBitSubscribeResponse>, WhiteBitSocketResponse<WhiteBitSubscribeResponse>>
    {
        private readonly MessagePath _methodPath = MessagePath.Get().Property("method");

        private readonly Action<DataEvent<WhiteBitOrderUpdate>> _handler;
        private readonly Action<DataEvent<WhiteBitOtoOrderUpdate>>? _otoHandler;

        private string[] _symbols;

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitOpenOrderSubscription(ILogger logger, IEnumerable<string> symbols, Action<DataEvent<WhiteBitOrderUpdate>> handler, Action<DataEvent<WhiteBitOtoOrderUpdate>>? otoHandler) : base(logger, true)
        {
            _handler = handler;
            _otoHandler = otoHandler;
            _symbols = symbols.ToArray();
            Topic = "OpenOrder";

            var checkers = new List<MessageCheck>();
            foreach (var symbol in symbols)
            {
                checkers.Add(new MessageCheck<WhiteBitSocketUpdate<WhiteBitOrderUpdate>>(MessageIdMatchType.Full, "ordersPending_update." + symbol, DoHandleMessage));
                checkers.Add(new MessageCheck<WhiteBitSocketUpdate<WhiteBitOtoOrderUpdate>>(MessageIdMatchType.Full, "otoOrdersPending_update." + symbol, DoHandleMessage));
            }

            MessageMatcher = MessageMatcher.Create(checkers.ToArray());
        }

        /// <inheritdoc />
        public override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "ordersPending_subscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        public override Query? GetUnsubQuery()
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "ordersPending_unsubscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<WhiteBitSocketUpdate<WhiteBitOtoOrderUpdate>> message)
        {
            _otoHandler?.Invoke(message.As(message.Data.Data, message.Data.Method, message.Data.Data!.Order.TriggerOrder?.Symbol, SocketUpdateType.Update)!);
            return CallResult.SuccessResult;
        }

        /// <inheritdoc />
        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<WhiteBitSocketUpdate<WhiteBitOrderUpdate>> message)
        {
            _handler.Invoke(message.As(message.Data.Data, message.Data.Method, message.Data.Data!.Order.Symbol, SocketUpdateType.Update)!);
            return CallResult.SuccessResult;
        }
    }
}
