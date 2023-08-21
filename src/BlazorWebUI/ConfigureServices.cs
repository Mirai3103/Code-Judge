﻿
using BlazorWebUI.Areas.Identity;
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.BlazorWebUI.Services;
using Code_Judge.Domain.Entities;
using Code_Judge.Infrastructure.Persistence;
using Microsoft.AspNetCore.Components.Authorization;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddBlazorWebUiServices(this IServiceCollection services)
    {
        services.AddAuthenticationCore();
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddHttpContextAccessor();
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();
        services.AddRazorPages();
        services.AddServerSideBlazor();

        services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
        return services;
    }

}
