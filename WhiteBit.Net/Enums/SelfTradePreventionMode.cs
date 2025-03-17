using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Self Trade Prevention mode
    /// </summary>
    [JsonConverter(typeof(EnumConverter<SelfTradePreventionMode>))]
    public enum SelfTradePreventionMode
    {
        /// <summary>
        /// None
        /// </summary>
        [Map("no")]
        None,
        /// <summary>
        /// Cancel both pre-existing and new order
        /// </summary>
        [Map("cancel_both")]
        CancelBoth,
        /// <summary>
        /// Cancel the new order
        /// </summary>
        [Map("cancel_new")]
        CancelNew,
        /// <summary>
        /// Cancel the pre-exisiting order
        /// </summary>
        [Map("cancel_old")]
        CancelOld
    }
}
