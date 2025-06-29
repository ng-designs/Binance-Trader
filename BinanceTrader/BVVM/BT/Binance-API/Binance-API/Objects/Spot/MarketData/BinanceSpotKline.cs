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
using BinanceAPI.Objects.Shared;
using Newtonsoft.Json;
using System;

namespace BinanceAPI.Objects.Spot.MarketData
{
    /// <summary>
    /// Candlestick information for symbol
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BinanceSpotKline : BinanceKlineBase
    {
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public BinanceSpotKline()
        {
        }

        /// <summary>
        /// The volume traded during this candlestick
        /// </summary>
        [ArrayProperty(5)]
        public override decimal BaseVolume { get; set; }

        /// <summary>
        /// The volume traded during this candlestick in the asset form
        /// </summary>
        [ArrayProperty(7)]
        public override decimal QuoteVolume { get; set; }

        /// <summary>
        /// Taker buy base asset volume
        /// </summary>
        [ArrayProperty(9)]
        public override decimal TakerBuyBaseVolume { get; set; }

        /// <summary>
        /// Taker buy quote asset volume
        /// </summary>
        [ArrayProperty(10)]
        public override decimal TakerBuyQuoteVolume { get; set; }

        /// <summary>
        /// Ignore
        /// </summary>
        [ArrayProperty(11)]
        public decimal? Ignore { get; set; }

        /// <summary>
        /// Конструктор для object[]
        /// </summary>
        public BinanceSpotKline(object[] values)
        {
            if (values == null || values.Length < 12)
                return;
            // Если базовый класс поддерживает инициализацию через object[], вызвать его конструктор
            // иначе инициализировать свойства вручную
            // Пример:
            if (values.Length > 0 && values[0] != null) OpenTime = Convert.ToInt64(values[0]);
            if (values.Length > 1 && values[1] != null) Open = Convert.ToDecimal(values[1]);
            if (values.Length > 2 && values[2] != null) High = Convert.ToDecimal(values[2]);
            if (values.Length > 3 && values[3] != null) Low = Convert.ToDecimal(values[3]);
            if (values.Length > 4 && values[4] != null) Close = Convert.ToDecimal(values[4]);
            if (values.Length > 5 && values[5] != null) BaseVolume = Convert.ToDecimal(values[5]);
            if (values.Length > 6 && values[6] != null) CloseTime = Convert.ToInt64(values[6]);
            if (values.Length > 7 && values[7] != null) QuoteVolume = Convert.ToDecimal(values[7]);
            if (values.Length > 8 && values[8] != null) TradeCount = Convert.ToInt32(values[8]);
            if (values.Length > 9 && values[9] != null) TakerBuyBaseVolume = Convert.ToDecimal(values[9]);
            if (values.Length > 10 && values[10] != null) TakerBuyQuoteVolume = Convert.ToDecimal(values[10]);
            if (values.Length > 11 && values[11] != null) Ignore = Convert.ToDecimal(values[11]);
        }
    }
}
