using IdentityServer.Configuration;
using IdentityServer.DTO;
using IdentityServer.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPersistedGrantService _persisteGrantsService;

        /*public UserController(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            _context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _serviceProvider = serviceScope.ServiceProvider;
        }*/
        public UserController(ApplicationDbContext context, IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, IPersistedGrantService persistedGrantService)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _userManager = userManager;
            _persisteGrantsService = persistedGrantService;
        }

        [HttpPost("register")]
        public IActionResult RegisterUser(RegisterInIdentityServerDTO registerDTO)
        {
            var result = AddUser(registerDTO);
            return result;
        }

        private IActionResult AddUser(RegisterInIdentityServerDTO registerDTO)
        {
            if(registerDTO == null || registerDTO.email == null || !new EmailAddressAttribute().IsValid(registerDTO.email) || registerDTO.userId == null || !Guid.TryParse(registerDTO.userId, out _ )|| registerDTO.phoneNumber == null || !IsPhoneNumber(new string(registerDTO.phoneNumber.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray())) || registerDTO.role == null || registerDTO.password == null || registerDTO.password.Length == 0)
            {
                return BadRequest();
            }
            if(!Role.IsRole(registerDTO.role))
            {
                return BadRequest();
            }

            if (_context.Users.Any(u => u.UserName == registerDTO.email || u.Id == registerDTO.userId))
            {
                return BadRequest();
            }

            if (registerDTO.role == Role.Patient)
            {

                ApplicationUser user = new ApplicationUser()
                {
                    UserName = registerDTO.email,
                    NormalizedUserName = registerDTO.email.ToUpper(),
                    Email = registerDTO.email,
                    NormalizedEmail = registerDTO.email.ToUpper(),
                    PhoneNumber = new string(registerDTO.phoneNumber.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray()),
                    Id = registerDTO.userId,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, registerDTO.password);
                user.PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(_context);
                userStore.CreateAsync(user).Wait();

                AssignRole(_serviceProvider, user.Email, registerDTO.role).Wait();

                return Ok();
            }

            return BadRequest();

        }

        public static async Task<IdentityResult> AssignRole(IServiceProvider services, string email, string role)
        {
            UserManager<ApplicationUser> _userManager = (UserManager<ApplicationUser>)services.GetService<UserManager<ApplicationUser>>();
            ApplicationUser user = await _userManager.FindByNameAsync(email);
            var result = await _userManager.AddToRoleAsync(user, role);

            return result;
        }

        private bool IsPhoneNumber(string s)
        {

            if (s.Length > 15)
                return false;
            if (s[0] == '+')
            {
                if (!ulong.TryParse(s.Substring(1), out _))
                    return false;
            }
            else
            {
                if (!ulong.TryParse(s, out _))
                    return false;
            }
            return true;
        }


        [HttpDelete("deletePatient/{userId}")]
        public async Task<IActionResult> DeletePatient(string userId)
        {
            if (userId == null)
            {
                return BadRequest();
            }
            if (!Guid.TryParse(userId, out _))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            if(await _userManager.IsInRoleAsync(user, Role.Patient) || await _userManager.IsInRoleAsync(user, Role.Doctor))
            {
                return await FindAndDeleteUser(user);
            }

            return NotFound();
        }

        private async Task<IActionResult> FindAndDeleteUser(ApplicationUser user)
        {
            var rolesForUser = await _userManager.GetRolesAsync(user);

            using (var transaction = _context.Database.BeginTransaction())
            {
                await _persisteGrantsService.RemoveAllGrantsAsync(user.Id);

                if(rolesForUser.Count() > 0)
                {
                    foreach(var role in rolesForUser.ToList())
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                }

                await _userManager.DeleteAsync(user);
                transaction.Commit();
            }

            return Ok();
        }

        [HttpGet("deleteDoctor/{doctorId}/{patientId}")]
        public async Task<IActionResult> DeleteDoctor(string doctorId, string patientId)
        {
            if (doctorId == null || patientId == null)
            {
                return BadRequest();
            }
            if (!Guid.TryParse(doctorId, out _) || !Guid.TryParse(patientId, out _))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(doctorId);

            if (user == null)
            {
                return NotFound();
            }

            if (await _userManager.IsInRoleAsync(user, Role.Doctor))
            {
                return await DeleteDoctor(user, patientId);
            }

            return NotFound();
        }

        private async Task<IActionResult> DeleteDoctor(ApplicationUser user, string patientId)
        {
            var rolesForUser = await _userManager.GetRolesAsync(user);

            using (var transaction = _context.Database.BeginTransaction())
            {
                await _persisteGrantsService.RemoveAllGrantsAsync(user.Id);

                if (rolesForUser.Count() > 0)
                {
                    foreach (var role in rolesForUser.ToList())
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                }

                await _userManager.DeleteAsync(user);
                user.Id = patientId;

                var userStore = new UserStore<ApplicationUser>(_context);
                userStore.CreateAsync(user).Wait();

                AssignRole(_serviceProvider, user.Email, Role.Patient).Wait();


                transaction.Commit();
            }

            return Ok();
        }

        [HttpGet("addDoctor/{patientId}/{doctorId}")]
        public async Task<IActionResult> AddDoctor(string patientId, string doctorId)
        {
            if (patientId == null || doctorId == null)
            {
                return BadRequest();
            }
            if (!Guid.TryParse(patientId, out _) || !Guid.TryParse(doctorId, out _))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(patientId);

            if (user == null)
            {
                return NotFound();
            }

            if (await _userManager.IsInRoleAsync(user, Role.Patient))
            {
                return await AddDoctor(user, doctorId);
            }

            return NotFound();
        }

        private async Task<IActionResult> AddDoctor(ApplicationUser user, string doctorId)
        {
            var rolesForUser = await _userManager.GetRolesAsync(user);

            using (var transaction = _context.Database.BeginTransaction())
            {
                await _persisteGrantsService.RemoveAllGrantsAsync(user.Id);

                if (rolesForUser.Count() > 0)
                {
                    foreach (var role in rolesForUser.ToList())
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                }

                await _userManager.DeleteAsync(user);
                user.Id = doctorId;

                var userStore = new UserStore<ApplicationUser>(_context);
                userStore.CreateAsync(user).Wait();

                AssignRole(_serviceProvider, user.Email, Role.Doctor).Wait();


                transaction.Commit();
            }

            return Ok();
        }

        [HttpPost("edit")]
        public IActionResult EditUser(RegisterInIdentityServerDTO editDTO)
        {
            var result = FindAndEditUser(editDTO).Result;
            return result;
        }

        private async Task<IActionResult> FindAndEditUser(RegisterInIdentityServerDTO editDTO)
        {
            if (editDTO == null || editDTO.email == null || !new EmailAddressAttribute().IsValid(editDTO.email) || editDTO.userId == null || !Guid.TryParse(editDTO.userId, out _) || editDTO.phoneNumber == null || !IsPhoneNumber(new string(editDTO.phoneNumber.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray())) || editDTO.role == null || editDTO.password == null || editDTO.password.Length == 0)
            {
                return BadRequest();
            }
            if (!Role.IsRole(editDTO.role))
            {
                return BadRequest();
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == editDTO.userId);

            if(user == null)
            {
                return NotFound();
            }
            if (_context.Users.Any(u => u.Id != editDTO.userId && u.UserName == editDTO.email))
            {
                return BadRequest();
            }

            var rolesForUser = await _userManager.GetRolesAsync(user);

            using (var transaction = _context.Database.BeginTransaction())
            {
                await _persisteGrantsService.RemoveAllGrantsAsync(user.Id);

                if (rolesForUser.Count() > 0)
                {
                    foreach (var role in rolesForUser.ToList())
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                }

                await _userManager.DeleteAsync(user);

                user.UserName = editDTO.email;
                user.NormalizedUserName = editDTO.email.ToUpper();
                user.Email = editDTO.email;
                user.NormalizedEmail = editDTO.email.ToUpper();
                user.PhoneNumber = editDTO.phoneNumber;

                var userStore = new UserStore<ApplicationUser>(_context);
                userStore.CreateAsync(user).Wait();

                AssignRole(_serviceProvider, user.Email, editDTO.role).Wait();

                transaction.Commit();
            }

            return Ok();

        }
    }
}
