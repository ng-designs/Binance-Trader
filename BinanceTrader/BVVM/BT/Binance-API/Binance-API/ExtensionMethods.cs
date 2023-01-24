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
using System.Runtime.InteropServices;
using System.Security;

namespace BinanceAPI
{
    /// <summary>
    /// Helper methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Add a parameter
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddParameter(this Dictionary<string, object> parameters, string key, string value)
        {
            parameters.Add(key, value);
        }

        /// <summary>
        /// Add an optional parameter. Not added if value is null
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOptionalParameter(this Dictionary<string, object> parameters, string key, object? value)
        {
            if (value != null)
                parameters.Add(key, value);
        }

        /// <summary>
        /// Create a query string of the specified parameters
        /// </summary>
        /// <param name="parameters">The parameters to use</param>
        /// <param name="urlEncodeValues">Whether or not the values should be url encoded</param>
        /// <param name="serializationType">How to serialize array parameters</param>
        /// <returns></returns>
        public static string CreateParamString(this Dictionary<string, object> parameters, bool urlEncodeValues, ArrayParametersSerialization serializationType)
        {
            var uriString = string.Empty;
            var arraysParameters = parameters.Where(p => p.Value.GetType().IsArray).ToList();
            foreach (var arrayEntry in arraysParameters)
            {
                if (serializationType == ArrayParametersSerialization.Array)
                    uriString += $"{string.Join("&", ((object[])(urlEncodeValues ? Uri.EscapeDataString(arrayEntry.Value.ToString()) : arrayEntry.Value)).Select(v => $"{arrayEntry.Key}[]={v}"))}&";
                else
                {
                    var array = (Array)arrayEntry.Value;
                    uriString += string.Join("&", array.OfType<object>().Select(a => $"{arrayEntry.Key}={Uri.EscapeDataString(a.ToString())}"));
                    uriString += "&";
                }
            }

            uriString += $"{string.Join("&", parameters.Where(p => !p.Value.GetType().IsArray).Select(s => $"{s.Key}={(urlEncodeValues ? Uri.EscapeDataString(s.Value.ToString()) : s.Value)}"))}";
            uriString = uriString.TrimEnd('&');
            return uriString;
        }

        /// <summary>
        /// Get the string the secure string is representing
        /// </summary>
        /// <param name="source">The source secure string</param>
        /// <returns></returns>
        public static string GetString(this SecureString source)
        {
            lock (source)
            {
                string result;
                var length = source.Length;
                var pointer = IntPtr.Zero;
                var chars = new char[length];

                try
                {
                    pointer = Marshal.SecureStringToBSTR(source);
                    Marshal.Copy(pointer, chars, 0, length);

                    result = string.Join("", chars);
                }
                finally
                {
                    if (pointer != IntPtr.Zero)
                    {
                        Marshal.ZeroFreeBSTR(pointer);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Create a secure string from a string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static SecureString ToSecureString(this string source)
        {
            var secureString = new SecureString();
            foreach (var c in source)
                secureString.AppendChar(c);
            secureString.MakeReadOnly();
            return secureString;
        }

        /// <summary>
        /// Validates an int is one of the allowed values
        /// </summary>
        /// <param name="value">Value of the int</param>
        /// <param name="argumentName">Name of the parameter</param>
        /// <param name="allowedValues">Allowed values</param>
        public static void ValidateIntValues(this int value, string argumentName, params int[] allowedValues)
        {
            if (!allowedValues.Contains(value))
                throw new ArgumentException(
                    $"{value} not allowed for parameter {argumentName}, allowed values: {string.Join(", ", allowedValues)}", argumentName);
        }

        /// <summary>
        /// Validates an int is between two values
        /// </summary>
        /// <param name="value">The value of the int</param>
        /// <param name="argumentName">Name of the parameter</param>
        /// <param name="minValue">Min value</param>
        /// <param name="maxValue">Max value</param>
        public static void ValidateIntBetween(this int value, string argumentName, int minValue, int maxValue)
        {
            if (value < minValue || value > maxValue)
                throw new ArgumentException(
                    $"{value} not allowed for parameter {argumentName}, min: {minValue}, max: {maxValue}", argumentName);
        }

        /// <summary>
        /// Validates a string is not null or empty
        /// </summary>
        /// <param name="value">The value of the string</param>
        /// <param name="argumentName">Name of the parameter</param>
        public static void ValidateNotNull(this string value, string argumentName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"No value provided for parameter {argumentName}", argumentName);
        }

        /// <summary>
        /// Validates a list is not null or empty
        /// </summary>
        /// <param name="value">The value of the object</param>
        /// <param name="argumentName">Name of the parameter</param>
        public static void ValidateNotNull<T>(this IEnumerable<T> value, string argumentName)
        {
            if (value == null || !value.Any())
                throw new ArgumentException($"No values provided for parameter {argumentName}", argumentName);
        }
    }
}
