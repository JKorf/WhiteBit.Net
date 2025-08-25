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
    internal class WhiteBitBookSubscription : Subscription<WhiteBitSocketResponse<WhiteBitSubscribeResponse>, WhiteBitSocketResponse<WhiteBitSubscribeResponse>>
    {
        private readonly SocketApiClient _client;
        private readonly Action<DataEvent<WhiteBitBookUpdate>> _handler;

        private readonly int _depth;
        private string _symbol;

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitBookSubscription(ILogger logger, SocketApiClient client, string symbol, int depth, Action<DataEvent<WhiteBitBookUpdate>> handler) : base(logger, false)
        {
            _client = client;
            _handler = handler;
            _symbol = symbol;
            _depth = depth;
            Topic = "OrderBook";

            MessageMatcher = MessageMatcher.Create<WhiteBitSocketUpdate<WhiteBitBookUpdate>>(MessageLinkType.Full, "depth_update." + symbol, DoHandleMessage);
        }

        /// <inheritdoc />
        protected override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "depth_subscribe",
                Request = [_symbol, _depth, "0", true]
            }, false);
        }

        /// <inheritdoc />
        protected override Query? GetUnsubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "depth_unsubscribe",
                Request = [_symbol, _depth]
            }, false);
        }

        /// <inheritdoc />
        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<WhiteBitSocketUpdate<WhiteBitBookUpdate>> message)
        {
            _handler.Invoke(message.As(message.Data.Data, message.Data.Method, message.Data.Data!.Symbol, message.Data.Data.Snapshot ? SocketUpdateType.Snapshot : SocketUpdateType.Update)!);
            return CallResult.SuccessResult;
        }
    }
}
