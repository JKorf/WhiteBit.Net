using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// System status
    /// </summary>
    public record WhiteBitSystemStatus
    {
        /// <summary>
        /// Status of the platform
        /// </summary>
        [JsonPropertyName("status")]
        public SystemStatus? SystemStatus { get; set; }
    }


}
