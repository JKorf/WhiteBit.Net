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
    internal class WhiteBitUserTradeSubscription : Subscription<WhiteBitSocketResponse<WhiteBitSubscribeResponse>, WhiteBitSocketResponse<WhiteBitSubscribeResponse>>
    {
        /// <inheritdoc />
        public override HashSet<string> ListenerIdentifiers { get; set; }

        private readonly Action<DataEvent<WhiteBitUserTradeUpdate>> _handler;

        private string[] _symbols;

        /// <inheritdoc />
        public override Type? GetMessageType(IMessageAccessor message)
        {
            return typeof(WhiteBitSocketUpdate<WhiteBitUserTradeUpdate>);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitUserTradeSubscription(ILogger logger, IEnumerable<string> symbols, Action<DataEvent<WhiteBitUserTradeUpdate>> handler) : base(logger, true)
        {
            _handler = handler;
            _symbols = symbols.ToArray();
            ListenerIdentifiers = new HashSet<string> { "deals_update" };
            Topic = "UserTrade";
        }

        /// <inheritdoc />
        public override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "deals_subscribe",
                Request = [_symbols]
            }, false);
        }

        /// <inheritdoc />
        public override Query? GetUnsubQuery()
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "deals_unsubscribe",
                Request = [_symbols]
            }, false);
        }

        /// <inheritdoc />
        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            var data = (WhiteBitSocketUpdate<WhiteBitUserTradeUpdate>)message.Data;

            _handler.Invoke(message.As(data.Data, data.Method, data.Data!.Symbol, SocketUpdateType.Update)!);
            return CallResult.SuccessResult;
        }
    }
}
