using Newtonsoft.Json;
using OloApiClient.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace OloApiClient
{
    /// <summary>
    /// OLO API Client
    /// </summary>
    public class OloClient
    {
        #region "Constants and local variables"

        private readonly string _apiKey;
        private readonly string _clientSecret;
        private readonly string _clientId;

        private readonly HttpClient _client;

        const string HttpContentType = "application/json";

        #endregion

        #region "CTOR"

        private OloClient() { }

        /// <summary>
        /// CTOR (alternative #1)
        /// <para>Use APIKEY as proof</para>
        /// </summary>
        /// <param name="handler">DelegatingHandler</param>
        /// <param name="baseApiUrl">Url to Test or Production</param>
        /// <param name="apiKey">apiKey</param>
        public OloClient(DelegatingHandler handler, string baseApiUrl, string apiKey)
        {
            _apiKey = apiKey;
            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseApiUrl)
            };
        }

        /// <summary>
        /// CTOR (alternative #2)
        /// <para>Use client id and secret as proof</para>
        /// </summary>
        /// <param name="handler">DelegatingHandler</param>
        /// <param name="baseApiUrl">Url to Test or Production</param>
        /// <param name="clientId">Client Id</param>
        /// <param name="clientSecret">Client Secret</param>
        public OloClient(DelegatingHandler handler, string baseApiUrl, string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseApiUrl)
            };
        }

        #endregion

        #region "GET"

        /// <summary>
        /// Get, as Json
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="parameters">(Optional) Parameters</param>
        /// <returns>JSON</returns>
        public dynamic Get(string path, IDictionary<string, string> parameters = null)
        {
            var responseContent = GetJsonAsString(path, parameters);
            return JsonConvert.DeserializeObject(responseContent);
        }

        /// <summary>
        /// GET, return string
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="parameters">(Optional) Parameters</param>
        /// <returns>String</returns>
        public string GetJsonAsString(string path, IDictionary<string, string> parameters = null)
        {
            var finalUrl = GetFinalUrl(path, parameters);

            AddHeaders("GET", string.Empty, finalUrl);

            var response = _client.GetAsync(finalUrl).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            ValidateResponse(response, responseContent);
            return responseContent;
        }

        #endregion

        #region "POST"

        /// <summary>
        /// Post, return JSON
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="data">(optional) Body Payload to POST</param>
        /// <param name="parameters">(Optional) Parameters</param>
        /// <returns>Results (JSON)</returns>
        public dynamic Post(string path, object data, IDictionary<string, string> parameters = null)
        {
            var responseContent = PostJsonAsString(path, data, parameters);
            return JsonConvert.DeserializeObject(responseContent);
        }

        /// <summary>
        /// Post Verb, return string
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="data">(optional) Body Payload to POST</param>
        /// <param name="parameters">(Optional) Parameters</param>
        /// <returns>Results</returns>
        public string PostJsonAsString(string path, object data, IDictionary<string, string> parameters = null)
        {
            var finalUrl = GetFinalUrl(path, parameters);

            string jData = string.Empty;
            if (data != null) jData = JsonConvert.SerializeObject(data);

            AddHeaders("POST", jData, finalUrl);

            HttpContent content = null;
            if (string.IsNullOrWhiteSpace(jData))
            {
                content = new StringContent(jData);
                content.Headers.Remove("content-type");
            }
            else
            {
                content = new StringContent(jData, Encoding.UTF8, HttpContentType);
            }

            var response = _client.PostAsync(finalUrl, content).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            ValidateResponse(response, responseContent);
            return responseContent;
        }

        /// <summary>
        /// Post To Get the ZIP of a batch
        /// <para>Do not use for regular POST operations</para>
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="data">(optional) Body Payload to POST</param>
        /// <param name="parameters">(Optional) Parameters</param>
        /// <returns>Results</returns>
        public byte[] PostToGetUrl(string path, object data, IDictionary<string, string> parameters = null)
        {
            var finalUrl = GetFinalUrl(path, parameters);

            string jData = string.Empty;

            if (data != null) jData = JsonConvert.SerializeObject(data);

            AddHeaders("POST", jData, finalUrl);

            HttpContent content = null;
            if (string.IsNullOrWhiteSpace(jData))
            {
                content = new StringContent(jData);
                content.Headers.Remove("content-type");
            }
            else
            {
                content = new StringContent(jData, Encoding.UTF8, HttpContentType);
            }

            var response = _client.PostAsync(finalUrl, content).Result;
            var responseContent = response.Content.ReadAsByteArrayAsync().Result;

            ValidateResponse(response, null);

            return responseContent;
        }

        #endregion

        #region "Delete"

        /// <summary>
        /// Delete Verb
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="parameters">(Optional) Parameters</param>
        /// <returns>Any payload</returns>
        public string Delete(string path, IDictionary<string, string> parameters = null)
        {
            var responseContent = string.Empty;
            var finalUrl = GetFinalUrl(path, parameters);

            AddHeaders("DELETE", string.Empty, finalUrl);

            var response = _client.DeleteAsync(finalUrl).Result;
            responseContent = response.Content.ReadAsStringAsync().Result;
            ValidateResponse(response, responseContent);
            return responseContent;
        }

        #endregion

        #region "Utilities"

        /// <summary>
        /// Make a final taget URL from the relative path + any parameters
        /// <para>If the apikey is set, it is added to the querystring</para>
        /// </summary>
        /// <param name="path">relative path</param>
        /// <param name="parameters">(optional) parameters</param>
        /// <returns>Final URL</returns>
        private string GetFinalUrl(string path, IDictionary<string, string> parameters)
        {
            var finalUrl = path;

            // Add in API key if present
            if (!string.IsNullOrWhiteSpace(_apiKey)) finalUrl += "?key=" + _apiKey;
            else finalUrl += "?";

            // Add parameters
            if (parameters != null)
            {
                foreach (var p in parameters)
                    finalUrl += "&" + p.Key + "=" + p.Value;
            }

            // Clean up trailing ? 
            if (finalUrl.EndsWith("?")) finalUrl = finalUrl.Substring(0, finalUrl.Length - 1);

            return finalUrl;
        }

        /// <summary>
        /// Validate the response andn throw the <see>OloException</see> exception if needed
        /// </summary>
        /// <param name="response">HttpResponseMessage</param>
        /// <param name="responseContent">String of Content Returned</param>
        /// <exception cref="OloException">Status Code is not ok, throw this exception</exception>
        private static void ValidateResponse(HttpResponseMessage response, string responseContent)
        {
            if (!response.IsSuccessStatusCode)
            {
                // Parse the response body for the OLO specific error
                Error error = null;
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) error = JsonConvert.DeserializeObject<Error>(responseContent);

                var message = string.Format("Error with {0} {1} http status code: {2}",
                        response.RequestMessage.Method,
                        response.RequestMessage.RequestUri,
                        response.StatusCode);
                
                // This client throws a nice custom exception
                throw new OloException(message, error);
            }
        }

        /// <summary>
        /// Add Authorization headers if client Id and Secret are present
        /// <para>Otherwise we assume IP Address Restictions + API KEY</para>
        /// </summary>
        /// <param name="verb">HTTP Verb</param>
        /// <param name="body">Body (or empty string)</param>
        /// <param name="uriPathAndQuery">URL Path and Query</param>
        private void AddHeaders(string verb, string body, string uriPathAndQuery)
        {
            if (string.IsNullOrWhiteSpace(_clientId) || string.IsNullOrWhiteSpace(_clientSecret)) return;

            var date = Rfc1123Date();
            var sig = MakeSignature(date, verb.ToUpperInvariant(), body, uriPathAndQuery);
            var auth = string.Format("OloSignature {0}:{1}", _clientId, sig);

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Date", date);
            _client.DefaultRequestHeaders.Add("Authorization", auth);
        }

        /// <summary>
        /// Current date RFC 1123
        /// <para>Less than obvious to compute</para>
        /// </summary>
        /// <returns>Rfc1123Date</returns>
        private string Rfc1123Date()
        {
            return DateTime.Now.ToUniversalTime().ToString("R");
        }

        /// <summary>
        /// Make the "Magic" OLO signature
        /// </summary>
        /// <param name="rfc1123Date">rfc 1123 Date</param>
        /// <param name="httpVerb">Verb</param>
        /// <param name="httpBody">Body</param>
        /// <param name="uriPathAndQuery">Relative URI and Query</param>
        /// <returns>Crypto Signature</returns>
        private string MakeSignature(string rfc1123Date, string httpVerb, string httpBody, string uriPathAndQuery)
        {
            // Verb has to be upper case
            httpVerb = httpVerb.ToUpperInvariant();

            // URL needs to be the full relative path
            Regex rgx = new Regex(@"/https?:\/{2}[^\/]+\//i");
            string url = rgx.Replace(uriPathAndQuery, "/");

            // Force empty content type on GET
            string contentType = (httpVerb == "GET") ? string.Empty : HttpContentType;

            // Force an empty content type if the body is empty
            if (string.IsNullOrWhiteSpace(httpBody))
            {
                httpBody = string.Empty;
                contentType = string.Empty;
            }

            // Hash the payload
            var payload = HashSha256(httpBody);

            // Here is the thing to HMAC, but notice that we set up empty fields in some cases
            var stringToSign = string.Join("\n",
                           _clientId,
                           httpVerb,
                           contentType,
                           payload,
                           url,
                           rfc1123Date);

            // HMAC => signature
            var signature = HmacSha256(_clientSecret, stringToSign);

            return signature;
        }

        #endregion

        #region "Hash"

        /// <summary>
        /// Returns Base64 SHA256 Hash of a string
        /// </summary>
        /// <param name="text">Thing to cypher</param>
        /// <returns>Base64 Hash</returns>
        private static string HashSha256(string text)
        {
            SHA256Managed sha256 = new SHA256Managed();

            byte[] bytes = Encoding.UTF8.GetBytes(text);
            byte[] hash = sha256.ComputeHash(bytes);
            var base64 = Convert.ToBase64String(hash);
            return base64;
        }

        /// <summary>
        /// Returns Base64 SHA256 HMAC as a string
        /// </summary>
        /// <param name="clientSecret">HMAC Key</param>
        /// <param name="text">Thing to cypher</param>
        /// <returns>Base64 HMAC</returns>
        private static string HmacSha256(string clientSecret, string text)
        {
            var result = string.Empty;
            var key = Encoding.UTF8.GetBytes(clientSecret);
            var buffer = Encoding.UTF8.GetBytes(text);
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                var hash = hmac.ComputeHash(buffer);
                result = Convert.ToBase64String(hash);
            }
            return result;
        }

        #endregion

    }

}
