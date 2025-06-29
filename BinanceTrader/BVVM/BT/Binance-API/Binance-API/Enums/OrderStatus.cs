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
    /// The status of an order
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Order is new
        /// </summary>
        New,

        /// <summary>
        /// Order is partially filled
        /// </summary>
        PartiallyFilled,

        /// <summary>
        /// Order is filled
        /// </summary>
        Filled,

        /// <summary>
        /// Order is canceled
        /// </summary>
        Canceled,

        /// <summary>
        /// Order is pending cancel
        /// </summary>
        PendingCancel,

        /// <summary>
        /// Order is rejected
        /// </summary>
        Rejected,

        /// <summary>
        /// Order is expired
        /// </summary>
        Expired,

        /// <summary>
        /// Order is new (insurance)
        /// </summary>
        NewInsurance,

        /// <summary>
        /// Order is new (adjustment)
        /// </summary>
        NewAdjustment,

        /// <summary>
        /// Order is expired (insurance)
        /// </summary>
        ExpiredInsurance,

        /// <summary>
        /// Order is expired (adjustment)
        /// </summary>
        ExpiredAdjustment,

        /// <summary>
        /// Insurance order
        /// </summary>
        Insurance,

        /// <summary>
        /// ADL order
        /// </summary>
        Adl
    }
}
