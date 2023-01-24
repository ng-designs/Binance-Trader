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

using BTNET.BV.Base;
using BTNET.BVVM;
using BTNET.BVVM.BT;
using BTNET.BVVM.Helpers;
using BTNET.BVVM.Log;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TJson.NET;

namespace BTNET.VM.ViewModels
{
    public class WatchlistViewModel : Core
    {
        private ObservableCollection<WatchlistItem> watchlistitems = new();
        private ObservableCollection<string> allWatchlistSymbols = new();

        private string? resetText;
        private string? selectedWatchlistSymbol;
        private bool watchlistSymbolsEnabled;
        private bool removeButtonEnabled = true;

        private const int WATCHLIST_LOOP_DELAY_MS = 20;
        private const int WATCHLIST_LOOP_EXPIRE_TIME_MS = 10000;

        public ICommand? RemoveFromWatchlistCommand { get; set; }
        public ICommand? AddToWatchlistCommand { get; set; }

        public void InitializeCommands()
        {
            RemoveFromWatchlistCommand = new DelegateCommand(RemoveWatchlistItem);
            AddToWatchlistCommand = new DelegateCommand(AddWatchlistItem);
        }

        public ObservableCollection<WatchlistItem> WatchListItems
        {
            get => watchlistitems;
            set
            {
                watchlistitems = value;
                PropChanged();
            }
        }

        public ObservableCollection<string> AllWatchlistSymbols
        {
            get => allWatchlistSymbols;
            set
            {
                allWatchlistSymbols = value;
                PropChanged();
            }
        }

        public bool RemoveButtonEnabled
        {
            get => removeButtonEnabled;
            set
            {
                removeButtonEnabled = value;
                PropChanged();
            }
        }

        public string? SelectedWatchlistSymbol
        {
            get => selectedWatchlistSymbol;
            set
            {
                selectedWatchlistSymbol = value;
                PropChanged();
            }
        }

        public bool WatchlistSymbolsEnabled
        {
            get => watchlistSymbolsEnabled;
            set
            {
                watchlistSymbolsEnabled = value;
                PropChanged();
            }
        }

        public string? ResetText
        {
            get => resetText;
            set
            {
                resetText = value;
                PropChanged();
            }
        }

        public async Task InitializeWatchListAsync()
        {
            try
            {
                WatchlistSymbolsEnabled = false;
                ResetText = "Loading";

                _ = Task.Run(() =>
                {
                    InvokeUI.CheckAccess(() =>
                    {
                        foreach (var symbol in Static.AllPricesUnfiltered)
                        {
                            AllWatchlistSymbols.Add(symbol.SymbolView.Symbol);
                        }
                    });
                });

                IEnumerable<string>? StoredwatchlistSymbols = Json.Load<List<string>>(App.Listofwatchlistsymbols, true);
                if (StoredwatchlistSymbols != null)
                {
                    List<string> removeDuplicates = new List<string>();

                    foreach (var symbol in StoredwatchlistSymbols)
                    {
                        if (!removeDuplicates.Contains(symbol))
                        {
                            removeDuplicates.Add(symbol);
                            await AddWatchlistItemAsync(symbol).ConfigureAwait(false);
                        }
                    }

                    WriteLog.Info("Loaded [" + removeDuplicates.Count + "] Watchlist symbols into Watchlist from File");
                    WatchMan.Load_Watchlist.SetCompleted();

                    var startTime = DateTime.UtcNow.Ticks;
                    var sw = Stopwatch.StartNew();
                    while (await Loop.Delay(startTime, WATCHLIST_LOOP_DELAY_MS, WATCHLIST_LOOP_EXPIRE_TIME_MS, () =>
                    {
                        WriteLog.Info("Watchlist is taking a long time to connect: [" + sw.ElapsedMilliseconds.ToString() + "ms]");
                    }).ConfigureAwait(false))
                    {
                        int check = 0;

                        foreach (var item in WatchListItems)
                        {
                            if (item.TickerStatus != Ticker.CONNECTED)
                            {
                                check++;
                            }
                        }

                        if (check == 0)
                        {
                            WriteLog.Info("Loaded Watchlist and Connected Sockets in [" + sw.ElapsedMilliseconds.ToString() + "ms]");
                            break;
                        }
                    }
                }
                else
                {
                    WatchMan.Load_Watchlist.SetCompleted();
                }

                WatchlistSymbolsEnabled = true;
                ResetText = "";
            }
            catch (Exception ex)
            {
                WatchMan.Load_Watchlist.SetError();
                WriteLog.Error("Watchlist Failed to Initialize: ", ex);
            }
        }

