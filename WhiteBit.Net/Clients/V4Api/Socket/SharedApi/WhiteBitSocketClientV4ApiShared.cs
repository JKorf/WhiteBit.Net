using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using CryptoExchange.Net.Objects.Sockets;
using System.Linq;
using CryptoExchange.Net.Objects;
using WhiteBit.Net.Enums;
using WhiteBit.Net.Objects.Models;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;

namespace WhiteBit.Net.Clients.V4Api
{
    internal partial class WhiteBitSocketClientV4SharedApi :
        SharedApiBase,
        IWhiteBitSocketClientV4ApiShared,
        IWhiteBitSocketClientV4SharedApi
    {
        private readonly WhiteBitSocketClientV4Api _api;

        private const string _exchange = "WhiteBit";
        private const string _topicSpotId = "WhiteBitSpot";
        private const string _topicFuturesId = "WhiteBitFutures";

        public override SharedClientInfo Discover() => SharedUtils.GetClientInfo(WhiteBitExchange.Metadata, this);

        public WhiteBitSocketClientV4SharedApi(WhiteBitSocketClientV4Api api)
            : base(
                  SharedTransport.Socket,
                  api.Exchange,
                  [TradingMode.Spot, TradingMode.PerpetualLinear],
                  () => api.Authenticated,
                  api.FormatSymbol)
        {
            _api = api;

            SetCapabilities(
                SubscribeBalanceOptions,
                SubscribeBookTickerOptions,
                SubscribeKlineOptions,
                SubscribeTickerOptions,
                SubscribeTradeOptions,
                SubscribeUserTradeOptions,
                SubscribeSpotOrderOptions,
                SubscribeFuturesOrderOptions,
                SubscribePositionOptions
                );
        }

    }
}
