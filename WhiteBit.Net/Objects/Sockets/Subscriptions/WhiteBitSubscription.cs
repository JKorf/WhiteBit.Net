using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WhiteBit.Net.Objects.Internal;
using WhiteBit.Net.Objects.Models;

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

            if (symbols == null)
                MessageMatcher = MessageMatcher.Create<WhiteBitSocketUpdate<T>>(MessageLinkType.Full, $"{topic}_update", DoHandleMessage);
            else if (symbols.Length == 0)
                MessageMatcher = MessageMatcher.Create<WhiteBitSocketUpdate<T>>(MessageLinkType.StartsWith, $"{topic}_update", DoHandleMessage);
            else
                MessageMatcher = MessageMatcher.Create(symbols.Select(x => new MessageHandlerLink<WhiteBitSocketUpdate<T>>(MessageLinkType.Full, $"{topic}_update.{x}", DoHandleMessage)).ToArray());

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
            //_handler.Invoke(message.As(message.Data.Data, message.Data.Method, null, (_firstUpdateSnapshot && ConnectionInvocations == 1) ? SocketUpdateType.Snapshot : SocketUpdateType.Update)!);
            return CallResult.SuccessResult;
        }
    }
}
