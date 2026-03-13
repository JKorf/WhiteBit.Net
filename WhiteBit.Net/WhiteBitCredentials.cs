using CryptoExchange.Net.Authentication;

namespace WhiteBit.Net
{
    /// <summary>
    /// WhiteBit credentials
    /// </summary>
    public class WhiteBitCredentials : ApiCredentials
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="apiKey">The API key</param>
        /// <param name="secret">The API secret</param>
        public WhiteBitCredentials(string apiKey, string secret) : this(new HMACCredential(apiKey, secret)) { }
       
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="credential">The HMAC credentials</param>
        public WhiteBitCredentials(HMACCredential credential) : base(credential) { }

        /// <inheritdoc />
        public override ApiCredentials Copy() => new WhiteBitCredentials(Hmac!);
    }
}
