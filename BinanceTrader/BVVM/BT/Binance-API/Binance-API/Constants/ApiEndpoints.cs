/*
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

namespace BinanceAPI.Constants
{
    /// <summary>
    /// API endpoint constants
    /// </summary>
    public static class ApiEndpoints
    {
        /// <summary>
        /// Base API URL
        /// </summary>
        public const string BaseUrl = "https://api.binance.com/";

        /// <summary>
        /// Testnet API URL
        /// </summary>
        public const string TestnetUrl = "https://testnet.binance.vision/";

        /// <summary>
        /// API version
        /// </summary>
        public const string ApiVersion = "v3";

        /// <summary>
        /// SAPI version
        /// </summary>
        public const string SapiVersion = "v1";

        /// <summary>
        /// User data stream version
        /// </summary>
        public const string UserDataStreamVersion = "v1";

        // Common endpoints
        public const string Ping = "ping";
        public const string Time = "time";
        public const string ExchangeInfo = "exchangeInfo";

        // Market data endpoints
        public const string OrderBook = "depth";
        public const string RecentTrades = "trades";
        public const string HistoricalTrades = "historicalTrades";
        public const string AggregatedTrades = "aggTrades";
        public const string Klines = "klines";
        public const string Ticker24Hr = "ticker/24hr";
        public const string TickerPrice = "ticker/price";
        public const string TickerBookTicker = "ticker/bookTicker";
        public const string AveragePrice = "avgPrice";

        // Order endpoints
        public const string Order = "order";
        public const string OrderTest = "order/test";
        public const string OpenOrders = "openOrders";
        public const string AllOrders = "allOrders";
        public const string MyTrades = "myTrades";

        // OCO endpoints
        public const string OcoOrder = "order/oco";
        public const string OcoOrderList = "orderList";
        public const string AllOcoOrders = "allOrderList";
        public const string OpenOcoOrders = "openOrderList";

        // Account endpoints
        public const string Account = "account";
        public const string AccountSnapshot = "accountSnapshot";
        public const string ApiTradingStatus = "account/apiTradingStatus";

        // Margin endpoints
        public const string MarginAccount = "margin/account";
        public const string MarginAsset = "margin/asset";
        public const string MarginPair = "margin/pair";
        public const string MarginAllAssets = "margin/allAssets";
        public const string MarginAllPairs = "margin/allPairs";
        public const string MarginPriceIndex = "margin/priceIndex";
        public const string MarginLoan = "margin/loan";
        public const string MarginRepay = "margin/repay";
        public const string MarginTransfer = "margin/transfer";
        public const string MarginMaxBorrowable = "margin/maxBorrowable";
        public const string MarginMaxTransferable = "margin/maxTransferable";
        public const string MarginInterestHistory = "margin/interestHistory";
        public const string MarginInterestRateHistory = "margin/interestRateHistory";
        public const string MarginForceLiquidationRec = "margin/forceLiquidationRec";
        public const string MarginTradeCoeff = "margin/tradeCoeff";

        // Isolated margin endpoints
        public const string IsolatedMarginAccount = "margin/isolated/account";
        public const string IsolatedMarginPair = "margin/isolated/pair";
        public const string IsolatedMarginAllPairs = "margin/isolated/allPairs";
        public const string IsolatedMarginTransfer = "margin/isolated/transfer";
        public const string IsolatedMarginAccountLimit = "margin/isolated/accountLimit";

        // User data stream endpoints
        public const string UserDataStream = "userDataStream";

        // Asset endpoints
        public const string AssetTradeFee = "asset/tradeFee";
        public const string AssetAssetDetail = "asset/assetDetail";
        public const string AssetDust = "asset/dust";
        public const string AssetDribblet = "asset/dribblet";

        // Capital endpoints
        public const string CapitalWithdrawApply = "capital/withdraw/apply";
        public const string CapitalWithdrawHistory = "capital/withdraw/history";
        public const string CapitalDepositHistory = "capital/deposit/hisrec";
        public const string CapitalDepositAddress = "capital/deposit/address";
        public const string CapitalConfigGetall = "capital/config/getall";

        // Fiat endpoints
        public const string FiatOrders = "fiat/orders";
        public const string FiatPayments = "fiat/payments";

        // Lending endpoints
        public const string LendingDailyProductList = "lending/daily/product/list";
        public const string LendingDailyUserLeftQuota = "lending/daily/userLeftQuota";
        public const string LendingDailyUserRedemptionQuota = "lending/daily/userRedemptionQuota";
        public const string LendingDailyTokenPosition = "lending/daily/token/position";
        public const string LendingUnionAccount = "lending/union/account";
        public const string LendingUnionPurchaseRecord = "lending/union/purchaseRecord";
        public const string LendingUnionRedemptionRecord = "lending/union/redemptionRecord";
        public const string LendingUnionInterestHistory = "lending/union/interestHistory";
        public const string LendingPositionChanged = "lending/positionChanged";

        // BNB Burn endpoints
        public const string BnbBurn = "bnbBurn";

        // Universal transfer endpoints
        public const string UniversalTransfer = "asset/transfer";
    }
} 