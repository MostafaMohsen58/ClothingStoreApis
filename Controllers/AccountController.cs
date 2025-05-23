using ClothingAPIs.DTO;
using ClothingAPIs.Helpers;
using ClothingAPIs.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ClothingAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase

    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSettings _emailsettings;


		[HttpGet("IsAuthenticated")]
		public IActionResult IsAuthenticated()
		{
			return Ok(new { User.Identity.IsAuthenticated, UserName=User.FindFirstValue(ClaimTypes.Name) });
		}
		public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,ApplicationDbContext context,IEmailSettings emailSettings )
        {
            _userManager = userManager;  
            _signInManager = signInManager;
            _context = context;
            _emailsettings = emailSettings;
		}
		[HttpGet("all-users")]
        [Authorize]
        public async Task<ActionResult<List<RegisterDTO2>>> GetAllUsers()
		{
			var users = _userManager.Users.ToList();

			var userDtos = users.Select(user => new RegisterDTO2
			{
				id = user.Id,
				Email = user.Email,
				DateOfBirth = user.DateOfBirth,
				FirstName = user.FirstName,
				LastName = user.LastName,
				PhoneNumber = user.PhoneNumber,
				UserName = user.UserName
			}).ToList();

			return userDtos;
		}
		[HttpPost("FakeLogin")]
        
		public async Task<IActionResult> FakeLogin()
		{
            var user = await _userManager.FindByIdAsync("7ba1220f-700a-49c3-9636-a693bb511559");
			await _signInManager.SignInAsync(user, isPersistent: true);
            return Ok(new {message= $"Logged in successfully." });
		}
        [HttpPost("register")]

        public async Task<IActionResult> Register(RegisterDTO registerUser)
        {
            if (ModelState.IsValid)
            {
                
                var existingUser = await _userManager.FindByEmailAsync(registerUser.Email);
                if (existingUser is not null)
                {
                    if (await _userManager.IsEmailConfirmedAsync(existingUser))
                    {
                        return BadRequest("This email is already registered.");
                    }
                    else
                    {
                        return BadRequest("This email is already registered but pending confirmation. Please check your inbox.");
                    }
                }

                if (string.IsNullOrWhiteSpace(registerUser.Password))
                {
                    return BadRequest("You have to enter a password.");
                }
                var order = new Order()
                {
					method = PaymentMethod.CreditCard,
					OrderDate = DateTime.Now,
                    
				};
                _context.Orders.Add(order);
				_context.SaveChanges();
				AppUser u = new AppUser
                {
                    FirstName = registerUser.FirstName,
                    LastName = registerUser.LastName,
                    DateOfBirth = registerUser.DateOfBirth,
                    UserName = registerUser.UserName,
                    PhoneNumber = registerUser.PhoneNumber,
                    Email = registerUser.Email,
                    CartId = order.Id,

				};
                IdentityResult result = await _userManager.CreateAsync(u, registerUser.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors.FirstOrDefault());
             
                }
               
                // Generate email confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(u);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = u.Id, token = token }, Request.Scheme);

				// Send confirmation email
				var email = new Email2
                {
                    To = u.Email,
                    Subject = "Confirm Email",
                    Body = $"Please confirm your email by clicking on this link: {confirmationLink}"
                };
				_emailsettings.SendEmail(email);


            }
            return Ok(new {message= "Account Created Successfully. Please check your email to confirm your account." });
        }
		[HttpGet("ConfirmEmail")]
		public async Task<IActionResult> ConfirmEmail(string userId, string token)
		{
			if (userId == null || token == null)
			{
				return BadRequest("Invalid email confirmation request.");
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			var result = await _userManager.ConfirmEmailAsync(user, token);
			if (result.Succeeded)
			{
				return Ok(new { message = "EmailConfirmed" });


			}
			else
			{
				return BadRequest(new { message = "Email confirmation failed." });
			}
		}

		[HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO LoginUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid login request.");
            }

            var user = await _userManager.FindByEmailAsync(LoginUser.UsernameOrEmail) ??
                       await _userManager.FindByNameAsync(LoginUser.UsernameOrEmail);

            if (user == null || !await _userManager.CheckPasswordAsync(user, LoginUser.Password))
            {
                return Unauthorized("Invalid email or password.");
            }
			if (!await _userManager.IsEmailConfirmedAsync(user))
			{
				return BadRequest("You must confirm your email before logging in");
			}

			await _signInManager.SignInAsync(user, isPersistent: LoginUser.RememberMe);

			return Ok(new { message = "Login successful." });
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully." });
        }


        [HttpGet("External-login")]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
           
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.Items["LoginProvider"] = provider;
            return Challenge(properties, provider);
        }

        [HttpGet("external-login-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {

            if (remoteError != null)
            {
                //return BadRequest(new { Error = $"External authentication error: {remoteError}" });
				return Redirect($"https://clothing-store-last.vercel.app/={WebUtility.UrlEncode($"External authentication error: {remoteError}")}");


			}

			var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
				//return BadRequest(new { Error = "Error loading external login information." });
				return Redirect($"https://clothing-store-last.vercel.app/={WebUtility.UrlEncode("Error loading external login information.")}");

			}

			var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
            {
                //return BadRequest(new { Error = "Email not provided by the external provider." });
                //return Redirect($"https://clothing-store-last.vercel.app/=Email not provided by the external provider.");
                return Redirect($"https://clothing-store-last.vercel.app/={WebUtility.UrlEncode("Email not provided by the external provider.")}");

			}
            while ((await _userManager.FindByNameAsync(Regex.Replace(name, "[^a-zA-Z0-9]", "")) is not null))
            {
                name = name + "x";
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = Regex.Replace(name, "[^a-zA-Z0-9]", ""),
                    Email = email,
                    FirstName = name,
                    LastName = name,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    //return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
					return Redirect($"https://clothing-store-last.vercel.app/={WebUtility.UrlEncode(string.Join(", ", result.Errors.Select(e => e.Description)))}");

				}

				await _userManager.AddLoginAsync(user, info);
            }
            else
            {
                await _userManager.AddLoginAsync(user, info);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return Redirect("https://clothing-store-last.vercel.app/");
            return Ok(new { Message = "External login successful", Email = email, Name = name });
        }


      
        

    }
}
