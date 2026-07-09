using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GreenLine.Classes
{
    public class PhonePeHelper
    {
        private static string _cachedToken = null;
        private static DateTime _tokenExpiry = DateTime.MinValue;
        private static readonly object _lock = new object();

        private const string BaseUrl = "https://api-preprod.phonepe.com/apis/pg-sandbox";
        private const string TokenUrl = BaseUrl + "/v1/oauth/token";
        private const string CreatePayLinkUrl = BaseUrl + "/paylinks/v1/pay";
        
        private const string ClientId = "GREENLINEUAT_26052110552";
        private const string ClientVersion = "GREENLINEUAT";
        private const string ClientSecret = "MDg1YmI1MWQtYzBhNi00ODg0LTg1YTYtMWI2ZjExNTdiZmIw";

        public static async Task<string> GetTokenAsync()
        {
            if (_cachedToken != null && DateTime.Now < _tokenExpiry)
            {
                return _cachedToken;
            }

            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "client_id", ClientId },
                    { "client_version", ClientVersion },
                    { "client_secret", ClientSecret },
                    { "grant_type", "client_credentials" }
                };

                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync(TokenUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseString);
                    if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.access_token))
                    {
                        lock (_lock)
                        {
                            _cachedToken = tokenResponse.access_token;
                            // Set expiry slightly earlier than actual to be safe (e.g. subtract 60 seconds)
                            int expiresIn = tokenResponse.expires_in > 60 ? tokenResponse.expires_in - 60 : tokenResponse.expires_in;
                            _tokenExpiry = DateTime.Now.AddSeconds(expiresIn);
                        }
                        return _cachedToken;
                    }
                }
                
                string errContent = "";
                try { errContent = await response.Content.ReadAsStringAsync(); } catch { }
                throw new Exception($"Failed to retrieve authentication token from PhonePe API. Status: {response.StatusCode}. Response: {errContent}");
            }
        }

        public static async Task<PayLinkResponseData> CreatePayLinkAsync(decimal amount, string merchantOrderId, string customerName, string customerPhone, string customerEmail)
        {
            string token = await GetTokenAsync();

            // Convert amount to paise (1 Rupee = 100 Paise)
            long amountInPaise = (long)(amount * 100);

            // Expiry 1 hour from now
            long expireAtEpoch = DateTimeOffset.Now.AddHours(1).ToUnixTimeMilliseconds();

            // Formulate phone number format: must be +91 followed by 10 digits
            string formattedPhone = FormatPhoneNumber(customerPhone);
            string formattedEmail = string.IsNullOrWhiteSpace(customerEmail) ? "info@greenline.com" : customerEmail.Trim();
            string formattedName = string.IsNullOrWhiteSpace(customerName) ? "Customer" : customerName.Trim();

            var payload = new
            {
                amount = amountInPaise,
                metaInfo = new Dictionary<string, string>
                {
                    { "udf1", "" }, { "udf2", "" }, { "udf3", "" }, { "udf4", "" }, { "udf5", "" },
                    { "udf6", "" }, { "udf7", "" }, { "udf8", "" }, { "udf9", "" }, { "udf10", "" },
                    { "udf11", "" }, { "udf12", "" }, { "udf13", "" }, { "udf14", "" }, { "udf15", "" }
                },
                paymentFlow = new
                {
                    type = "PAYLINK",
                    customerDetails = new
                    {
                        name = formattedName,
                        phoneNumber = formattedPhone,
                        email = formattedEmail
                    },
                    notificationChannels = new
                    {
                        SMS = true,
                        EMAIL = true
                    },
                    expireAt = expireAtEpoch
                },
                merchantOrderId = merchantOrderId
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "O-Bearer " + token);

                string jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(CreatePayLinkUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<PhonePeApiResponse<PayLinkResponseData>>(responseString);
                    if (apiResponse != null && apiResponse.success && apiResponse.data != null)
                    {
                        return apiResponse.data;
                    }

                    var directData = JsonConvert.DeserializeObject<PayLinkResponseData>(responseString);
                    if (directData != null && !string.IsNullOrEmpty(directData.GetUrl()))
                    {
                        directData.merchantOrderId = merchantOrderId;
                        directData.payLinkId = directData.orderId;
                        directData.paymentUrl = directData.GetUrl();
                        directData.payLink = directData.paymentUrl;
                        directData.shortUrl = directData.paymentUrl;
                        directData.status = directData.state ?? "CREATED";
                        if (long.TryParse(directData.expireAt, out long exp))
                        {
                            directData.expiryTime = exp;
                        }

                        try
                        {
                            string url = directData.paymentUrl;
                            int tokenIdx = url.IndexOf("token=");
                            if (tokenIdx >= 0)
                            {
                                string tokenVal = url.Substring(tokenIdx + 6);
                                int ampIdx = tokenVal.IndexOf('&');
                                if (ampIdx >= 0)
                                {
                                    tokenVal = tokenVal.Substring(0, ampIdx);
                                }
                                directData.token = tokenVal;
                            }
                        }
                        catch
                        {
                            directData.token = "";
                        }

                        return directData;
                    }

                    string errMsg = (apiResponse != null && !string.IsNullOrEmpty(apiResponse.message)) ? apiResponse.message : "Invalid API response format.";
                    throw new Exception("PhonePe Error: " + errMsg);
                }
                else
                {
                    throw new Exception($"PhonePe API call failed. Status: {response.StatusCode}. Response: {responseString}");
                }
            }
        }

        public static async Task<string> CheckPaymentStatusAsync(string merchantOrderId)
        {
            string token = await GetTokenAsync();
            string statusUrl = $"{BaseUrl}/paylinks/v1/{merchantOrderId}/status?details=true";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "O-Bearer " + token);

                var response = await client.GetAsync(statusUrl);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<PhonePeApiResponse<StatusResponseData>>(responseString);
                    if (apiResponse != null && apiResponse.success && apiResponse.data != null)
                    {
                        return apiResponse.data.status;
                    }

                    var directData = JsonConvert.DeserializeObject<StatusResponseData>(responseString);
                    if (directData != null)
                    {
                        return directData.status ?? directData.state ?? "UNKNOWN";
                    }

                    return "UNKNOWN";
                }
                else
                {
                    // If it is 404 or something, the order might not exist or expired
                    return "UNKNOWN";
                }
            }
        }

        public static async Task<StatusResponseData> GetPaymentStatusDetailsAsync(string merchantOrderId)
        {
            string token = await GetTokenAsync();
            string statusUrl = $"{BaseUrl}/paylinks/v1/{merchantOrderId}/status?details=true";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "O-Bearer " + token);

                var response = await client.GetAsync(statusUrl);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<PhonePeApiResponse<StatusResponseData>>(responseString);
                    if (apiResponse != null && apiResponse.success && apiResponse.data != null)
                    {
                        return apiResponse.data;
                    }

                    var directData = JsonConvert.DeserializeObject<StatusResponseData>(responseString);
                    if (directData != null)
                    {
                        directData.status = directData.status ?? directData.state ?? "UNKNOWN";
                        return directData;
                    }
                }
                return null;
            }
        }

        private static string FormatPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return "+919999999999"; // default fallback
            }

            string clean = "";
            foreach (char c in phone)
            {
                if (char.IsDigit(c))
                {
                    clean += c;
                }
            }

            if (clean.Length > 10)
            {
                clean = clean.Substring(clean.Length - 10);
            }

            if (clean.Length == 10)
            {
                return "+91" + clean;
            }

            return "+919999999999"; // default fallback if invalid length
        }

        public class TokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
        }

        public class PhonePeApiResponse<T>
        {
            public bool success { get; set; }
            public string code { get; set; }
            public string message { get; set; }
            public T data { get; set; }
        }

        public class PayLinkResponseData
        {
            public string merchantOrderId { get; set; }
            public string payLinkId { get; set; }
            public string paymentUrl { get; set; }
            public string shortUrl { get; set; }
            public string payLink { get; set; } // Map payLink just in case
            public string token { get; set; }
            public string status { get; set; }
            public long expiryTime { get; set; }

            // Direct response fields mapping
            public string orderId { get; set; }
            public string state { get; set; }
            public string paylinkUrl { get; set; }
            public string expireAt { get; set; }

            public string GetUrl()
            {
                if (!string.IsNullOrEmpty(shortUrl)) return shortUrl;
                if (!string.IsNullOrEmpty(paymentUrl)) return paymentUrl;
                if (!string.IsNullOrEmpty(payLink)) return payLink;
                if (!string.IsNullOrEmpty(paylinkUrl)) return paylinkUrl;
                return "";
            }
        }

        public class StatusResponseData
        {
            public string merchantId { get; set; }
            public string transactionId { get; set; }
            public string phonePeTransactionId { get; set; }
            public long amount { get; set; }
            public string status { get; set; }
            public string paymentMode { get; set; }
            public string referenceNumber { get; set; }

            // Direct response fields mapping
            public string state { get; set; }
        }
    }
}
