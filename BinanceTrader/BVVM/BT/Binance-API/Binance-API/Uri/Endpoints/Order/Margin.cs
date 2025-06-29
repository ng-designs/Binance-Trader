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

using BinanceAPI.Requests;

#pragma warning disable CS1591 // WIP

namespace BinanceAPI.UriBase
{
    public class Margin
    {
        public const string NEW_TEST_ORDER = "order/test";
        public const string NEW_OCO_ORDER = "order/oco";
        public const string NEW_ORDER = "order";

        public const string ORDER = "order";
        public const string OCO_ORDER = "orderList";
        public const string OPEN_ORDERS = "openOrders";
        public const string ALL_ORDERS = "allOrders";

        public const string ALL_TRADES = "myTrades";

        public const string ALL_OCO_ORDERS = "allOrderList";
        public const string ALL_OPEN_OCO_ORDERS = "openOrderList";

        // Margin Account Endpoints
        public const string MARGIN_ACCOUNT = "margin/account";
        public const string MARGIN_ASSET = "margin/asset";
        public const string MARGIN_PAIR = "margin/pair";
        public const string MARGIN_ALL_ASSETS = "margin/allAssets";
        public const string MARGIN_ALL_PAIRS = "margin/allPairs";
        public const string MARGIN_PRICE_INDEX = "margin/priceIndex";

        // Margin Loan/Repay Endpoints
        public const string MARGIN_LOAN = "margin/loan";
        public const string MARGIN_REPAY = "margin/repay";
        public const string MARGIN_MAX_BORROWABLE = "margin/maxBorrowable";
        public const string MARGIN_MAX_TRANSFERABLE = "margin/maxTransferable";

        // Margin Transfer Endpoints
        public const string MARGIN_TRANSFER = "margin/transfer";
        public const string MARGIN_TRANSFER_HISTORY = "margin/transfer";

        // Isolated Margin Endpoints
        public const string ISOLATED_MARGIN_ACCOUNT = "margin/isolated/account";
        public const string ISOLATED_MARGIN_PAIR = "margin/isolated/pair";
        public const string ISOLATED_MARGIN_ALL_PAIRS = "margin/isolated/allPairs";
        public const string ISOLATED_MARGIN_TRANSFER = "margin/isolated/transfer";

        public const string API = "sapi";
        public const string VERSION = "1";

