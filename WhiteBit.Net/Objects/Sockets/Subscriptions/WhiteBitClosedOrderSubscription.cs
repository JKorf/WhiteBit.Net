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
    internal class WhiteBitClosedOrderSubscription : Subscription<WhiteBitSocketResponse<WhiteBitSubscribeResponse>, WhiteBitSocketResponse<WhiteBitSubscribeResponse>>
    {
        /// <inheritdoc />
        public override HashSet<string> ListenerIdentifiers { get; set; }

        private readonly Action<DataEvent<IEnumerable<WhiteBitClosedOrder>>> _handler;

        private readonly int _orderFilter;
        private IEnumerable<string> _symbols;

        /// <inheritdoc />
        public override Type? GetMessageType(IMessageAccessor message)
        {
            return typeof(WhiteBitSocketUpdate<IEnumerable<WhiteBitClosedOrder>>);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitClosedOrderSubscription(ILogger logger, IEnumerable<string> symbols, int orderFilter, Action<DataEvent<IEnumerable<WhiteBitClosedOrder>>> handler) : base(logger, true)
        {
            _handler = handler;
            _symbols = symbols;
            _orderFilter = orderFilter;
            ListenerIdentifiers =  new HashSet<string> { "ordersExecuted_update" };
        }

        /// <inheritdoc />
        public override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "ordersExecuted_subscribe",
                Request = [_symbols, _orderFilter]
            }, false);
        }

        /// <inheritdoc />
        public override Query? GetUnsubQuery()
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "ordersExecuted_unsubscribe",
                Request = [_symbols, _orderFilter]
            }, false);
        }

        /// <inheritdoc />
        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            var data = (WhiteBitSocketUpdate<IEnumerable<WhiteBitClosedOrder>>)message.Data;

            _handler.Invoke(message.As(data.Data, data.Method, data.Data.First().Symbol, SocketUpdateType.Update)!);
            return new CallResult(null);
        }
    }
}
