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
using BinanceAPI.ClientHosts;
using BinanceAPI.Converters;
using BinanceAPI.Enums;
using BinanceAPI.Objects;
using BinanceAPI.Objects.Spot.MarketData;
using BinanceAPI.Objects.Spot.MarketStream;
using BinanceAPI.Objects.Spot.UserStream;
using BinanceAPI.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BinanceAPI.SocketSubClients
{
    /// <summary>
    /// Spot streams
    /// </summary>
    public class BinanceSocketClientSpot
    {
        #region fields

        private const string depthStreamEndpoint = "@depth";
        private const string bookTickerStreamEndpoint = "@bookTicker";
        private const string allBookTickerStreamEndpoint = "!bookTicker";
        private const string klineStreamEndpoint = "@kline";
        private const string tradesStreamEndpoint = "@trade";
        private const string aggregatedTradesStreamEndpoint = "@aggTrade";
        private const string symbolTickerStreamEndpoint = "@ticker";
        private const string allSymbolTickerStreamEndpoint = "!ticker@arr";
        private const string partialBookDepthStreamEndpoint = "@depth";
        private const string symbolMiniTickerStreamEndpoint = "@miniTicker";
        private const string allSymbolMiniTickerStreamEndpoint = "!miniTicker@arr";

        private const string executionUpdateEvent = "executionReport";
        private const string ocoOrderUpdateEvent = "listStatus";
        private const string accountPositionUpdateEvent = "outboundAccountPosition";
        private const string balanceUpdateEvent = "balanceUpdate";

        #endregion fields

        private readonly SocketClientHost _baseClient;

        internal BinanceSocketClientSpot(SocketClientHost baseClient)
        {
            _baseClient = baseClient;
        }

        #region Aggregate Trade Streams

        /// <summary>
        /// Subscribes to the aggregated trades update stream for the provided symbol
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#aggregate-trade-streams</para>
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToAggregatedTradeUpdatesAsync(string symbol, Action<DataEvent<BinanceStreamAggregatedTrade>> onMessage) =>
       await SubscribeToAggregatedTradeUpdatesAsync(new[] { symbol }, onMessage).ConfigureAwait(false);

        /// <summary>
        /// Subscribes to the aggregated trades update stream for the provided symbols
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#aggregate-trade-streams</para>
        /// </summary>
        /// <param name="symbols">The symbols</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToAggregatedTradeUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<BinanceStreamAggregatedTrade>> onMessage)

        {
            symbols.ValidateNotNull(nameof(symbols));
            var handler = new Action<DataEvent<BinanceCombinedStream<BinanceStreamAggregatedTrade>>>(data => onMessage(data.As(data.Data.Data, data.Data.Data.Symbol)));

            symbols = symbols.Select(a => a.ToLower(CultureInfo.InvariantCulture) + aggregatedTradesStreamEndpoint).ToArray();

            return await Subscribe(symbols, handler).ConfigureAwait(false);
        }

        #endregion Aggregate Trade Streams

        #region Trade Streams

        /// <summary>
        /// Subscribes to the trades update stream for the provided symbol
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#trade-streams</para>
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<BinanceStreamTrade>> onMessage) =>

       await SubscribeToTradeUpdatesAsync(new[] { symbol }, onMessage).ConfigureAwait(false);

        /// <summary>
        /// Subscribes to the trades update stream for the provided symbols
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#trade-streams</para>
        /// </summary>
        /// <param name="symbols">The symbols</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToTradeUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<BinanceStreamTrade>> onMessage)

        {
            symbols.ValidateNotNull(nameof(symbols));
            var handler = new Action<DataEvent<BinanceCombinedStream<BinanceStreamTrade>>>(data => onMessage(data.As(data.Data.Data, data.Data.Data.Symbol)));

            symbols = symbols.Select(a => a.ToLower(CultureInfo.InvariantCulture) + tradesStreamEndpoint).ToArray();

            return await Subscribe(symbols, handler).ConfigureAwait(false);
        }

        #endregion Trade Streams

        #region Kline/Candlestick Streams

        /// <summary>
        /// Subscribes to the candlestick update stream for the provided symbol
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#kline-candlestick-streams</para>
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="interval">The interval of the candlesticks</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<BinanceStreamKlineData>> onMessage) =>

        await SubscribeToKlineUpdatesAsync(new[] { symbol }, interval, onMessage).ConfigureAwait(false);

        /// <summary>
        /// Subscribes to the candlestick update stream for the provided symbol and intervals
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#kline-candlestick-streams</para>
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="intervals">The intervals of the candlesticks</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToKlineUpdatesAsync(string symbol, IEnumerable<KlineInterval> intervals, Action<DataEvent<BinanceStreamKlineData>> onMessage) =>

        await SubscribeToKlineUpdatesAsync(new[] { symbol }, intervals, onMessage).ConfigureAwait(false);

        /// <summary>
        /// Subscribes to the candlestick update stream for the provided symbols and interval
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#kline-candlestick-streams</para>
        /// </summary>
        /// <param name="symbols">The symbols</param>
        /// <param name="interval">The interval of the candlesticks</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToKlineUpdatesAsync(IEnumerable<string> symbols, KlineInterval interval, Action<DataEvent<BinanceStreamKlineData>> onMessage) =>

        await SubscribeToKlineUpdatesAsync(symbols, new[] { interval }, onMessage).ConfigureAwait(false);

        /// <summary>
        /// Subscribes to the candlestick update stream for the provided symbols and intervals
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#kline-candlestick-streams</para>
        /// </summary>
        /// <param name="symbols">The symbols</param>
        /// <param name="intervals">The intervals of the candlesticks</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToKlineUpdatesAsync(IEnumerable<string> symbols, IEnumerable<KlineInterval> intervals, Action<DataEvent<BinanceStreamKlineData>> onMessage)

        {
            symbols.ValidateNotNull(nameof(symbols));

            var handler = new Action<DataEvent<BinanceCombinedStream<BinanceStreamKlineData>>>(data => onMessage(data.As<BinanceStreamKlineData>(data.Data.Data, data.Data.Data.Symbol)));

            symbols = symbols.SelectMany(a => intervals.Select(i => a.ToLower(CultureInfo.InvariantCulture) + klineStreamEndpoint + "_" + JsonConvert.SerializeObject(i, new KlineIntervalConverter(false)))).ToArray();


            return await Subscribe(symbols, handler).ConfigureAwait(false);
        }

        #endregion Kline/Candlestick Streams

        #region Individual Symbol Mini Ticker Stream

        /// <summary>
        /// Subscribes to mini ticker updates stream for a specific symbol
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#individual-symbol-mini-ticker-stream</para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToSymbolMiniTickerUpdatesAsync(string symbol, Action<DataEvent<BinanceStreamMiniTick>> onMessage) =>

        await SubscribeToSymbolMiniTickerUpdatesAsync(new[] { symbol }, onMessage).ConfigureAwait(false);

        /// <summary>
        /// Subscribes to mini ticker updates stream for a specific symbol
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#individual-symbol-mini-ticker-stream</para>
        /// </summary>
        /// <param name="symbols">The symbols to subscribe to</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToSymbolMiniTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<BinanceStreamMiniTick>> onMessage)

        {
            symbols.ValidateNotNull(nameof(symbols));

            var handler = new Action<DataEvent<BinanceCombinedStream<BinanceStreamMiniTick>>>(data => onMessage(data.As<BinanceStreamMiniTick>(data.Data.Data, data.Data.Data.Symbol)));

            symbols = symbols.Select(a => a.ToLower(CultureInfo.InvariantCulture) + symbolMiniTickerStreamEndpoint).ToArray();

            return await Subscribe(symbols, handler).ConfigureAwait(false);
        }

        #endregion Individual Symbol Mini Ticker Stream

        #region All Market Mini Tickers Stream

        /// <summary>
        /// Subscribes to mini ticker updates stream for all symbols
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#all-market-mini-tickers-stream</para>
        /// </summary>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToAllSymbolMiniTickerUpdatesAsync(Action<DataEvent<IEnumerable<BinanceStreamMiniTick>>> onMessage)

        {
            var handler = new Action<DataEvent<BinanceCombinedStream<IEnumerable<BinanceStreamMiniTick>>>>(data => onMessage(data.As<IEnumerable<BinanceStreamMiniTick>>(data.Data.Data, data.Data.Stream)));


            return await Subscribe(new[] { allSymbolMiniTickerStreamEndpoint }, handler).ConfigureAwait(false);
        }

        #endregion All Market Mini Tickers Stream

        #region Individual Symbol Book Ticker Streams

        /// <summary>
        /// Subscribes to the book ticker update stream for the provided symbol
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#individual-symbol-book-ticker-streams</para>
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToBookTickerUpdatesAsync(string symbol, Action<DataEvent<BinanceStreamBookPrice>> onMessage) =>

      await SubscribeToBookTickerUpdatesAsync(new[] { symbol }, onMessage).ConfigureAwait(false);

        /// <summary>
        /// Subscribes to the book ticker update stream for the provided symbols
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#individual-symbol-book-ticker-streams</para>
        /// </summary>
        /// <param name="symbols">The symbols</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToBookTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<BinanceStreamBookPrice>> onMessage)

        {
            symbols.ValidateNotNull(nameof(symbols));

            var handler = new Action<DataEvent<BinanceCombinedStream<BinanceStreamBookPrice>>>(data => onMessage(data.As(data.Data.Data, data.Data.Data.Symbol)));

            symbols = symbols.Select(a => a.ToLower(CultureInfo.InvariantCulture) + bookTickerStreamEndpoint).ToArray();

            return await Subscribe(symbols, handler).ConfigureAwait(false);
        }

        #endregion Individual Symbol Book Ticker Streams

        #region All Book Tickers Stream

        /// <summary>
        /// Subscribes to the book ticker update stream for all symbols
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#all-book-tickers-stream</para>
        /// </summary>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToAllBookTickerUpdatesAsync(Action<DataEvent<BinanceStreamBookPrice>> onMessage)

        {
            var handler = new Action<DataEvent<BinanceCombinedStream<BinanceStreamBookPrice>>>(data => onMessage(data.As(data.Data.Data, data.Data.Data.Symbol)));

            return await Subscribe(new[] { allBookTickerStreamEndpoint }, handler).ConfigureAwait(false);
        }

        #endregion All Book Tickers Stream

        #region Partial Book Depth Streams

        /// <summary>
        /// Subscribes to the depth updates for the provided symbol
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#partial-book-depth-streams</para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe on</param>
        /// <param name="levels">The amount of entries to be returned in the update</param>
        /// <param name="updateInterval">Update interval in milliseconds, either 100 or 1000. Defaults to 1000</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToPartialOrderBookUpdatesAsync(string symbol, int levels, int? updateInterval, Action<DataEvent<BinanceOrderBook>> onMessage) =>