        public Margin()
        {
            // Order
            POST_NEW_ORDER_NewOrder = GetUriString.Combine(NEW_ORDER, API, VERSION);
            POST_NEW_ORDER_TEST_NewOrderTest = GetUriString.Combine(NEW_TEST_ORDER, API, VERSION);
            GET_SINGLE_ORDER_GetSingleOrder = GetUriString.Combine(ORDER, API, VERSION);
            GET_OPEN_ORDERS_GetAllOpenOrders = GetUriString.Combine(OPEN_ORDERS, API, VERSION);
            GET_ALL_ORDERS_GetAllOrders = GetUriString.Combine(ALL_ORDERS, API, VERSION);
            DELETE_CANCEL_ORDER_CancelOrder = GetUriString.Combine(ORDER, API, VERSION);
            DELETE_CANCEL_ALL_ORDERS_CancelAllOpenOrders = GetUriString.Combine(OPEN_ORDERS, API, VERSION);

            // Trades
            GET_ALL_TRADES_GetAllTrades = GetUriString.Combine(ALL_TRADES, API, VERSION);

            // Oco Order
            POST_NEW_ORDER_OCO_NewOcoOrder = GetUriString.Combine(NEW_OCO_ORDER, API, VERSION);
            GET_ORDER_OCO_GetOcoOrder = GetUriString.Combine(OCO_ORDER, API, VERSION);
            GET_ALL_ORDER_OCO_GetAllOcoOrders = GetUriString.Combine(ALL_OCO_ORDERS, API, VERSION);
            GET_ALL_OPEN_OCO_ORDERS_GetAllOpenOcoOrders = GetUriString.Combine(ALL_OPEN_OCO_ORDERS, API, VERSION);
            DELETE_CANCEL_ORDER_OCO_CancelOcoOrder = GetUriString.Combine(OCO_ORDER, API, VERSION);

            // Margin Account
            GET_MARGIN_ACCOUNT_GetMarginAccount = GetUriString.Combine(MARGIN_ACCOUNT, API, VERSION);
            GET_MARGIN_ASSET_GetMarginAsset = GetUriString.Combine(MARGIN_ASSET, API, VERSION);
            GET_MARGIN_PAIR_GetMarginPair = GetUriString.Combine(MARGIN_PAIR, API, VERSION);
            GET_MARGIN_ALL_ASSETS_GetMarginAllAssets = GetUriString.Combine(MARGIN_ALL_ASSETS, API, VERSION);
            GET_MARGIN_ALL_PAIRS_GetMarginAllPairs = GetUriString.Combine(MARGIN_ALL_PAIRS, API, VERSION);
            GET_MARGIN_PRICE_INDEX_GetMarginPriceIndex = GetUriString.Combine(MARGIN_PRICE_INDEX, API, VERSION);

            // Margin Loan/Repay
            POST_MARGIN_LOAN_NewMarginLoan = GetUriString.Combine(MARGIN_LOAN, API, VERSION);
            POST_MARGIN_REPAY_NewMarginRepay = GetUriString.Combine(MARGIN_REPAY, API, VERSION);
            GET_MARGIN_LOAN_GetMarginLoan = GetUriString.Combine(MARGIN_LOAN, API, VERSION);
            GET_MARGIN_REPAY_GetMarginRepay = GetUriString.Combine(MARGIN_REPAY, API, VERSION);
            GET_MARGIN_MAX_BORROWABLE_GetMaxBorrowable = GetUriString.Combine(MARGIN_MAX_BORROWABLE, API, VERSION);
            GET_MARGIN_MAX_TRANSFERABLE_GetMaxTransferable = GetUriString.Combine(MARGIN_MAX_TRANSFERABLE, API, VERSION);

            // Margin Transfer
            POST_MARGIN_TRANSFER_NewMarginTransfer = GetUriString.Combine(MARGIN_TRANSFER, API, VERSION);
            GET_MARGIN_TRANSFER_HISTORY_GetMarginTransferHistory = GetUriString.Combine(MARGIN_TRANSFER_HISTORY, API, VERSION);

            // Isolated Margin
            GET_ISOLATED_MARGIN_ACCOUNT_GetIsolatedMarginAccount = GetUriString.Combine(ISOLATED_MARGIN_ACCOUNT, API, VERSION);
            GET_ISOLATED_MARGIN_PAIR_GetIsolatedMarginPair = GetUriString.Combine(ISOLATED_MARGIN_PAIR, API, VERSION);
            GET_ISOLATED_MARGIN_ALL_PAIRS_GetIsolatedMarginAllPairs = GetUriString.Combine(ISOLATED_MARGIN_ALL_PAIRS, API, VERSION);
            POST_ISOLATED_MARGIN_TRANSFER_NewIsolatedMarginTransfer = GetUriString.Combine(ISOLATED_MARGIN_TRANSFER, API, VERSION);
        }

        /// <summary>
        /// [POST] https://binance-docs.github.io/apidocs/spot/en/#margin-account-new-order-trade
        /// </summary>
        public readonly string POST_NEW_ORDER_NewOrder;

        /// <summary>
        /// [POST] https://binance-docs.github.io/apidocs/spot/en/#margin-account-new-oco-trade
        /// </summary>
        public readonly string POST_NEW_ORDER_OCO_NewOcoOrder;

        /// <summary>
        /// [POST] https://binance-docs.github.io/apidocs/spot/en/#margin-account-test-new-order-trade
        /// </summary>
        public readonly string POST_NEW_ORDER_TEST_NewOrderTest;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#margin-account-query-order-user_data
        /// </summary>
        public readonly string GET_SINGLE_ORDER_GetSingleOrder;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#margin-account-current-open-orders-user_data
        /// </summary>
        public readonly string GET_OPEN_ORDERS_GetAllOpenOrders;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#margin-account-all-orders-user_data
        /// </summary>
        public readonly string GET_ALL_ORDERS_GetAllOrders;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#margin-account-trade-list-user_data
        /// </summary>
        public readonly string GET_ALL_TRADES_GetAllTrades;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#margin-account-query-oco-user_data
        /// </summary>
        public readonly string GET_ORDER_OCO_GetOcoOrder;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#margin-account-query-all-oco-user_data
        /// </summary>
        public readonly string GET_ALL_ORDER_OCO_GetAllOcoOrders;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#margin-account-query-open-oco-user_data
        /// </summary>
        public readonly string GET_ALL_OPEN_OCO_ORDERS_GetAllOpenOcoOrders;

