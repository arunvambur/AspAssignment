using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Models
{
    public class UserUrlModel
    {
        public string UrlId { get; set; }
        public string Email { get; set; }
        public string ShortUrl { get; set; }
        public string ActualUrl { get; set; }
        public string Description { get; set; }
    }
}
