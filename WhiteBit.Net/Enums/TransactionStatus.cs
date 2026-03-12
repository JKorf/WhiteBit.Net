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
        /// ["<c>1</c>"] Pending
        /// </summary>
        [Map("1", "2", "6", "10", "11", "12", "13", "14", "15", "16", "17")]
        Pending,
        /// <summary>
        /// ["<c>3</c>"] Success
        /// </summary>
        [Map("3", "7")]
        Success,
        /// <summary>
        /// ["<c>4</c>"] Canceled
        /// </summary>
        [Map("4", "9")]
        Canceled,
        /// <summary>
        /// ["<c>5</c>"] Unconfirmed by user
        /// </summary>
        [Map("5")]
        UnconfirmedByUser,
        /// <summary>
        /// ["<c>31</c>"] Frozen
        /// </summary>
        [Map("31")]
        Frozen,
        /// <summary>
        /// ["<c>22</c>"] Uncredited
        /// </summary>
        [Map("22")]
        Uncredited,
        /// <summary>
        /// ["<c>18</c>"] Partial success
        /// </summary>
        [Map("18")]
        PartialSuccess,
        /// <summary>
        /// ["<c>27</c>"] Awaiting Travel Rule verification
        /// </summary>
        [Map("27")]
        AwaitingVerification,
        /// <summary>
        /// ["<c>28</c>"] Travel Rule verification in progress
        /// </summary>
        [Map("28")]
        ConfirmationInProgress
    }
}
