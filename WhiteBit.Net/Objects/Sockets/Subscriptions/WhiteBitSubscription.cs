using CryptoExchange.Net;
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
    internal class WhiteBitSubscription<T> : Subscription<WhiteBitSocketResponse<WhiteBitSubscribeResponse>, WhiteBitSocketResponse<WhiteBitSubscribeResponse>>
    {
        private readonly Action<DataEvent<T>> _handler;

        private bool _firstUpdateSnapshot;
        private string _topic;
        private string[]? _symbols;

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitSubscription(ILogger logger, string topic, string[]? symbols, Action<DataEvent<T>> handler, bool auth, bool firstUpdateSnapshot) : base(logger, auth)
        {
            _handler = handler;
            _topic = topic;
            _firstUpdateSnapshot = firstUpdateSnapshot;
            _symbols = symbols;
            Topic = topic;

            if (symbols == null)
                MessageMatcher = MessageMatcher.Create<WhiteBitSocketUpdate<T>>(MessageIdMatchType.Full, $"{topic}_update", DoHandleMessage);
            else if (symbols.Length == 0)
                MessageMatcher = MessageMatcher.Create<WhiteBitSocketUpdate<T>>(MessageIdMatchType.StartsWith, $"{topic}_update", DoHandleMessage);
            else
                MessageMatcher = MessageMatcher.Create(symbols.Select(x => new MessageCheck<WhiteBitSocketUpdate<T>>(MessageIdMatchType.Full, $"{topic}_update.{x}", DoHandleMessage)).ToArray());
        }

        /// <inheritdoc />
        public override Query? GetSubQuery(SocketConnection connection)
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = _topic + "_subscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        public override Query? GetUnsubQuery()
        {
            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new Internal.WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = _topic + "_unsubscribe",
                Request = _symbols
            }, false);
        }

        /// <inheritdoc />
        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<WhiteBitSocketUpdate<T>> message)
        {
            _handler.Invoke(message.As(message.Data.Data, message.Data.Method, null, (_firstUpdateSnapshot && ConnectionInvocations == 1) ? SocketUpdateType.Snapshot : SocketUpdateType.Update)!);
            return CallResult.SuccessResult;
        }
    }
}
