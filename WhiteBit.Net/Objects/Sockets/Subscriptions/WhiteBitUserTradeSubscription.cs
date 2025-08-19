using System;
using System.Collections.Generic;
using System.Linq;
using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
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
    internal class WhiteBitUserTradeSubscription : Subscription<WhiteBitSocketResponse<WhiteBitSubscribeResponse>, WhiteBitSocketResponse<WhiteBitSubscribeResponse>>
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
            MessageMatcher = MessageMatcher.Create< WhiteBitSocketUpdate<WhiteBitUserTradeUpdate>>(MessageLinkType.Full, "deals_update", DoHandleMessage);
        }

        /// <inheritdoc />
        public override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "deals_subscribe",
                Request = [_symbols]
            }, false);
        }

        /// <inheritdoc />
        public override Query? GetUnsubQuery()
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "deals_unsubscribe",
                Request = [_symbols]
            }, false);
        }

        /// <inheritdoc />
        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<WhiteBitSocketUpdate<WhiteBitUserTradeUpdate>> message)
        {
            _handler.Invoke(message.As(message.Data.Data, message.Data.Method, message.Data.Data!.Symbol, SocketUpdateType.Update)!);
            return CallResult.SuccessResult;
        }
    }
}
