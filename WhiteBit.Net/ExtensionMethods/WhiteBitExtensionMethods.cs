using CryptoExchange.Net.SharedApis;
using System;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.ExtensionMethods
{
    /// <summary>
    /// Extension methods specific to using the WhiteBit API
    /// </summary>
    public static class WhiteBitExtensionMethods
    {
        /// <summary>
        /// Converts a <see cref="SharedPositionSide"/> to a <see cref="PositionSide"/>.
        /// </summary>
        /// <param name="side">The shared position side to convert.</param>
        /// <returns>The corresponding <see cref="PositionSide"/> value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the provided side is not supported.</exception>
        public static PositionSide ToPositionSide(this SharedPositionSide side)
        {
            return side switch
            {
                SharedPositionSide.Long => PositionSide.Long,
                SharedPositionSide.Short => PositionSide.Short,
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };
        }
    }
}
