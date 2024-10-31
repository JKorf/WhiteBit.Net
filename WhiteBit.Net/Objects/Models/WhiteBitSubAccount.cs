using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Sub account info
    /// </summary>
    public record WhiteBitSubAccount
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Alias
        /// </summary>
        [JsonPropertyName("alias")]
        public string Alias { get; set; } = string.Empty;
        /// <summary>
        /// User id
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;
        /// <summary>
        /// Email
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Status
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// Color
        /// </summary>
        [JsonPropertyName("color")]
        public string Color { get; set; } = string.Empty;
        /// <summary>
        /// Kyc
        /// </summary>
        [JsonPropertyName("kyc")]
        public WhiteBitSubAccountKyc Kyc { get; set; } = null!;
        /// <summary>
        /// Permissions
        /// </summary>
        [JsonPropertyName("permissions")]
        public WhiteBitSubAccountPermissions Permissions { get; set; } = null!;
    }

    /// <summary>
    /// Kyc info
    /// </summary>
    public record WhiteBitSubAccountKyc
    {
        /// <summary>
        /// Share kyc
        /// </summary>
        [JsonPropertyName("shareKyc")]
        public bool ShareKyc { get; set; }
        /// <summary>
        /// Kyc status
        /// </summary>
        [JsonPropertyName("kycStatus")]
        public string KycStatus { get; set; } = string.Empty;
    }

    /// <summary>
    /// Permissions
    /// </summary>
    public record WhiteBitSubAccountPermissions
    {
        /// <summary>
        /// Spot enabled
        /// </summary>
        [JsonPropertyName("spotEnabled")]
        public bool SpotEnabled { get; set; }
        /// <summary>
        /// Collateral enabled
        /// </summary>
        [JsonPropertyName("collateralEnabled")]
        public bool CollateralEnabled { get; set; }
    }


}
