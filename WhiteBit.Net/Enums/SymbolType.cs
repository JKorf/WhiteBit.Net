using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Type of symbol
    /// </summary>
    public enum SymbolType
    {
        /// <summary>
        /// Spot symbol
        /// </summary>
        [Map("spot")]
        Spot,
        /// <summary>
        /// Futures symbol
        /// </summary>
        [Map("futures")]
        Futures
    }
}
