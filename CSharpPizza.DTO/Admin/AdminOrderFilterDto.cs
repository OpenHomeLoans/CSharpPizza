namespace CSharpPizza.DTO.Admin;

public class AdminOrderFilterDto
{
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? CustomerName { get; set; }
}