using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Text;
using WhiteBit.Net.Interfaces.Clients.V4Api;

namespace WhiteBit.Net.Clients.V4Api
{
    internal partial class WhiteBitSocketClientV4Api : IWhiteBitSocketClientV4ApiShared
    {
        public string Exchange => "WhiteBit";

        public TradingMode[] SupportedTradingModes => new[] { TradingMode.Spot };

        public void SetDefaultExchangeParameter(string key, object value) => ExchangeParameters.SetStaticParameter(Exchange, key, value);
        public void ResetDefaultExchangeParameters() => ExchangeParameters.ResetStaticParameters();
    }
}
