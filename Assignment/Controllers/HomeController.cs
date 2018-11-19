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
        public IActionResult Start(User userModel)
        {

            using (var client = new HttpClient())
            {
                // Assuming the API is in the same web application. 
                string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

                User user = new User()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = userModel.Email,
                    Url = baseUrl + "/" + userModel.Url,
                    Description = userModel.Description
                };


                client.BaseAddress = new Uri(baseUrl);
                var postTask = client.PostAsJsonAsync<User>("/api/Users",
                                              user);

                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return View("Added", user);
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

                        var filteredList = users.Where(t => t.Email == userModel.Key).ToList();

                        return View("ListUrl", filteredList);
                    }
                }


            }

            return View();
        }

        public IActionResult DynamicUrl()
        {
            return View("PageNotFound");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
