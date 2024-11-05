using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Activation condition
    /// </summary>
    public enum ActivationCondition
    {
        /// <summary>
        /// Price condition higher than x
        /// </summary>
        [Map("gte")]
        GreaterThanOrEqual,
        /// <summary>
        /// Price condition lower than x
        /// </summary>
        [Map("lte")]
        LessThanOrEqu
    }
}
