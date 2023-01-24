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

using BinanceAPI.Enums;
using BinanceAPI.Objects.Spot.MarketData;
using BTNET.BV.Base;
using BTNET.BV.Enum;
using BTNET.BVVM.BT;
using BTNET.BVVM.Helpers;
using BTNET.VM.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace BTNET.BVVM
{
    internal class Search : Core
    {
        private static bool ShouldDestroy(BinanceSymbol test)
        {
            switch (test.Status)
            {
                case SymbolStatus.Close:
                case SymbolStatus.Break:
                    return true;

                default:

                    if (Static.CurrentTradingMode == TradingMode.Spot)
                    {
                        if (!test.Permissions.Contains(AccountType.Spot))
                        {
                            return true;
                        }
                    }
                    else if (Static.CurrentTradingMode == TradingMode.Margin)
                    {
                        if (!test.Permissions.Contains(AccountType.Margin))
                        {
                            return true;
                        }

                        if (MarginPairs.AllCross != null)
                        {
                            var pairExists = MarginPairs.AllCross.Where(t => t.Symbol == test.Name).FirstOrDefault();
                            if (pairExists == null)
                            {
                                return true;
                            }
                        }
                    }
                    else if (Static.CurrentTradingMode == TradingMode.Isolated)
                    {
                        if (!test.Permissions.Contains(AccountType.Margin))
                        {
                            return true;
                        }

                        if (MarginPairs.AllIsolated != null)
                        {
                            var pairExists = MarginPairs.AllIsolated.Where(t => t.Symbol == test.Name).FirstOrDefault();
                            if (pairExists == null)
                            {
                                return true;
                            }
                        }
                    }

                    break;
            }

            return false;
        }

        public static async Task<bool> SearchPricesUpdateAsync()
        {
            var result = await Client.Local.Spot.Market.GetPricesAsync().ConfigureAwait(false);
            var ex = StoredExchangeInfo.Get();
            var allSymbols = Static.AllPrices.ToList();

            if (result != null && ex != null)
            {
                if (result.Success)
                {
                    var prices = result.Data.Select(r => new BinanceSymbolViewModel(r.Symbol, r.Price));
                    Static.AllPricesUnfiltered = prices.ToList();
                    foreach (var pr in prices)
                    {
                        BinanceSymbolViewModel f = allSymbols.SingleOrDefault(allSymbols => allSymbols.SymbolView.Symbol == pr.SymbolView.Symbol);
                        if (f != null)
                        {
                            BinanceSymbol exSymbol = ex.Symbols.Where(exsym => exsym.Name == f.SymbolView.Symbol).FirstOrDefault();
                            if (exSymbol != null)
                            {
                                if (ShouldDestroy(exSymbol))
                                {
                                    allSymbols.Remove(f);
                                }
                            }
                        }
                        else
                        {
                            BinanceSymbol exSymbol = ex.Symbols.Where(exsym => exsym.Name == pr.SymbolView.Symbol).FirstOrDefault();
                            if (exSymbol != null)
                            {
                                if (!ShouldDestroy(exSymbol))
                                {
                                    allSymbols.Add(pr);
                                }
                            }
                        }
                    }

                    Static.AllPrices = allSymbols;

                    InvokeUI.CheckAccess(() =>
                    {
                        MainVM.AllSymbolsOnUI = Static.AllPrices;
                    });

                    WatchMan.SearchPrices.SetWorking();
                }
                else
                {
                    WatchMan.SearchPrices.SetError();
                    _ = Prompt.ShowBox($"Error requesting All Price data: {result.Error!.Message}", "error");
                }
            }

            return true;
        }
    }
}
