﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;

namespace Nop.Plugin.Api.Common.IdentityServer.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection) => collection.ToScrubbedDictionary();

        public static string ToFormPost(this NameValueCollection collection)
        {
            var builder = new StringBuilder(128);
            const string inputFieldFormat = "<input type='hidden' name='{0}' value='{1}' />\n";

            foreach (string name in collection)
            {
                string[] values = collection.GetValues(name);
                string value = values.First();
                value = HtmlEncoder.Default.Encode(value);
                builder.AppendFormat(inputFieldFormat, name, value);
            }

            return builder.ToString();
        }

        public static NameValueCollection ToNameValueCollection(this Dictionary<string, string> data)
        {
            var result = new NameValueCollection();

            if (data == null || data.Count == 0) return result;

            foreach (string name in data.Keys)
            {
                string value = data[name];
                if (value != null) result.Add(name, value);
            }

            return result;
        }

        public static string ToQueryString(this NameValueCollection collection)
        {
            if (collection.Count == 0) return string.Empty;

            var builder = new StringBuilder(128);
            var first = true;
            foreach (string name in collection)
            {
                string[] values = collection.GetValues(name);
                if (values == null || values.Length == 0)
                    first = AppendNameValuePair(builder, first, true, name, string.Empty);
                else
                    foreach (string value in values)
                        first = AppendNameValuePair(builder, first, true, name, value);
            }

            return builder.ToString();
        }

        public static Dictionary<string, string> ToScrubbedDictionary(this NameValueCollection collection, params string[] nameFilter)
        {
            var dict = new Dictionary<string, string>();

            if (collection == null || collection.Count == 0) return dict;

            foreach (string name in collection)
            {
                string value = collection.Get(name);
                if (value != null)
                {
                    if (nameFilter.Contains(name)) value = "***REDACTED***";
                    dict.Add(name, value);
                }
            }

            return dict;
        }

        internal static string ConvertFormUrlEncodedSpacesToUrlEncodedSpaces(string str)
        {
            if (str != null && str.IndexOf('+') >= 0) str = str.Replace("+", "%20");
            return str;
        }

        private static bool AppendNameValuePair(StringBuilder builder, bool first, bool urlEncode, string name, string value)
        {
            string effectiveName = name ?? string.Empty;
            string encodedName = urlEncode ? UrlEncoder.Default.Encode(effectiveName) : effectiveName;

            string effectiveValue = value ?? string.Empty;
            string encodedValue = urlEncode ? UrlEncoder.Default.Encode(effectiveValue) : effectiveValue;
            encodedValue = ConvertFormUrlEncodedSpacesToUrlEncodedSpaces(encodedValue);

            if (first)
                first = false;
            else
                builder.Append("&");

            builder.Append(encodedName);
            if (!string.IsNullOrEmpty(encodedValue))
            {
                builder.Append("=");
                builder.Append(encodedValue);
            }

            return first;
        }
    }
}