using InsureYouAI.Dtos;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        public RegisterController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserRegisterDto createUserRegisterDto)
        {
            AppUser appUser = new AppUser()
            {
                Name = createUserRegisterDto.Name,
                Email = createUserRegisterDto.Email,
                Surname = createUserRegisterDto.Surname,
                UserName = createUserRegisterDto.Username,
                ImageUrl = "Test",
                Description = "Açıklama"
            };
            await _userManager.CreateAsync(appUser, createUserRegisterDto.Password);
            return RedirectToAction("UserList");
        }
    }
}
