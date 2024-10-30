using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Kline interval
    /// </summary>
    public enum KlineInterval
    {
        /// <summary>
        /// One minute
        /// </summary>
        [Map("60")]
        OneMinute = 60,
        /// <summary>
        /// Three minutes
        /// </summary>
        [Map("180")]
        ThreeMinute = 60 * 3,
        /// <summary>
        /// Five minutes
        /// </summary>
        [Map("300")]
        FiveMinutes = 60 * 5,
        /// <summary>
        /// Fifteen minutes
        /// </summary>
        [Map("900")]
        FifteenMinutes = 60 * 15,
        /// <summary>
        /// Thirty minutes
        /// </summary>
        [Map("1800")]
        ThirtyMinutes = 60 * 30,
        /// <summary>
        /// One hour
        /// </summary>
        [Map("3600")]
        OneHour = 60 * 60,
        /// <summary>
        /// Two hours
        /// </summary>
        [Map("7200")]
        TwoHours = 60 * 60 * 2,
        /// <summary>
        /// Four hours
        /// </summary>
        [Map("14400")]
        FourHours = 60 * 60 * 4,
        /// <summary>
        /// Six hours
        /// </summary>
        [Map("21600")]
        SixHours = 60 * 60 * 6,
        /// <summary>
        /// Twelve hours
        /// </summary>
        [Map("43200")]
        TwelveHours = 60 * 60 * 12,
        /// <summary>
        /// One day
        /// </summary>
        [Map("86400")]
        OneDay = 60 * 60 * 24,
        /// <summary>
        /// Three days
        /// </summary>
        [Map("259200")]
        ThreeDays = 60 * 60 * 24 * 3,
        /// <summary>
        /// One week
        /// </summary>
        [Map("604800")]
        OneWeek = 60 * 60 * 24 * 7,
        /// <summary>
        /// One month
        /// </summary>
        [Map("2592000")]
        OneMonth = 60 * 60 * 24 * 30
    }
}
