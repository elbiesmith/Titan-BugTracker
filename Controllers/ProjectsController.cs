using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Titan_BugTracker.Data;
using Titan_BugTracker.Extensions;
using Titan_BugTracker.Models;
using Titan_BugTracker.Models.Enums;
using Titan_BugTracker.Models.ViewModels;
using Titan_BugTracker.Services.Interfaces;

namespace Titan_BugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTProjectService _projectService;
        private readonly IBTRolesService _rolesService;
        private readonly IBTFileService _fileService;
        private readonly UserManager<BTUser> _userManager;

        public ProjectsController(ApplicationDbContext context, IBTCompanyInfoService companyInfoService,
                                                                IBTProjectService projectService, IBTRolesService rolesService,
                                                                IBTFileService fileService, Microsoft.AspNetCore.Identity.UserManager<BTUser> userManager)
        {
            _context = context;
            _companyInfoService = companyInfoService;
            _projectService = projectService;
            _rolesService = rolesService;
            _fileService = fileService;
            _userManager = userManager;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Project> allProjects = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            return View(allProjects);
        }

        public async Task<IActionResult> AllProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Project> allProjects = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            return View(allProjects);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Project> allProjects = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            return View(allProjects);
        }

        public async Task<IActionResult> MyProjects()
        {
            string userId = _userManager.GetUserId(User);

            List<Project> myProjects = await _projectService.GetUserProjectsAsync(userId);
            return View(myProjects);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> ArchiveProject(int id)
        {
            try
            {
                int companyId = User.Identity.GetCompanyId().Value;

                Project project = await _projectService.GetProjectByIdAsync(id, companyId);
                await _projectService.ArchiveProjectAsync(project);
            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToAction("AllProjects");
        }

        public async Task<IActionResult> ArchivedProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Project> allProjects = await _projectService.GetArchivedProjectsByCompany(companyId);
            return View(allProjects);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignMembers(int id)
        {
            ProjectMembersViewModel model = new();
            int companyId = User.Identity.GetCompanyId().Value;

            // All company projects
            List<Project> projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            Project project = projects.FirstOrDefault(p => p.Id == id);

            model.Project = project;
            List<BTUser> developers = await _rolesService.GetUsersInRoleAsync(Roles.Developer.ToString(), companyId);
            List<BTUser> submitters = await _rolesService.GetUsersInRoleAsync(Roles.Submitter.ToString(), companyId);

            List<BTUser> users = developers.Concat(submitters).ToList();

            List<string> members = project.Members.Select(m => m.Id).ToList();
            model.Users = new MultiSelectList(users, "Id", "FullName", members);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignMembers(ProjectMembersViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SelectedUsers != null)
                {
                    List<string> memberIds = (await _projectService.GetAllProjectMembersExceptPMAsync(model.Project.Id)).Select(u => u.Id).ToList();

                    foreach (string item in memberIds)
                    {
                        await _projectService.RemoveUserFromProjectAsync(item, model.Project.Id);
                    }

                    foreach (string item in model.SelectedUsers)
                    {
                        await _projectService.AddUserToProjectAsync(item, model.Project.Id);
                    }

                    //return RedirectToAction("Details", "Projects", new { id = model.Project.Id })
                }
            }

            return RedirectToAction("AssignMembers", new { id = model.Project.Id });
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProjectDetailsViewModel model = new();

            int companyId = User.Identity.GetCompanyId().Value;
            model.Project = await _projectService.GetProjectByIdAsync((int)id, companyId);

            if (model.Project == null)
            {
                return NotFound();
            }

            model.ProjectManager = await _projectService.GetProjectManagerAsync(id.Value);

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignPMIndex()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            PMIndexViewModel model = new();
            model.Projects = await _projectService.GetUnassignedProjectsAsync(companyId);
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId), "Id", "FullName");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPMIndex(PMIndexViewModel model, int projectId)
        {
            //int companyId = User.Identity.GetCompanyId().Value;
            if (ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {
                    if (model.PmId != null)
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, projectId);
                    }
                }
            }

            return RedirectToAction("ManageProjects", "Projects");
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            AddProjectWithPMViewModel model = new();

            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId), "Id", "FullName");

            model.PriorityList = new SelectList(_context.ProjectPriorities, "Id", "Name");

            return View(model);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
        {
            if (model != null)
            {
                try
                {
                    if (model.Project.FormFile != null)
                    {
                        model.Project.FileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.FormFile);
                        model.Project.FileName = model.Project.FormFile.FileName;
                        model.Project.FileContentType = model.Project.FormFile.ContentType;
                    }

                    model.Project.CompanyId = User.Identity.GetCompanyId().Value;

                    await _projectService.AddNewProjectAsync(model.Project);

                    //Add PM
                    if (!string.IsNullOrEmpty(model.PmId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return RedirectToAction("ManageProjects");
            }
            return RedirectToAction("Create");
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int companyId = User.Identity.GetCompanyId().Value;
            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Description", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartDate,EndDate,FormFile, FileData, FileName, FileContentType,CompanyId,ProjectPriorityId")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    byte[] newImageData = await _fileService.ConvertFileToByteArrayAsync(project.FormFile);

                    if (project.FileData != newImageData && newImageData != null)
                    {
                        project.FileData = newImageData;
                        project.FileName = project.FormFile.FileName;
                        project.FileContentType = project.FormFile.ContentType;
                    }

                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("AllProjects");
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Description", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Id", project.ProjectPriorityId);
            return View(project);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}