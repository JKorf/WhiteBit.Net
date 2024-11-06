using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WhiteBit.Net.Enums;
using WhiteBit.Net.Objects.Internal;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Objects.Sockets.Subscriptions
{
    /// <inheritdoc />
    internal class WhiteBitBookSubscription : Subscription<WhiteBitSocketResponse<WhiteBitSubscribeResponse>, WhiteBitSocketResponse<WhiteBitSubscribeResponse>>
    {
        /// <inheritdoc />
        public override HashSet<string> ListenerIdentifiers { get; set; }

        private readonly Action<DataEvent<WhiteBitBookUpdate>> _handler;

        private readonly int _depth;
        private string _symbol;

        /// <inheritdoc />
        public override Type? GetMessageType(IMessageAccessor message)
        {
            return typeof(WhiteBitSocketUpdate<WhiteBitBookUpdate>);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitBookSubscription(ILogger logger, string symbol, int depth, Action<DataEvent<WhiteBitBookUpdate>> handler) : base(logger, false)
        {
            _handler = handler;
            _symbol = symbol;
            _depth = depth;
            Topic = "OrderBook";
            ListenerIdentifiers =  new HashSet<string> { "depth_update" + "." + symbol };
        }

        /// <inheritdoc />
        public override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "depth_subscribe",
                Request = [_symbol, _depth, "0", true]
            }, false);
        }

        /// <inheritdoc />
        public override Query? GetUnsubQuery()
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "depth_unsubscribe",
                Request = [_symbol, _depth]
            }, false);
        }

        /// <inheritdoc />
        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            var data = (WhiteBitSocketUpdate<WhiteBitBookUpdate>)message.Data;
            _handler.Invoke(message.As(data.Data, data.Method, data.Data!.Symbol, data.Data.Snapshot ? SocketUpdateType.Snapshot : SocketUpdateType.Update)!);
            return new CallResult(null);
        }
    }
}
