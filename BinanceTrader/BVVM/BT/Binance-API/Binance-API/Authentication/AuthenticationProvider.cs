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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BinanceAPI.Authentication
{
    /// <summary>
    /// Base class for authentication providers
    /// </summary>
    public class AuthenticationProvider : IDisposable
    {
        private readonly object signLock = new();
        private readonly HMACSHA256 encryptor;

        /// <summary>
        /// The provided credentials
        /// </summary>
        protected private static ApiCredentials? Credentials { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="credentials"></param>
        public AuthenticationProvider(ApiCredentials credentials)
        {
            if (credentials.Secret == null)
                throw new ArgumentException("No valid API credentials provided. Key/Secret needed.");

            Credentials = credentials;
            encryptor = new HMACSHA256(Encoding.ASCII.GetBytes(credentials.Secret.GetString()));
        }

        /// <summary>
        /// Add authentication to the parameter list based on the provided credentials
        /// </summary>
        /// <param name="uri">The uri the request is for</param>
        /// <param name="method">The HTTP method of the request</param>
        /// <param name="parameters">The provided parameters for the request</param>
        /// <param name="signed">Wether or not the request needs to be signed. If not typically the parameters list can just be returned</param>
        /// <param name="parameterPosition">Where parameters are placed, in the URI or in the request body</param>
        /// <param name="arraySerialization">How array parameters are serialized</param>
        /// <returns>Should return the original parameter list including any authentication parameters needed</returns>
        public virtual Dictionary<string, object> AddAuthenticationToParameters(string uri, HttpMethod method, Dictionary<string, object> parameters, bool signed, HttpMethodParameterPosition parameterPosition, ArrayParametersSerialization arraySerialization)
        {
            if (!signed)
                return parameters;

            parameters.AddParameter("timestamp", ServerTimeClient.GetRequestTimestamp());

            string signData;
            if (parameterPosition == HttpMethodParameterPosition.InUri)
            {
                signData = parameters.CreateParamString(true, arraySerialization);
            }
            else
            {
                var formData = HttpUtility.ParseQueryString(string.Empty);
                foreach (var kvp in parameters.OrderBy(p => p.Key))
                {
                    if (kvp.Value.GetType().IsArray)
                    {
                        var array = (Array)kvp.Value;
                        foreach (var value in array)
                            formData.Add(kvp.Key, value.ToString());
                    }
                    else
                        formData.Add(kvp.Key, kvp.Value.ToString());
                }
                signData = formData.ToString();
            }

            lock (signLock)
                parameters.Add("signature", ByteToString(encryptor.ComputeHash(Encoding.UTF8.GetBytes(signData))));
            return parameters;
        }

        /// <summary>
        /// Add authentication to the header dictionary based on the provided credentials
        /// </summary>
        /// <param name="uri">The uri the request is for</param>
        /// <param name="method">The HTTP method of the request</param>
        /// <param name="parameters">The provided parameters for the request</param>
        /// <param name="signed">Wether or not the request needs to be signed. If not typically the parameters list can just be returned</param>
        /// <param name="parameterPosition">Where post parameters are placed, in the URI or in the request body</param>
        /// <param name="arraySerialization">How array parameters are serialized</param>
        /// <returns>Should return a dictionary containing any header key/value pairs needed for authenticating the request</returns>
        public virtual Dictionary<string, string> AddAuthenticationToHeaders(string uri, HttpMethod method, Dictionary<string, object> parameters, bool signed, HttpMethodParameterPosition parameterPosition, ArrayParametersSerialization arraySerialization)
        {
            if (Credentials == null || Credentials.Key == null)
                throw new ArgumentException("No valid API credentials provided. Key/Secret needed.");

            return new Dictionary<string, string> { { "X-MBX-APIKEY", Credentials.Key.GetString() } };
        }

        /// <summary>
        /// Convert byte array to hex
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        protected static string ByteToString(byte[] buff)
        {
            var result = string.Empty;
            foreach (var t in buff)
                result += t.ToString("X2"); /* hex format */
            return result;
        }

        /// <summary>
        /// Dispose the Authentication Provider
        /// </summary>
        public void Dispose()
        {
            encryptor.Dispose();
            Credentials?.Dispose();
        }
    }
}
