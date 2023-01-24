﻿/*
*MIT License
*
*Copyright (c) 2022 S Christison
*
*Permission is hereby granted, free of charge, to any person obtaining a copy
*of this software and associated documentation files (the "Software"), to deal
*in the Software without restriction, including without limitation the rights
*to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*copies of the Software, and to permit persons to whom the Software is
*furnished to do so, subject to the following conditions:
*
*The above copyright notice and this permission notice shall be included in all
*copies or substantial portions of the Software.
*
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
*SOFTWARE.
*/

using BinanceAPI.Enums;
using BinanceAPI.Objects.Shared;
using BinanceAPI.Objects.Spot.SpotData;
using BinanceAPI.Objects.Spot.UserStream;
using BinanceAPI.Objects.Spot.WalletData;
using BTNET.BV.Enum;
using BTNET.BVVM;
using BTNET.BVVM.BT;
using BTNET.BVVM.BT.Orders;
using BTNET.VM.ViewModels;

namespace BTNET.BV.Base
{
    internal class OrderBase : Core
    {
        public static OrderViewModel NewScraperOrder(BinancePlacedOrder o, TradingMode tradingMode)
        {
            BinanceTradeFee fee = TradeFee.GetTradeFee(o.Symbol);
            var baseFee = TradeFeeNoInfo(o.Type, fee);
            var orderFee = OrderHelper.Fee(TradeFeeNoInfo(o.Type, fee), o.Price, o.QuantityFilled);

            decimal orderPrice;
            if (o.AverageFillPrice > decimal.Zero)
            {
                orderPrice = (decimal)o.AverageFillPrice;
            }
            else
            {
                orderPrice = o.Price;
            }

            OrderViewModel order = new OrderViewModel()
            {
                OrderId = o.OrderId,
                Symbol = o.Symbol,
                Quantity = o.Quantity,
                CumulativeQuoteQuantityFilled = o.QuoteQuantityFilled,
                QuantityFilled = o.QuantityFilled,
                OrderFee = baseFee,
                Price = orderPrice,
                CreateTime = o.CreateTime,
                UpdateTime = o.UpdateTime,
                Side = o.Side,
                Status = o.Status,
                Type = o.Type,
                PurchasedByScraper = true,
                IsMaker = MakerNoInfo(o.Type, fee),
                Fee = orderFee * App.FEE_MULTIPLIER,
                MinPos = orderFee * App.MIN_PNL_FEE_MULTIPLIER,
                TimeInForce = OrderHelper.TIF(o.TimeInForce.ToString()),
                Fulfilled = OrderHelper.Fulfilled(o.Quantity, o.QuantityFilled),
                OrderTradingMode = tradingMode,
            };

            order.Total = o.QuoteQuantityFilled;
            return order;
        }

        public static OrderViewModel NewOrderFromServer(BinanceOrderBase o, TradingMode tradingMode)
        {
            BinanceTradeFee fee = TradeFee.GetTradeFee(o.Symbol);
            var baseFee = TradeFeeNoInfo(o.Type, fee);
            var orderFee = OrderHelper.Fee(TradeFeeNoInfo(o.Type, fee), o.Price, o.QuantityFilled);

            decimal orderPrice;
            if (o.AverageFillPrice > decimal.Zero)
            {
                orderPrice = (decimal)o.AverageFillPrice;
            }
            else
            {
                orderPrice = o.Price;
            }

            OrderViewModel order = new OrderViewModel()
            {
                OrderId = o.OrderId,
                Symbol = o.Symbol,
                Quantity = o.Quantity,
                CumulativeQuoteQuantityFilled = o.QuoteQuantityFilled,
                QuantityFilled = o.QuantityFilled,
                OrderFee = baseFee,
                Price = orderPrice,
                CreateTime = o.CreateTime,
                UpdateTime = o.UpdateTime,
                Side = o.Side,
                Status = o.Status,
                Type = o.Type,
                IsMaker = MakerNoInfo(o.Type, fee),
                Fee = orderFee * App.FEE_MULTIPLIER,
                MinPos = orderFee * App.MIN_PNL_FEE_MULTIPLIER,
                TimeInForce = OrderHelper.TIF(o.TimeInForce.ToString()),
                Fulfilled = OrderHelper.Fulfilled(o.Quantity, o.QuantityFilled),
                OrderTradingMode = tradingMode
            };

            order.Total = o.QuoteQuantityFilled;
            return order;
        }

        public static OrderViewModel NewOrderOnUpdate(BinanceStreamOrderUpdate data, decimal convertedPrice, TradingMode tradingmode)
        {
            BinanceTradeFee fee = TradeFee.GetTradeFee(data.Symbol);
            decimal baseFee = TradeFeeTakerMaker(data.BuyerIsMaker, fee);
            decimal orderFee = OrderHelper.Fee(baseFee, convertedPrice, data.QuantityFilled);

            OrderViewModel order = new OrderViewModel()
            {
                OrderId = data.OrderId,
                Symbol = data.Symbol,
                Quantity = data.Quantity,
                CumulativeQuoteQuantityFilled = data.QuoteQuantityFilled,
                QuantityFilled = data.QuantityFilled,
                OrderFee = baseFee,
                Price = convertedPrice,
                CreateTime = data.CreateTime,
                UpdateTime = data.UpdateTime,
                Status = data.Status,
                Side = data.Side,
                Type = data.Type,
                IsMaker = data.BuyerIsMaker,
                Fee = orderFee * App.FEE_MULTIPLIER,
                MinPos = orderFee * App.MIN_PNL_FEE_MULTIPLIER,
                TimeInForce = OrderHelper.TIF(data.TimeInForce.ToString()),
                Fulfilled = OrderHelper.Fulfilled(data.Quantity, data.QuantityFilled),
                OrderTradingMode = tradingmode,
            };

            order.Total = data.QuoteQuantityFilled;
            return order;
        }

        // Use OnOrderUpdate to decide if order is maker/taker
        private static decimal TradeFeeTakerMaker(bool buyerIsMaker, BinanceTradeFee btf)
        {
            if (buyerIsMaker)
            {
                return btf.MakerFee;
            }

            return btf.TakerFee;
        }

        /// <summary>
        /// Orders that weren't stored and didn't recieve any OrderUpdates
        /// Orders placed outside the app while the app is closed fall into this category
        /// Orders that were placed before you started using the app fall into this category
        /// Orders that got lost for some reason fall into this category
        /// </summary>
        /// <param name="o"></param>
        /// <param name="btf"></param>
        /// <returns></returns>
        private static decimal TradeFeeNoInfo(OrderType o, BinanceTradeFee btf)
        {
            return o switch
            {
                OrderType.Market => btf.TakerFee,
                OrderType.LimitMaker => btf.MakerFee,
                _ => btf.TakerFee >= btf.MakerFee ? btf.TakerFee : btf.MakerFee,
            };
        }

        /// <summary>
        /// When you retrieve orders from the server manually it doesn't tell you if they were maker or taker orders, This information only comes from OnOrderUpdates
        /// Like the trade fee if there is no OnOrderUpdate information for this order then it is a best guess.
        /// In the case that we can't guess it will select the higher fee
        /// </summary>
        /// <param name="o">Order Type for the Order</param>
        /// <returns></returns>
        private static bool MakerNoInfo(OrderType o, BinanceTradeFee btf)
        {
            return o switch
            {
                OrderType.Market => false,
                OrderType.LimitMaker => true,
                _ => btf.TakerFee >= btf.MakerFee,
            };
        }
    }
}
