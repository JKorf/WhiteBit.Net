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
        /// <inheritdoc />
        public override HashSet<string> ListenerIdentifiers { get; set; }

        private readonly Action<DataEvent<T>> _handler;

        private bool _firstUpdateSnapshot;
        private string _topic;
        private string[]? _symbols;

        /// <inheritdoc />
        public override Type? GetMessageType(IMessageAccessor message)
        {
            return typeof(WhiteBitSocketUpdate<T>);
        }

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
            ListenerIdentifiers = symbols?.Any() == true ? new HashSet<string>(symbols.Select(x => topic + "_update." + x)) : new HashSet<string> { topic + "_update" };
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
        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            var data = (WhiteBitSocketUpdate<T>)message.Data;

            _handler.Invoke(message.As(data.Data, data.Method, null, (_firstUpdateSnapshot && ConnectionInvocations == 1) ? SocketUpdateType.Snapshot : SocketUpdateType.Update)!);
            return new CallResult(null);
        }
    }
}
