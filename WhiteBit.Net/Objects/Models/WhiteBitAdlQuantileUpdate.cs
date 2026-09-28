using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// ADL quantile update
    /// </summary>
    public record WhiteBitAdlQuantileUpdate
    {
        /// <summary>
        /// Symbol
        /// </summary>
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Long ADL quantile
        /// </summary>
        public int AdlQuantileLong { get; set; }
        /// <summary>
        /// Short ADL quantile
        /// </summary>
        public int AdlQuantileShort { get; set; }
    }
}
