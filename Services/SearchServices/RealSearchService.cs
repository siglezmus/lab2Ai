using Lab2.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Services.SearchServices
{
    class RealSearchService : ISearchService<string, string>
    {
        public async Task<List<SearchResult<string>>> ProcessSearchRequestAsync(SearchRequest<string> request)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.dataforseo.com/"),
                DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes("pturnavanguard@gmail.com:b7f16291b3e565e3"))) }
            };
            var postData = new List<object>
            {
                new
                {
                    location_code = 21118, // Kyiv city,Ukraine
                    language_name = "English",
                    keyword = request.SearchValue,
                }
            };
            var requests = new List<Task<HttpResponseMessage>>();
            var content = new StringContent(JsonConvert.SerializeObject(postData));
            var searchEngines = new string[] { "google", "bing", "yahoo", "yandex" };
            foreach (var se in searchEngines)
            {
                requests.Add(httpClient.PostAsync($"/v3/serp/{se}/organic/live/regular", content));
            }
            try
            {
                var responses = await Task.WhenAll(requests);
                var results = new List<SearchResult<string>>();
                foreach (var r in responses)
                {
                    try
                    {
                        var result = JObject.Parse(await r.Content.ReadAsStringAsync());
                        if (result.TryGetValue("tasks", out JToken tasks))
                        {
                            var task = tasks.ToObject<JObject[]>()?.FirstOrDefault();
                            if (task == null)
                            {
                                throw new Exception($"Error in search service occurred, while trying to get results");
                            }
                            var se = task["data"]["se"];
                            if (result.TryGetValue("status_code", out JToken statusCode) && result.TryGetValue("status_message", out JToken statusMessage))
                            {
                                if (statusCode.ToObject<int>() == 20000)
                                {
                                    if (task.TryGetValue("result", out JToken taskResults))
                                    {
                                        if ((taskResults as JArray)?.FirstOrDefault() is JObject taskResult &&
                                            taskResult.TryGetValue("items", out JToken items))
                                        {
                                            var resultItems = items.ToObject<JObject[]>();
                                            if (resultItems != null && resultItems.Length > 0)
                                            {
                                                var domains = resultItems.Select(i => i.GetValue("domain").ToObject<string>()).Where(domain => !string.IsNullOrEmpty(domain)).Distinct().Take(10);
                                                if (domains.Any())
                                                {
                                                    results.Add(new SearchResult<string>(domains.Select(domain => new FoundItem<string>(domain)), se.ToString()));
                                                }
                                                else
                                                {
                                                    throw new Exception($"Error in search service occurred." +
                                                                 $"SE: {se} didn't found any results");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception($"Error in search service occurred." +
                                                    $"SE: {se} didn't found any results");
                                        }
                                    }
                                }
                                else
                                    throw new Exception($"Error in search service occurred. Code: {statusCode} Message: {statusMessage} " +
                                        $"SE: {se}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred while trying to get response from search engine", ex.Message);
                        continue;
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
