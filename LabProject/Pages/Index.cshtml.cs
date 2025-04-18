using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace LabProject.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private const int PageSize = 10;

    [BindProperty]
    public ClassInformationModel ClassInformationModel { get; set; } = new ClassInformationModel();

    [BindProperty]
    public int? EditingId { get; set; }

    public ClassInformationTable TableModel { get; set; } = new();
    private static List<ClassInformationModel> _allClasses = GenerateSampleData();


    public List<ClassInformationModel> Classes { get; set; } = new List<ClassInformationModel>();

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }
    private static List<ClassInformationModel> GenerateSampleData()
    {
        var sampleData = new List<ClassInformationModel>();
        var random = new Random(42); // Fixed seed for consistent data

        for (int i = 1; i <= 100; i++)
        {
            var model = new ClassInformationModel
            {
                Id = i,
                ClassName = $"Class {(char)('A' + (i % 26))} - {i}",
                StudentCount = random.Next(1, 101),
                Description = $"Sample description for class {i}"
            };
            sampleData.Add(model);
        }
        return sampleData;
    }
    
    // Filtering and pagination logic
    public void OnGet(int? pageNumber, string? searchTerm, int? minStudents, int? maxStudents)
    {
        // Apply filters
        var query = _allClasses.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => c.ClassName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || c.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (minStudents.HasValue)
        {
            query = query.Where(c => c.StudentCount >= minStudents.Value);
        }

        if (maxStudents.HasValue)
        {
            query = query.Where(c => c.StudentCount <= maxStudents.Value);
        }

        // Calculate pagination
        var totalItems = query.Count();
        var currentPage = pageNumber ?? 1;
        var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

        // Apply pagination
        var items = query
            .Skip((currentPage - 1) * PageSize)
            .Take(PageSize)
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
    }

    public IActionResult OnPostAdd()
    {
        if (!ModelState.IsValid)
        {
            // Reload the table model for the current page
            OnGet(null, null, null, null);
            return Page();
        }

        if (EditingId.HasValue)
        {
            // Update existing class
            var existingClass = _allClasses.Find(c => c.Id == EditingId.Value);
            if (existingClass != null)
            {
                ClassInformationModel.Id = EditingId.Value;  // Keep the existing ID
                _allClasses.Remove(existingClass);
            }
        }
        else
        {
            // Only generate new ID for new classes
            ClassInformationModel.Id = ClassInformationModel.GenerateNewId();
        }

        _allClasses.Add(ClassInformationModel);
        
        // Reset form and redirect to first page
        return RedirectToPage("/Index");
    }

    public IActionResult OnGetDelete(int id, int? pageNumber, string? searchTerm, int? minStudents, int? maxStudents)
    {
        var classToRemove = _allClasses.Find(c => c.Id == id);
        if (classToRemove != null)
        {
            _allClasses.Remove(classToRemove);
        }

        // Redirect back to the same page with all query parameters
        return RedirectToPage("/Index", new { pageNumber, searchTerm, minStudents, maxStudents });
    }

    public IActionResult OnGetEdit(int id)
    {
        var classToEdit = _allClasses.Find(c => c.Id == id);
        if (classToEdit != null)
        {
            ClassInformationModel = new ClassInformationModel
            {
                Id = classToEdit.Id,
                ClassName = classToEdit.ClassName,
                StudentCount = classToEdit.StudentCount,
                Description = classToEdit.Description
            };
            EditingId = id;
        }

        Classes = _allClasses;
        return Page();
    }
}
