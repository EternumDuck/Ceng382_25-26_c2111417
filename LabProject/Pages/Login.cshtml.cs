using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using LabProject.Models;

namespace LabProject.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public LoginModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public void OnGet()
        {
            // Clear errors
            ErrorMessage = null;
        }

        public IActionResult OnPostAsync()
        {
            // Read users from JSON file
            var usersFilePath = Path.Combine(_environment.WebRootPath, "data", "users.json");
            if (!System.IO.File.Exists(usersFilePath))
            {
                ErrorMessage = "User database not found.";
                return Page();
            }

            var jsonString = System.IO.File.ReadAllText(usersFilePath);
            var users = JsonSerializer.Deserialize<List<User>>(jsonString);

            // Validate user credentials user is active
            var user = users?.FirstOrDefault(u => 
                u.Username == Username && 
                u.Password == Password && 
                u.IsActive);

            if (user == null)
            {
                ErrorMessage = "Username or password is incorrect.";
                return Page();
            }
            // Genereate a session token not escure 
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            //4 Store user information in session
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

            // cookies
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("username", user.Username, cookieOptions);
            Response.Cookies.Append("token", token, cookieOptions);
            Response.Cookies.Append("session_id", HttpContext.Session.Id, cookieOptions);

            return RedirectToPage("/Index");
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();

            Response.Cookies.Delete("username");
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("session_id");

            return RedirectToPage("/Login");
        }
    }
}