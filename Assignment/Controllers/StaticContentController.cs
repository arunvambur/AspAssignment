using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Controllers
{
    public class StaticContentController : Controller
    {
        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}