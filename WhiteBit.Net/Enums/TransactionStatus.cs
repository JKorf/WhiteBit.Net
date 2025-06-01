using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Transaction status
    /// </summary>
    [JsonConverter(typeof(EnumConverter<TransactionStatus>))]
    public enum TransactionStatus
    {
        /// <summary>
        /// Pending
        /// </summary>
        [Map("1", "2", "6", "10", "11", "12", "13", "14", "15", "16", "17")]
        Pending,
        /// <summary>
        /// Success
        /// </summary>
        [Map("3", "7")]
        Success,
        /// <summary>
        /// Canceled
        /// </summary>
        [Map("4", "9")]
        Canceled,
        /// <summary>
        /// Unconfirmed by user
        /// </summary>
        [Map("5")]
        UnconfirmedByUser,
        /// <summary>
        /// Frozen
        /// </summary>
        [Map("31")]
        Frozen,
        /// <summary>
        /// Uncredited
        /// </summary>
        [Map("22")]
        Uncredited,
        /// <summary>
        /// Partial success
        /// </summary>
        [Map("18")]
        PartialSuccess,
        /// <summary>
        /// Awaiting Travel Rule verification
        /// </summary>
        [Map("27")]
        AwaitingVerification,
        /// <summary>
        /// Travel Rule verification in progress
        /// </summary>
        [Map("28")]
        ConfirmationInProgress
    }
}
