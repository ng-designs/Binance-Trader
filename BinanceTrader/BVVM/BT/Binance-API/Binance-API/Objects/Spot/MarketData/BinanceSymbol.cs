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

using BinanceAPI.Converters;
using BinanceAPI.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BinanceAPI.Objects.Spot.MarketData
{
    /// <summary>
    /// Symbol info
    /// </summary>
    public class BinanceSymbol
    {
        /// <summary>
        /// The symbol
        /// </summary>
        [JsonProperty("symbol")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The status of the symbol
        /// </summary>
        [JsonProperty("status")]
        [JsonConverter(typeof(SymbolStatusConverter))]
        public SymbolStatus Status { get; set; }

        /// <summary>
        /// The base asset
        /// </summary>
        [JsonProperty("baseAsset")]
        public string BaseAsset { get; set; } = string.Empty;

        /// <summary>
        /// The precision of the base asset
        /// </summary>
        [JsonProperty("baseAssetPrecision")]
        public int BaseAssetPrecision { get; set; }

        /// <summary>
        /// The quote asset
        /// </summary>
        [JsonProperty("quoteAsset")]
        public string QuoteAsset { get; set; } = string.Empty;

        /// <summary>
        /// The precision of the quote asset
        /// </summary>
        [JsonProperty("quotePrecision")]
        public int QuoteAssetPrecision { get; set; }

        /// <summary>
        /// Allowed order types
        /// </summary>
        [JsonProperty("orderTypes")]
        [JsonConverter(typeof(OrderTypeConverter))]
        public IEnumerable<OrderType> OrderTypes { get; set; } = Array.Empty<OrderType>();

        /// <summary>
        /// Ice berg orders allowed
        /// </summary>
        [JsonProperty("icebergAllowed")]
        public bool IceBergAllowed { get; set; }

        /// <summary>
        /// Spot trading orders allowed
        /// </summary>
        [JsonProperty("isSpotTradingAllowed")]
        public bool IsSpotTradingAllowed { get; set; }

        /// <summary>
        /// Margin trading orders allowed
        /// </summary>
        [JsonProperty("isMarginTradingAllowed")]
        public bool IsMarginTradingAllowed { get; set; }

        /// <summary>
        /// If OCO(One Cancels Other) orders are allowed
        /// </summary>
        [JsonProperty("ocoAllowed")]
        public bool OCOAllowed { get; set; }

        /// <summary>
        /// Whether or not it is allowed to specify the quantity of a market order in the quote asset
        /// </summary>
        [JsonProperty("quoteOrderQtyMarketAllowed")]
        public bool QuoteOrderQuantityMarketAllowed { get; set; }

        /// <summary>
        /// The precision of the base asset commission
        /// </summary>
        [JsonProperty("baseCommissionPrecision")]
        public int BaseCommissionPrecision { get; set; }

        /// <summary>
        /// The precision of the quote asset commission
        /// </summary>
        [JsonProperty("quoteCommissionPrecision")]
        public int QuoteCommissionPrecision { get; set; }

        /// <summary>
        /// True if the symbol allows trailing stops
        /// </summary>
        [JsonProperty("allowTrailingStop")]
        public bool AllowTrailingStop { get; set; }

        /// <summary>
        /// True if the symbol allows Cancel Replace
        /// </summary>
        [JsonProperty("cancelReplaceAllowed")]
        public bool CancelReplaceAllowed { get; set; }

        /// <summary>
        /// Permissions types
        /// </summary>
        [JsonProperty("permissions")]
        [JsonConverter(typeof(AccountTypeConverter))]
        public IEnumerable<AccountType> Permissions { get; set; } = Array.Empty<AccountType>();

        /// <summary>
        /// Filters for order on this symbol
        /// </summary>
        [JsonProperty("filters")]
        public IEnumerable<BinanceSymbolFilter> Filters { get; set; } = Array.Empty<BinanceSymbolFilter>();

        /// <summary>
        /// Filter for max amount of iceberg parts for this symbol
        /// </summary>
        [JsonIgnore]
        public BinanceSymbolIcebergPartsFilter? IceBergPartsFilter => Filters.OfType<BinanceSymbolIcebergPartsFilter>().FirstOrDefault();

        /// <summary>
        /// Filter for max accuracy of the quantity for this symbol
        /// </summary>
        [JsonIgnore]
        public BinanceSymbolLotSizeFilter? LotSizeFilter => Filters.OfType<BinanceSymbolLotSizeFilter>().FirstOrDefault();

        /// <summary>
        /// Filter for max accuracy of the quantity for this symbol, specifically for market orders
        /// </summary>
        [JsonIgnore]
        public BinanceSymbolMarketLotSizeFilter? MarketLotSizeFilter => Filters.OfType<BinanceSymbolMarketLotSizeFilter>().FirstOrDefault();

        /// <summary>
        /// Filter for max number of orders for this symbol
        /// </summary>
        [JsonIgnore]
        public BinanceSymbolMaxOrdersFilter? MaxOrdersFilter => Filters.OfType<BinanceSymbolMaxOrdersFilter>().FirstOrDefault();

        /// <summary>
        /// Filter for max algorithmic orders for this symbol
        /// </summary>
        [JsonIgnore]
        public BinanceSymbolMaxAlgorithmicOrdersFilter? MaxAlgorithmicOrdersFilter => Filters.OfType<BinanceSymbolMaxAlgorithmicOrdersFilter>().FirstOrDefault();

        /// <summary>
        /// Filter for the minimal size of an order for this symbol
        /// </summary>
        [JsonIgnore]
        public BinanceSymbolMinNotionalFilter? MinNotionalFilter => Filters.OfType<BinanceSymbolMinNotionalFilter>().FirstOrDefault();

        /// <summary>
        /// Filter for the notional range allowed for an order on a symbol.
        /// </summary>
        [JsonIgnore]
        public BinanceSymbolNotionalFilter? NotionalFilter => Filters.OfType<BinanceSymbolNotionalFilter>().FirstOrDefault();

        /// <summary>
        /// Filter for the max accuracy of the price for this symbol
        /// </summary>
        [JsonIgnore]
        public BinanceSymbolPriceFilter? PriceFilter => Filters.OfType<BinanceSymbolPriceFilter>().FirstOrDefault();

        /// <summary>
        /// Filter for the maximum deviation of the price
        /// </summary>
        [JsonIgnore]
        public BinanceSymbolPercentPriceFilter? PricePercentFilter => Filters.OfType<BinanceSymbolPercentPriceFilter>().FirstOrDefault();

        /// <summary>
        /// Filter for the maximum position on a symbol
        /// </summary>
        [JsonIgnore]
        public BinanceSymbolMaxPositionFilter? MaxPositionFilter => Filters.OfType<BinanceSymbolMaxPositionFilter>().FirstOrDefault();

        /// <summary>
        /// Filter for the trailing delta values
        /// </summary>
        [JsonIgnore]
        public BinanceSymbolTrailingDeltaFilter? TrailingDeltaFilter => Filters.OfType<BinanceSymbolTrailingDeltaFilter>().FirstOrDefault();

        /// <summary>
        /// Filter for the max number of iceberg orders allowed on a symbol
        /// </summary>
        [JsonIgnore]
        public BinanceMaxNumberOfIcebergOrdersFilter? MaxIcebergFilter => Filters.OfType<BinanceMaxNumberOfIcebergOrdersFilter>().FirstOrDefault();
    }
}
