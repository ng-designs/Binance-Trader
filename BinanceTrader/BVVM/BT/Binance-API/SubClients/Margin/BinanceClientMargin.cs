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
using BinanceAPI.Interfaces.SubClients;
using BinanceAPI.Objects;
using BinanceAPI.Objects.Spot.IsolatedMarginData;
using BinanceAPI.Objects.Spot.MarginData;
using BinanceAPI.Requests;
using BinanceAPI.Time;
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
    /// Margin endpoints
    /// </summary>
    public class BinanceClientMargin
    {
        private const string marginApi = "sapi";
        private const string marginVersion = "1";

        // Margin
        private const string marginTransferEndpoint = "margin/transfer";

        private const string marginBorrowEndpoint = "margin/loan";
        private const string marginRepayEndpoint = "margin/repay";
        private const string getLoanEndpoint = "margin/loan";
        private const string getRepayEndpoint = "margin/repay";
        private const string marginAccountInfoEndpoint = "margin/account";
        private const string maxBorrowableEndpoint = "margin/maxBorrowable";
        private const string maxTransferableEndpoint = "margin/maxTransferable";
        private const string transferHistoryEndpoint = "margin/transfer";
        private const string interestHistoryEndpoint = "margin/interestHistory";
        private const string interestRateHistoryEndpoint = "margin/interestRateHistory";

        private const string interestMarginDataEndpoint = "margin/crossMarginData";
        private const string interestIsolatedMarginDataEndpoint = "margin/isolatedMarginData";

        private const string forceLiquidationHistoryEndpoint = "margin/forceLiquidationRec";
        private const string marginLevelInformation = "margin/tradeCoeff";

        private const string isolatedMarginTransferHistoryEndpoint = "margin/isolated/transfer";
        private const string isolatedMarginAccountEndpoint = "margin/isolated/account";
        private const string isolatedMarginAccountLimitEndpoint = "margin/isolated/accountLimit";
        private const string transferIsolatedMarginAccountEndpoint = "margin/isolated/transfer";

        private readonly BinanceClientHost _baseClient;

        /// <summary>
        /// Margin market endpoints
        /// </summary>
        public BinanceClientMarginMarket Market { get; }

        /// <summary>
        /// Margin order endpoints
        /// </summary>
        public BinanceClientMarginOrders Order { get; }

        /// <summary>
        /// Margin user stream endpoints
        /// </summary>
        public IBinanceClientUserStream UserStream { get; }

        /// <summary>
        /// Isolated margin user stream endpoints
        /// </summary>
        public BinanceClientIsolatedMarginUserStream IsolatedUserStream { get; }

        internal BinanceClientMargin(BinanceClientHost baseClient)
        {
            _baseClient = baseClient;
            Market = new BinanceClientMarginMarket(_baseClient);
            Order = new BinanceClientMarginOrders(_baseClient);
            UserStream = new BinanceClientMarginUserStream(_baseClient);
            IsolatedUserStream = new BinanceClientIsolatedMarginUserStream(_baseClient);
        }

        #region Margin Level Information

        /// <summary>
        /// Get personal margin level information for your account
        /// </summary>
        /// <param name="email">account email</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Margin Level Information</returns>
        public async Task<WebCallResult<BinanceMarginLevel>> GetMarginLevelInformationAsync(string email, int? receiveWindow = null, CancellationToken ct = default)
        {
            email.ValidateNotNull(nameof(email));

            var parameters = new Dictionary<string, object>
            {
                { "email", email },
            };

            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceMarginLevel>(
                UriClient.GetBaseAddress() + GetUriString.Combine(marginLevelInformation, marginApi, marginVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion

        #region Margin Account Transfer

        /// <summary>
        /// Execute transfer between spot account and cross margin account.
        /// </summary>
        /// <param name="asset">The asset being transferred, e.g., BTC</param>
        /// <param name="amount">The amount to be transferred</param>
        /// <param name="type">TransferDirection (MainToMargin/MarginToMain)</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Transaction Id</returns>
        public async Task<WebCallResult<BinanceTransaction>> TransferAsync(string asset, decimal amount, TransferDirectionType type, int? receiveWindow = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));

            var parameters = new Dictionary<string, object>
            {
                { "asset", asset },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "type", JsonConvert.SerializeObject(type, new TransferDirectionTypeConverter(false)) },
            };
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceTransaction>(
                UriClient.GetBaseAddress() + GetUriString.Combine(marginTransferEndpoint, marginApi, marginVersion), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Margin Account Transfer

        #region Margin Account Borrow

        /// <summary>
        /// Borrow. Apply for a loan.
        /// </summary>
        /// <param name="asset">The asset being borrow, e.g., BTC</param>
        /// <param name="amount">The amount to be borrow</param>
        /// <param name="isIsolated">For isolated margin or not</param>
        /// <param name="symbol">The isolated symbol</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Transaction Id</returns>
        public async Task<WebCallResult<BinanceTransaction>> BorrowAsync(string asset, decimal amount, bool? isIsolated = null, string? symbol = null, int? receiveWindow = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            if (isIsolated == true && symbol == null)
                throw new ArgumentException("Symbol should be specified when using isolated margin");

            var parameters = new Dictionary<string, object>
            {
                { "asset", asset },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
            };
            parameters.AddOptionalParameter("isIsolated", isIsolated?.ToString().ToLower());
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceTransaction>(
                UriClient.GetBaseAddress() + GetUriString.Combine(marginBorrowEndpoint, marginApi, marginVersion), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Margin Account Borrow

        #region Margin Account Repay

        /// <summary>
        /// Repay loan for margin account.
        /// </summary>
        /// <param name="asset">The asset being repay, e.g., BTC</param>
        /// <param name="amount">The amount to be borrow</param>
        /// <param name="isIsolated">For isolated margin or not</param>
        /// <param name="symbol">The isolated symbol</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Transaction Id</returns>
        public async Task<WebCallResult<BinanceTransaction>> RepayAsync(string asset, decimal amount, bool? isIsolated = null, string? symbol = null, int? receiveWindow = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));

            var parameters = new Dictionary<string, object>
            {
                { "asset", asset },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
            };
            parameters.AddOptionalParameter("isIsolated", isIsolated?.ToString().ToLower());
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceTransaction>(
                UriClient.GetBaseAddress() + GetUriString.Combine(marginRepayEndpoint, marginApi, marginVersion), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Margin Account Repay

        #region Get Transfer History

        /// <summary>
        /// Get history of transfers
        /// </summary>
        /// <param name="direction">The direction of the the transfers to retrieve</param>
        /// <param name="page">Results page</param>
        /// <param name="startTime">Filter by startTime from</param>
        /// <param name="endTime">Filter by endTime from</param>
        /// <param name="limit">Limit of the amount of results</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of transfers</returns>
        public async Task<WebCallResult<BinanceQueryRecords<BinanceTransferHistory>>> GetTransferHistoryAsync(TransferDirection direction, int? page = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 100);

            var parameters = new Dictionary<string, object>
            {
                { "direction", JsonConvert.SerializeObject(direction, new TransferDirectionConverter(false)) },
            };
            parameters.AddOptionalParameter("size", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("current", page?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("startTime", startTime.HasValue ? JsonConvert.SerializeObject(startTime.Value.Ticks, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("endTime", endTime.HasValue ? JsonConvert.SerializeObject(endTime.Value.Ticks, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceQueryRecords<BinanceTransferHistory>>(
                UriClient.GetBaseAddress() + GetUriString.Combine(transferHistoryEndpoint, marginApi, marginVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Get Transfer History

        #region Query Loan Record

        /// <summary>
        /// Query loan records
        /// </summary>
        /// <param name="asset">The records asset</param>
        /// <param name="transactionId">The id of loan transaction</param>
        /// <param name="startTime">Time to start getting records from</param>
        /// <param name="endTime">Time to stop getting records to</param>
        /// <param name="current">Number of page records</param>
        /// <param name="isolatedSymbol">Filter by isolated symbol</param>
        /// <param name="limit">The records count size need show</param>
        /// <param name="archived">Set to true for archived data from 6 months ago</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Loan records</returns>
        public async Task<WebCallResult<BinanceQueryRecords<BinanceLoan>>> GetLoansAsync(string asset, long? transactionId = null, DateTime? startTime = null, DateTime? endTime = null, int? current = 1, int? limit = 10, string? isolatedSymbol = null, bool? archived = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            limit?.ValidateIntBetween(nameof(limit), 1, 100);

            var parameters = new Dictionary<string, object>
            {
                { "asset", asset },
            };
            parameters.AddOptionalParameter("txId", transactionId?.ToString(CultureInfo.InvariantCulture));

            // TxId or startTime must be sent. txId takes precedence.
            if (!transactionId.HasValue)
            {
                parameters.AddOptionalParameter("startTime", startTime != null ? TimeHelper.ToUnixTimestamp(startTime.Value.Ticks).ToString(CultureInfo.InvariantCulture) : null);
            }

            parameters.AddOptionalParameter("endTime", endTime != null ? TimeHelper.ToUnixTimestamp(endTime.Value.Ticks).ToString(CultureInfo.InvariantCulture) : null);
            parameters.AddOptionalParameter("current", current?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("size", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("archived", archived);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceQueryRecords<BinanceLoan>>(
                UriClient.GetBaseAddress() + GetUriString.Combine(getLoanEndpoint, marginApi, marginVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Query Loan Record

        #region Query Repay Record

        /// <summary>
        /// Query repay records
        /// </summary>
        /// <param name="asset">The records asset</param>
        /// <param name="transactionId">The id of repay transaction</param>
        /// <param name="startTime">Time to start getting records from</param>
        /// <param name="endTime">Time to stop getting records to</param>
        /// <param name="current">Filter by number</param>
        /// <param name="isolatedSymbol">Filter by isolated symbol</param>
        /// <param name="size">The records count size need show</param>
        /// <param name="archived">Set to true for archived data from 6 months ago</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Repay records</returns>
        public async Task<WebCallResult<BinanceQueryRecords<BinanceRepay>>> GetRepaysAsync(string asset, long? transactionId = null, DateTime? startTime = null, DateTime? endTime = null, int? current = null, int? size = null, string? isolatedSymbol = null, bool? archived = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));

            var parameters = new Dictionary<string, object>
            {
                { "asset", asset },
            };
            parameters.AddOptionalParameter("txId", transactionId?.ToString(CultureInfo.InvariantCulture));

            // TxId or startTime must be sent. txId takes precedence.
            if (!transactionId.HasValue)
            {
                parameters.AddOptionalParameter("startTime", startTime != null ? TimeHelper.ToUnixTimestamp(startTime.Value.Ticks).ToString(CultureInfo.InvariantCulture) : null);
            }

            parameters.AddOptionalParameter("endTime", endTime != null ? TimeHelper.ToUnixTimestamp(endTime.Value.Ticks).ToString(CultureInfo.InvariantCulture) : null);
            parameters.AddOptionalParameter("current", current?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("size", size?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("isolatedSymbol", isolatedSymbol);
            parameters.AddOptionalParameter("archived", archived);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceQueryRecords<BinanceRepay>>(
                UriClient.GetBaseAddress() + GetUriString.Combine(getRepayEndpoint, marginApi, marginVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Query Repay Record

        #region Get Interest History

        /// <summary>
        /// Get history of interest
        /// </summary>
        /// <param name="asset">Filter by asset</param>
        /// <param name="current">Results page</param>
        /// <param name="startTime">Filter by startTime from</param>
        /// <param name="endTime">Filter by endTime from</param>
        /// <param name="isolatedSymbol">Filter by isolated symbol</param>
        /// <param name="limit">Limit of the amount of results</param>
        /// <param name="archived">Set to true for archived data from 6 months ago</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of interest events</returns>
        public async Task<WebCallResult<BinanceQueryRecords<BinanceInterestHistory>>> GetInterestHistoryAsync(string? asset = null, int? current = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, string? isolatedSymbol = null, bool? archived = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 100);

            var parameters = new Dictionary<string, object>
            {
            };
            parameters.AddOptionalParameter("asset", asset);
            parameters.AddOptionalParameter("size", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("current", current?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("isolatedSymbol", isolatedSymbol);
            parameters.AddOptionalParameter("archived", archived);
            parameters.AddOptionalParameter("startTime", startTime.HasValue ? JsonConvert.SerializeObject(startTime.Value.Ticks, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("endTime", endTime.HasValue ? JsonConvert.SerializeObject(endTime.Value.Ticks, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceQueryRecords<BinanceInterestHistory>>(
                UriClient.GetBaseAddress() + GetUriString.Combine(interestHistoryEndpoint, marginApi, marginVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Get Interest History

        #region Get Interest Rate History

        /// <summary>
        /// Get history of interest rate
        /// </summary>
        /// <param name="asset">Filter by asset</param>
        /// <param name="vipLevel">Vip level</param>
        /// <param name="startTime">Filter by startTime from</param>
        /// <param name="endTime">Filter by endTime from</param>
        /// <param name="limit">Limit of the amount of results</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of interest rate</returns>
        public async Task<WebCallResult<IEnumerable<BinanceInterestRateHistory>>> GetInterestRateHistoryAsync(string asset, string? vipLevel = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            asset?.ValidateNotNull(nameof(asset));
            limit?.ValidateIntBetween(nameof(limit), 1, 100);

            var parameters = new Dictionary<string, object>
            {
                { "asset", asset! }
            };
            parameters.AddOptionalParameter("vipLevel", vipLevel?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("size", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("startTime", startTime.HasValue ? JsonConvert.SerializeObject(startTime.Value.Ticks, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("endTime", endTime.HasValue ? JsonConvert.SerializeObject(endTime.Value.Ticks, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceInterestRateHistory>>(
                UriClient.GetBaseAddress() + GetUriString.Combine(interestRateHistoryEndpoint, marginApi, marginVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Get Interest Rate History

        #region Get Interest Rate Margin Data

        /// <summary>
        /// Get interest margin data
        /// </summary>
        /// <param name="asset">Filter by asset</param>
        /// <param name="vipLevel">Vip level</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        public async Task<WebCallResult<IEnumerable<BinanceInterestMarginData>>> GetInterestMarginDataAsync(string? asset = null, string? vipLevel = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            asset?.ValidateNotNull(nameof(asset));

            var parameters = new Dictionary<string, object>
            {
            };

            parameters.AddOptionalParameter("coin", asset);
            parameters.AddOptionalParameter("vipLevel", vipLevel?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceInterestMarginData>>(
                UriClient.GetBaseAddress() + GetUriString.Combine(interestMarginDataEndpoint, marginApi, marginVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Get Interest Rate Margin Data

        #region Get Interest Rate Isolated Margin Data

        /// <summary>
        /// Get interest isolated margin data
        /// </summary>
        /// <param name="asset">Filter by asset</param>
        /// <param name="vipLevel">Vip level</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        public async Task<WebCallResult<IEnumerable<BinanceInterestIsolatedMarginData>>> GetInterestIsolatedMarginDataAsync(string? asset = null, string? vipLevel = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            asset?.ValidateNotNull(nameof(asset));

            var parameters = new Dictionary<string, object>
            {
            };

            parameters.AddOptionalParameter("coin", asset);
            parameters.AddOptionalParameter("vipLevel", vipLevel?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceInterestIsolatedMarginData>>(
                UriClient.GetBaseAddress() + GetUriString.Combine(interestIsolatedMarginDataEndpoint, marginApi, marginVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Get Interest Rate Isolated Margin Data

        #region Get Force Liquidation Record

        /// <summary>
        /// Get history of forced liquidations
        /// </summary>
        /// <param name="page">Results page</param>
        /// <param name="startTime">Filter by startTime from</param>
        /// <param name="endTime">Filter by endTime from</param>
        /// <param name="isolatedSymbol">Filter by isolated symbol</param>
        /// <param name="limit">Limit of the amount of results</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of forced liquidations</returns>
        public async Task<WebCallResult<BinanceQueryRecords<BinanceForcedLiquidation>>> GetForceLiquidationHistoryAsync(int? page = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, string? isolatedSymbol = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 100);

            var parameters = new Dictionary<string, object>
            {
            };
            parameters.AddOptionalParameter("size", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("page", page?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("isolatedSymbol", isolatedSymbol);
            parameters.AddOptionalParameter("startTime", startTime.HasValue ? JsonConvert.SerializeObject(startTime.Value.Ticks, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("endTime", endTime.HasValue ? JsonConvert.SerializeObject(endTime.Value.Ticks, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceQueryRecords<BinanceForcedLiquidation>>(
                UriClient.GetBaseAddress() + GetUriString.Combine(forceLiquidationHistoryEndpoint, marginApi, marginVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Get Force Liquidation Record

        #region Query Margin Account Details

        /// <summary>
        /// Query margin account details
        /// </summary>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The margin account information</returns>
        public async Task<WebCallResult<BinanceMarginAccount>> GetMarginAccountInfoAsync(long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
            };
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceMarginAccount>(
                UriClient.GetBaseAddress() + GetUriString.Combine(marginAccountInfoEndpoint, marginApi, marginVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Query Margin Account Details

        #region Query Max Borrow

        /// <summary>
        /// Query max borrow amount
        /// </summary>
        /// <param name="asset">The records asset</param>
        /// <param name="isolatedSymbol">The isolated symbol</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Return max amount</returns>
        public async Task<WebCallResult<BinanceMarginAmount>> GetMaxBorrowAmountAsync(string asset, string? isolatedSymbol = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));

            var parameters = new Dictionary<string, object>
            {
                { "asset", asset },
            };

            parameters.AddOptionalParameter("isolatedSymbol", isolatedSymbol);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceMarginAmount>(
                UriClient.GetBaseAddress() + GetUriString.Combine(maxBorrowableEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Query Max Borrow

        #region Query Max Transfer-Out Amount

        /// <summary>
        /// Query max transfer-out amount
        /// </summary>
        /// <param name="asset">The records asset</param>
        /// <param name="isolatedSymbol">The isolated symbol</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Return max amount</returns>
        public async Task<WebCallResult<decimal>> GetMaxTransferAmountAsync(string asset, string? isolatedSymbol = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));

            var parameters = new Dictionary<string, object>
            {
                { "asset", asset },
            };

            parameters.AddOptionalParameter("isolatedSymbol", isolatedSymbol);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            var result = await _baseClient.SendRequestInternal<BinanceMarginAmount>(
                UriClient.GetBaseAddress() + GetUriString.Combine(maxTransferableEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);

            if (!result)
                return new WebCallResult<decimal>(result.ResponseStatusCode, result.ResponseHeaders, 0, result.Error);

            return result.As(result.Data.Amount);
        }

        #endregion Query Max Transfer-Out Amount

        /// <summary>
        /// Get history of transfer to and from the isolated margin account
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="asset">Filter by asset</param>
        /// <param name="from">Filter by direction</param>
        /// <param name="to">Filter by direction</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="current">Current page</param>
        /// <param name="limit">Page size</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<BinanceQueryRecords<BinanceIsolatedMarginTransfer>>>
            GetIsolatedMarginAccountTransferHistoryAsync(string symbol, string? asset = null,
                IsolatedMarginTransferDirection? from = null, IsolatedMarginTransferDirection? to = null,
                DateTime? startTime = null, DateTime? endTime = null, int? current = 1, int? limit = 10,
                int? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                {"symbol", symbol},
            };

            parameters.AddOptionalParameter("asset", asset);
            parameters.AddOptionalParameter("from",
                !from.HasValue
                    ? null
                    : JsonConvert.SerializeObject(from, new IsolatedMarginTransferDirectionConverter(false)));
            parameters.AddOptionalParameter("to",
                !to.HasValue
                    ? null
                    : JsonConvert.SerializeObject(to, new IsolatedMarginTransferDirectionConverter(false)));
            parameters.AddOptionalParameter("startTime",
                startTime != null
                    ? TimeHelper.ToUnixTimestamp(startTime.Value.Ticks).ToString(CultureInfo.InvariantCulture)
                    : null);
            parameters.AddOptionalParameter("endTime",
                endTime != null
                    ? TimeHelper.ToUnixTimestamp(endTime.Value.Ticks).ToString(CultureInfo.InvariantCulture)
                    : null);
            parameters.AddOptionalParameter("current", current?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("size", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow",
                receiveWindow?.ToString(CultureInfo.InvariantCulture) ??
                _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceQueryRecords<BinanceIsolatedMarginTransfer>>(
                UriClient.GetBaseAddress() + GetUriString.Combine(isolatedMarginTransferHistoryEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Isolated margin account info
        /// </summary>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<BinanceIsolatedMarginAccount>> GetIsolatedMarginAccountAsync(
            int? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
            };

            parameters.AddOptionalParameter("recvWindow",
                receiveWindow?.ToString(CultureInfo.InvariantCulture) ??
                _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceIsolatedMarginAccount>(
                UriClient.GetBaseAddress() + GetUriString.Combine(isolatedMarginAccountEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Disabled an isolated margin account
        /// </summary>
        /// <param name="symbol">Symbol to enable isoldated margin account for</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<CreateIsolatedMarginAccountResult>> DisableIsolatedMarginAccountAsync(string symbol,
            int? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                {"symbol", symbol}
            };

            parameters.AddOptionalParameter("recvWindow",
                receiveWindow?.ToString(CultureInfo.InvariantCulture) ??
                _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<CreateIsolatedMarginAccountResult>(
                UriClient.GetBaseAddress() + GetUriString.Combine(isolatedMarginAccountEndpoint, "sapi", "1"), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Enable an isolated margin account
        /// </summary>
        /// <param name="symbol">Symbol to enable isoldated margin account for</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<CreateIsolatedMarginAccountResult>> EnableIsolatedMarginAccountAsync(string symbol,
            int? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                {"symbol", symbol}
            };

            parameters.AddOptionalParameter("recvWindow",
                receiveWindow?.ToString(CultureInfo.InvariantCulture) ??
                _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<CreateIsolatedMarginAccountResult>(
                UriClient.GetBaseAddress() + GetUriString.Combine(isolatedMarginAccountEndpoint, "sapi", "1"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get max number of enabled isolated margin accounts
        /// </summary>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IsolatedMarginAccountLimit>> GetEnabledIsolatedMarginAccountLimitAsync(
            int? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
            };

            parameters.AddOptionalParameter("recvWindow",
                receiveWindow?.ToString(CultureInfo.InvariantCulture) ??
                _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IsolatedMarginAccountLimit>(
                UriClient.GetBaseAddress() + GetUriString.Combine(isolatedMarginAccountLimitEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Transfer from or to isolated margin account
        /// </summary>
        /// <param name="asset">The asset</param>
        /// <param name="symbol">Isolated symbol</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<BinanceTransaction>> IsolatedMarginAccountTransferAsync(string asset,
            string symbol, IsolatedMarginTransferDirection from, IsolatedMarginTransferDirection to, decimal amount,
            int? receiveWindow = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            symbol.ValidateNotNull(nameof(symbol));

            var parameters = new Dictionary<string, object>
            {
                {"asset", asset},
                {"symbol", symbol},
                {"transFrom", JsonConvert.SerializeObject(from, new IsolatedMarginTransferDirectionConverter(false))},
                {"transTo", JsonConvert.SerializeObject(to, new IsolatedMarginTransferDirectionConverter(false))},
                {"amount", amount.ToString(CultureInfo.InvariantCulture)},
            };
            parameters.AddOptionalParameter("recvWindow",
                receiveWindow?.ToString(CultureInfo.InvariantCulture) ??
                _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceTransaction>(
                UriClient.GetBaseAddress() + GetUriString.Combine(transferIsolatedMarginAccountEndpoint, "sapi", "1"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }
    }
}
