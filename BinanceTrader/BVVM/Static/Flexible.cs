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

using BinanceAPI.Objects.Spot.LendingData;
using BTNET.BVVM.Helpers;
using BTNET.BVVM.Log;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BTNET.BVVM
{
    internal class Flexible : Core
    {
        private const int START_PAGE = 1;
        private const int FULL_PAGE_NUMBER = 100;
        private const int ATTEMPT_EXPIRE_TIME_MS = 10000;
        private const int ATTEMPT_LOOP_DELAY_MS = 1;

        public static async Task<IEnumerable<BinanceSavingsProduct>> GetAllFlexibleProductsListAsync()
        {
            Collection<BinanceSavingsProduct> result = new Collection<BinanceSavingsProduct>();
            try
            {
                int page = START_PAGE;
                var startTicks = DateTime.UtcNow.Ticks;
                while (await Loop.Delay(startTicks, ATTEMPT_LOOP_DELAY_MS, ATTEMPT_EXPIRE_TIME_MS,
                    () =>
                    {
                        WriteLog.Error("[1] Failed to update Flexible Products");
                    }))
                {
                    var productsPage = await Client.Local.Lending.GetFlexibleProductListAsync(pageSize: FULL_PAGE_NUMBER, page: page, receiveWindow: 2000).ConfigureAwait(false);
                    if (productsPage == null || !productsPage.Success)
                    {
                        WriteLog.Error("[2] Failed to update Flexible Products");
                        break;
                    }

                    // Empty page - Completed
                    if (productsPage.Data.Count() == 0)
                    {
                        break;
                    }

                    // Add Products
                    foreach (BinanceSavingsProduct product in productsPage.Data)
                    {
                        result.Add(product);
                    }

                    // Incomplete page; don't need another request
                    if (productsPage.Data.Count() < FULL_PAGE_NUMBER)
                    {
                        break;
                    }

                    // Step Page
                    page++;
                }
            }
            catch (Exception ex)
            {
                WriteLog.Error("Get Flexible Products Failed, Try again in a few moments", ex);
            }

            return result;
        }
    }
}
