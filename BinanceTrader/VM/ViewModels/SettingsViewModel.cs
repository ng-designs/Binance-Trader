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

using BinanceAPI.ClientBase;
using BTNET.BV.Abstract;
using BTNET.BVVM;
using BTNET.BVVM.BT.Orders;
using BTNET.BVVM.Helpers;
using BTNET.BVVM.Identity;
using Newtonsoft.Json;
using System.IO;
using System.Security;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TJson.NET;

namespace BTNET.VM.ViewModels
{
    public class SettingsViewModel : Core
    {
        #region [Commands]

        public void InitializeCommands()
        {
            SaveSettingsCommand = new DelegateCommand(SaveSettings);
            ChangeSettingsCommand = new DelegateCommand(ChangeSettings);
            ShowBorrowInfoCommand = new DelegateCommand(ShowBorrowInfo);
            ShowMarginInfoCommand = new DelegateCommand(ShowMarginInfo);
            ShowSymbolInfoCommand = new DelegateCommand(ShowSymbolInfo);
            ShowIsolatedInfoCommand = new DelegateCommand(ShowIsolatedInfo);
            ShowBreakDownInfoCommand = new DelegateCommand(ShowBreakDownInfo);
            TransparentTitleBarCommand = new DelegateCommand(TransparentTitleBar);
            CheckForUpdatesCommand = new DelegateCommand(CheckForUpdate);
            AutoSaveSettingsCommand = new DelegateCommand(AutoSave);
            KeepFirstOrderCommand = new DelegateCommand(KeepFirstOrder);
            DangerousButtonsEnabledCommand = new DelegateCommand(DangerousButtons);
            ShowScraperButtonCommand = new DelegateCommand(ScraperButton);
            TrackLongShortCommand = new DelegateCommand(TrackLongShort);
            FilterCurrentOrdersCommand = new DelegateCommand(FilterCurrent);
        }

        public ICommand? TransparentTitleBarCommand { get; set; }

        public ICommand? ShowIsolatedInfoCommand { get; set; }
        public ICommand? ShowSymbolInfoCommand { get; set; }
        public ICommand? ShowBorrowInfoCommand { get; set; }
        public ICommand? ShowMarginInfoCommand { get; set; }

        public ICommand? ShowScraperButtonCommand { get; set; }

        public ICommand? ShowBreakDownInfoCommand { get; set; }

        public ICommand? SaveSettingsCommand { get; set; }
        public ICommand? ChangeSettingsCommand { get; set; }

        public ICommand? CheckForUpdatesCommand { get; set; }

        public ICommand? AutoSaveSettingsCommand { get; set; }

        public ICommand? KeepFirstOrderCommand { get; set; }

        public ICommand? DangerousButtonsEnabledCommand { get; set; }

        public ICommand? TrackLongShortCommand { get; set; }

        public ICommand? FilterCurrentOrdersCommand { get; set; }

        #endregion [Commands]

        private bool saveEnabled = false;
        private bool changeEnabled = false;
        private bool apiKeyEnabled = true;
        private bool apiSecretEnabled = true;
        private bool showSymbolInfoIsChecked = true;
        private bool showBorrowInfoIsChecked = true;
        private bool showMarginInfoIsChecked = true;
        private bool showIsolatedInfoIsChecked = true;
        private bool showBreakDownInfoIsChecked = true;
        private bool transparentTitleBarIsChecked = false;
        private bool checkForUpdatesIsChecked = false;
        private bool dangerousButtonsIsChecked = false;
        private bool autoSaveIsChecked = false;
        private bool keepFirstOrderIsChecked = false;
        private bool showScraperButtonIsChecked = true;
        private bool checkForUpdateCheckBoxEnabled = false;
        private bool trackLongShortIsChecked = true;
        private string isUpToDate = string.Empty;
        private string apiKey = string.Empty;
        private string apiSecret = string.Empty;
        private Brush titleBarBrush = Static.Gray;
        private bool filterCurrentOrdersIsChecked;
        private bool filterCurrentOrdersIsEnabled;

        public string IsUpToDate
        {
            get => isUpToDate;
            set
            {
                isUpToDate = value;
                PropChanged();
            }
        }

        public string ApiSecret
        {
            get => apiSecret;
            set
            {
                apiSecret = value;
                PropChanged();
            }
        }

