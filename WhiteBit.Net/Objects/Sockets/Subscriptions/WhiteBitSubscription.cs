using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using WhiteBit.Net.Objects.Internal;

namespace WhiteBit.Net.Objects.Sockets.Subscriptions
{
    /// <inheritdoc />
    internal class WhiteBitSubscription<T> : Subscription
    {
        private readonly SocketApiClient _client;
        private readonly Action<DateTime, string?, int, WhiteBitSocketUpdate<T>> _handler;

        private string _topic;
        private string[]? _symbols;

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitSubscription(ILogger logger, SocketApiClient client, string topic, string[]? symbols, Action<DateTime, string?, int, WhiteBitSocketUpdate<T>> handler, bool auth) : base(logger, auth)
        {
            _client = client;
            _handler = handler;
            _topic = topic;
            _symbols = symbols;
            Topic = topic;

            IndividualSubscriptionCount = symbols?.Length ?? 1;

            MessageRouter = MessageRouter.CreateWithOptionalTopicFilters<WhiteBitSocketUpdate<T>>($"{topic}_update", symbols, DoHandleMessage);
        }

        /// <inheritdoc />
        protected override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = _topic + "_subscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        protected override Query? GetUnsubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(_client, new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = _topic + "_unsubscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, WhiteBitSocketUpdate<T> message)
        {
            _handler.Invoke(receiveTime, originalData, ConnectionInvocations, message);
            return CallResult.SuccessResult;
        }
    }
}
