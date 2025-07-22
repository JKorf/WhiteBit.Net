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
        private readonly Action<DataEvent<WhiteBitKlineUpdate[]>> _handler;

        private readonly KlineInterval _interval;
        private string _symbol;

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitKlineSubscription(ILogger logger, string symbol, KlineInterval interval, Action<DataEvent<WhiteBitKlineUpdate[]>> handler) : base(logger, false)
        {
            _handler = handler;
            _symbol = symbol;
            _interval = interval;
            Topic = "Klines";
            MessageMatcher = MessageMatcher.Create<WhiteBitSocketUpdate<WhiteBitKlineUpdate[]>>(MessageIdMatchType.Full, "candles_update." + symbol, DoHandleMessage);
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
        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<WhiteBitSocketUpdate<WhiteBitKlineUpdate[]>> message)
        {
            _handler.Invoke(message.As(message.Data.Data, message.Data.Method, message.Data.Data!.First().Symbol, SocketUpdateType.Update)!);
            return CallResult.SuccessResult;
        }
    }
}
