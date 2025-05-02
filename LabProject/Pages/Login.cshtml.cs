using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject.Models;
using LabProject.Data;

namespace LabProject.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SchoolDbContext _context;

        [BindProperty]
        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        public string? ErrorMessage { get; set; }

        public LoginModel(SchoolDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            ErrorMessage = null;
        }

        public IActionResult OnPostAsync()
        {
            var user = _context.Users.FirstOrDefault(u => 
                u.Username == Username && 
                u.Password == Password && 
                u.IsActive);

            if (user == null)
            {
                ErrorMessage = "Username or password is incorrect.";
                return Page();
            }

            //session token 
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var sessionId = Guid.NewGuid().ToString();
            
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", sessionId);

            Response.Cookies.Append("username", user.Username);
            Response.Cookies.Append("token", token);
            Response.Cookies.Append("session_id", sessionId);

            return RedirectToPage("/Index");
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return RedirectToPage("/Index");
        }
    }
}