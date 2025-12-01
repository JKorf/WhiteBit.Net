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
    internal class WhiteBitMarginBalanceSubscription : Subscription<WhiteBitSocketResponse<WhiteBitSubscribeResponse>, WhiteBitSocketResponse<WhiteBitSubscribeResponse>>
    {
        private readonly SocketApiClient _client;
        private readonly Action<DataEvent<WhiteBitMarginBalance[]>> _handler;

        private string[] _symbols;

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitMarginBalanceSubscription(ILogger logger, SocketApiClient client, IEnumerable<string> symbols, Action<DataEvent<WhiteBitMarginBalance[]>> handler) : base(logger, true)
        {
            _client = client;
            _handler = handler;
            _symbols = symbols.ToArray();
            Topic = "MarginBalance";

            MessageMatcher = MessageMatcher.Create<WhiteBitSocketUpdate<WhiteBitMarginBalance[]>>(MessageLinkType.Full, "balanceMargin_update", DoHandleMessage);
            MessageRouter = MessageRouter.CreateWithoutTopicFilter<WhiteBitSocketUpdate<WhiteBitMarginBalance[]>>("balanceMargin_update", DoHandleMessage);
        }

        /// <inheritdoc />
        protected override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "balanceMargin_subscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        protected override Query? GetUnsubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "balanceMargin_unsubscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, WhiteBitSocketUpdate<WhiteBitMarginBalance[]> message)
        {
            _handler.Invoke(
                new DataEvent<WhiteBitMarginBalance[]>(message.Data!, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                );

            return CallResult.SuccessResult;
        }
    }
}
