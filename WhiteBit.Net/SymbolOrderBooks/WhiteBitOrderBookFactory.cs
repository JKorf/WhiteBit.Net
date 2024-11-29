using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using WhiteBit.Net.Interfaces;
using WhiteBit.Net.Interfaces.Clients;
using WhiteBit.Net.Objects.Options;

namespace WhiteBit.Net.SymbolOrderBooks
{
    /// <summary>
    /// WhiteBit order book factory
    /// </summary>
    public class WhiteBitOrderBookFactory : IWhiteBitOrderBookFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceProvider">Service provider for resolving logging and clients</param>
        public WhiteBitOrderBookFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            V4 = new OrderBookFactory<WhiteBitOrderBookOptions>(CreateV4, Create);
        }
                
         /// <inheritdoc />
        public IOrderBookFactory<WhiteBitOrderBookOptions> V4 { get; }

        /// <inheritdoc />
        public ISymbolOrderBook Create(SharedSymbol symbol, Action<WhiteBitOrderBookOptions>? options = null)
        {
            var symbolName = symbol.GetSymbol(WhiteBitExchange.FormatSymbol);
            return CreateV4(symbolName, options);
        }
                
         /// <inheritdoc />
        public ISymbolOrderBook CreateV4(string symbol, Action<WhiteBitOrderBookOptions>? options = null)
            => new WhiteBitV4SymbolOrderBook(symbol, options, 
                                                          _serviceProvider.GetRequiredService<ILoggerFactory>(),
                                                          _serviceProvider.GetRequiredService<IWhiteBitRestClient>(),
                                                          _serviceProvider.GetRequiredService<IWhiteBitSocketClient>());

    }
}
