using Microsoft.EntityFrameworkCore;

public class EmployeeDatabase : DbContext
{
    public DbSet<Employee> Employees { get; set; }

    public EmployeeDatabase(DbContextOptions<EmployeeDatabase> options) : base(options)
    {

    }
}