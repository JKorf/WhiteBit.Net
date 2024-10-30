using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Balance
    /// </summary>
    public record WhiteBitMainBalance
    {
        /// <summary>
        /// Asset
        /// </summary>
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// Main balance
        /// </summary>
        [JsonPropertyName("main_balance")]
        public decimal MainBalance { get; set; }
    }
}
