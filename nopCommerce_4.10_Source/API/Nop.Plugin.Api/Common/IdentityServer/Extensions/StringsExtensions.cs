using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Nop.Plugin.Api.Common.IdentityServer.Extensions
{
    internal static class StringExtensions
    {
        [DebuggerStepThrough]
        public static string AddHashFragment(this string url, string query)
        {
            if (!url.Contains("#")) url += "#";

            return url + query;
        }

        [DebuggerStepThrough]
        public static string AddQueryString(this string url, string query)
        {
            if (!url.Contains("?"))
                url += "?";
            else if (!url.EndsWith("&")) url += "&";

            return url + query;
        }

        [DebuggerStepThrough]
        public static string AddQueryString(this string url, string name, string value) => url.AddQueryString(name + "=" + UrlEncoder.Default.Encode(value));

        [DebuggerStepThrough]
        public static string CleanUrlPath(this string url)
        {
            if (string.IsNullOrWhiteSpace(url)) url = "/";

            if (url != "/" && url.EndsWith("/")) url = url.Substring(0, url.Length - 1);

            return url;
        }

        [DebuggerStepThrough]
        public static string EnsureLeadingSlash(this string url)
        {
            if (!url.StartsWith("/")) return "/" + url;

            return url;
        }

        [DebuggerStepThrough]
        public static string EnsureTrailingSlash(this string url)
        {
            if (!url.EndsWith("/")) return url + "/";

            return url;
        }

        [DebuggerStepThrough]
        public static IEnumerable<string> FromSpaceSeparatedString(this string input)
        {
            input = input.Trim();
            return input.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static string GetOrigin(this string url)
        {
            if (url != null)
            {
                var uri = new Uri(url);
                if (uri.Scheme == "http" || uri.Scheme == "https") return $"{uri.Scheme}://{uri.Authority}";
            }

            return null;
        }

        [DebuggerStepThrough]
        public static bool IsLocalUrl(this string url) => !string.IsNullOrEmpty(url) &&

                                                          // Allows "/" or "/foo" but not "//" or "/\".
                                                          (url[0] == '/' && (url.Length == 1 || url[1] != '/' && url[1] != '\\') ||

                                                           // Allows "~/" or "~/foo".
                                                           url.Length > 1 && url[0] == '~' && url[1] == '/');

        [DebuggerStepThrough]
        public static bool IsMissing(this string value) => string.IsNullOrWhiteSpace(value);

        [DebuggerStepThrough]
        public static bool IsMissingOrTooLong(this string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            if (value.Length > maxLength) return true;

            return false;
        }

        [DebuggerStepThrough]
        public static bool IsPresent(this string value) => !string.IsNullOrWhiteSpace(value);

        public static List<string> ParseScopesString(this string scopes)
        {
            if (scopes.IsMissing()) return null;

            scopes = scopes.Trim();
            List<string> parsedScopes = scopes.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();

            if (parsedScopes.Any())
            {
                parsedScopes.Sort();
                return parsedScopes;
            }

            return null;
        }

        [DebuggerStepThrough]
        public static NameValueCollection ReadQueryStringAsNameValueCollection(this string url)
        {
            if (url != null)
            {
                int idx = url.IndexOf('?');
                if (idx >= 0) url = url.Substring(idx + 1);
                Dictionary<string, StringValues> query = QueryHelpers.ParseNullableQuery(url);
                if (query != null) return query.AsNameValueCollection();
            }

            return new NameValueCollection();
        }

        [DebuggerStepThrough]
        public static string RemoveLeadingSlash(this string url)
        {
            if (url != null && url.StartsWith("/")) url = url.Substring(1);

            return url;
        }

        [DebuggerStepThrough]
        public static string RemoveTrailingSlash(this string url)
        {
            if (url != null && url.EndsWith("/")) url = url.Substring(0, url.Length - 1);

            return url;
        }

        [DebuggerStepThrough]
        public static string ToSpaceSeparatedString(this IEnumerable<string> list)
        {
            if (list == null) return "";

            var sb = new StringBuilder(100);

            foreach (string element in list) sb.Append(element + " ");

            return sb.ToString().Trim();
        }
    }
}