        public string ApiKey
        {
            get => apiKey;
            set
            {
                apiKey = value;
                PropChanged();
            }
        }

        public bool ApiKeyEnabled
        {
            get => apiKeyEnabled;
            set
            {
                apiKeyEnabled = value;
                PropChanged();
            }
        }

        public bool ApiSecretEnabled
        {
            get => apiSecretEnabled;
            set
            {
                apiSecretEnabled = value;
                PropChanged();
            }
        }

        public bool SaveEnabled
        {
            get => saveEnabled;
            set
            {
                saveEnabled = value;
                PropChanged();
            }
        }

        public bool ChangeEnabled
        {
            get => changeEnabled;
            set
            {
                changeEnabled = value;
                PropChanged();
            }
        }

        public bool FilterCurrentOrdersIsEnabled
        {
            get => filterCurrentOrdersIsEnabled;
            set
            {
                filterCurrentOrdersIsEnabled = value;
                PropChanged();
            }
        }

        public bool FilterCurrentOrdersIsChecked
        {
            get => filterCurrentOrdersIsChecked;
            set
            {
                filterCurrentOrdersIsChecked = value;
                PropChanged();
            }
        }

        public bool AutoSaveIsChecked
        {
            get => autoSaveIsChecked;
            set
            {
                autoSaveIsChecked = value;
                PropChanged();
            }
        }

        public bool KeepFirstOrderIsChecked
        {
            get => keepFirstOrderIsChecked;
            set
            {
                keepFirstOrderIsChecked = value;
                PropChanged();
            }
        }

        public bool ShowScraperButtonIsChecked
        {
            get => showScraperButtonIsChecked;
            set
            {
                showScraperButtonIsChecked = value;
                PropChanged();
            }
        }

        public bool DangerousButtonsIsChecked
        {
            get => dangerousButtonsIsChecked;
            set
            {
                dangerousButtonsIsChecked = value;
                PropChanged();
            }
        }

        public bool ShowSymbolInfoIsChecked
        {
            get => showSymbolInfoIsChecked;
            set
            {
                showSymbolInfoIsChecked = value;
                PropChanged();
            }
        }

        public bool ShowBorrowInfoIsChecked
        {
            get => showBorrowInfoIsChecked;
            set
            {
                showBorrowInfoIsChecked = value;
                PropChanged();
            }
        }

        public bool ShowMarginInfoIsChecked
        {
            get => showMarginInfoIsChecked;
            set
            {
                showMarginInfoIsChecked = value;
                PropChanged();
            }
        }

        public bool ShowIsolatedInfoIsChecked
        {
            get => showIsolatedInfoIsChecked;
            set
            {
                showIsolatedInfoIsChecked = value;
                PropChanged();
            }
        }

        public bool ShowBreakDownInfoIsChecked
        {
            get => showBreakDownInfoIsChecked;
            set
            {
                showBreakDownInfoIsChecked = value;
                PropChanged();
            }
        }

        public bool TransparentTitleBarIsChecked
        {
            get => transparentTitleBarIsChecked;
            set
            {
                transparentTitleBarIsChecked = value;
                PropChanged();
            }
        }

        public bool CheckForUpdatesIsChecked
        {
            get => checkForUpdatesIsChecked;
            set
            {
                checkForUpdatesIsChecked = value;
                PropChanged();
            }
        }

        public bool CheckForUpdateCheckBoxEnabled
        {
            get => checkForUpdateCheckBoxEnabled;
            set
            {
                checkForUpdateCheckBoxEnabled = value;
                PropChanged();
            }
        }

        public bool TrackLongShortIsChecked
        {
            get => trackLongShortIsChecked;
            set
            {
                trackLongShortIsChecked = value;
                PropChanged();
            }
        }

        public Brush TitleBarBrush
        {
            get => titleBarBrush;
            set
            {
                titleBarBrush = value;
                PropChanged();
            }
        }

        public void FilterCurrent(object o)
        {
            FilterCurrentOrdersIsEnabled = false;
            FilterCurrentOrdersIsChecked = !FilterCurrentOrdersIsChecked;
            OrderManager.LastTotal = 0;
        }

        public void TrackLongShort(object o)
        {
            TrackLongShortIsChecked = !TrackLongShortIsChecked;
            QuoteVM.RunningTotalTask = 0;
        }

        public void ScraperButton(object o)
        {
            ShowScraperButtonIsChecked = !ShowScraperButtonIsChecked;
        }

