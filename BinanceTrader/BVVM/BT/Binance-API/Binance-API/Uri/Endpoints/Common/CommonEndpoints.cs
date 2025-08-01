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

using BinanceAPI.Requests;

#pragma warning disable CS1591 // WIP

namespace BinanceAPI.UriBase
{
    public class CommonEndpoints
    {
        public const string GET_SERVER_TIME = "time";
        public const string PING = "ping";
        public const string EXCHANGE_INFO = "exchangeInfo";

        public const string API = "api";
        public const string SIGNED = "3";

        public CommonEndpoints()
        {
            API_V3_TIME_SyncTime = GetUriString.Combine(GET_SERVER_TIME, API, SIGNED);
            API_V3_PING_Ping = GetUriString.Combine(PING, API, SIGNED);
            API_V3_EXCHANGE_INFO_ExchangeInfo = GetUriString.Combine(EXCHANGE_INFO, API, SIGNED);
        }

        /// <summary>
        /// https://binance-docs.github.io/apidocs/spot/en/#check-server-time
        /// </summary>
        public readonly string API_V3_TIME_SyncTime;

        /// <summary>
        /// https://binance-docs.github.io/apidocs/spot/en/#test-connectivity
        /// </summary>
        public readonly string API_V3_PING_Ping;

        /// <summary>
        /// https://binance-docs.github.io/apidocs/spot/en/#exchange-information
        /// </summary>
        public readonly string API_V3_EXCHANGE_INFO_ExchangeInfo;
    }
}
