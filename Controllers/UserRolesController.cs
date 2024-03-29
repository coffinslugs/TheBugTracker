﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheBugTracker.Data;
using TheBugTracker.Extensions;
using TheBugTracker.Models;
using TheBugTracker.Models.ViewModels;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyInfoService _companyInfoService;

        public UserRolesController(IBTRolesService rolesService, IBTCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            // Add instance of the ViewModel as a List (model)
            List<ManageUserRolesViewModel> model = new();

            // Get CompanyId
            int companyId = User.Identity.GetCompanyId().Value;

            // Get all Company users
            List<BTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);

            // Loop over the users to populate the ViewModel
            foreach(BTUser user in users)
            {
                // - instantiate ViewModel
                ManageUserRolesViewModel viewModel = new();
                viewModel.BTUser = user;

                // - use _rolesService
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);

                // - create multi-select list
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);

                model.Add(viewModel);
            }

            // Return model to the view
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            // Get Company Id
            int companyId = User.Identity.GetCompanyId().Value;

            // Instantiate BTUser
            BTUser bTUser = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.BTUser.Id);

            // Get Roles for User
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(bTUser);

            // Get Selected Role
            string userRole = member.SelectedRoles.FirstOrDefault();

            if (!string.IsNullOrEmpty(userRole))
            {
                // Remove User from their Roles
                if (await _rolesService.RemoveUserFromRolesAsync(bTUser, roles))
                {
                    // Add User to the new Roles
                    await _rolesService.AddUserToRoleAsync(bTUser, userRole);
                }
            }
            // Navigate back to the View
            return RedirectToAction(nameof(ManageUserRoles));
        }

    }
}
