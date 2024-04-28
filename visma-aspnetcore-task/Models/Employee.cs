using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

public class Employee
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(50)]
    public string LastName { get; set; }

    [Required]
    public DateTime Birthdate { get; set; }

    [Required]
    public DateTime EmploymentDate { get; set; }

    [ForeignKey("Boss")]
    [AllowNull]
    public int? BossId { get; set; }
    public Employee? Boss { get; set; }

    [Required]
    public string HomeAddress { get; set; }

    [Required]
    public decimal CurrentSalary { get; set; }

    [Required]
    public string Role { get; set; }

}


