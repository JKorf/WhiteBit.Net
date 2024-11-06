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
    internal class WhiteBitSpotBalanceSubscription : Subscription<WhiteBitSocketResponse<WhiteBitSubscribeResponse>, WhiteBitSocketResponse<WhiteBitSubscribeResponse>>
    {
        /// <inheritdoc />
        public override HashSet<string> ListenerIdentifiers { get; set; }

        private readonly Action<DataEvent<Dictionary<string, WhiteBitTradeBalance>>> _handler;

        private IEnumerable<string> _symbols;

        /// <inheritdoc />
        public override Type? GetMessageType(IMessageAccessor message)
        {
            return typeof(WhiteBitSocketUpdate<IEnumerable<Dictionary<string, WhiteBitTradeBalance>>>);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitSpotBalanceSubscription(ILogger logger, IEnumerable<string> symbols, Action<DataEvent<Dictionary<string, WhiteBitTradeBalance>>> handler) : base(logger, true)
        {
            _handler = handler;
            _symbols = symbols;
            ListenerIdentifiers =  new HashSet<string> { "balanceSpot_update" };
        }

        /// <inheritdoc />
        public override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "balanceSpot_subscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        public override Query? GetUnsubQuery()
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "balanceSpot_unsubscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            var data = (WhiteBitSocketUpdate<IEnumerable<Dictionary<string, WhiteBitTradeBalance>>>)message.Data;

            _handler.Invoke(message.As(data.Data.First(), data.Method, null, SocketUpdateType.Update)!);
            return new CallResult(null);
        }
    }
}
