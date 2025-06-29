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
using BinanceAPI.Enums;
using BinanceAPI.Objects;
using BinanceAPI.Objects.Spot.MarketData;
using BinanceAPI.Objects.Spot.WalletData;
using BinanceAPI.Requests;
using BinanceAPI.UriBase;
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

        #region Test connectivity

        /// <summary>
        /// Pings the Binance API and returns the round trip time
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Round trip time in milliseconds</returns>
        public async Task<WebCallResult<long>> PingAsync(CancellationToken ct = default)
        {
            Pong.Restart();
            var result = await _baseClient.SendRequestInternal<object>(UriClient.GetBaseAddress() + GetUriString.Combine(pingEndpoint, api, publicVersion), HttpMethod.Get, ct, new Dictionary<string, object>()).ConfigureAwait(false);
            Pong.Stop();
            return result.As(Pong.ElapsedMilliseconds);
        }

        #endregion Test connectivity

        #region Check server time

        /// <summary>
        /// Requests the server for the local time. This function also determines the offset between server and local time and uses this for subsequent API calls
        /// </summary>
        /// <param name="resetAutoTimestamp">Whether the response should be used to reset the auto timestamp calculation</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Server time</returns>
        public async Task<WebCallResult<DateTime>> GetServerTimeAsync(bool resetAutoTimestamp = false, CancellationToken ct = default)
        {
            var url = UriClient.GetBaseAddress() + GetUriString.Combine("time", api, publicVersion);
            var result = await _baseClient.SendRequestInternal<BinanceCheckTime>(url, HttpMethod.Get, ct, new Dictionary<string, object>()).ConfigureAwait(false);
            if (result)
            {
                if (resetAutoTimestamp)
                    _baseClient.ResetAutoTimestamp();
            }
            return result.As(result.Data?.ServerTime ?? default);
        }

        #endregion Check server time

        #region Exchange info

        /// <summary>
        /// Get's information about the exchange including rate limits and information on the provided symbols
        /// </summary>
        /// <param name="symbols">The symbols to get information for</param>
        /// <param name="permissions">The account type</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Exchange info</returns>
        public async Task<WebCallResult<BinanceExchangeInfo>> GetExchangeInfoAsync(IEnumerable<string>? symbols = null, AccountType[]? permissions = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();

            if (symbols?.Count() > 1)
            {
                parameters.Add(permissions != null ? "permissions" : "symbols", JsonConvert.SerializeObject(symbols));
            }
            else if (symbols?.Any() == true)
            {
                parameters.Add(permissions != null ? "permissions" : "symbol", symbols.First());
            }

            if (permissions?.Count() > 1)
            {
                List<string> list = new();
                foreach (var permission in permissions)
                {
                    list.Add(permission.ToString().ToUpper());
                }

                parameters.Add("permissions", JsonConvert.SerializeObject(list));
            }
            else if (permissions?.Any() == true)
            {
                parameters.Add("permissions", permissions.First().ToString().ToUpper());
            }

            return await _baseClient.SendRequestInternal<BinanceExchangeInfo>(UriClient.GetBaseAddress() + GetUriString.Combine(exchangeInfoEndpoint, api, publicVersion), HttpMethod.Get, ct, parameters: parameters, arraySerialization: ArrayParametersSerialization.Array).ConfigureAwait(false);
        }

        /// <summary>
        /// Get's information about the exchange including rate limits and information on the provided symbols
        /// </summary>
        /// <param name="symbols">The symbols to get information for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Exchange info</returns>
        public async Task<WebCallResult<BinanceExchangeInfo>> GetExchangeInfoAsync(IEnumerable<string> symbols, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();

            if (symbols.Count() > 1)
            {
                parameters.Add("symbols", JsonConvert.SerializeObject(symbols));
            }
            else if (symbols.Any())
            {
                parameters.Add("symbol", symbols.First());
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

        #endregion Exchange info

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
