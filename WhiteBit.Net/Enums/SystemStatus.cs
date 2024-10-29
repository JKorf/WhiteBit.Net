using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Status of the system
    /// </summary>
    public enum SystemStatus
    {
        /// <summary>
        /// System operational
        /// </summary>
        [Map("1")]
        Operational,
        /// <summary>
        /// System maintenance
        /// </summary>
        [Map("0")]
        Maintenance
    }
}
