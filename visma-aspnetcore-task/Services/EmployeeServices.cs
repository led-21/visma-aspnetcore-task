using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using visma_aspnetcore_task.Interfaces;

namespace visma_aspnetcore_task.Services;

public class EmployeeServices(EmployeeDatabase database) : IEmployeeServices
{
    public async Task<IResult> GetAllEmployeeAsync()
    {
        var employees = await database.Employees.ToListAsync();
        return Results.Ok(employees);
    }

    public async Task<IResult> GetEmployeeByIdAsync(int id)
    {
        Employee? employee = (await database.Employees.ToListAsync()).Find(x => x.Id == id);

        if (employee == null)
            return TypedResults.NotFound("Employee id not exist.");

        return Results.Ok(employee);
    }

    public async Task<IResult> GetEmployeeByNameAndBithdateIntervalAsync(string name, DateTime startDate,
        DateTime endDate)
    {
        var employees = (await database.Employees.ToListAsync()).Where(
            x => x.FirstName == name
                 && x.Birthdate >= startDate
                 && x.Birthdate <= endDate).ToList();

        return Results.Ok(employees);
    }

    public async Task<IResult> GetAllEmployeeByBossIdAsync(int id)
    {
        var employees = (await database.Employees.ToListAsync()).Where(
            x => x?.BossId == id).ToList();

        return Results.Ok(employees);
    }

    public async Task<IResult> GetEmployeeCountAverageByRoleAsync(string role)
    {
        var employees = (await database.Employees.ToListAsync()).Where(
            x => x.Role == role).ToList();

        if (employees.Count == 0)
            return TypedResults.NotFound("No employee for this role.");

        return Results.Ok(
            new
            {
                role = role,
                employeeCount = employees.Count,
                averangeSalary = employees.Average(x => x.CurrentSalary)
            });
    }

    public async Task<IResult> AddEmployeeAsync(EmployeeDTO? employeeDto)
    {
        if (employeeDto != null)
        {
            Employee employee = new()
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Birthdate = employeeDto.Birthdate,
                EmploymentDate = employeeDto.EmploymentDate,
                BossId = employeeDto.BossId,
                HomeAddress = employeeDto.HomeAddress,
                CurrentSalary = employeeDto.CurrentSalary,
                Role = employeeDto.Role
            };
            employee.Boss = await database.Employees.FindAsync(employeeDto.BossId);
            await database.Employees.AddAsync(employee);
            await database.SaveChangesAsync();

            return TypedResults.Created($"employees/{employee.Id}", employee);
        }

        return TypedResults.Problem("Add employee fail");
    }

    public async Task<IResult> UpdateEmployeeAsync(int id, EmployeeDTO employee)
    {
        Employee? employeeToUpdate = await database.Employees.FindAsync(id);

        if (employeeToUpdate != null)
        {
            employeeToUpdate.FirstName = employee.FirstName;
            employeeToUpdate.LastName = employee.LastName;
            employeeToUpdate.Birthdate = employee.Birthdate;
            employeeToUpdate.EmploymentDate = employee.EmploymentDate;
            employeeToUpdate.BossId = employee.BossId;
            employeeToUpdate.HomeAddress = employee.HomeAddress;
            employeeToUpdate.CurrentSalary = employee.CurrentSalary;
            employeeToUpdate.Role = employee.Role;

            await database.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound("Employee id not exist.");
    }

    public async Task<IResult> UpdateEmployeeSalaryAsync(int id, decimal salary)
    {
        if (salary < 0)
            return TypedResults.BadRequest("Current salary must be non-negative");

        Employee? employeeToUpdate = await database.Employees.FindAsync(id) as Employee;
        if (employeeToUpdate != null)
        {
            employeeToUpdate.CurrentSalary = salary;
            await database.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound("Employee id not exist.");
    }

    public async Task<IResult> DeleteEmployeeAsync(int id)
    {
        Employee? employee = await database.Employees.FindAsync(id) as Employee;
        if (employee != null)
        {
            database.Employees.Remove(employee);
            await database.SaveChangesAsync();
            return Results.NoContent();
        }

        return TypedResults.NotFound("Employee id not exist.");
    }
}