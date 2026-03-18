using CryptoExchange.Net.Authentication;

namespace WhiteBit.Net
{
    /// <summary>
    /// WhiteBit API credentials
    /// </summary>
    public class WhiteBitCredentials : HMACCredential
    {
        /// <summary>
        /// Create new credentials providing only credentials in HMAC format
        /// </summary>
        /// <param name="key">API key</param>
        /// <param name="secret">API secret</param>
        public WhiteBitCredentials(string key, string secret) : base(key, secret)
        {
        }
    }
}
