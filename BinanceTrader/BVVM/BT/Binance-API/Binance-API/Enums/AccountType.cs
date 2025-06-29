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
    /// The type of an account
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// Unknown account type
        /// </summary>
        Unknown,

        /// <summary>
        /// Spot account
        /// </summary>
        Spot,

        /// <summary>
        /// Margin account
        /// </summary>
        Margin,

        /// <summary>
        /// Futures account
        /// </summary>
        Futures,

        /// <summary>
        /// Options account
        /// </summary>
        Options,

        /// <summary>
        /// Isolated margin account
        /// </summary>
        IsolatedMargin,

        /// <summary>
        /// Unified account
        /// </summary>
        Unified,

        /// <summary>
        /// Funding account
        /// </summary>
        Funding,

        /// <summary>
        /// Portfolio margin account
        /// </summary>
        PortfolioMargin,

        /// <summary>
        /// Leveraged account
        /// </summary>
        Leveraged,

        /// <summary>
        /// Trading group 002
        /// </summary>
        TRD_GRP_002,

        /// <summary>
        /// Trading group 003
        /// </summary>
        TRD_GRP_003,

        /// <summary>
        /// Trading group 004
        /// </summary>
        TRD_GRP_004,

        /// <summary>
        /// Trading group 005
        /// </summary>
        TRD_GRP_005,

        /// <summary>
        /// Trading group 006
        /// </summary>
        TRD_GRP_006,

        /// <summary>
        /// Trading group 007
        /// </summary>
        TRD_GRP_007,

        /// <summary>
        /// Trading group 008
        /// </summary>
        TRD_GRP_008,

        /// <summary>
        /// Trading group 009
        /// </summary>
        TRD_GRP_009,

        /// <summary>
        /// Trading group 010
        /// </summary>
        TRD_GRP_010
    }
}
