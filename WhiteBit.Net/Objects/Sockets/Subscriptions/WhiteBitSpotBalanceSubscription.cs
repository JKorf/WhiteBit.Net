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
    internal class WhiteBitSpotBalanceSubscription : Subscription
    {
        private readonly SocketApiClient _client;
        private readonly Action<DataEvent<Dictionary<string, WhiteBitTradeBalance>>> _handler;

        private string[] _symbols;

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitSpotBalanceSubscription(ILogger logger, SocketApiClient client, IEnumerable<string> symbols, Action<DataEvent<Dictionary<string, WhiteBitTradeBalance>>> handler) : base(logger, true)
        {
            _client = client;
            _handler = handler;
            _symbols = symbols.ToArray();
            Topic = "SpotBalance";

            MessageMatcher = MessageMatcher.Create<WhiteBitSocketUpdate<Dictionary<string, WhiteBitTradeBalance>[]>>(MessageLinkType.Full, "balanceSpot_update", DoHandleMessage);
            MessageRouter = MessageRouter.CreateWithoutTopicFilter<WhiteBitSocketUpdate<Dictionary<string, WhiteBitTradeBalance>[]>>("balanceSpot_update", DoHandleMessage);
        }

        /// <inheritdoc />
        protected override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "balanceSpot_subscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        protected override Query? GetUnsubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "balanceSpot_unsubscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, WhiteBitSocketUpdate<Dictionary<string, WhiteBitTradeBalance>[]> message)
        {
            var balances = message.Data!.First();
            foreach (var item in balances)
                item.Value.Asset = item.Key;

            _handler.Invoke(
                new DataEvent<Dictionary<string, WhiteBitTradeBalance>>(balances, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update));
            return CallResult.SuccessResult;
        }
    }
}
