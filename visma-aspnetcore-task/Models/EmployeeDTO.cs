using System.ComponentModel.DataAnnotations;

public class EmployeeDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthdate { get; set; }
    public DateTime EmploymentDate { get; set; }
    public int? BossId { get; set; }
    public string HomeAddress { get; set; }
    public decimal CurrentSalary { get; set; }
    public string Role { get; set; }
}