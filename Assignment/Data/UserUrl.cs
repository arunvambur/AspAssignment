using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Data
{
    public class UserUrl
    {
        public string UrlId { get; set; }
        public string UserId { get; set; }
        public string ShortUrl { get; set; }
        public string ActualUrl { get; set; }
        public string Description { get; set; }
    }
}
