using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using System;
using System.Collections.Generic;
using WhiteBit.Net.Objects.Internal;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Objects.Sockets
{
    internal class WhiteBitQuery<T> : Query<WhiteBitSocketResponse<T>>
    {
        private readonly SocketApiClient _client;

        public WhiteBitQuery(SocketApiClient client, WhiteBitSocketRequest request, bool authenticated, int weight = 1) : base(request, authenticated, weight)
        {
            _client = client;
            MessageMatcher = MessageMatcher.Create<WhiteBitSocketResponse<T>>(MessageLinkType.Full, request.Id.ToString(), HandleMessage);
        }

        public CallResult<WhiteBitSocketResponse<T>> HandleMessage(SocketConnection connection, DataEvent<WhiteBitSocketResponse<T>> message)
        {
            if (message.Data.Error != null)
                return new CallResult<WhiteBitSocketResponse<T>>(new ServerError(message.Data.Error.Code, _client.GetErrorInfo(message.Data.Error.Code, message.Data.Error.Message)));

            return message.ToCallResult();
        }
    }
}
