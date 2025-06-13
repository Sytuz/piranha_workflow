/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Identity;
using Piranha.AspNetCore.Identity.Data;
using Piranha.Services;

namespace Piranha.AspNetCore.Identity;

/// <summary>
/// Default identity security seed.
/// </summary>
public class DefaultIdentitySeed : IIdentitySeed
{
    /// <summary>
    /// The private DbContext.
    /// </summary>
    private readonly IDb _db;

    /// <summary>
    /// The private user manager.
    /// </summary>
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// The Workflow service.
    /// </summary>
    private readonly IWorkflowService _workflowService;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The current DbContext</param>
    /// <param name="userManager">The current UserManager</param>
    /// <param name="workflowService">The workflow service</param>
    public DefaultIdentitySeed(IDb db, UserManager<User> userManager, IWorkflowService workflowService)
    {
        _db = db;
        _userManager = userManager;
        _workflowService = workflowService;
    }

    /// <summary>
    /// Create the seed data.
    /// </summary>
    public async Task CreateAsync()
    {
        if (!_db.Users.Any())
        {
            // Create admin user
            var admin = new User
            {
                UserName = "admin",
                Email = "admin@piranhacms.org",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var adminResult = await _userManager.CreateAsync(admin, "password");
            if (adminResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(admin, "SysAdmin");
            }

            // Create Author user
            var author = new User
            {
                UserName = "author",
                Email = "author@piranhacms.org",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var authorResult = await _userManager.CreateAsync(author, "password");
            if (authorResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(author, "Author");
            }

            // Create Reviewer user
            var reviewer = new User
            {
                UserName = "reviewer",
                Email = "reviewer@piranhacms.org",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var reviewerResult = await _userManager.CreateAsync(reviewer, "password");
            if (reviewerResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(reviewer, "Reviewer");
            }

            // Create Legal user
            var legal = new User
            {
                UserName = "legal",
                Email = "legal@piranhacms.org",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var legalResult = await _userManager.CreateAsync(legal, "password");
            if (legalResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(legal, "Legal");
            }

            // Create Publisher user
            var publisher = new User
            {
                UserName = "publisher",
                Email = "publisher@piranhacms.org",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var publisherResult = await _userManager.CreateAsync(publisher, "password");
            if (publisherResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(publisher, "Publisher");
            }

            // Update the default workflow roles
            _workflowService.InitializeDefaultWorkflowRolesAsync().GetAwaiter().GetResult();
        }
    }
}