await SubscribeToPartialOrderBookUpdatesAsync(new[] { symbol }, levels, updateInterval, onMessage).ConfigureAwait(false);

        /// <summary>
        /// Subscribes to the depth updates for the provided symbols
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#partial-book-depth-streams</para>
        /// </summary>
        /// <param name="symbols">The symbols to subscribe on</param>
        /// <param name="levels">The amount of entries to be returned in the update of each symbol</param>
        /// <param name="updateInterval">Update interval in milliseconds, either 100 or 1000. Defaults to 1000</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToPartialOrderBookUpdatesAsync(IEnumerable<string> symbols, int levels, int? updateInterval, Action<DataEvent<BinanceOrderBook>> onMessage)

        {
            symbols.ValidateNotNull(nameof(symbols));

            levels.ValidateIntValues(nameof(levels), 5, 10, 20);
            updateInterval?.ValidateIntValues(nameof(updateInterval), 100, 1000);
            var handler = new Action<DataEvent<BinanceCombinedStream<BinanceOrderBook>>>(data =>

            {
                data.Data.Data.Symbol = data.Data.Stream.Split('@')[0];
                onMessage(data.As<BinanceOrderBook>(data.Data.Data, data.Data.Data.Symbol));
            });

            symbols = symbols.Select(a =>
                a.ToLower(CultureInfo.InvariantCulture) + partialBookDepthStreamEndpoint + levels +
                (updateInterval.HasValue ? $"@{updateInterval.Value}ms" : "")).ToArray();

            return await Subscribe(symbols, handler).ConfigureAwait(false);
        }

        #endregion Partial Book Depth Streams

        #region Diff. Depth Stream

        /// <summary>
        /// Subscribes to the order book updates for the provided symbol
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#diff-depth-stream</para>
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="updateInterval">Update interval in milliseconds, either 100 or 1000. Defaults to 1000</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToOrderBookUpdatesAsync(string symbol, int? updateInterval, Action<DataEvent<BinanceEventOrderBook>> onMessage) =>

        await SubscribeToOrderBookUpdatesAsync(new[] { symbol }, updateInterval, onMessage).ConfigureAwait(false);

        /// <summary>
        /// Subscribes to the depth update stream for the provided symbols
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#diff-depth-stream</para>
        /// </summary>
        /// <param name="symbols">The symbols</param>
        /// <param name="updateInterval">Update interval in milliseconds, either 100 or 1000. Defaults to 1000</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToOrderBookUpdatesAsync(IEnumerable<string> symbols, int? updateInterval, Action<DataEvent<BinanceEventOrderBook>> onMessage)

        {
            symbols.ValidateNotNull(nameof(symbols));

            updateInterval?.ValidateIntValues(nameof(updateInterval), 100, 1000);
            var handler = new Action<DataEvent<BinanceCombinedStream<BinanceEventOrderBook>>>(data => onMessage(data.As<BinanceEventOrderBook>(data.Data.Data, data.Data.Data.Symbol)));

            symbols = symbols.Select(a => a.ToLower(CultureInfo.InvariantCulture) + depthStreamEndpoint + (updateInterval.HasValue ? $"@{updateInterval.Value}ms" : "")).ToArray();

            return await Subscribe(symbols, handler).ConfigureAwait(false);
        }

        #endregion Diff. Depth Stream

        #region Individual Symbol Ticker Streams

        /// <summary>
        /// Subscribes to ticker updates stream for a specific symbol
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#individual-symbol-ticker-streams</para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToSymbolTickerUpdatesAsync(string symbol, Action<DataEvent<BinanceStreamTick>> onMessage) =>

    await SubscribeToSymbolTickerUpdatesAsync(new[] { symbol }, onMessage).ConfigureAwait(false);

        /// <summary>
        /// Subscribes to ticker updates stream for a specific symbol
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#individual-symbol-ticker-streams</para>
        /// </summary>
        /// <param name="symbols">The symbols to subscribe to</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToSymbolTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<BinanceStreamTick>> onMessage)

        {
            symbols.ValidateNotNull(nameof(symbols));
            var handler = new Action<DataEvent<BinanceCombinedStream<BinanceStreamTick>>>(data => onMessage(data.As<BinanceStreamTick>(data.Data.Data, data.Data.Data.Symbol)));

            symbols = symbols.Select(a => a.ToLower(CultureInfo.InvariantCulture) + symbolTickerStreamEndpoint).ToArray();

            return await Subscribe(symbols, handler).ConfigureAwait(false);
        }

        #endregion Individual Symbol Ticker Streams

        #region All Market Tickers Streams

        /// <summary>
        /// Subscribes to ticker updates stream for all symbols
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#all-market-tickers-stream</para>
        /// </summary>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToAllSymbolTickerUpdatesAsync(Action<DataEvent<IEnumerable<BinanceStreamTick>>> onMessage)
        {
            var handler = new Action<DataEvent<BinanceCombinedStream<IEnumerable<BinanceStreamTick>>>>(data => onMessage(data.As<IEnumerable<BinanceStreamTick>>(data.Data.Data, data.Data.Stream)));
            return await Subscribe(new[] { allSymbolTickerStreamEndpoint }, handler).ConfigureAwait(false);
        }

        #endregion All Market Tickers Streams

        #region User Data Stream

        /// <summary>
        /// Subscribes to the account update stream. Prior to using this, the BinanceClient.Spot.UserStreams.StartUserStream method should be called.
        /// <para>[WSS] https://binance-docs.github.io/apidocs/spot/en/#user-data-streams</para>
        /// </summary>
        /// <param name="listenKey">Listen key retrieved by the StartUserStream method</param>
        /// <param name="onOrderUpdateMessage">The event handler for whenever an order status update is received</param>
        /// <param name="onAccountPositionMessage">The event handler for whenever an account position update is received. Account position updates are a list of changed funds</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<BaseSocketClient>> SubscribeToUserDataUpdatesAsync(
          string listenKey,
          Action<DataEvent<BinanceStreamOrderUpdate>>? onOrderUpdateMessage,
          Action<DataEvent<BinanceStreamPositionsUpdate>>? onAccountPositionMessage)
        {
            listenKey.ValidateNotNull(nameof(listenKey));
            var handler = new Action<DataEvent<string>>(data =>
            {
                var combinedToken = JToken.Parse(data.Data);
                var token = combinedToken["data"];
                if (token == null)
                {
                    return;
                }

                var evnt = token["e"];
                if (evnt == null)
                {
                    return;
                }

                switch (evnt.ToString())
                {
                    case executionUpdateEvent:
                        {
                            var result = Json.Deserialize<BinanceStreamOrderUpdate>(token);
                            if (result)
                                onOrderUpdateMessage?.Invoke(data.As(result.Data, result.Data.OrderId.ToString()));

                            break;
                        }
                    case accountPositionUpdateEvent:
                        {
                            var result = Json.Deserialize<BinanceStreamPositionsUpdate>(token);
                            if (result)
                                onAccountPositionMessage?.Invoke(data.As(result.Data));

                            break;
                        }
                    default: break;
                }
            });

            return await Subscribe(new[] { listenKey }, handler, true).ConfigureAwait(false);
        }

        #endregion User Data Stream

        private async Task<CallResult<BaseSocketClient>> Subscribe<T>(IEnumerable<string> topics, Action<DataEvent<T>> onData, bool userStream = false)
        {
            var connectSubscription = await _baseClient.SubscribeInternal(UriClient.GetBaseAddress() + "stream", topics, onData, userStream).ConfigureAwait(false);
            if (connectSubscription.Success)
            {
                await connectSubscription.Data.InternalConnectSocketAsync().ConfigureAwait(false);
            }

            return connectSubscription;
        }
    }
}
