using System;
using System.Collections.Generic;
using System.Linq;
using CryptoExchange.Net;
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
    internal class WhiteBitClosedOrderSubscription : Subscription<WhiteBitSocketResponse<WhiteBitSubscribeResponse>, WhiteBitSocketResponse<WhiteBitSubscribeResponse>>
    {
        /// <inheritdoc />
        public override HashSet<string> ListenerIdentifiers { get; set; }

        private readonly Action<DataEvent<WhiteBitClosedOrder[]>> _handler;

        private readonly int _orderFilter;
        private string[] _symbols;

        /// <inheritdoc />
        public override Type? GetMessageType(IMessageAccessor message)
        {
            return typeof(WhiteBitSocketUpdate<WhiteBitClosedOrder[]>);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitClosedOrderSubscription(ILogger logger, IEnumerable<string> symbols, int orderFilter, Action<DataEvent<WhiteBitClosedOrder[]>> handler) : base(logger, true)
        {
            _handler = handler;
            _symbols = symbols.ToArray();
            _orderFilter = orderFilter;
            ListenerIdentifiers = new HashSet<string> { "ordersExecuted_update" };
            Topic = "ClosedOrder";
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
            var data = (WhiteBitSocketUpdate<WhiteBitClosedOrder[]>)message.Data;

            _handler.Invoke(message.As(data.Data, data.Method, data.Data!.First().Symbol, SocketUpdateType.Update)!);
            return CallResult.SuccessResult;
        }
    }
}
