using System;
using CryptoExchange.Net.Interfaces;

namespace WhiteBit.Net.Objects.Internal
{
    internal class WhiteBitNonceProvider : INonceProvider
    {
        private readonly static object _nonceLock = new object();
        private static long? _lastNonce;

        /// <inheritdoc />
        public long GetNonce()
        {
            lock (_nonceLock)
            {
                var nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (_lastNonce.HasValue && nonce <= _lastNonce.Value)
                    nonce = _lastNonce.Value + 1;
                _lastNonce = nonce;
                return nonce;
            }
        }
    }
}
