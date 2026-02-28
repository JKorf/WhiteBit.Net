using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using System;
using WhiteBit.Net.Objects.Internal;

namespace WhiteBit.Net.Objects.Sockets
{
    internal class WhiteBitQuery<T> : Query<WhiteBitSocketResponse<T>>
    {
        private readonly SocketApiClient _client;

        public WhiteBitQuery(SocketApiClient client, WhiteBitSocketRequest request, bool authenticated, int weight = 1) : base(request, authenticated, weight)
        {
            _client = client;

            MessageRouter = MessageRouter.CreateWithoutTopicFilter<WhiteBitSocketResponse<T>>(request.Id.ToString(), HandleMessage);
        }

        public CallResult<WhiteBitSocketResponse<T>> HandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, WhiteBitSocketResponse<T> message)
        {
            if (message.Error != null)
                return new CallResult<WhiteBitSocketResponse<T>>(new ServerError(message.Error.Code, _client.GetErrorInfo(message.Error.Code, message.Error.Message)));

            return new CallResult<WhiteBitSocketResponse<T>>(message, originalData, null);
        }
    }
}
