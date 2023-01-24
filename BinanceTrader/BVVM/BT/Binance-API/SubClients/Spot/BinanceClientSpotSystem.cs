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
using BinanceAPI.Enums;
using BinanceAPI.Objects;
using BinanceAPI.Objects.Spot.MarketData;
using BinanceAPI.Objects.Spot.WalletData;
using BinanceAPI.Requests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceAPI.SubClients.Spot
{
    /// <summary>
    /// Spot system endpoints
    /// </summary>
    public class BinanceClientSpotSystem
    {
        private readonly Stopwatch Pong = new Stopwatch();

        private const string api = "api";
        private const string publicVersion = "3";

        private const string pingEndpoint = "ping";

        private const string exchangeInfoEndpoint = "exchangeInfo";
        private const string systemStatusEndpoint = "system/status";

        private readonly BinanceClientHost _baseClient;

        internal BinanceClientSpotSystem(BinanceClientHost baseClient)
        {
            _baseClient = baseClient;
        }

        #region Test Connectivity

        /// <summary>
        /// Pings the Binance API
        /// </summary>
        /// <returns>Ping as Ticks</returns>
        public async Task<WebCallResult<long>> PingAsync(CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            Pong.Restart();
            var result = await _baseClient.SendRequestInternal<object>(UriClient.GetBaseAddress() + GetUriString.Combine(pingEndpoint, api, publicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
            return new WebCallResult<long>(result.ResponseStatusCode, result.ResponseHeaders, Pong.ElapsedTicks, result.Error);
        }

        #endregion Test Connectivity

        #region Exchange Information

        /// <summary>
        /// Gets information about the exchange including rate limits and symbol list
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Exchange info</returns>
        public Task<WebCallResult<BinanceExchangeInfo>> GetExchangeInfoAsync(CancellationToken ct = default)
             => GetExchangeInfoAsync(Array.Empty<string>(), false, ct);

        /// <summary>
        /// Get's information about the exchange including rate limits and information on the provided symbol
        /// </summary>
        /// <param name="symbol">Symbol to get data for token</param>
        /// <param name="permissions">true to use account type permissions instead of symbols</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Exchange info</returns>
        public Task<WebCallResult<BinanceExchangeInfo>> GetExchangeInfoAsync(string symbol, bool permissions = false, CancellationToken ct = default)
             => GetExchangeInfoAsync(new string[] { symbol }, permissions, ct);

        /// <summary>
        /// Get's information about the exchange including rate limits and information on the provided symbol
        /// </summary>
        /// <param name="permissions">account type</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Exchange info</returns>
        public Task<WebCallResult<BinanceExchangeInfo>> GetExchangeInfoAsync(AccountType permissions, CancellationToken ct = default)
             => GetExchangeInfoAsync(new AccountType[] { permissions }, ct);

        /// <summary>
        /// Get's information about the exchange including rate limits and information on the provided symbols
        /// </summary>
        /// <param name="symbols">Symbols to get data for token</param>
        /// <param name="permissions">true to use account type permissions instead of symbols</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Exchange info</returns>
        public async Task<WebCallResult<BinanceExchangeInfo>> GetExchangeInfoAsync(IEnumerable<string> symbols, bool permissions = false, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();

            if (symbols.Count() > 1)
            {
                parameters.Add(permissions ? "permissions" : "symbols", JsonConvert.SerializeObject(symbols));
            }
            else if (symbols.Any())
            {
                parameters.Add(permissions ? "permissions" : "symbol", symbols.First());
            }

            return await _baseClient.SendRequestInternal<BinanceExchangeInfo>(UriClient.GetBaseAddress() + GetUriString.Combine(exchangeInfoEndpoint, api, publicVersion), HttpMethod.Get, ct, parameters: parameters, arraySerialization: ArrayParametersSerialization.Array).ConfigureAwait(false);
        }

        /// <summary>
        /// Get's information about the exchange including rate limits and information on the provided symbols
        /// </summary>
        /// <param name="permissions">account type</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Exchange info</returns>
        public async Task<WebCallResult<BinanceExchangeInfo>> GetExchangeInfoAsync(AccountType[] permissions, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();

            if (permissions.Count() > 1)
            {
                List<string> list = new();
                foreach (var permission in permissions)
                {
                    list.Add(permission.ToString().ToUpper());
                }

                parameters.Add("permissions", JsonConvert.SerializeObject(list));
            }
            else if (permissions.Any())
            {
                parameters.Add("permissions", permissions.First().ToString().ToUpper());
            }

            return await _baseClient.SendRequestInternal<BinanceExchangeInfo>(UriClient.GetBaseAddress() + GetUriString.Combine(exchangeInfoEndpoint, api, publicVersion), HttpMethod.Get, ct, parameters: parameters, arraySerialization: ArrayParametersSerialization.Array).ConfigureAwait(false);
        }

        #endregion Exchange Information

        #region System status

        /// <summary>
        /// Gets the status of the Binance platform
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The system status</returns>
        public async Task<WebCallResult<BinanceSystemStatus>> GetSystemStatusAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestInternal<BinanceSystemStatus>(UriClient.GetBaseAddress() + GetUriString.Combine(systemStatusEndpoint, "sapi", "1"), HttpMethod.Get, ct, new Dictionary<string, object>(), false).ConfigureAwait(false);
        }

        #endregion System status
    }
}
