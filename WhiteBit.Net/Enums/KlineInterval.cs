using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Kline interval
    /// </summary>
    [JsonConverter(typeof(EnumConverter<KlineInterval>))]
    public enum KlineInterval
    {
        /// <summary>
        /// ["<c>60</c>"] One minute
        /// </summary>
        [Map("60")]
        OneMinute = 60,
        /// <summary>
        /// ["<c>180</c>"] Three minutes
        /// </summary>
        [Map("180")]
        ThreeMinutes = 60 * 3,
        /// <summary>
        /// ["<c>300</c>"] Five minutes
        /// </summary>
        [Map("300")]
        FiveMinutes = 60 * 5,
        /// <summary>
        /// ["<c>900</c>"] Fifteen minutes
        /// </summary>
        [Map("900")]
        FifteenMinutes = 60 * 15,
        /// <summary>
        /// ["<c>1800</c>"] Thirty minutes
        /// </summary>
        [Map("1800")]
        ThirtyMinutes = 60 * 30,
        /// <summary>
        /// ["<c>3600</c>"] One hour
        /// </summary>
        [Map("3600")]
        OneHour = 60 * 60,
        /// <summary>
        /// ["<c>7200</c>"] Two hours
        /// </summary>
        [Map("7200")]
        TwoHours = 60 * 60 * 2,
        /// <summary>
        /// ["<c>14400</c>"] Four hours
        /// </summary>
        [Map("14400")]
        FourHours = 60 * 60 * 4,
        /// <summary>
        /// ["<c>21600</c>"] Six hours
        /// </summary>
        [Map("21600")]
        SixHours = 60 * 60 * 6,
        /// <summary>
        /// ["<c>43200</c>"] Twelve hours
        /// </summary>
        [Map("43200")]
        TwelveHours = 60 * 60 * 12,
        /// <summary>
        /// ["<c>86400</c>"] One day
        /// </summary>
        [Map("86400")]
        OneDay = 60 * 60 * 24,
        /// <summary>
        /// ["<c>259200</c>"] Three days
        /// </summary>
        [Map("259200")]
        ThreeDays = 60 * 60 * 24 * 3,
        /// <summary>
        /// ["<c>604800</c>"] One week
        /// </summary>
        [Map("604800")]
        OneWeek = 60 * 60 * 24 * 7,
        /// <summary>
        /// ["<c>2592000</c>"] One month
        /// </summary>
        [Map("2592000")]
        OneMonth = 60 * 60 * 24 * 30
    }
}
