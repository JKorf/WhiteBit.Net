using CryptoExchange.Net.SharedApis;
using WhiteBit.Net.Enums;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Clients.V4Api
{
    internal partial class WhiteBitSocketClientV4SharedApi
    {
        private SharedOrderStatus ParseOrderStatus(WhiteBitOrderUpdate update)
        {
            if (update.Event == OrderEvent.New
                || update.Order.Status == OrderStatus.Open
                || update.Order.Status == OrderStatus.PartiallyFilled)
            {
                return SharedOrderStatus.Open;
            }

            if (update.Order.OrderType == OrderType.Market
                || update.Order.OrderType == OrderType.MarketBase
                || update.Order.OrderType == OrderType.CollateralMarket
                || update.Order.QuantityRemaining == 0)
            {
                return SharedOrderStatus.Filled;
            }

            return SharedOrderStatus.Canceled;
        }

        private SharedOrderType ParseOrderType(OrderType type, bool postOnly)
        {
            if (type == OrderType.Market || type == OrderType.CollateralMarket || type == OrderType.CollateralTriggerStopMarket) return SharedOrderType.Market;
            if (type == OrderType.MarketBase) return SharedOrderType.Market;
            if ((type == OrderType.Limit || type == OrderType.CollateralLimit) && postOnly) return SharedOrderType.LimitMaker;
            if (type == OrderType.Limit || type == OrderType.CollateralLimit || type == OrderType.CollateralStopLimit) return SharedOrderType.Limit;

            return SharedOrderType.Other;
        }

        private SharedTimeInForce? ParseTimeInForce(WhiteBitOrder order)
        {
            if (order.ImmediateOrCancel == true)
                return SharedTimeInForce.ImmediateOrCancel;

            return null;
        }
    }
}