        public void DangerousButtons(object o)
        {
            DangerousButtonsIsChecked = !DangerousButtonsIsChecked;
        }

        public void KeepFirstOrder(object o)
        {
            KeepFirstOrderIsChecked = !KeepFirstOrderIsChecked;
        }

        public void AutoSave(object o)
        {
            AutoSaveIsChecked = !AutoSaveIsChecked;
        }

        public void TransparentTitleBar(object o)
        {
            TransparentTitleBarIsChecked = !TransparentTitleBarIsChecked;
            ConfigureTitleBar();
        }

        private void CheckForUpdate(object o)
        {
            CheckForUpdatesIsChecked = !CheckForUpdatesIsChecked;
        }

        private void ShowBreakDownInfo(object o)
        {
            ShowBreakDownInfoIsChecked = !ShowBreakDownInfoIsChecked;
            BorrowVM.ShowBreakdown = ShowBreakDownInfoIsChecked;
        }

        private void ShowIsolatedInfo(object o)
        {
            ShowIsolatedInfoIsChecked = !ShowIsolatedInfoIsChecked;
            BorrowVM.IsolatedInfoVisible = showMarginInfoIsChecked;
        }

        private void ShowMarginInfo(object o)
        {
            ShowMarginInfoIsChecked = !ShowMarginInfoIsChecked;
            BorrowVM.MarginInfoVisible = ShowMarginInfoIsChecked;
        }

        private void ShowBorrowInfo(object o)
        {
            ShowBorrowInfoIsChecked = !ShowBorrowInfoIsChecked;
            BorrowVM.BorrowInfoVisible = ShowBorrowInfoIsChecked;
        }

        private void ShowSymbolInfo(object o)
        {
            ShowSymbolInfoIsChecked = !ShowSymbolInfoIsChecked;
        }

        private void ChangeSettings(object o)
        {
            Enable();
        }

        private void SaveSettings(object o)
        {
            if (!string.IsNullOrEmpty(ApiKey) && !string.IsNullOrEmpty(ApiSecret))
            {
                if (ApiKey == Settings.KEY_LOADED_FROM_FILE || ApiSecret == Settings.KEY_LOADED_FROM_FILE)
                {
                    MessageBox.Show(Settings.KEYS_NOT_VALID, Settings.KEY_MISSING);
                    Enable();
                    return;
                }

                // Create Directory if it doesn't exist
                Directory.CreateDirectory(App.SettingsPath);

                // Encrypt API Key/Secret and store them in a Secure String
                SecureString key = UniqueIdentity.Encrypt(ApiKey.ToSecure(), Settings.TEMP_CLIENT_PASSWORD);
                SecureString sec = UniqueIdentity.Encrypt(ApiSecret.ToSecure(), Settings.TEMP_CLIENT_PASSWORD);

                // Remove Keys from UI
                ApiKey = Settings.KEY_SAVED; // Old Value
                ApiSecret = Settings.KEY_SAVED;
                ApiKey = Settings.KEY_SAVED_TO_FILE; // New Value
                ApiSecret = Settings.KEY_SAVED_TO_FILE;

                // Write Encrypted Keys to File
                File.WriteAllText(App.KeyFile, JsonConvert.SerializeObject(new ApiKeys { ApiKey = key.GetSecure(), ApiSecret = sec.GetSecure() }));

                // Set Authentication
                BaseClient.SetAuthentication(UniqueIdentity.Decrypt(key, Settings.TEMP_CLIENT_PASSWORD), UniqueIdentity.Decrypt(sec, Settings.TEMP_CLIENT_PASSWORD));

                // Dispose Encrypted Keys
                key.Dispose();
                sec.Dispose();

                // Show Message
                MessageBox.Show(Settings.KEYS_ENCRYPTED_JSON, Settings.KEYS_ENCRYPTED);

                // Disable UI Controls where keys were Set
                Disable();

                // Attempt to Start User Streams
                MainContext.StartUserStreamsAsync();
            }
            else
            {
                MessageBox.Show(Settings.KEYS_ENTER, Settings.KEY_MISSING);
                Enable();
            }
        }

        public int BoolToInt(bool value)
        {
            return value == true ? 2 : 1;
        }

