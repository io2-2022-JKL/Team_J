using IdentityServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Configuration
{
    public class UserInitializer
    {
        public static void Initialize(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            string[] roles = new string[] { Role.Admin, Role.Doctor, Role.Patient };
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            foreach(string role in roles)
            {

                if(!context.Roles.Any(r => r.Name == role))
                {
                    /*context.Roles.Add(new IdentityRole()
                    {
                        Name = role,
                        NormalizedName = role.ToUpper()
                    });
                    */
                    var newRole = new IdentityRole()
                    {
                        Name = role,
                        NormalizedName = role.ToUpper()
                    };
                    _ = roleManager.CreateAsync(newRole).Result;
                }
            }

            //context.SaveChanges();

            var patients = Patients();
            var patientsPasswords = PatientsPasswords();

            if (patients.Count() != patientsPasswords.Count())
                throw new ArgumentException();

            for(int i = 0; i < patients.Count(); i++)
            { 
                if(!context.Users.Any(p => p.UserName == patients[i].UserName))
                {
                    var password = new PasswordHasher<ApplicationUser>();
                    var hashed = password.HashPassword(patients[i], patientsPasswords[i]);
                    patients[i].PasswordHash = hashed;

                    var userStore = new UserStore<ApplicationUser>(context);
                    userStore.CreateAsync(patients[i]).Wait();
                }

                AssignRole(serviceScope.ServiceProvider, patients[i].Email, Role.Patient).Wait();
            }

            var doctors = Doctors();
            var doctorsPasswords = DoctorsPasswords();

            if (doctors.Count() != doctorsPasswords.Count())
                throw new ArgumentException();

            for (int i = 0; i < doctors.Count(); i++)
            {
                if (!context.Users.Any(d => d.UserName == doctors[i].UserName))
                {
                    var password = new PasswordHasher<ApplicationUser>();
                    var hashed = password.HashPassword(doctors[i], doctorsPasswords[i]);
                    doctors[i].PasswordHash = hashed;

                    var userStore = new UserStore<ApplicationUser>(context);
                    userStore.CreateAsync(doctors[i]).Wait();
                }

                AssignRole(serviceScope.ServiceProvider, doctors[i].Email, Role.Doctor).Wait();
            }

            var admins = Admins();
            var adminsPasswords = AdminsPasswords();

            if (admins.Count() != adminsPasswords.Count())
                throw new ArgumentException();

            for (int i = 0; i < admins.Count(); i++)
            {
                if (!context.Users.Any(a => a.UserName == admins[i].UserName))
                {
                    var password = new PasswordHasher<ApplicationUser>();
                    var hashed = password.HashPassword(admins[i], adminsPasswords[i]);
                    admins[i].PasswordHash = hashed;

                    var userStore = new UserStore<ApplicationUser>(context);
                    userStore.CreateAsync(admins[i]).Wait();
                }

                AssignRole(serviceScope.ServiceProvider, admins[i].Email, Role.Admin).Wait();
            }

            context.SaveChangesAsync();

        }

        public static async Task<IdentityResult> AssignRole(IServiceProvider services, string email, string role)
        {
            /*using var scope = services.CreateScope();
                UserManager<ApplicationUser> _userManager = (UserManager<ApplicationUser>)scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                ApplicationUser user = await _userManager.FindByNameAsync(email);
                var result = await _userManager.AddToRoleAsync(user, role);

                return result;
            */
            UserManager<ApplicationUser> _userManager = (UserManager<ApplicationUser>)services.GetService<UserManager<ApplicationUser>>();
            ApplicationUser user = await _userManager.FindByNameAsync(email);
            var result = await _userManager.AddToRoleAsync(user, role);

            return result;
        }

        private static List<ApplicationUser> Patients()
        {
            List<ApplicationUser> data = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "pParker@gmail.com",
                    NormalizedUserName = "PPARKER@GMAIL.COM",
                    Email = "pParker@gmail.com",
                    NormalizedEmail = "PPARKER@GMAIL.COM",
                    PhoneNumber = "489657215",
                    Id = Guid.Parse("F969FFD0-6DBC-4900-8EB8-B4FE25906A74").ToString(),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                },
                new ApplicationUser
                {
                    UserName = "james.bond@mi6.gov.uk",
                    NormalizedUserName = "JAMES.BOND@MI6.GOV.UK",
                    Email = "james.bond@mi6.gov.uk",
                    NormalizedEmail = "JAMES.BOND@MI6.GOV.UK",
                    PhoneNumber = "+44007",
                    Id = Guid.Parse("ACD9FA16-404E-4305-B57F-93659054BE7E").ToString(),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                },
                new ApplicationUser
                {
                    UserName = "01168691@pw.edu.pl",
                    NormalizedUserName = "01168691@PW.EDU.PL",
                    Email = "01168691@pw.edu.pl",
                    NormalizedEmail = "01168691@PW.EDU.PL",
                    PhoneNumber = "+48967123729",
                    Id = Guid.Parse("F982AAA8-4BE7-4115-A4F9-6CBAB37AE726").ToString(),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                },
                new ApplicationUser
                {
                    UserName = "korwinKrul@wp.pl",
                    NormalizedUserName = "KORWINKRUL@WP.PL",
                    Email = "korwinKrul@wp.pl",
                    NormalizedEmail = "KORWINKRUL@WP.PL",
                    PhoneNumber = "445445445",
                    Id = Guid.Parse("1448BE96-C2DE-4FDB-93C5-3CAF1DE2F8A0").ToString(),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                },
                new ApplicationUser
                {
                    UserName = "adi222@wp.pl",
                    NormalizedUserName = "ADI222@WP.PL",
                    Email = "adi222@wp.pl",
                    NormalizedEmail = "ADI222@WP.PL",
                    PhoneNumber = "+48982938179",
                    Id = Guid.Parse("815A0A02-D036-41C6-89C2-20C614214047").ToString(),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                }
            };
            return data;
        }

        private static List<string> PatientsPasswords()
        {
            var data = new List<string>
            {
                "Password",
                "AllH4ilTheQu33n",
                "3b6L\\\"[>k4?V?x?#",
                "5Procent",
                "haslohaslo"
            };
            return data;
        }

        private static List<ApplicationUser> Doctors()
        {
            var data = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "aaeeshaAAR@doktor.org.pl",
                    NormalizedUserName = "AAEESHAAAR@DOKTOR.ORG.PL",
                    Email = "aaeeshaAAR@doktor.org.pl",
                    NormalizedEmail = "AAEESHAAAR@DOKTOR.ORG.PL",
                    PhoneNumber = "863928017",
                    Id = Guid.Parse("89A11879-4EDF-4A67-A6F7-23C76763A418").ToString(),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                },
                new ApplicationUser
                {
                    UserName = "sylwesterS@doktor.org.pl",
                    NormalizedUserName = "SYLWESTERS@DOKTOR.ORG.PL",
                    Email = "sylwesterS@doktor.org.pl",
                    NormalizedEmail = "SYLWESTERS@DOKTOR.ORG.PL",
                    PhoneNumber = "+48964937619",
                    Id = Guid.Parse("9D77B5E9-2823-4274-B326-D371E5582274").ToString(),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                }
            };
            return data;
        }

        private static List<string> DoctorsPasswords()
        {
            var data = new List<string>
            {
                "?f$#Ybe72qnAu>7*",
                "-EV92QbHF$!8keH="
            };
            return data;
        }

        private static List<ApplicationUser> Admins()
        {
            var data = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "admin@systemszczepien.org.pl",
                    NormalizedUserName = "ADMIN@SYSTEMSZCZEPIEN.ORG.PL",
                    Email = "admin@systemszczepien.org.pl",
                    NormalizedEmail = "ADMIN@SYSTEMSZCZEPIEN.ORG.PL",
                    PhoneNumber = "+48987654321",
                    Id = Guid.Parse("F72A1DDA-B5FA-4FC9-BA56-1924F93D6634").ToString(),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                }
            };
            return data;
        }

        private static List<string> AdminsPasswords()
        {
            var data = new List<string>
            {
                "1234"
            };
            return data;
        }

    }
}
