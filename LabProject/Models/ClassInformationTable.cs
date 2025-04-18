using System.Collections.Generic;

public class ClassInformationTable
{
    public List<ClassInformationModel> Items { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public int? MinStudents { get; set; }
    public int? MaxStudents { get; set; }
}