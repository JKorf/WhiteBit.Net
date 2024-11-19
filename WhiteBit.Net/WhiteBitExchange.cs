using CryptoExchange.Net.Objects;
using CryptoExchange.Net.RateLimiting.Filters;
using CryptoExchange.Net.RateLimiting.Guards;
using CryptoExchange.Net.RateLimiting.Interfaces;
using CryptoExchange.Net.RateLimiting;
using System;
using System.Collections.Generic;
using System.Text;
using CryptoExchange.Net.SharedApis;

namespace WhiteBit.Net
{
    /// <summary>
    /// WhiteBit exchange information and configuration
    /// </summary>
    public static class WhiteBitExchange
    {
        /// <summary>
        /// Exchange name
        /// </summary>
        public static string ExchangeName => "WhiteBit";

        /// <summary>
        /// Exchange name
        /// </summary>
        public static string DisplayName => "WhiteBit";

        /// <summary>
        /// Url to exchange image
        /// </summary>
        public static string ImageUrl { get; } = "https://raw.githubusercontent.com/JKorf/WhiteBit.Net/master/WhiteBit.Net/Icon/icon.png";

        /// <summary>
        /// Url to the main website
        /// </summary>
        public static string Url { get; } = "https://www.whitebit.com";

        /// <summary>
        /// Urls to the API documentation
        /// </summary>
        public static string[] ApiDocsUrl { get; } = new[] {
            "https://docs.whitebit.com/"
            };

        /// <summary>
        /// Format a base and quote asset to an WhiteBit recognized symbol 
        /// </summary>
        /// <param name="baseAsset">Base asset</param>
        /// <param name="quoteAsset">Quote asset</param>
        /// <param name="tradingMode">Trading mode</param>
        /// <param name="deliverTime">Delivery time for delivery futures</param>
        /// <returns></returns>
        public static string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null)
        {
            if (tradingMode == TradingMode.Spot)
                return baseAsset + "_" + quoteAsset;

            return baseAsset + "_PERP";
        }

        /// <summary>
        /// Rate limiter configuration for the WhiteBit API
        /// </summary>
        public static WhiteBitRateLimiters RateLimiter { get; } = new WhiteBitRateLimiters();
    }

    /// <summary>
    /// Rate limiter configuration for the WhiteBit API
    /// </summary>
    public class WhiteBitRateLimiters
    {
        /// <summary>
        /// Event for when a rate limit is triggered
        /// </summary>
        public event Action<RateLimitEvent> RateLimitTriggered;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        internal WhiteBitRateLimiters()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Initialize();
        }

        private void Initialize()
        {
            WhiteBit = new RateLimitGate("WhiteBit");
            WhiteBit.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);

            WhiteBitSocket = new RateLimitGate("WhiteBit Socket")
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerConnection, new LimitItemTypeFilter(RateLimitItemType.Request), 200, TimeSpan.FromMinutes(1), RateLimitWindowType.Sliding))
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerHost, new LimitItemTypeFilter(RateLimitItemType.Connection), 1000, TimeSpan.FromMinutes(1), RateLimitWindowType.Sliding));
        }


        internal IRateLimitGate WhiteBit { get; private set; }
        internal IRateLimitGate WhiteBitSocket { get; private set; }

    }
}
