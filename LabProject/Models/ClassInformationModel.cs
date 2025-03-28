using System.ComponentModel.DataAnnotations;

public class ClassInformationModel
{
    private static int _idCounter = 0;

    public int Id { get; set; }

    [Required(ErrorMessage = "Class name is required")]
    [StringLength(100, ErrorMessage = "Class name cannot exceed 100 characters")]
    public string ClassName { get; set; }

    [Required(ErrorMessage = "Student count is required")]
    [Range(1, 100, ErrorMessage = "Student count must be between 1 and 100")]
    public int StudentCount { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; }

    public ClassInformationModel()
    {
        Id = 0;  // Don't set Id in constructor
        ClassName = string.Empty;
        StudentCount = 0;
        Description = string.Empty;
    }

    public static int GenerateNewId()
    {
        return ++_idCounter;
    }
}