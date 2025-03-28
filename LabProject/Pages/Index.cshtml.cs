using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace LabProject.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    [BindProperty]
    public ClassInformationModel ClassInformationModel { get; set; } = new ClassInformationModel();

    [BindProperty]
    public int? EditingId { get; set; }

    public List<ClassInformationModel> Classes { get; set; } = new List<ClassInformationModel>();

    private static List<ClassInformationModel> _allClasses = new List<ClassInformationModel>();

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        Classes = _allClasses;
    }

    public IActionResult OnPostAdd()
    {
        if (!ModelState.IsValid)
        {
            Classes = _allClasses;
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
        Classes = _allClasses;

        // Reset form
        ClassInformationModel = new ClassInformationModel();
        EditingId = null;

        return Page();
    }

    public IActionResult OnGetDelete(int id)
    {
        var classToRemove = _allClasses.Find(c => c.Id == id);
        if (classToRemove != null)
        {
            _allClasses.Remove(classToRemove);
        }

        Classes = _allClasses;
        return Page();
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
