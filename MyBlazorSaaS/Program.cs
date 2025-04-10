using MyBlazorSaaS.Client.Pages;
using MyBlazorSaaS.Components;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCascadingAuthenticationState();

builder.Services
    .AddAuth0WebAppAuthentication(options => {
      options.Domain = builder.Configuration["Auth0:Domain"];
      options.ClientId = builder.Configuration["Auth0:ClientId"];
      options.Scope = "openid profile email";
    });

builder.Services
    .AddAuth0WebAppAuthentication("OnboardingScheme", options => {
      options.Domain = builder.Configuration["Auth0:Domain"];
      options.ClientId = builder.Configuration["Auth0:ManagementClientId"];
      options.ClientSecret = builder.Configuration["Auth0:ManagementClientSecret"];
      options.Scope = "openid profile email";
      options.CookieAuthenticationScheme = "OnboardingCookieScheme";
      options.CallbackPath = "/onboarding/callback";
    });

builder.Services
    .AddAuthorization(options => {
      options.AddPolicy("VerifiedEmail", policy =>
        policy.RequireAssertion(context =>
          context.User.HasClaim(claim =>
            ( claim.Type == "email_verified" &&
              claim.Value?.ToLowerInvariant() == "true"
            )
          )
        )
      );
    });

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IAuth0Management>(sp => 
  new Auth0Management(
    builder.Configuration["Auth0:Domain"],
    builder.Configuration["Auth0:ManagementClientId"],
    builder.Configuration["Auth0:ManagementClientSecret"]
  )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/account/login", async (HttpContext httpContext, string returnUrl = "/", string organizationId = "") =>
{
  var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
          .WithRedirectUri(returnUrl)
          .WithOrganization(organizationId)
          .Build();

  await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});

app.MapGet("/account/logout", async (HttpContext httpContext, string returnUrl = "/") =>
{
  var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
          .WithRedirectUri(returnUrl)
          .Build();

  await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
  await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

  await httpContext.SignOutAsync("OnboardingScheme", authenticationProperties);
  await httpContext.SignOutAsync("OnboardingCookieScheme");
});

app.MapGet("/account/signup", async (HttpContext httpContext, string login_hint) =>
{
  var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
          .WithRedirectUri("/onboarding/verify")
          .WithParameter("screen_hint", "signup")
          .WithParameter("login_hint", login_hint)
          .Build();

  await httpContext.ChallengeAsync("OnboardingScheme", authenticationProperties);
});

app.MapGet("/api/internalData", () =>
{
    var data = Enumerable.Range(1, 5).Select(index =>
        Random.Shared.Next(1, 100))
        .ToArray();

    return data;
})
.RequireAuthorization();

app.MapGet("/account/invitation", async (HttpContext httpContext, string organization, string invitation) =>
{
  var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
          .WithOrganization(organization)
          .WithInvitation(invitation)
          .Build();

  await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MyBlazorSaaS.Client._Imports).Assembly);

app.Run();
