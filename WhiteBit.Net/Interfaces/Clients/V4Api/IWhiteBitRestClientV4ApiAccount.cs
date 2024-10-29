using System.Threading.Tasks;
using System.Threading;
using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Interfaces.Clients.V4Api
{
    /// <summary>
    /// WhiteBit V4 account endpoints. Account endpoints include balance info, withdraw/deposit info and requesting and account settings
    /// </summary>
    public interface IWhiteBitRestClientV4ApiAccount
    {
        /// <summary>
        /// Get main account balances
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#main-balance" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<Dictionary<string, WhiteBitMainBalance>>> GetMainBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Get deposit address
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#main-balance" /></para>
        /// </summary>
        /// <param name="asset">The asset</param>
        /// <param name="network">Network to use</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitDepositAddressInfo>> GetDepositAddressAsync(string asset, string? network = null, CancellationToken ct = default);

    }
}
