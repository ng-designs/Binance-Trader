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

using BinanceAPI.Converters;
using Newtonsoft.Json;
using System;

namespace BinanceAPI.Objects.Spot.MarketData
{
    /// <summary>
    /// 24hr ticker
    /// </summary>
    public class Binance24HPrice
    {
        /// <summary>
        /// The symbol the price is for
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The price change percentage
        /// </summary>
        [JsonProperty("priceChangePercent")]
        public decimal PriceChangePercent { get; set; }

        /// <summary>
        /// The weighted average price
        /// </summary>
        [JsonProperty("weightedAvgPrice")]
        public decimal WeightedAveragePrice { get; set; }

        /// <summary>
        /// The previous close price
        /// </summary>
        [JsonProperty("prevClosePrice")]
        public decimal PreviousClosePrice { get; set; }

        /// <summary>
        /// The last price
        /// </summary>
        [JsonProperty("lastPrice")]
        public decimal LastPrice { get; set; }

        /// <summary>
        /// The last quantity
        /// </summary>
        [JsonProperty("lastQty")]
        public decimal LastQuantity { get; set; }

        /// <summary>
        /// The bid price
        /// </summary>
        [JsonProperty("bidPrice")]
        public decimal BidPrice { get; set; }

        /// <summary>
        /// The bid quantity
        /// </summary>
        [JsonProperty("bidQty")]
        public decimal BidQuantity { get; set; }

        /// <summary>
        /// The ask price
        /// </summary>
        [JsonProperty("askPrice")]
        public decimal AskPrice { get; set; }

        /// <summary>
        /// The ask quantity
        /// </summary>
        [JsonProperty("askQty")]
        public decimal AskQuantity { get; set; }

        /// <summary>
        /// The open price
        /// </summary>
        [JsonProperty("openPrice")]
        public decimal OpenPrice { get; set; }

        /// <summary>
        /// The high price
        /// </summary>
        [JsonProperty("highPrice")]
        public decimal HighPrice { get; set; }

        /// <summary>
        /// The low price
        /// </summary>
        [JsonProperty("lowPrice")]
        public decimal LowPrice { get; set; }

        /// <summary>
        /// The volume
        /// </summary>
        [JsonProperty("volume")]
        public decimal Volume { get; set; }

        /// <summary>
        /// The quote volume
        /// </summary>
        [JsonProperty("quoteVolume")]
        public decimal QuoteVolume { get; set; }

        /// <summary>
        /// The open time
        /// </summary>
        [JsonProperty("openTime")]
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// The close time
        /// </summary>
        [JsonProperty("closeTime")]
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime CloseTime { get; set; }

        /// <summary>
        /// The first trade id
        /// </summary>
        [JsonProperty("firstId")]
        public long FirstTradeId { get; set; }

        /// <summary>
        /// The last trade id
        /// </summary>
        [JsonProperty("lastId")]
        public long LastTradeId { get; set; }

        /// <summary>
        /// The trade count
        /// </summary>
        [JsonProperty("count")]
        public long TradeCount { get; set; }
    }
} 