        public void SaveOnClose()
        {
            SettingsObject settings = new(
                BoolToInt(ShowBorrowInfoIsChecked),
                BoolToInt(ShowSymbolInfoIsChecked),
                BoolToInt(ShowBreakDownInfoIsChecked),
                BoolToInt(ShowMarginInfoIsChecked),
                BoolToInt(ShowIsolatedInfoIsChecked),
                BoolToInt(TransparentTitleBarIsChecked),
                BoolToInt(CheckForUpdatesIsChecked),
                BoolToInt(TradeVM.UseLimitSellBool),
                BoolToInt(BorrowVM.BorrowSell),
                BoolToInt(BorrowVM.BorrowBuy),
                BoolToInt(TradeVM.UseLimitBuyBool),
                BoolToInt(false), // todo: add notification setting
                BoolToInt(AutoSaveIsChecked),
                BoolToInt(KeepFirstOrderIsChecked),
                BoolToInt(DangerousButtonsIsChecked),
                VisibilityVM.WatchListHeight,
                BoolToInt(ShowScraperButtonIsChecked),
                BoolToInt(TrackLongShortIsChecked));
            if (settings != null)
            {
                Json.Save(settings, App.Settings);
            }

            SettingsObjectPanels settingsPanels = new(
                VisibilityVM.PanelBreakdownLeft, VisibilityVM.PanelBreakdownTop,
                VisibilityVM.PanelInfoBoxLeft, VisibilityVM.PanelInfoBoxTop,
                VisibilityVM.PanelRealTimeLeft, VisibilityVM.PanelRealTimeTop,
                VisibilityVM.PanelBorrowBoxLeft, VisibilityVM.PanelBorrowBoxTop,
                VisibilityVM.PanelMarginInfoLeft, VisibilityVM.PanelMarginInfoTop,
                VisibilityVM.OrderListHeight,
                VisibilityVM.TradeInfoLeft, VisibilityVM.TradeInfoTop,
                VisibilityVM.ScraperLeft, VisibilityVM.ScraperTop,
                VisibilityVM.InsightsPanelLeft, VisibilityVM.InsightsPanelTop);

            if (settingsPanels != null)
            {
                Json.Save(settingsPanels, App.SettingsPanels);
            }

            SettingsObjectScraper settingsScraper = new(
                ScraperVM.SellPercent,
                ScraperVM.ReverseDownPercent,
                ScraperVM.PriceBias,
                ScraperVM.WaitTimeCount,
                ScraperVM.WaitTime,
                ScraperVM.ScraperCounter.GuesserResetShortCountUp, ScraperVM.ScraperCounter.GuesserResetShortCountDown, ScraperVM.ScraperCounter.GuesserDivPercent,
                ScraperVM.ScraperCounter.GuesserResetLongCountUp, ScraperVM.ScraperCounter.GuesserResetLongCountDown, ScraperVM.ScraperCounter.GuesserResetTime,
                ScraperVM.ScraperCounter.GuesserResetTimeBias, ScraperVM.ScraperCounter.GuesserUpBias, ScraperVM.ScraperCounter.GuesserDownBias, ScraperVM.ScraperCounter.GuesserDeadTime,
                BoolToInt(ScraperVM.UseLimitBuy), BoolToInt(ScraperVM.UseLimitSell), BoolToInt(ScraperVM.UseLimitAdd), BoolToInt(ScraperVM.UseLimitClose), ScraperVM.GuesserReverseBias,
                BoolToInt(ScraperVM.DontGuessBuyIsChecked), BoolToInt(ScraperVM.DontGuessSellIsChecked));

            if (settingsScraper != null)
            {
                Json.Save(settingsScraper, App.SettingsScraper);
            }
        }

        public void ConfigureTitleBar()
        {
            if (TransparentTitleBarIsChecked == true)
            {
                TitleBarBrush = Static.Transparent;
            }
            else
            {
                TitleBarBrush = Static.Gray;
            }
        }

        internal void Enable()
        {
            Settings.KeysLoaded = false;
            ApiKey = string.Empty;
            ApiSecret = string.Empty;
            ApiKeyEnabled = true;
            ApiSecretEnabled = true;
            SaveEnabled = true;
            ChangeEnabled = false;
        }

        internal void Disable()
        {
            Settings.KeysLoaded = true;
            ApiKeyEnabled = false;
            ApiSecretEnabled = false;
            SaveEnabled = false;
            ChangeEnabled = true;
        }
    }
}
