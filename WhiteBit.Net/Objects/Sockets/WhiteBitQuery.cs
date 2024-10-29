using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using System.Collections.Generic;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Objects.Sockets
{
    internal class WhiteBitQuery<T> : Query<T>
    {
        public override HashSet<string> ListenerIdentifiers { get; set; }

        public WhiteBitQuery(WhiteBitModel request, bool authenticated, int weight = 1) : base(request, authenticated, weight)
        {
            ListenerIdentifiers = new HashSet<string> { };
        }

        public override CallResult<T> HandleMessage(SocketConnection connection, DataEvent<T> message)
        {
            return message.ToCallResult();
        }
    }
}
