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

using BinanceAPI.ClientHosts;
using BinanceAPI.Converters;
using BinanceAPI.Enums;
using BinanceAPI.Objects;
using BinanceAPI.Objects.Spot.MarketData;
using BinanceAPI.Objects.Spot.MarketStream;
using BinanceAPI.Objects.Spot.WalletData;
using BinanceAPI.Requests;
using BinanceAPI.Time;
using BinanceAPI.UriBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceAPI.SubClients.Spot
{
    /// <summary>
    /// Spot market endpoints
    /// </summary>
    public class BinanceClientSpotMarket
    {
        private const string orderBookEndpoint = "depth";
        private const string aggregatedTradesEndpoint = "aggTrades";
        private const string recentTradesEndpoint = "trades";
        private const string historicalTradesEndpoint = "historicalTrades";
        private const string klinesEndpoint = "klines";
        private const string price24HEndpoint = "ticker/24hr";
        private const string allPricesEndpoint = "ticker/price";
        private const string bookPricesEndpoint = "ticker/bookTicker";
        private const string averagePriceEndpoint = "avgPrice";
        private const string tradeFeeEndpoint = "asset/tradeFee";

        private const string api = "api";
        private const string publicVersion = "3";

        private readonly BinanceClientHost _baseClient;

        internal BinanceClientSpotMarket(BinanceClientHost baseClient)
        {
            _baseClient = baseClient;
        }

        #region Order Book

        /// <summary>
        /// Gets the order book for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The order book for the symbol</returns>
        public async Task<WebCallResult<BinanceOrderBook>> GetOrderBookAsync(string symbol, int? limit = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));

            var result = await _baseClient.SendRequestInternal<BinanceOrderBook>(UriClient.GetBaseAddress() + GetUriString.Combine(orderBookEndpoint, api, publicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
            if (result)
                result.Data.Symbol = symbol;
            return result;
        }

        #endregion Order Book

        #region Recent trades

        /// <summary>
        /// Gets the recent trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get recent trades for</param>
        /// <param name="limit">Result limit</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Recent trades</returns>
        public async Task<WebCallResult<IEnumerable<BinanceRecentTrade>>> GetRecentTradesAsync(string symbol, int? limit = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceRecentTrade>>(UriClient.GetBaseAddress() + GetUriString.Combine(recentTradesEndpoint, api, publicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        #endregion Recent trades

        #region Historical trades

        /// <summary>
        /// Gets the historical trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get recent trades for</param>
        /// <param name="limit">Result limit</param>
        /// <param name="fromId">From which trade id on results should be retrieved</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Historical trades</returns>
        public async Task<WebCallResult<IEnumerable<BinanceRecentTrade>>> GetHistoricalTradesAsync(string symbol, int? limit = null, long? fromId = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("fromId", fromId?.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceRecentTrade>>(UriClient.GetBaseAddress() + GetUriString.Combine(historicalTradesEndpoint, api, publicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        #endregion Historical trades

        #region Aggregated trades

        /// <summary>
        /// Gets compressed, aggregate trades. Trades that fill at the time, from the same order, with the same price will have the quantity aggregated.
        /// </summary>
        /// <param name="symbol">The symbol to get the trades for</param>
        /// <param name="fromId">ID to get aggregate trades from INCLUSIVE.</param>
        /// <param name="startTime">Time to start getting trades from</param>
        /// <param name="endTime">Time to stop getting trades from</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The aggregated trades list</returns>
        public async Task<WebCallResult<IEnumerable<BinanceAggregatedTrade>>> GetAggregatedTradesAsync(string symbol, long? fromId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };
            parameters.AddOptionalParameter("fromId", fromId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("startTime", startTime?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("endTime", endTime?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceAggregatedTrade>>(UriClient.GetBaseAddress() + GetUriString.Combine(aggregatedTradesEndpoint, api, publicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        #endregion Aggregated trades

        #region Kline/Candlestick data

        /// <summary>
        /// Gets the candlestick data for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="interval">The candlestick timespan</param>
        /// <param name="startTime">Start time to get candlestick data</param>
        /// <param name="endTime">End time to get candlestick data</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The candlestick data for the provided symbol</returns>
        public async Task<WebCallResult<IEnumerable<BinanceSpotKline>>> GetKlinesAsync(string symbol, KlineInterval interval, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "interval", JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)) }
            };
            parameters.AddOptionalParameter("startTime", startTime?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("endTime", endTime?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));

            var result = await _baseClient.SendRequestInternal<IEnumerable<object[]>>(UriClient.GetBaseAddress() + GetUriString.Combine(klinesEndpoint, api, publicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
            return result.As(result.Data?.Select(item => new BinanceSpotKline(item)) ?? Array.Empty<BinanceSpotKline>());
        }

        #endregion Kline/Candlestick data

        #region 24hr ticker

        /// <summary>
        /// Gets the 24hr ticker for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the ticker for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The 24hr ticker for the symbol</returns>
        public async Task<WebCallResult<Binance24HPrice>> GetTicker24HAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };

            return await _baseClient.SendRequestInternal<Binance24HPrice>(UriClient.GetBaseAddress() + GetUriString.Combine(price24HEndpoint, api, publicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the 24hr ticker for all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>24hr ticker for all symbols</returns>
        public async Task<WebCallResult<IEnumerable<Binance24HPrice>>> GetTickers24HAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestInternal<IEnumerable<Binance24HPrice>>(UriClient.GetBaseAddress() + GetUriString.Combine(price24HEndpoint, api, publicVersion), HttpMethod.Get, ct, new Dictionary<string, object>()).ConfigureAwait(false);
        }

        #endregion 24hr ticker

        #region Symbol price ticker

        /// <summary>
        /// Gets the price for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get price for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Price of symbol</returns>
        public async Task<WebCallResult<BinancePrice>> GetTickerPriceAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };

            return await _baseClient.SendRequestInternal<BinancePrice>(UriClient.GetBaseAddress() + GetUriString.Combine(allPricesEndpoint, api, publicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the price for all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of prices</returns>
        public async Task<WebCallResult<IEnumerable<BinancePrice>>> GetTickerPricesAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestInternal<IEnumerable<BinancePrice>>(UriClient.GetBaseAddress() + GetUriString.Combine(allPricesEndpoint, api, publicVersion), HttpMethod.Get, ct, new Dictionary<string, object>()).ConfigureAwait(false);
        }

        #endregion Symbol price ticker

        #region Symbol order book ticker

        /// <summary>
        /// Gets the best price/quantity on the order book for a symbol.
        /// </summary>
        /// <param name="symbol">Symbol to get book price for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Book price</returns>
        public async Task<WebCallResult<BinanceBookPrice>> GetTickerBookPriceAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };

            return await _baseClient.SendRequestInternal<BinanceBookPrice>(UriClient.GetBaseAddress() + GetUriString.Combine(bookPricesEndpoint, api, publicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the best price/quantity on the order book for all symbols.
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of book prices</returns>
        public async Task<WebCallResult<IEnumerable<BinanceBookPrice>>> GetTickerBookPricesAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestInternal<IEnumerable<BinanceBookPrice>>(UriClient.GetBaseAddress() + GetUriString.Combine(bookPricesEndpoint, api, publicVersion), HttpMethod.Get, ct, new Dictionary<string, object>()).ConfigureAwait(false);
        }

        #endregion Symbol order book ticker

        #region Average price

        /// <summary>
        /// Gets the average price for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Average price</returns>
        public async Task<WebCallResult<BinanceAveragePrice>> GetCurrentAvgPriceAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };

            return await _baseClient.SendRequestInternal<BinanceAveragePrice>(UriClient.GetBaseAddress() + GetUriString.Combine(averagePriceEndpoint, api, publicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        #endregion Average price

        #region GetTradeFee

        /// <summary>
        /// Gets the trade fee for a symbol
        /// </summary>
        /// <param name="symbol">Symbol to get withdrawal fee for</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Trade fees</returns>
        public async Task<WebCallResult<IEnumerable<BinanceTradeFee>>> GetTradeFeeAsync(string? symbol = null, int? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
            };

            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            var result = await _baseClient.SendRequestInternal<IEnumerable<BinanceTradeFee>>(UriClient.GetBaseAddress() + GetUriString.Combine(tradeFeeEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
            return result;
        }

        #endregion GetTradeFee

        #region GetPrices

        /// <summary>
        /// Gets the price for all symbols (alias for GetTickerPricesAsync)
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of prices</returns>
        public async Task<WebCallResult<IEnumerable<BinancePrice>>> GetPricesAsync(CancellationToken ct = default)
        {
            return await GetTickerPricesAsync(ct);
        }

        #endregion GetPrices
    }
}
