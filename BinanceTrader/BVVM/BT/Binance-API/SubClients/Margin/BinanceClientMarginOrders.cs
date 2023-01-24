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

using BinanceAPI.ClientHosts;
using BinanceAPI.Converters;
using BinanceAPI.Enums;
using BinanceAPI.Objects;
using BinanceAPI.Objects.Shared;
using BinanceAPI.Objects.Spot.MarginData;
using BinanceAPI.Objects.Spot.SpotData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceAPI.SubClients.Margin
{
    /// <summary>
    /// Margin order endpoints
    /// </summary>
    public class BinanceClientMarginOrders
    {
        private readonly BinanceClientHost _baseClient;

        internal BinanceClientMarginOrders(BinanceClientHost baseClient)
        {
            _baseClient = baseClient;
        }

        #region Margin Account New Order

        /// <summary>
        /// Margin account new order
        /// <para>[POST] https://binance-docs.github.io/apidocs/spot/en/#margin-account-new-order-trade</para>
        /// </summary>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="side">The order side (buy/sell)</param>
        /// <param name="type">The order type</param>
        /// <param name="timeInForce">Lifetime of the order (GoodTillCancel/ImmediateOrCancel/FillOrKill)</param>
        /// <param name="quantity">The amount of the symbol</param>
        /// <param name="price">The price to use</param>
        /// <param name="stopPrice">Used for stop orders</param>
        /// <param name="sideEffectType">Side effect type for this order</param>
        /// <param name="isIsolated">For isolated margin or not</param>
        /// <param name="orderResponseType">Used for the response JSON</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Id's for the placed order</returns>
        public async Task<WebCallResult<BinancePlacedOrder>> PlaceMarginOrderLimitAsync(string symbol,
            OrderSide side,
            OrderType type,
            decimal? quantity = null,
            decimal? price = null,
            TimeInForce? timeInForce = null,
            decimal? stopPrice = null,
            SideEffectType? sideEffectType = null,
            bool? isIsolated = null,
            OrderResponseType? orderResponseType = null,
            int? receiveWindow = null,
            CancellationToken ct = default)
        {
            return await _baseClient.PlaceOrderInternalLimit(UriClient.GetBaseAddress() + UriClient.GetEndpoint.Order.Margin.POST_NEW_ORDER_NewOrder,
                symbol,
                side,
                type,
                quantity,
                price,
                timeInForce,
                stopPrice,
                sideEffectType,
                isIsolated,
                orderResponseType,
                receiveWindow,
                ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Margin account new order
        /// <para>[POST] https://binance-docs.github.io/apidocs/spot/en/#margin-account-new-order-trade</para>
        /// </summary>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="side">The order side (buy/sell)</param>
        /// <param name="type">The order type</param>
        /// <param name="quantity">The amount of the symbol</param>
        /// <param name="stopPrice">Used for stop orders</param>
        /// <param name="sideEffectType">Side effect type for this order</param>
        /// <param name="isIsolated">For isolated margin or not</param>
        /// <param name="orderResponseType">Used for the response JSON</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Id's for the placed order</returns>
        public async Task<WebCallResult<BinancePlacedOrder>> PlaceMarginOrderMarketAsync(string symbol,
            OrderSide side,
            OrderType type,
            decimal? quantity = null,
            decimal? stopPrice = null,
            SideEffectType? sideEffectType = null,
            bool? isIsolated = null,
            OrderResponseType? orderResponseType = null,
            int? receiveWindow = null,
            CancellationToken ct = default)
        {
            return await _baseClient.PlaceOrderInternalMarket(UriClient.GetBaseAddress() + UriClient.GetEndpoint.Order.Margin.POST_NEW_ORDER_NewOrder,
                symbol,
                side,
                type,
                quantity,
                stopPrice,
                sideEffectType,
                isIsolated,
                orderResponseType,
                receiveWindow,
                ct).ConfigureAwait(false);
        }

        #endregion Margin Account New Order

        #region Margin Account Cancel Order

        /// <summary>
        /// Cancel an active order for margin account
        /// <para>[DELETE] https://binance-docs.github.io/apidocs/spot/en/#margin-account-cancel-order-trade</para>
        /// </summary>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="orderId">The order id of the order</param>
        /// <param name="isIsolated">For isolated margin or not</param>
        /// <param name="origClientOrderId">The client order id of the order</param>
        /// <param name="newClientOrderId">Unique identifier for this cancel</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Id's for canceled order</returns>
        public async Task<WebCallResult<BinanceCanceledOrder>> CancelMarginOrderAsync(string symbol, long? orderId = null, string? origClientOrderId = null, string? newClientOrderId = null, bool? isIsolated = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            if (!orderId.HasValue && string.IsNullOrEmpty(origClientOrderId))
                throw new ArgumentException("Either orderId or origClientOrderId must be sent");

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
            };
            parameters.AddOptionalParameter("orderId", orderId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("origClientOrderId", origClientOrderId);
            parameters.AddOptionalParameter("isIsolated", isIsolated);
            parameters.AddOptionalParameter("newClientOrderId", newClientOrderId);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceCanceledOrder>(UriClient.GetBaseAddress() + UriClient.GetEndpoint.Order.Margin.DELETE_CANCEL_ORDER_CancelOrder, HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Margin Account Cancel Order

        #region Margin Account Cancel All Open Orders

        /// <summary>
        /// Cancel all active orders for a symbol
        /// <para>[DELETE] https://binance-docs.github.io/apidocs/spot/en/#margin-account-cancel-all-open-orders-on-a-symbol-trade</para>
        /// </summary>
        /// <param name="symbol">The symbol the to cancel orders for</param>
        /// <param name="isIsolated">For isolated margin or not</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Id's for canceled order</returns>
        public async Task<WebCallResult<IEnumerable<BinanceCanceledOrder>>> CancelOpenMarginOrdersAsync(string symbol, bool? isIsolated = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
            };
            parameters.AddOptionalParameter("isIsolated", isIsolated);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceCanceledOrder>>(UriClient.GetBaseAddress() + UriClient.GetEndpoint.Order.Margin.DELETE_CANCEL_ALL_ORDERS_CancelAllOpenOrders, HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Margin Account Cancel All Open Orders

        #region Query Margin Account's Order

        /// <summary>
        /// Retrieves data for a specific margin account order. Either orderId or origClientOrderId should be provided.
        /// <para>[GET] https://binance-docs.github.io/apidocs/spot/en/#query-margin-account-39-s-order-user_data</para>
        /// </summary>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="isIsolated">For isolated margin or not</param>
        /// <param name="orderId">The order id of the order</param>
        /// <param name="origClientOrderId">The client order id of the order</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The specific margin account order</returns>
        public async Task<WebCallResult<BinanceOrderBase>> GetMarginAccountOrderAsync(string symbol, long? orderId = null, string? origClientOrderId = null, bool? isIsolated = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            if (orderId == null && origClientOrderId == null)
                throw new ArgumentException("Either orderId or origClientOrderId should be provided");

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
            };
            parameters.AddOptionalParameter("isIsolated", isIsolated);
            parameters.AddOptionalParameter("orderId", orderId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("origClientOrderId", origClientOrderId);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceOrderBase>(UriClient.GetBaseAddress() + UriClient.GetEndpoint.Order.Margin.GET_SINGLE_ORDER_GetSingleOrder, HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Query Margin Account's Order

        #region Query Margin Account's Open Order

        /// <summary>
        /// Gets a list of open margin account orders
        /// <para>https://binance-docs.github.io/apidocs/spot/en/#query-margin-account-39-s-open-orders-user_data</para>
        /// </summary>
        /// <param name="symbol">The symbol to get open orders for</param>
        /// <param name="isIsolated">For isolated margin or not</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open margin account orders</returns>
        public async Task<WebCallResult<IEnumerable<BinanceOrderBase>>> GetMarginAccountOpenOrdersAsync(string? symbol = null, bool? isIsolated = null, int? receiveWindow = null, CancellationToken ct = default)
        {
            if (isIsolated == true && symbol == null)
                throw new ArgumentException("Symbol must be provided for isolated margin");

            var parameters = new Dictionary<string, object>
            {
            };
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("isIsolated", isIsolated);

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceOrderBase>>(UriClient.GetBaseAddress() + UriClient.GetEndpoint.Order.Margin.GET_OPEN_ORDERS_GetAllOpenOrders, HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Query Margin Account's Open Order

        #region Query Margin Account's All Order

        /// <summary>
        /// Gets all margin account orders for the provided symbol
        /// <para>[GET] https://binance-docs.github.io/apidocs/spot/en/#query-margin-account-39-s-all-orders-user_data</para>
        /// </summary>
        /// <param name="symbol">The symbol to get orders for</param>
        /// <param name="isIsolated">For isolated margin or not</param>
        /// <param name="orderId">If set, only orders with an order id higher than the provided will be returned</param>
        /// <param name="startTime">If set, only orders placed after this time will be returned</param>
        /// <param name="endTime">If set, only orders placed before this time will be returned</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of margin account orders</returns>
        public async Task<WebCallResult<IEnumerable<BinanceOrderBase>>> GetMarginAccountOrdersAsync(string symbol, long? orderId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, bool? isIsolated = null, int? receiveWindow = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 500);

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
            };
            parameters.AddOptionalParameter("orderId", orderId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("isIsolated", isIsolated);
            parameters.AddOptionalParameter("startTime", startTime.HasValue ? JsonConvert.SerializeObject(startTime.Value.Ticks, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("endTime", endTime.HasValue ? JsonConvert.SerializeObject(endTime.Value.Ticks, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceOrderBase>>(UriClient.GetBaseAddress() + UriClient.GetEndpoint.Order.Margin.GET_ALL_ORDERS_GetAllOrders, HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Query Margin Account's All Order

        #region Margin Account New OCO Order

        /// <summary>
        /// Places a new margin OCO(One cancels other) order
        /// <para>[POST] https://binance-docs.github.io/apidocs/spot/en/#margin-account-new-oco-trade</para>
        /// </summary>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="side">The order side (buy/sell)</param>
        /// <param name="stopLimitTimeInForce">Lifetime of the stop order (GoodTillCancel/ImmediateOrCancel/FillOrKill)</param>
        /// <param name="quantity">The amount of the symbol</param>
        /// <param name="price">The price to use</param>
        /// <param name="stopPrice">The stop price</param>
        /// <param name="stopLimitPrice">The price for the stop limit order</param>
        /// <param name="stopClientOrderId">Client id for the stop order</param>
        /// <param name="limitClientOrderId">Client id for the limit order</param>
        /// <param name="listClientOrderId">Client id for the order list</param>
        /// <param name="limitIcebergQuantity">Iceberg quantity for the limit order</param>
        /// <param name="sideEffectType">Side effect type</param>
        /// <param name="isIsolated">Is isolated</param>
        /// <param name="orderResponseType">Order response type</param>
        /// <param name="stopIcebergQuantity">Iceberg quantity for the stop order</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Order list info</returns>
        public async Task<WebCallResult<BinanceMarginOrderOcoList>> PlaceMarginOCOOrderAsync(string symbol,
            OrderSide side,
            decimal price,
            decimal stopPrice,
            decimal quantity,
            decimal? stopLimitPrice = null,
            TimeInForce? stopLimitTimeInForce = null,
            decimal? stopIcebergQuantity = null,
            decimal? limitIcebergQuantity = null,
            SideEffectType? sideEffectType = null,
            bool? isIsolated = null,
            string? listClientOrderId = null,
            string? limitClientOrderId = null,
            string? stopClientOrderId = null,
            OrderResponseType? orderResponseType = null,
            int? receiveWindow = null,
            CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "side", JsonConvert.SerializeObject(side, new OrderSideConverter(false)) },
                { "quantity", quantity.ToString(CultureInfo.InvariantCulture) },
                { "price", price.ToString(CultureInfo.InvariantCulture) },
                { "stopPrice", stopPrice.ToString(CultureInfo.InvariantCulture) },
            };
            parameters.AddOptionalParameter("stopLimitPrice", stopLimitPrice?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("isIsolated", isIsolated?.ToString());
            parameters.AddOptionalParameter("sideEffectType", sideEffectType == null ? null : JsonConvert.SerializeObject(sideEffectType, new SideEffectTypeConverter(false)));
            parameters.AddOptionalParameter("listClientOrderId", listClientOrderId);
            parameters.AddOptionalParameter("limitClientOrderId", limitClientOrderId);
            parameters.AddOptionalParameter("stopClientOrderId", stopClientOrderId);
            parameters.AddOptionalParameter("limitIcebergQty", limitIcebergQuantity?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("newOrderRespType", orderResponseType == null ? null : JsonConvert.SerializeObject(orderResponseType, new OrderResponseTypeConverter(false)));
            parameters.AddOptionalParameter("stopIcebergQty", stopIcebergQuantity?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("stopLimitTimeInForce", stopLimitTimeInForce == null ? null : JsonConvert.SerializeObject(stopLimitTimeInForce, new TimeInForceConverter(false)));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceMarginOrderOcoList>(UriClient.GetBaseAddress() + UriClient.GetEndpoint.Order.Margin.POST_NEW_ORDER_OCO_NewOcoOrder, HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Margin Account New OCO Order

        #region Cancel OCO

        /// <summary>
        /// Cancels a pending margin oco order
        /// <para>[DELETE] https://binance-docs.github.io/apidocs/spot/en/#margin-account-cancel-oco-trade</para>
        /// </summary>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="isIsolated">For isolated margin or not</param>
        /// <param name="orderListId">The id of the order list to cancel</param>
        /// <param name="listClientOrderId">The client order id of the order list to cancel</param>
        /// <param name="newClientOrderId">The new client order list id for the order list</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Id's for canceled order</returns>
        public async Task<WebCallResult<BinanceMarginOrderOcoList>> CancelMarginOcoOrderAsync(string symbol, bool? isIsolated = null, long? orderListId = null, string? listClientOrderId = null, string? newClientOrderId = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            if (!orderListId.HasValue && string.IsNullOrEmpty(listClientOrderId))
                throw new ArgumentException("Either orderListId or listClientOrderId must be sent");

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
            };
            parameters.AddOptionalParameter("isIsolated", isIsolated?.ToString());
            parameters.AddOptionalParameter("orderListId", orderListId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("listClientOrderId", listClientOrderId);
            parameters.AddOptionalParameter("newClientOrderId", newClientOrderId);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceMarginOrderOcoList>(UriClient.GetBaseAddress() + UriClient.GetEndpoint.Order.Margin.DELETE_CANCEL_ORDER_OCO_CancelOcoOrder, HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Cancel OCO

        #region Query OCO

        /// <summary>
        /// Retrieves data for a specific margin oco order. Either orderListId or listClientOrderId should be provided.
        /// <para>[GET] https://binance-docs.github.io/apidocs/spot/en/#query-margin-account-39-s-oco-user_data</para>
        /// </summary>
        /// <param name="symbol">Mandatory for isolated margin, not supported for cross margin</param>
        /// <param name="isIsolated">For isolated margin or not</param>
        /// <param name="orderListId">The list order id of the order</param>
        /// <param name="origClientOrderId">Either orderListId or listClientOrderId must be provided</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The specific order list</returns>
        public async Task<WebCallResult<BinanceMarginOrderOcoList>> GetMarginOcoOrderAsync(string? symbol = null, bool? isIsolated = null, long? orderListId = null, string? origClientOrderId = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            if (orderListId == null && origClientOrderId == null)
                throw new ArgumentException("Either orderListId or origClientOrderId must be sent");

            var parameters = new Dictionary<string, object>
            {
            };
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("isIsolated", isIsolated.ToString());
            parameters.AddOptionalParameter("orderListId", orderListId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("origClientOrderId", origClientOrderId);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceMarginOrderOcoList>(UriClient.GetBaseAddress() + UriClient.GetEndpoint.Order.Margin.GET_ORDER_OCO_GetOcoOrder, HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Query OCO

        #region Query all OCO

        /// <summary>
        /// Retrieves a list of margin oco orders matching the parameters
        /// <para>[GET] https://binance-docs.github.io/apidocs/spot/en/#query-margin-account-39-s-all-oco-user_data</para>
        /// </summary>
        /// <param name="symbol">Mandatory for isolated margin, not supported for cross margin</param>
        /// <param name="isIsolated">For isolated margin or not</param>
        /// <param name="fromId">Only return oco orders with id higher than this</param>
        /// <param name="startTime">Only return oco orders placed later than this. Only valid if fromId isn't provided</param>
        /// <param name="endTime">Only return oco orders placed before this. Only valid if fromId isn't provided</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Order lists matching the parameters</returns>
        public async Task<WebCallResult<IEnumerable<BinanceMarginOrderOcoList>>> GetMarginOcoOrdersAsync(string? symbol = null, bool? isIsolated = null, long? fromId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            if (fromId != null && (startTime != null || endTime != null))
                throw new ArgumentException("Start/end time can only be provided without fromId parameter");

            limit?.ValidateIntBetween(nameof(limit), 1, 1000);

            var parameters = new Dictionary<string, object>
            {
            };
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("isIsolated", isIsolated?.ToString());
            parameters.AddOptionalParameter("fromId", fromId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("startTime", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("endTime", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceMarginOrderOcoList>>(UriClient.GetBaseAddress() + UriClient.GetEndpoint.Order.Margin.GET_ALL_OPEN_OCO_ORDERS_GetAllOpenOcoOrders, HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Query all OCO

        #region Query Open OCO

        /// <summary>
        /// Retrieves a list of open margin oco orders
        /// <para>[GET] https://binance-docs.github.io/apidocs/spot/en/#query-margin-account-39-s-open-oco-user_data</para>
        /// </summary>
        /// <param name="symbol">Mandatory for isolated margin, not supported for cross margin</param>
        /// <param name="isIsolated">For isolated margin or not</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Open order lists</returns>
        public async Task<WebCallResult<IEnumerable<BinanceMarginOrderOcoList>>> GetOpenOcoOrdersAsync(string? symbol = null, bool? isIsolated = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
            };
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("isIsolated", isIsolated?.ToString());
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceMarginOrderOcoList>>(UriClient.GetBaseAddress() + UriClient.GetEndpoint.Order.Margin.GET_ALL_ORDER_OCO_GetAllOcoOrders, HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Query Open OCO

        #region Query Margin Account's Trade List

        /// <summary>
        /// Gets all user margin account trades for provided symbol
        /// <para>[GET] https://binance-docs.github.io/apidocs/spot/en/#query-margin-account-39-s-trade-list-user_data</para>
        /// </summary>
        /// <param name="symbol">Symbol to get trades for</param>
        /// <param name="limit">The max number of results</param>
        /// <param name="isIsolated">For isolated margin or not</param>
        /// <param name="startTime">Orders newer than this date will be retrieved</param>
        /// <param name="endTime">Orders older than this date will be retrieved</param>
        /// <param name="fromId">TradeId to fetch from. Default gets most recent trades</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of margin account trades</returns>
        public async Task<WebCallResult<IEnumerable<BinanceTrade>>> GetMarginAccountUserTradesAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, long? fromId = null, bool? isIsolated = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 1000);

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
            };
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("isIsolated", isIsolated);
            parameters.AddOptionalParameter("fromId", fromId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("startTime", startTime.HasValue ? JsonConvert.SerializeObject(startTime.Value.Ticks, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("endTime", endTime.HasValue ? JsonConvert.SerializeObject(endTime.Value.Ticks, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceTrade>>(UriClient.GetBaseAddress() + UriClient.GetEndpoint.Order.Margin.GET_ALL_TRADES_GetAllTrades, HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Query Margin Account's Trade List
    }
}
