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

namespace BinanceAPI.Enums
{
    /// <summary>
    /// The type of an order
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Limit order
        /// </summary>
        Limit,

        /// <summary>
        /// Market order
        /// </summary>
        Market,

        /// <summary>
        /// Stop loss order
        /// </summary>
        StopLoss,

        /// <summary>
        /// Stop loss limit order
        /// </summary>
        StopLossLimit,

        /// <summary>
        /// Take profit order
        /// </summary>
        TakeProfit,

        /// <summary>
        /// Take profit limit order
        /// </summary>
        TakeProfitLimit,

        /// <summary>
        /// Limit maker order
        /// </summary>
        LimitMaker,

        /// <summary>
        /// Stop market order
        /// </summary>
        StopMarket,

        /// <summary>
        /// Take profit market order
        /// </summary>
        TakeProfitMarket,

        /// <summary>
        /// Trailing stop market order
        /// </summary>
        TrailingStopMarket,

        /// <summary>
        /// One cancels other order
        /// </summary>
        Oco
    }
}
