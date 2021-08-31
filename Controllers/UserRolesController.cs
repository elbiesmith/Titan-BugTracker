using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Titan_BugTracker.Extensions;
using Titan_BugTracker.Models;
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
            List<ManageUserRolesViewModel> model = new();
            List<BTUser> users = await _companyInfoService.GetAllMembersAsync(_companyId);

            foreach (BTUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.BTUser = user;
                var selectedRoles = await _roleService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _roleService.GetRolesAsync(), "Name", "Name", selectedRoles);

                model.Add(viewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            return View();
        }
    }
}