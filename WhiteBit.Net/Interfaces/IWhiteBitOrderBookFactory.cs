using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.SharedApis;
using System;
using WhiteBit.Net.Objects.Options;

namespace WhiteBit.Net.Interfaces
{
    /// <summary>
    /// WhiteBit local order book factory
    /// </summary>
    public interface IWhiteBitOrderBookFactory
    {
        
        /// <summary>
        /// V4 order book factory methods
        /// </summary>
        IOrderBookFactory<WhiteBitOrderBookOptions> V4 { get; }


        /// <summary>
        /// Create a SymbolOrderBook for the symbol
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="options">Book options</param>
        /// <returns></returns>
        ISymbolOrderBook Create(SharedSymbol symbol, Action<WhiteBitOrderBookOptions>? options = null);

        
        /// <summary>
        /// Create a new V4 local order book instance
        /// </summary>
        ISymbolOrderBook CreateV4(string symbol, Action<WhiteBitOrderBookOptions>? options = null);

    }
}