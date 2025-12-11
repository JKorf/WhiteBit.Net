using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using Microsoft.Extensions.Logging;
using System;
using WhiteBit.Net.Objects.Internal;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Objects.Sockets.Subscriptions
{
    /// <inheritdoc />
    internal class WhiteBitBookSubscription : Subscription
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
            MessageRouter = MessageRouter.CreateWithTopicFilter<WhiteBitSocketUpdate<WhiteBitBookUpdate>>("depth_update", symbol, DoHandleMessage);
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
                Request = [_symbol]
            }, false);
        }

        /// <inheritdoc />
        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, WhiteBitSocketUpdate<WhiteBitBookUpdate> message)
        {
            _handler.Invoke(
                new DataEvent<WhiteBitBookUpdate>(message.Data!, receiveTime, originalData)
                    .WithStreamId(message.Method)
                    .WithSymbol(message.Data!.Symbol)
                    .WithUpdateType(message.Data!.Snapshot ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                );
            return CallResult.SuccessResult;
        }
    }
}
