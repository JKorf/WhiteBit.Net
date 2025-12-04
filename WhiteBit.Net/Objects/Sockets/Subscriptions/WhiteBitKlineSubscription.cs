using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
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
    internal class WhiteBitKlineSubscription : Subscription
    {
        private readonly SocketApiClient _client;
        private readonly Action<DataEvent<WhiteBitKlineUpdate[]>> _handler;

        private readonly KlineInterval _interval;
        private string _symbol;

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitKlineSubscription(ILogger logger, SocketApiClient client, string symbol, KlineInterval interval, Action<DataEvent<WhiteBitKlineUpdate[]>> handler) : base(logger, false)
        {
            _client = client;
            _handler = handler;
            _symbol = symbol;
            _interval = interval;
            Topic = "Klines";

            MessageMatcher = MessageMatcher.Create<WhiteBitSocketUpdate<WhiteBitKlineUpdate[]>>(MessageLinkType.Full, "candles_update." + symbol, DoHandleMessage);
            MessageRouter = MessageRouter.CreateWithTopicFilter<WhiteBitSocketUpdate<WhiteBitKlineUpdate[]>>("candles_update", symbol, DoHandleMessage);
        }

        /// <inheritdoc />
        protected override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "candles_subscribe",
                Request = [_symbol, (int)_interval]
            }, false);
        }

        /// <inheritdoc />
        protected override Query? GetUnsubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "candles_unsubscribe",
                Request = [_symbol, (int)_interval]
            }, false);
        }

        /// <inheritdoc />
        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, WhiteBitSocketUpdate<WhiteBitKlineUpdate[]> message)
        {
            _handler.Invoke(
                new DataEvent<WhiteBitKlineUpdate[]>(message.Data!, receiveTime, originalData)
                    .WithStreamId(message.Method)
                    .WithSymbol(message.Data!.First().Symbol)
                    .WithUpdateType(SocketUpdateType.Update)
                );

            return CallResult.SuccessResult;
        }
    }
}
