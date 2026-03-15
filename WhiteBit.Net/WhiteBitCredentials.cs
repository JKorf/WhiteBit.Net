using CryptoExchange.Net.Authentication;
using System;

namespace WhiteBit.Net
{
    /// <summary>
    /// WhiteBit credentials
    /// </summary>
    public class WhiteBitCredentials : ApiCredentials
    {
        /// <summary>
        /// </summary>
        [Obsolete("Parameterless constructor is only for deserialization purposes and should not be used directly. Use parameterized constructor instead.")]
        public WhiteBitCredentials() { }

        /// <summary>
        /// Create credentials using an HMAC key and secret
        /// </summary>
        /// <param name="apiKey">The API key</param>
        /// <param name="secret">The API secret</param>
        public WhiteBitCredentials(string apiKey, string secret) : this(new HMACCredential(apiKey, secret)) { }

        /// <summary>
        /// Create WhiteBit credentials using HMAC credentials
        /// </summary>
        /// <param name="credential">The HMAC credentials</param>
        public WhiteBitCredentials(HMACCredential credential) : base(credential) { }

        /// <inheritdoc />
#pragma warning disable CS0618 // Type or member is obsolete
        public override ApiCredentials Copy() => new WhiteBitCredentials { CredentialPairs = CredentialPairs };
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
