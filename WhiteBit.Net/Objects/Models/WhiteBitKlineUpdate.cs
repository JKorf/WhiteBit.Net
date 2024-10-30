﻿using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Kline update
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public record WhiteBitKlineUpdate
    {
        /// <summary>
        /// Open timestamp
        /// </summary>
        [ArrayProperty(0)]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime OpenTime { get; set; }
        /// <summary>
        /// Open price
        /// </summary>
        [ArrayProperty(1)]
        public decimal OpenPrice { get; set; }
        /// <summary>
        /// Close price
        /// </summary>
        [ArrayProperty(2)]
        public decimal ClosePrice { get; set; }
        /// <summary>
        /// High price
        /// </summary>
        [ArrayProperty(3)]
        public decimal HighPrice { get; set; }
        /// <summary>
        /// Low price
        /// </summary>
        [ArrayProperty(4)]
        public decimal LowPrice { get; set; }
        /// <summary>
        /// Volume in base asset
        /// </summary>
        [ArrayProperty(5)]
        public decimal Volume { get; set; }
        /// <summary>
        /// Volume in quote asset
        /// </summary>
        [ArrayProperty(6)]
        public decimal QuoteVolume { get; set; }
        /// <summary>
        /// Symbol name
        /// </summary>
        [ArrayProperty(7)]
        public string Symbol { get; set; } = string.Empty;
    }


}
