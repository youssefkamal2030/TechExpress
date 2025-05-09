using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechXpress.DataAccess.Interfaces;
using TechXpress.Models.Dto_s;
using TechXpress.Models.entitis;
using TechXpress.Services.Interfaces;

namespace TechXpress.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _jwtTokenService;
        private readonly IMapper _mapper;

        public AuthService(
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            ITokenService tokenService,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDTO>();
            
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map<UserDTO>(user);
                userDto.Roles = roles.ToList();
                userDtos.Add(userDto);
            }
            
            return userDtos;
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

        public async Task<bool> UpdateUserRolesAsync(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Get current roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            
            // Remove user from all current roles
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                var errorMessage = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed to remove current roles: {errorMessage}");
            }

            // Add user to new roles
            if (roles != null && roles.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, roles);
                if (!addResult.Succeeded)
                {
                    var errorMessage = string.Join(", ", addResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to add new roles: {errorMessage}");
                }
            }

            return true;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Check if this is the last admin
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                if (adminUsers.Count <= 1)
                {
                    throw new Exception("Cannot delete the last admin user");
                }
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to delete user: {errorMessage}");
            }

            return true;
        }

        // Profile management methods with AutoMapper
        public async Task<ProfileDTO> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Use AutoMapper to map User to ProfileDTO
            return _mapper.Map<ProfileDTO>(user);
        }

        public async Task<bool> UpdateUserProfileAsync(ProfileDTO profileDto)
        {
            var user = await _userManager.FindByIdAsync(profileDto.Id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Store original email to check if it changed
            var originalEmail = user.Email;

            // Use AutoMapper to map ProfileDTO to User
            _mapper.Map(profileDto, user);

            // Save changes
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to update profile: {errorMessage}");
            }

            // If email was changed, update normalized email and set email confirmation to false
            if (originalEmail != user.Email)
            {
                user.NormalizedEmail = _userManager.NormalizeEmail(user.Email);
                user.EmailConfirmed = false;
                await _userManager.UpdateAsync(user);
            }

            return true;
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to change password: {errorMessage}");
            }

            return true;
        }
    }
}