        public void AddWatchlistItem(object o)
        {
            if (SelectedWatchlistSymbol == null)
            {
                return;
            }

            _ = Task.Run(async () =>
            {
                if (!WatchListItems.Where(t => t.WatchlistSymbol == SelectedWatchlistSymbol).Any())
                {
                    if (!await AddWatchlistItemAsync(SelectedWatchlistSymbol).ConfigureAwait(false))
                    {
                        Prompt.ShowBox("Please enter a valid Symbol", "Invalid Symbol");
                    }
                }
                else
                {
                    Prompt.ShowBox("Symbol already on Watchlist", "Already Exists");
                }
            }).ConfigureAwait(false);
        }

        public Task<bool> AddWatchlistItemAsync(string Symbol)
        {
            if (Static.AllPricesUnfiltered.Where(x => x.SymbolView.Symbol == Symbol).FirstOrDefault() != null)
            {
                var temp = Static.ManageExchangeInfo.GetStoredSymbolInformation(Symbol);
                if (temp != null)
                {
                    WatchlistItem watchListItem = new WatchlistItem(temp);
                    watchListItem.SubscribeWatchListItemSocket();

                    InvokeUI.CheckAccess(() =>
                    {
                        WatchListItems.Add(watchListItem);
                        WatchListItems = new ObservableCollection<WatchlistItem>(WatchListItems.OrderByDescending(d => d.WatchlistSymbol));
                    });

                    return Task.FromResult(true);
                }
            }

            var s = "Failed to add to Watchlist: " + Symbol;
            WriteLog.Error(s);
            NotifyVM.Notification(s);
            return Task.FromResult(false);
        }

        public void RemoveWatchlistItem(object o)
        {
            var watchlistItem = (WatchlistItem)o;

            if (watchlistItem != null)
            {
                _ = Task.Run(() =>
                {
                    WriteLog.Info("Removed Watchlist Symbol: " + watchlistItem.WatchlistSymbol);
                    InvokeUI.CheckAccess(() =>
                    {
                        WatchListItems.Remove(watchlistItem);
                    });

                    Tickers.RemoveOwnership(watchlistItem.WatchlistSymbol, BV.Enum.Owner.Watchlist);
                }).ConfigureAwait(false);
            }
            else
            {
                WriteLog.Error("Error Removing Watchlist Symbol at: [" + DateTime.UtcNow.ToString() + "]");
            }
        }

        public void StoreWatchListItemsSymbols()
        {
            if (WatchListItems != null)
            {
                List<string> WatchlistitemSymbols = new List<string>();

                foreach (WatchlistItem watchlistItem in WatchListItems)
                {
                    var ws = watchlistItem.WatchlistSymbol;
                    if (ws != null)
                    {
                        if (!WatchlistitemSymbols.Contains(ws))
                        {
                            WatchlistitemSymbols.Add(ws);
                        }
                    }
                }

                if (WatchlistitemSymbols.Count > 0)
                {
                    Json.Save(WatchlistitemSymbols, App.Listofwatchlistsymbols);
                }
                else
                {
                    try
                    {
                        if (File.Exists(App.Listofwatchlistsymbols))
                        {
                            File.Delete(App.Listofwatchlistsymbols);
                        }
                    }
                    catch
                    {
                        // It didn't exist or we don't have permission to delete it, its locked, etc.
                    }
                }
            }
        }
    }
}
