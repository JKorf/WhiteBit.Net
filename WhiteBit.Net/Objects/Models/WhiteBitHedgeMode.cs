using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Hedge mode status
    /// </summary>
    public record WhiteBitHedgeMode
    {
        /// <summary>
        /// In hedge mode
        /// </summary>
        [JsonPropertyName("hedgeMode")]
        public bool HedgeMode { get; set; }
    }


}
