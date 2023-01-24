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

using BTNET.BVVM;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BTNET.VM.Views
{
    public partial class LogView : Window
    {
        public LogView(MainContext datacontext)
        {
            AllowsTransparency = true;

            InitializeComponent();
            Topmost = true;
            DataContext = datacontext;
        }

        private void DragWindowOrMaximize(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == App.RAPID_CLICKS_TO_MAXIMIZE_WINDOW)
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }

            BorderBrush = Brushes.Transparent;
            BorderThickness = MainContext.BorderAdjustment(WindowState, true);
            DragMove();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BorderBrush = Brushes.Transparent;
            BorderThickness = MainContext.BorderAdjustment(WindowState, true);
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Log.ScrollToEnd();
        }

        private void Window_Activated(object sender, System.EventArgs e)
        {
            Log.ScrollToEnd();
        }
    }
}
