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

using System.Diagnostics.CodeAnalysis;

namespace BinanceAPI.Sockets
{
    /// <summary>
    /// An update received from a socket update subscription
    /// </summary>
    /// <typeparam name="T">The type of the data</typeparam>
    public class DataEvent<T>
    {
        /// <summary>
        /// The topic of the update, what symbol/asset etc..
        /// </summary>
        [AllowNull]
        public string Topic { get; set; }

        /// <summary>
        /// The received data deserialized into an object
        /// </summary>
        public T Data { get; set; }

        internal DataEvent(T data, [AllowNull] string topic)
        {
            Data = data;
            Topic = topic;
        }

        /// <summary>
        /// Create a new DataEvent with data in the from of type K based on the current DataEvent.
        /// </summary>
        /// <typeparam name="K">The type of the new data</typeparam>
        /// <param name="data">The new data</param>
        /// <returns></returns>
        public DataEvent<K> As<K>(K data)
        {
            return new DataEvent<K>(data, Topic);
        }

        /// <summary>
        /// Create a new DataEvent with data in the from of type K based on the current DataEvent.
        /// </summary>
        /// <typeparam name="K">The type of the new data</typeparam>
        /// <param name="data">The new data</param>
        /// <param name="topic">The new topic</param>
        /// <returns></returns>
        public DataEvent<K> As<K>(K data, [AllowNull] string topic)
        {
            return new DataEvent<K>(data, topic);
        }
    }
}
