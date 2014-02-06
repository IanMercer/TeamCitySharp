﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Security.Authentication;
using System.Threading.Tasks;
using TeamCitySharp.DomainEntities;
using File = System.IO.File;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace TeamCitySharp.Connection
{
    internal class TeamCityCaller : ITeamCityCaller
    {
        private readonly Credentials _configuration = new Credentials();

        public TeamCityCaller(string hostName, bool useSsl)
        {
            if (string.IsNullOrEmpty(hostName))
                throw new ArgumentNullException("hostName");

            _configuration.UseSSL = useSsl;
            _configuration.HostName = hostName;
        }

        public void Connect(string userName, string password, bool actAsGuest)
        {
            _configuration.Password = password;
            _configuration.UserName = userName;
            _configuration.ActAsGuest = actAsGuest;
            if (!actAsGuest && (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password)))
                throw new ArgumentException("If you are not acting as a guest you must supply userName and password");
        }

        // GET

        public Task<T> GetFormat<T>(string urlPart, params object[] parts)
        {
            return Get<T>(string.Format(urlPart, parts));
        }

        private JsonMediaTypeFormatter[] Formatters
        {
            get
            {
                return new[]
                {
                    new JsonMediaTypeFormatter { SerializerSettings = 
                        new JsonSerializerSettings
                        {
                            Converters = new List<JsonConverter>
                            {
                                new TeamCityDateTimeConverter()
//                                new IsoDateTimeConverter()
                            }
                        }
                    }
                };
            }
        }

        public async Task<T> Get<T>(string urlPart)
        {
            var response = await GetResponse(urlPart);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<T>(this.Formatters);
            return result;
        }

        private async Task<HttpResponseMessage> GetResponse(string urlPart)
        {
            var url = CreateUrl(urlPart);
            var client = CreateHttpClient("application/json");
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.Unauthorized)
                throw new AuthenticationException("Authentication failed for " + url);
            response.EnsureSuccessStatusCode();
            return response;
        }

        // POST

        public Task<U> PostFormat<T, U>(T data, string contenttype, string urlPart, params object[] parts)
        {
            return Post<T, U>(data, contenttype, string.Format(urlPart, parts));
        }

        public async Task<U> Post<T, U>(T data, string contentType, string urlPart)
        {
            var mediaTypeFormatter = (contentType == "text/xml") ? (MediaTypeFormatter)(new XmlMediaTypeFormatter()) : new JsonMediaTypeFormatter();
            var client = CreateHttpClient(null);
            var response = await client.PostAsync<T>(CreateUrl(urlPart), data, mediaTypeFormatter);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<U>(this.Formatters);
        }

        // PUT

        public Task<U> PutFormat<T, U>(T data, string contenttype, string urlPart, params object[] parts)
        {
            return Put<T, U>(data, contenttype, string.Format(urlPart, parts));
        }

        public async Task<U> Put<T, U>(T data, string contentType, string urlPart)
        {
            var mediaTypeFormatter = (contentType == "text/xml") ? (MediaTypeFormatter)(new XmlMediaTypeFormatter()) : new JsonMediaTypeFormatter();
            var client = CreateHttpClient(null);
            var response = await client.PutAsync<T>(CreateUrl(urlPart), data, mediaTypeFormatter);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<U>(this.Formatters);
        }

        // DELETE

        public Task<bool> DeleteFormat(string urlPart, params object[] parts)
        {
            return Delete(string.Format(urlPart, parts));
        }

        public async Task<bool> Delete(string urlPart)
        {
            var client = CreateHttpClient("text/plain");
            var response = await client.DeleteAsync(CreateUrl(urlPart));
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }

        // TEAMCITY SPECIFIC METHODS

        public async Task GetDownloadFormat(Action<string> downloadHandler, string urlPart, params object[] parts)
        {
            var url = CreateUrl(string.Format(urlPart, parts));

            if (downloadHandler == null)
            {
                throw new ArgumentException("A download handler must be specfied.");
            }

            string tempFileName = Path.GetRandomFileName();

            try
            {
                using (var client = CreateHttpClient("application/json"))
                {
                    var stream = await client.GetStreamAsync(url);

                    using (var fileStream = File.Create(tempFileName))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        await stream.CopyToAsync(fileStream);
                        fileStream.Close();
                    }
                }
                downloadHandler.Invoke(tempFileName);
            }
            finally
            {
                if (File.Exists(tempFileName))
                {
                    File.Delete(tempFileName);
                }
            }
        }

        public async Task<string> StartBackup(string urlPart)
        {
            var url = CreateUrl(urlPart);
            var httpClient = CreateHttpClient("text/plain");
            var response = await httpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }

        public async Task<bool> Authenticate(string urlPart)
        {
            try
            {
                var httpClient = CreateHttpClient("text/plain");
                var response = await httpClient.GetAsync(CreateUrl(urlPart));
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode;
            }
            catch (System.Net.Http.HttpRequestException exception)
            {
                throw new AuthenticationException(exception.Message, exception);
            }
        }


        private string CreateUrl(string urlPart)
        {
            if (string.IsNullOrEmpty(urlPart))
                throw new ArgumentException("Url must be specfied");
            var protocol = _configuration.UseSSL ? "https://" : "http://";
            var authType = _configuration.ActAsGuest ? "/guestAuth" : "/httpAuth";
            return string.Format("{0}{1}{2}{3}", protocol, _configuration.HostName, authType, urlPart);
        }

        private HttpClient CreateHttpClient(string accept)
        {
            HttpClient httpClient;
            if (!_configuration.ActAsGuest)
            {
                var credentials = new NetworkCredential(_configuration.UserName, _configuration.Password);
                var handler = new HttpClientHandler { Credentials = credentials };
                handler.PreAuthenticate = true;

                httpClient = new HttpClient(handler);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic");
            }
            else
            {
                httpClient = new HttpClient();
            }
            if (accept != null)
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
            }
            else
            {
                // HttpClient can easily deserialize XML or JSON
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
            }
            return httpClient;
        }

        // only used by the artifact listing methods since i havent found a way to deserialize them into a domain entity
        public async Task<string> GetRaw(string urlPart)
        {
            if (string.IsNullOrEmpty(urlPart))
                throw new ArgumentException("Url must be specfied");

            var url = CreateUrl(urlPart);

            var httpClient = CreateHttpClient("text/plain");
            var response = await httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private string GetContentType(string data)
        {
            if (data.StartsWith("<"))
                return "application/xml";
            return "text/plain";
        }
    }
}