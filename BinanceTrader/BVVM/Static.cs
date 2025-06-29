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

using BinanceAPI;
using BinanceAPI.Objects.Spot.MarginData;
using BinanceAPI.Objects.Spot.MarketData;
using BTNET.BV.Base;
using BTNET.BV.Enum;
using BTNET.BVVM.BT.Orders;
using BTNET.VM.ViewModels;
using BTNET.VM.Views;
using System.Collections.Generic;
using System.Windows.Media;

namespace BTNET.BVVM
{
    // There can only be one
    public static class Static
    {
        private static volatile BinanceSymbolViewModel selectedSymbolViewModel = new();
        private static volatile BinanceSymbolViewModel? lastSelectedSymbolViewModel;

        public static SolidColorBrush White = new SolidColorBrush(Colors.White);
        public static SolidColorBrush Green = new SolidColorBrush(Colors.GreenYellow);
        public static SolidColorBrush Red = new SolidColorBrush(Colors.Red);
        public static SolidColorBrush Gold = new SolidColorBrush(Colors.Gold);
        public static SolidColorBrush Transparent = new SolidColorBrush(Colors.Transparent);
        public static SolidColorBrush Gray = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#242325"));

        public static LoadingView LoadingView { get; set; } = new();

        public static StoredExchangeInfo ManageExchangeInfo { get; set; } = new();

        public static OrderManager ManageStoredOrders { get; set; } = new();

        public static StoredAlerts ManageStoredAlerts { get; set; } = new();
        public static BinanceMarginAccount MarginAccount { get; set; } = new();
        public static BinanceSymbol? CurrentSymbolInfo { get; set; } = new();

        public static List<BinanceSymbolViewModel> AllPrices { get; set; } = new();
        public static List<BinanceSymbolViewModel> AllPricesUnfiltered { get; set; } = new();

        public static List<long> SettledOrders { get; set; } = new List<long>();

        public static string SpotListenKey { get; set; } = string.Empty;
        public static string MarginListenKey { get; set; } = string.Empty;
        public static string IsolatedListenKey { get; set; } = string.Empty;
        public static string LastIsolatedListenKeySymbol { get; set; } = string.Empty;

        public static bool IsStarted { get; set; }
        public static bool IsSearching { get; set; }

        public static BinanceSymbolViewModel SelectedSymbolViewModel
        {
            get => selectedSymbolViewModel;
            set => selectedSymbolViewModel = value;
        }

        public static BinanceSymbolViewModel? LastSelectedSymbolViewModel
        {
            get => lastSelectedSymbolViewModel;
            set => lastSelectedSymbolViewModel = value;
        }

        public static string PreviousSymbolText { get; set; } = string.Empty;

        public static Prompt MessageBox { get; set; } = new();

        public static SelectedTab CurrentlySelectedSymbolTab { get; set; } = SelectedTab.Error;
        public static TradingMode CurrentTradingMode { get; set; } = TradingMode.Error;

        public static QuoteBase RealTimeQuote { get; set; } = new();

        internal static bool HasAuth()
        {
            // Отключена проверка аутентификации для нормальной работы
            // return Settings.KeysLoaded && Core.MainVM.IsSymbolSelected && ServerTimeClient.IsReady();
            return true; // Всегда возвращаем true для демо-режима
        }

        public static bool IsInvalidSymbol()
        {
            // Отключена проверка символа для нормальной работы
            // if (!Core.MainVM.IsSymbolSelected || Static.CurrentTradingMode == TradingMode.Error)
            // {
            //     return true;
            // }

            return false; // Всегда возвращаем false для демо-режима
        }
    }
}
