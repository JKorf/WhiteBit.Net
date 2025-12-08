using System;
using System.Collections.Generic;
using System.Linq;
using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
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
    internal class WhiteBitUserTradeSubscription : Subscription
    {
        private readonly SocketApiClient _client;

        private readonly Action<DataEvent<WhiteBitUserTradeUpdate>> _handler;

        private string[] _symbols;

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitUserTradeSubscription(ILogger logger, SocketApiClient client, IEnumerable<string> symbols, Action<DataEvent<WhiteBitUserTradeUpdate>> handler) : base(logger, true)
        {
            _client = client;
            _handler = handler;
            _symbols = symbols.ToArray();
            Topic = "UserTrade";
            MessageMatcher = MessageMatcher.Create<WhiteBitSocketUpdate<WhiteBitUserTradeUpdate>>(MessageLinkType.Full, "deals_update", DoHandleMessage);
            MessageRouter = MessageRouter.CreateWithoutTopicFilter<WhiteBitSocketUpdate<WhiteBitUserTradeUpdate>>("deals_update", DoHandleMessage);
        }

        /// <inheritdoc />
        protected override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "deals_subscribe",
                Request = [_symbols]
            }, false);
        }

        /// <inheritdoc />
        protected override Query? GetUnsubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "deals_unsubscribe",
                Request = [_symbols]
            }, false);
        }

        /// <inheritdoc />
        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, WhiteBitSocketUpdate<WhiteBitUserTradeUpdate> message)
        {
            _handler.Invoke(
                new DataEvent<WhiteBitUserTradeUpdate>(message.Data!, receiveTime, originalData)
                .WithStreamId(message.Method)
                .WithSymbol(message.Data!.Symbol)
                .WithUpdateType(SocketUpdateType.Update)
                );
            //_handler.Invoke(message.As(message.Data.Data, message.Data.Method, message.Data.Data!.Symbol, SocketUpdateType.Update)!);
            return CallResult.SuccessResult;
        }
    }
}
