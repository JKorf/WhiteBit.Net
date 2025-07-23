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
    internal class WhiteBitSpotBalanceSubscription : Subscription<WhiteBitSocketResponse<WhiteBitSubscribeResponse>, WhiteBitSocketResponse<WhiteBitSubscribeResponse>>
    {
        private readonly Action<DataEvent<Dictionary<string, WhiteBitTradeBalance>>> _handler;

        private string[] _symbols;

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitSpotBalanceSubscription(ILogger logger, IEnumerable<string> symbols, Action<DataEvent<Dictionary<string, WhiteBitTradeBalance>>> handler) : base(logger, true)
        {
            _handler = handler;
            _symbols = symbols.ToArray();
            Topic = "SpotBalance";
            MessageMatcher = MessageMatcher.Create<WhiteBitSocketUpdate<Dictionary<string, WhiteBitTradeBalance>[]>>(MessageLinkType.Full, "balanceSpot_update", DoHandleMessage);
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
        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<WhiteBitSocketUpdate<Dictionary<string, WhiteBitTradeBalance>[]>> message)
        {
            var balances = message.Data.Data!.First();
            foreach (var item in balances)
                item.Value.Asset = item.Key;

            _handler.Invoke(message.As(balances, message.Data.Method, null, SocketUpdateType.Update)!);
            return CallResult.SuccessResult;
        }
    }
}
