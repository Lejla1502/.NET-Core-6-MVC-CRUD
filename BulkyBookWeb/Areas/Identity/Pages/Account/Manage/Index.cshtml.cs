// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyBookWeb.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            public string Id { get; set; }
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            public string StreetAddress { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string PostalCode { get; set; }
        }

        private async Task LoadAsync(IdentityUser? user, string? userId)
        {
            
            if (userId == null || userId=="")
            {
                var id = await _userManager.GetUserIdAsync(user);    
                var userName = await _userManager.GetUserNameAsync(user);
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

                Username = userName;

                var appUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == user.Id);

                Input = new InputModel
                {
                    Id=id,
                    PhoneNumber = phoneNumber,
                    StreetAddress = appUser.StreetAddress,
                    City = appUser.City,
                    State = appUser.State,
                    PostalCode = appUser.PostalCode
                };
            }
            else
            {
                var appUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);

                Username = appUser.UserName;

                Input = new InputModel
                {
                    Id=appUser.Id,
                    PhoneNumber = appUser.PhoneNumber,
                    StreetAddress = appUser.StreetAddress,
                    City = appUser.City,
                    State = appUser.State,
                    PostalCode = appUser.PostalCode
                };
            }
        }

        public async Task<IActionResult> OnGetAsync(string? userId)
        {
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (userId != "" || userId != null)
                await LoadAsync(user, userId);
            else
                await LoadAsync(user, null);
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user, Input.Id);
                return Page();
            }
            if (Input.Id == null || Input.Id == "")
            {
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (Input.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        StatusMessage = "Unexpected error when trying to set phone number.";
                        return RedirectToPage();
                    }
                }

                var appUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == user.Id);
                appUser.StreetAddress = Input.StreetAddress;
                appUser.City = Input.City;
                appUser.State = Input.State;
                appUser.PostalCode = Input.PostalCode;

                _unitOfWork.ApplicationUser.Update(appUser);
                _unitOfWork.Save();

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your profile has been updated";
                return RedirectToPage();
            }
            else
            {
                var appUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == Input.Id);
                appUser.StreetAddress = Input.StreetAddress;
                appUser.City = Input.City;
                appUser.State = Input.State;
                appUser.PostalCode = Input.PostalCode;
                appUser.PhoneNumber = Input.PhoneNumber;

                _unitOfWork.ApplicationUser.Update(appUser);
                _unitOfWork.Save();

                string url = "/Identity/Account/Manage?userId="+Input.Id;
                return LocalRedirect(url);
            }
           
        }
    }
}
