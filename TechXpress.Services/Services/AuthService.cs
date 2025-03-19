using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TechXpress.DataAccess.Interfaces;
using TechXpress.Models.entitis;
using TechXpress.Services.Interfaces;

namespace TechXpress.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _jwtTokenService;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager,ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = tokenService;
        }

        public async Task<string> LoginUserAsync(User user)
        {
         
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser == null)
            {
                throw new Exception("User not found");
            }

            
            var signInResult = await _signInManager.PasswordSignInAsync(
                existingUser.UserName,
                user.password, 
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    throw new Exception("User is locked out");
                }
                else if (signInResult.IsNotAllowed)
                {
                    throw new Exception("Login not allowed for this user");
                }
                else
                {
                    throw new Exception("Incorrect password");
                }
            }

          
            var token = _jwtTokenService.GenerateToken(existingUser); 
            return token;
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
           
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
            {
                throw new Exception("Email already in use");
            }

           
            var result = await _userManager.CreateAsync(user, user.password);
            if (!result.Succeeded)
            {
              
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errorMessage);
            }

            return true;
        }
    }
}