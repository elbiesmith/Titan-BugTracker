using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Titan_BugTracker.Controllers
{
    public class UserRolesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