        /// <summary>
        /// [DELETE] https://binance-docs.github.io/apidocs/spot/en/#margin-account-cancel-order-trade
        /// </summary>
        public readonly string DELETE_CANCEL_ORDER_CancelOrder;

        /// <summary>
        /// [DELETE] https://binance-docs.github.io/apidocs/spot/en/#margin-account-cancel-oco-trade
        /// </summary>
        public readonly string DELETE_CANCEL_ORDER_OCO_CancelOcoOrder;

        /// <summary>
        /// [DELETE] https://binance-docs.github.io/apidocs/spot/en/#margin-account-cancel-all-open-orders-on-a-symbol-trade
        /// </summary>
        public readonly string DELETE_CANCEL_ALL_ORDERS_CancelAllOpenOrders;

        // Margin Account Endpoints
        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#margin-account-information-user_data
        /// </summary>
        public readonly string GET_MARGIN_ACCOUNT_GetMarginAccount;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-margin-asset-market_data
        /// </summary>
        public readonly string GET_MARGIN_ASSET_GetMarginAsset;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-cross-margin-pair-market_data
        /// </summary>
        public readonly string GET_MARGIN_PAIR_GetMarginPair;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#get-all-margin-assets-market_data
        /// </summary>
        public readonly string GET_MARGIN_ALL_ASSETS_GetMarginAllAssets;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#get-all-cross-margin-pairs-market_data
        /// </summary>
        public readonly string GET_MARGIN_ALL_PAIRS_GetMarginAllPairs;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-margin-priceindex-market_data
        /// </summary>
        public readonly string GET_MARGIN_PRICE_INDEX_GetMarginPriceIndex;

        // Margin Loan/Repay Endpoints
        /// <summary>
        /// [POST] https://binance-docs.github.io/apidocs/spot/en/#margin-account-borrow-margin
        /// </summary>
        public readonly string POST_MARGIN_LOAN_NewMarginLoan;

        /// <summary>
        /// [POST] https://binance-docs.github.io/apidocs/spot/en/#margin-account-repay-margin
        /// </summary>
        public readonly string POST_MARGIN_REPAY_NewMarginRepay;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#margin-account-borrow-history-user_data
        /// </summary>
        public readonly string GET_MARGIN_LOAN_GetMarginLoan;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#margin-account-repay-history-user_data
        /// </summary>
        public readonly string GET_MARGIN_REPAY_GetMarginRepay;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-max-borrow-user_data
        /// </summary>
        public readonly string GET_MARGIN_MAX_BORROWABLE_GetMaxBorrowable;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-max-transfer-out-amount-user_data
        /// </summary>
        public readonly string GET_MARGIN_MAX_TRANSFERABLE_GetMaxTransferable;

        // Margin Transfer Endpoints
        /// <summary>
        /// [POST] https://binance-docs.github.io/apidocs/spot/en/#margin-account-transfer-margin
        /// </summary>
        public readonly string POST_MARGIN_TRANSFER_NewMarginTransfer;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#margin-transfer-history-user_data
        /// </summary>
        public readonly string GET_MARGIN_TRANSFER_HISTORY_GetMarginTransferHistory;

        // Isolated Margin Endpoints
        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#isolated-margin-account-information-user_data
        /// </summary>
        public readonly string GET_ISOLATED_MARGIN_ACCOUNT_GetIsolatedMarginAccount;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-isolated-margin-pair-market_data
        /// </summary>
        public readonly string GET_ISOLATED_MARGIN_PAIR_GetIsolatedMarginPair;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#get-all-isolated-margin-pairs-market_data
        /// </summary>
        public readonly string GET_ISOLATED_MARGIN_ALL_PAIRS_GetIsolatedMarginAllPairs;

        /// <summary>
        /// [POST] https://binance-docs.github.io/apidocs/spot/en/#isolated-margin-account-transfer-margin
        /// </summary>
        public readonly string POST_ISOLATED_MARGIN_TRANSFER_NewIsolatedMarginTransfer;
    }
}
