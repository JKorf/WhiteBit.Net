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
    internal class WhiteBitKlineSubscription : Subscription<WhiteBitSocketResponse<WhiteBitSubscribeResponse>, WhiteBitSocketResponse<WhiteBitSubscribeResponse>>
    {
        /// <inheritdoc />
        public override HashSet<string> ListenerIdentifiers { get; set; }

        private readonly Action<DataEvent<IEnumerable<WhiteBitKlineUpdate>>> _handler;

        private readonly KlineInterval _interval;
        private string _symbol;

        /// <inheritdoc />
        public override Type? GetMessageType(IMessageAccessor message)
        {
            return typeof(WhiteBitSocketUpdate<IEnumerable<WhiteBitKlineUpdate>>);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitKlineSubscription(ILogger logger, string symbol, KlineInterval interval, Action<DataEvent<IEnumerable<WhiteBitKlineUpdate>>> handler) : base(logger, false)
        {
            _handler = handler;
            _symbol = symbol;
            _interval = interval;
            Topic = "Klines";
            ListenerIdentifiers =  new HashSet<string> { "candles_update" + "." + symbol };
        }

        /// <inheritdoc />
        public override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "candles_subscribe",
                Request = [_symbol, (int)_interval]
            }, false);
        }

        /// <inheritdoc />
        public override Query? GetUnsubQuery()
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "candles_unsubscribe",
                Request = [_symbol, (int)_interval]
            }, false);
        }

        /// <inheritdoc />
        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            var data = (WhiteBitSocketUpdate<IEnumerable<WhiteBitKlineUpdate>>)message.Data;

            _handler.Invoke(message.As(data.Data, data.Method, data.Data.First().Symbol, SocketUpdateType.Update)!);
            return new CallResult(null);
        }
    }
}
