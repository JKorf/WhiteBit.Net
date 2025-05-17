using System;
using System.Collections.Generic;
using System.Linq;
using CryptoExchange.Net;
using CryptoExchange.Net.Converters.MessageParsing;
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
    internal class WhiteBitOpenOrderSubscription : Subscription<WhiteBitSocketResponse<WhiteBitSubscribeResponse>, WhiteBitSocketResponse<WhiteBitSubscribeResponse>>
    {
        private readonly MessagePath _methodPath = MessagePath.Get().Property("method");

        /// <inheritdoc />
        public override HashSet<string> ListenerIdentifiers { get; set; }

        private readonly Action<DataEvent<WhiteBitOrderUpdate>> _handler;
        private readonly Action<DataEvent<WhiteBitOtoOrderUpdate>>? _otoHandler;

        private string[] _symbols;

        /// <inheritdoc />
        public override Type? GetMessageType(IMessageAccessor message)
        {
            if (message.GetValue<string>(_methodPath)!.Equals("ordersPendingUpdate")
                || message.GetValue<string>(_methodPath)!.Equals("ordersPending_update"))
                return typeof(WhiteBitSocketUpdate<WhiteBitOrderUpdate>);

            return typeof(WhiteBitSocketUpdate<WhiteBitOtoOrderUpdate>);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitOpenOrderSubscription(ILogger logger, IEnumerable<string> symbols, Action<DataEvent<WhiteBitOrderUpdate>> handler, Action<DataEvent<WhiteBitOtoOrderUpdate>>? otoHandler) : base(logger, true)
        {
            _handler = handler;
            _otoHandler = otoHandler;
            _symbols = symbols.ToArray();
            ListenerIdentifiers = new HashSet<string>();
            Topic = "OpenOrder";

            foreach (var symbol in symbols)
            {
                ListenerIdentifiers.Add("ordersPending_update." + symbol);
                ListenerIdentifiers.Add("otoOrdersPending_update." + symbol);
            }
        }

        /// <inheritdoc />
        public override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "ordersPending_subscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        public override Query? GetUnsubQuery()
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "ordersPending_unsubscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            if (message.Data is WhiteBitSocketUpdate<WhiteBitOrderUpdate> orderUpdate)
            {
                _handler.Invoke(message.As(orderUpdate.Data, orderUpdate.Method, orderUpdate.Data!.Order.Symbol, SocketUpdateType.Update)!);
            }
            else if (message.Data is WhiteBitSocketUpdate<WhiteBitOtoOrderUpdate> otoOrderUpdate)
            {
                _otoHandler?.Invoke(message.As(otoOrderUpdate.Data, otoOrderUpdate.Method, otoOrderUpdate.Data!.Order.TriggerOrder?.Symbol, SocketUpdateType.Update)!);
            }

            return CallResult.SuccessResult;
        }
    }
}
