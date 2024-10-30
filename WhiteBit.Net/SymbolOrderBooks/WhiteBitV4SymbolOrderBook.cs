using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.OrderBook;
using Microsoft.Extensions.Logging;
using WhiteBit.Net.Clients;
using WhiteBit.Net.Interfaces.Clients;
using WhiteBit.Net.Objects.Models;
using WhiteBit.Net.Objects.Options;

namespace WhiteBit.Net.SymbolOrderBooks
{
    /// <summary>
    /// Implementation for a synchronized order book. After calling Start the order book will sync itself and keep up to date with new data. It will automatically try to reconnect and resync in case of a lost/interrupted connection.
    /// Make sure to check the State property to see if the order book is synced.
    /// </summary>
    public class WhiteBitV4SymbolOrderBook : SymbolOrderBook
    {
        private readonly bool _clientOwner;
        private readonly IWhiteBitRestClient _restClient;
        private readonly IWhiteBitSocketClient _socketClient;
        private readonly TimeSpan _initialDataTimeout;

        /// <summary>
        /// Create a new order book instance
        /// </summary>
        /// <param name="symbol">The symbol the order book is for</param>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public WhiteBitV4SymbolOrderBook(string symbol, Action<WhiteBitOrderBookOptions>? optionsDelegate = null)
            : this(symbol, optionsDelegate, null, null, null)
        {
            _clientOwner = true;
        }

        /// <summary>
        /// Create a new order book instance
        /// </summary>
        /// <param name="symbol">The symbol the order book is for</param>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        /// <param name="logger">Logger</param>
        /// <param name="restClient">Rest client instance</param>
        /// <param name="socketClient">Socket client instance</param>
        public WhiteBitV4SymbolOrderBook(
            string symbol,
            Action<WhiteBitOrderBookOptions>? optionsDelegate,
            ILoggerFactory? logger,
            IWhiteBitRestClient? restClient,
            IWhiteBitSocketClient? socketClient) : base(logger, "WhiteBit", "V4", symbol)
        {
            var options = WhiteBitOrderBookOptions.Default.Copy();
            if (optionsDelegate != null)
                optionsDelegate(options);
            Initialize(options);

            _strictLevels = true;
            _sequencesAreConsecutive = options?.Limit == null;

            Levels = options?.Limit ?? 20;
            _initialDataTimeout = options?.InitialDataTimeout ?? TimeSpan.FromSeconds(30);
            _clientOwner = socketClient == null;
            _socketClient = socketClient ?? new WhiteBitSocketClient();
            _restClient = restClient ?? new WhiteBitRestClient();
        }

        /// <inheritdoc />
        protected override async Task<CallResult<UpdateSubscription>> DoStartAsync(CancellationToken ct)
        {
            var result = await _socketClient.V4Api.SubscribeToOrderBookUpdatesAsync(Symbol, Levels!.Value, ProcessUpdate).ConfigureAwait(false);
            if (!result)
                return result;

            if (ct.IsCancellationRequested)
            {
                await result.Data.CloseAsync().ConfigureAwait(false);
                return result.AsError<UpdateSubscription>(new CancellationRequestedError());
            }

            Status = OrderBookStatus.Syncing;

            var setResult = await WaitForSetOrderBookAsync(_initialDataTimeout, ct).ConfigureAwait(false);
            return setResult ? result : new CallResult<UpdateSubscription>(setResult.Error!);
        }


        private void ProcessUpdate(DataEvent<WhiteBitBookUpdate> data)
        {
            if (data.Data.Snapshot)
            {
                SetInitialOrderBook(DateTime.UtcNow.Ticks, data.Data.OrderBook.Bids, data.Data.OrderBook.Asks);
            }
            else
            {
                UpdateOrderBook(DateTime.UtcNow.Ticks, data.Data.OrderBook.Bids, data.Data.OrderBook.Asks);
            }
        }


        /// <inheritdoc />
        protected override void DoReset()
        {
        }

        /// <inheritdoc />
        protected override async Task<CallResult<bool>> DoResyncAsync(CancellationToken ct)
        {
            return await WaitForSetOrderBookAsync(_initialDataTimeout, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (_clientOwner)
            {
                _restClient?.Dispose();
                _socketClient?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
