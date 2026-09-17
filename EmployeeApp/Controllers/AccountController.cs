using System.Text.Json;
using EmployeeApp.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = new IdentityUser
            {
                Email = vm.Email,
                UserName = vm.Email
            };

            var result = await _userManager.CreateAsync(user, vm.Password);

            if (result.Succeeded)
            {
                // Assign role only after user creation succeeds
                await _userManager.AddToRoleAsync(user, "User");

                // Redirect to Login so the user logs in and receives an API JWT token
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1. Locate user in local MVC DB
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // 2. Validate password & check lockout status WITHOUT issuing cookie
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                try
                {
                    var client = _httpClientFactory.CreateClient("EmployeeAPI");
                    var loginPayload = new { Email = model.Email, Password = model.Password };

                    // Fixed: Endpoints are relative to BaseUrl (e.g., BaseUrl = "http://domain/api/")
                    var apiResponse = await client.PostAsJsonAsync("Auth/login", loginPayload);

                    if (apiResponse.IsSuccessStatusCode)
                    {
                        var resultData = await apiResponse.Content.ReadFromJsonAsync<JsonElement>();
                        if (resultData.TryGetProperty("token", out var tokenElement))
                        {
                            var tokenString = tokenElement.GetString();

                            // 3. Attach JWT token inside AuthenticationProperties
                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = model.RememberMe
                            };
                            authProperties.StoreTokens(new[]
                            {
                                new AuthenticationToken { Name = "access_token", Value = tokenString! }
                            });

                            // 4. Issue MVC Auth Cookie containing encrypted JWT token
                            await _signInManager.SignInAsync(user, authProperties);

                            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                return Redirect(returnUrl);

                            return RedirectToAction("Index", "Home");
                        }
                    }

                    ModelState.AddModelError(string.Empty, "Unable to establish API session. Please try again.");
                    return View(model);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Backend API service is unavailable.");
                    return View(model);
                }
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account locked due to too many failed attempts. Try again later.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); // Automatically clears cookie and stored JWT token
            return RedirectToAction("Index", "Home");
        }
    }
}