using System.Security.Claims;
using Code_Judge.Domain.Entities;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;

namespace Code_Judge.Infrastructure.Identity;

public class ProfileService: IProfileService
{
    protected UserManager<ApplicationUser> _userManager;

    public ProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user!.Id),
            new (ClaimTypes.Email, user.Email!),
           new (ClaimTypes.Name, user.UserName!),
           new (ClaimTypes.GivenName, user.Id),
        };
        context.IssuedClaims.AddRange(claims);
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        
        context.IsActive = (user != null) ;
    }
}