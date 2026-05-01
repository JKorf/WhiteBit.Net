using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Cancel order request
    /// </summary>
    public record WhiteBitCancelRequest
    {
        /// <summary>
        /// ["<c>market</c>"] Symbol name
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>orderId</c>"] Cancel by order id
        /// </summary>
        [JsonPropertyName("orderId"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? OrderId { get; set; }
        /// <summary>
        /// ["<c>clientOrderId</c>"] Cancel by client order id
        /// </summary>
        [JsonPropertyName("clientOrderId"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ClientOrderId { get; set; }
    }
}
