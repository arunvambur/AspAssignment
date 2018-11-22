using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Assignment.Data;
using Assignment.Models;
using Newtonsoft.Json;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Start()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Start")]
        public async Task<IActionResult> Start(UserUrlModel model)
        {
            using (var client = new HttpClient())
            {
                // Assuming the API is in the same web application. 
                string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";


                //Check if the user is already available in db
                client.BaseAddress = new Uri(baseUrl);
                var responseMessage = client.GetAsync("/api/Users");

                responseMessage.Wait();

                User user = null;
                var result = responseMessage.Result;
                if (result.IsSuccessStatusCode)
                {
                    var stream = await result.Content.ReadAsStreamAsync();
                    using (JsonReader jsonReader = new JsonTextReader(new System.IO.StreamReader(stream)))
                    {
                        var serializer = new JsonSerializer();
                        var users = serializer.Deserialize<IEnumerable<User>>(jsonReader);

                        user = users.Where(t => t.Email == model.Email).FirstOrDefault();
                    }
                }

                if (user == null)
                {
                    user = new User()
                    {
                        UserId = Guid.NewGuid().ToString(),
                        Email = model.Email
                    };

                    var postTask = client.PostAsJsonAsync<User>("/api/Users", user);

                    postTask.Wait();
                    
                    if (!postTask.Result.IsSuccessStatusCode)
                        return View("Error");
                    /*else
                    {
                        var stream = await result.Content.ReadAsStreamAsync();
                        using (JsonReader jsonReader = new JsonTextReader(new System.IO.StreamReader(stream)))
                        {
                            var serializer = new JsonSerializer();
                            user = serializer.Deserialize<User>(jsonReader);
                        }
                    }*/
                }

                UserUrl userUrl = new UserUrl
                {
                    UrlId = Guid.NewGuid().ToString(),
                    UserId = user.UserId,
                    ActualUrl = model.ActualUrl,
                    Description = model.Description
                };
                userUrl.ShortUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/Home/DynamicUrl?id=" + userUrl.UrlId;


                var usersUrlTask = client.PostAsJsonAsync<UserUrl>("/api/UserUrls", userUrl);

                usersUrlTask.Wait();

                if (usersUrlTask.Result.IsSuccessStatusCode)
                {
                    return View("Added", 
                        new UserUrlModel { UrlId = userUrl.UrlId, Email = user.Email, ShortUrl = userUrl.ShortUrl, ActualUrl = userUrl.ActualUrl, Description = userUrl.Description });
                }
            }

            return View();
        }

        public IActionResult SearchUrl()
        {
            return View();
        }


        [HttpPost]
        [ActionName("SearchUrl")]
        public async Task<IActionResult> SearchUrl(SearchModel userModel)
        {
            using (var client = new HttpClient())
            {
                // Assuming the API is in the same web application. 
                string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";


                client.BaseAddress = new Uri(baseUrl);
                var responseMessage = client.GetAsync("/api/Users");

                responseMessage.Wait();

                var result = responseMessage.Result;
                if (result.IsSuccessStatusCode)
                {
                    var stream = await result.Content.ReadAsStreamAsync();
                    using (JsonReader jsonReader = new JsonTextReader(new System.IO.StreamReader(stream)))
                    {
                        var serializer = new JsonSerializer();
                        var users = serializer.Deserialize<IEnumerable<User>>(jsonReader);

                        var user = users.Where(t => t.Email == userModel.Key).FirstOrDefault();

                        if (user == null)
                        {
                            return View();
                        }

                        var urlResponse = client.GetAsync("/api/UserUrls");
                        urlResponse.Wait();
                        if(urlResponse.Result.IsSuccessStatusCode)
                        {
                            var urlStream = await urlResponse.Result.Content.ReadAsStreamAsync();
                            using (JsonReader reader = new JsonTextReader(new System.IO.StreamReader(urlStream)))
                            {
                                var urlSerializer = new JsonSerializer();
                                var userUrls = urlSerializer.Deserialize<IEnumerable<UserUrl>>(reader);

                                var filteredUrl = userUrls.Where(t => t.UserId == user.UserId);

                                List<UserUrlModel> userUrlModels = new List<UserUrlModel>();
                                foreach(var fu in filteredUrl)
                                {
                                    userUrlModels.Add(
                                        new UserUrlModel
                                        {
                                            UrlId = fu.UrlId,
                                            Email = user.Email,
                                            ActualUrl = fu.ActualUrl,
                                            ShortUrl = fu.ShortUrl,
                                            Description = fu.Description
                                        });
                                }

                                return View("ListUrl", userUrlModels);
                            }
                        }
                    }
                }
                
            }

            return View();
        }

        public async Task<IActionResult> DynamicUrl(string id)
        {
            using (var client = new HttpClient())
            {
                string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
                client.BaseAddress = new Uri(baseUrl);

                var urlResponse = client.GetAsync("/api/UserUrls");
                urlResponse.Wait();
                if (urlResponse.Result.IsSuccessStatusCode)
                {
                    var urlStream = await urlResponse.Result.Content.ReadAsStreamAsync();
                    using (JsonReader reader = new JsonTextReader(new System.IO.StreamReader(urlStream)))
                    {
                        var urlSerializer = new JsonSerializer();
                        var userUrls = urlSerializer.Deserialize<IEnumerable<UserUrl>>(reader);

                        var filteredUrl = userUrls.Where(t => t.UrlId == id).FirstOrDefault();

                        if (filteredUrl != null)
                            return Redirect(filteredUrl.ActualUrl);
                    }
                }
            }


            return View("Start");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
