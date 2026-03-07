using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Sub account info
    /// </summary>
    [SerializationModel]
    public record WhiteBitSubAccount
    {
        /// <summary>
        /// ["<c>id</c>"] Id
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>alias</c>"] Alias
        /// </summary>
        [JsonPropertyName("alias")]
        public string Alias { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>userId</c>"] User id
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>email</c>"] Email
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>status</c>"] Status
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>color</c>"] Color
        /// </summary>
        [JsonPropertyName("color")]
        public string Color { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>kyc</c>"] Kyc
        /// </summary>
        [JsonPropertyName("kyc")]
        public WhiteBitSubAccountKyc Kyc { get; set; } = null!;
        /// <summary>
        /// ["<c>permissions</c>"] Permissions
        /// </summary>
        [JsonPropertyName("permissions")]
        public WhiteBitSubAccountPermissions Permissions { get; set; } = null!;
    }

    /// <summary>
    /// Kyc info
    /// </summary>
    [SerializationModel]
    public record WhiteBitSubAccountKyc
    {
        /// <summary>
        /// ["<c>shareKyc</c>"] Share kyc
        /// </summary>
        [JsonPropertyName("shareKyc")]
        public bool ShareKyc { get; set; }
        /// <summary>
        /// ["<c>kycStatus</c>"] Kyc status
        /// </summary>
        [JsonPropertyName("kycStatus")]
        public string KycStatus { get; set; } = string.Empty;
    }

    /// <summary>
    /// Permissions
    /// </summary>
    [SerializationModel]
    public record WhiteBitSubAccountPermissions
    {
        /// <summary>
        /// ["<c>spotEnabled</c>"] Spot enabled
        /// </summary>
        [JsonPropertyName("spotEnabled")]
        public bool SpotEnabled { get; set; }
        /// <summary>
        /// ["<c>collateralEnabled</c>"] Collateral enabled
        /// </summary>
        [JsonPropertyName("collateralEnabled")]
        public bool CollateralEnabled { get; set; }
    }


}
