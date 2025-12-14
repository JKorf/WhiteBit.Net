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
    internal class WhiteBitClosedOrderSubscription : Subscription
    {
        private readonly SocketApiClient _client;
        private readonly Action<DataEvent<WhiteBitClosedOrder[]>> _handler;

        private readonly int _orderFilter;
        private string[] _symbols;

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitClosedOrderSubscription(ILogger logger, SocketApiClient client, IEnumerable<string> symbols, int orderFilter, Action<DataEvent<WhiteBitClosedOrder[]>> handler) : base(logger, true)
        {
            _client = client;
            _handler = handler;
            _symbols = symbols.ToArray();
            _orderFilter = orderFilter;
            MessageMatcher = MessageMatcher.Create<WhiteBitSocketUpdate<WhiteBitClosedOrder[]>>(MessageLinkType.Full, "ordersExecuted_update", DoHandleMessage);
            MessageRouter = MessageRouter.CreateWithoutTopicFilter<WhiteBitSocketUpdate<WhiteBitClosedOrder[]>>("depth_update", DoHandleMessage);
            Topic = "ClosedOrder";
        }

        /// <inheritdoc />
        protected override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "ordersExecuted_subscribe",
                Request = [_symbols, _orderFilter]
            }, false);
        }

        /// <inheritdoc />
        protected override Query? GetUnsubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "ordersExecuted_unsubscribe",
                Request = [_symbols, _orderFilter]
            }, false);
        }

        /// <inheritdoc />
        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, WhiteBitSocketUpdate<WhiteBitClosedOrder[]> message)
        {
            _handler.Invoke(
                new DataEvent<WhiteBitClosedOrder[]>(WhiteBitExchange.ExchangeName, message.Data!, receiveTime, originalData)
                    .WithStreamId(message.Method)
                    .WithSymbol(message.Data!.First().Symbol)
                    .WithUpdateType(SocketUpdateType.Update)
                );

            return CallResult.SuccessResult;
        }
    }
}
