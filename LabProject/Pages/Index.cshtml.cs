using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject.Utilities;
using LabProject.Data;
using LabProject.Models;

namespace LabProject.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly SchoolDbContext _context;
    private const int PageSize = 10;

    [BindProperty]
    public ClassInformationModel ClassInformationModel { get; set; } = new ClassInformationModel();

    [BindProperty]
    public int? EditingId { get; set; }

    [BindProperty]
    public string[]? SelectedColumns { get; set; }

    public ClassInformationTable TableModel { get; set; } = new();

    public List<ClassInformationModel> Classes { get; set; } = new List<ClassInformationModel>();
    
    public string[] AvailableColumns => new[] { "Id", "ClassName", "StudentCount", "Description" };

    public IndexModel(ILogger<IndexModel> logger, SchoolDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    private bool ValidateAuthentication()
    {
        var sessionUsername = HttpContext.Session.GetString("username");
        var sessionToken = HttpContext.Session.GetString("token");
        var sessionId = HttpContext.Session.GetString("session_id");

        if (string.IsNullOrEmpty(sessionUsername) || string.IsNullOrEmpty(sessionToken) || string.IsNullOrEmpty(sessionId))
        {
            return false;
        }

        var cookieUsername = Request.Cookies["username"];
        var cookieToken = Request.Cookies["token"];
        var cookieSessionId = Request.Cookies["session_id"];

        return sessionUsername == cookieUsername && 
               sessionToken == cookieToken && 
               sessionId == cookieSessionId;
    }
    
    public IActionResult OnGet(int? pageNumber, string? searchTerm, int? minStudents, int? maxStudents)
    {
        if (!ValidateAuthentication())
        {
            return RedirectToPage("/Login");
        }

        var query = _context.Classes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => c.Name.Contains(searchTerm) || c.Description.Contains(searchTerm));
        }

        if (minStudents.HasValue)
        {
            query = query.Where(c => c.PersonCount >= minStudents.Value);
        }

        if (maxStudents.HasValue)
        {
            query = query.Where(c => c.PersonCount <= maxStudents.Value);
        }

        // Calculate pagination
        var totalItems = query.Count();
        var currentPage = pageNumber ?? 1;
        var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

        // Apply pagination
        var items = query
            .Skip((currentPage - 1) * PageSize)
            .Take(PageSize)
            .Select(c => new ClassInformationModel 
            {
                Id = c.Id,
                ClassName = c.Name,
                StudentCount = c.PersonCount,
                Description = c.Description
            })
            .ToList();

        // Update table model
        TableModel = new ClassInformationTable
        {
            Items = items,
            CurrentPage = currentPage,
            TotalPages = totalPages,
            PageSize = PageSize,
            SearchTerm = searchTerm,
            MinStudents = minStudents,
            MaxStudents = maxStudents
        };

        return Page();
    }

    public IActionResult OnPostAdd()
    {
        if (!ModelState.IsValid)
        {
            OnGet(null, null, null, null);
            return Page();
        }

        var classEntity = new Class
        {
            Name = ClassInformationModel.ClassName,
            PersonCount = ClassInformationModel.StudentCount,
            Description = ClassInformationModel.Description,
            IsActive = true
        };

        if (EditingId.HasValue)
        {
            var existingClass = _context.Classes.Find(EditingId.Value);
            if (existingClass != null)
            {
                existingClass.Name = classEntity.Name;
                existingClass.PersonCount = classEntity.PersonCount;
                existingClass.Description = classEntity.Description;
                _context.Classes.Update(existingClass);
            }
        }
        else
        {
            _context.Classes.Add(classEntity);
        }

        _context.SaveChanges();
        return RedirectToPage("/Index");
    }

    public IActionResult OnGetDelete(int id, int? pageNumber, string? searchTerm, int? minStudents, int? maxStudents)
    {
        var classToRemove = _context.Classes.Find(id);
        if (classToRemove != null)
        {
            _context.Classes.Remove(classToRemove);
            _context.SaveChanges();
        }

        return RedirectToPage("/Index", new { pageNumber, searchTerm, minStudents, maxStudents });
    }

    public IActionResult OnGetEdit(int id)
    {
        var classToEdit = _context.Classes.Find(id);
        if (classToEdit != null)
        {
            ClassInformationModel = new ClassInformationModel
            {
                Id = classToEdit.Id,
                ClassName = classToEdit.Name,
                StudentCount = classToEdit.PersonCount,
                Description = classToEdit.Description
            };
            EditingId = id;
        }

        // Load classes for the current page
        OnGet(null, null, null, null);
        return Page();
    }

    public IActionResult OnPostExportJson(bool exportFiltered = false, string? searchTerm = null, int? minStudents = null, int? maxStudents = null, int? pageNumber = null)
    {
        var query = _context.Classes.AsQueryable();
        
        if (exportFiltered)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm) || c.Description.Contains(searchTerm));
            }

            if (minStudents.HasValue)
            {
                query = query.Where(c => c.PersonCount >= minStudents.Value);
            }

            if (maxStudents.HasValue)
            {
                query = query.Where(c => c.PersonCount <= maxStudents.Value);
            }

            if (pageNumber.HasValue)
            {
                query = query.Skip((pageNumber.Value - 1) * PageSize).Take(PageSize);
            }
        }

        var dataToExport = query
            .Select(c => new ClassInformationModel 
            {
                Id = c.Id,
                ClassName = c.Name,
                StudentCount = c.PersonCount,
                Description = c.Description
            })
            .ToList();

        var jsonData = JsonExportUtil.Instance.ExportToJson(dataToExport, SelectedColumns);
        var fileName = exportFiltered ? (pageNumber.HasValue ? $"page_{pageNumber}_classes.json" : "filtered_classes.json") : "all_classes.json";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
        
        return File(bytes, "application/json", fileName);
    }
}
