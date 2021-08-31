using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Titan_BugTracker.Extensions;
using Titan_BugTracker.Models.ViewModels;
using Titan_BugTracker.Services.Interfaces;

namespace Titan_BugTracker.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTRolesService _roleService;
        private readonly int _companyId;

        public UserRolesController(IBTCompanyInfoService companyInfoService, IBTRolesService roleService, IHttpContextAccessor contextAccessor)
        {
            _companyInfoService = companyInfoService;
            _roleService = roleService;

            if (contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                _companyId = contextAccessor.HttpContext.User.Identity.GetCompanyId().Value;
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            return View();
        }
    }
}