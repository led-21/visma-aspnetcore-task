using Microsoft.EntityFrameworkCore;
using System.Data;

public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder route)
    {
        route = route.MapGroup("employees");

        //🔲 Getting a particular employee by ID
        route.MapGet("/{id:int}", GetEmployeeById);

        //🔲 Search for employees by name and birthdate interval
        route.MapGet("/{name}/{startDate:datetime}/{endDate:datetime}", GetEmployeeByNameAndBithdateInterval);

        //🔲 Getting all employees
        route.MapGet("/", GetAllEmployee);

        //🔲 Getting all employees by boss id
        route.MapGet("/boss/{id:int}", GetAllEmployeeByBossId);

        //🔲 Getting employee count and average salary for particular Role
        route.MapGet("/{role}", GetEmployeeCountAverageByRole);

        //🔲 Adding new employee
        route.MapPost("/", AddEmployee)
            .AddEndpointFilter<EmployeeCreateEndPointFilter>();

        //🔲 Updating employee
        route.MapPut("/", UpdateEmployee)
            .AddEndpointFilter<EmployeeUpdateEndpointFilter>();

        //🔲 Updating just employee salary
        route.MapPut("/{id:int}/{salary:decimal}", UpdateEmployeeSalary);

        //🔲 Deleting employee
        route.MapDelete("/", DeleteEmployee);
    }

    public static async Task<IResult> GetAllEmployee(EmployeeDatabase database)
    {
        var employees = await database.Employees.ToListAsync();
        return Results.Ok(employees);
    }

    public static async Task<IResult> GetEmployeeById(int id, EmployeeDatabase database)
    {
        Employee? employee = (await database.Employees.ToListAsync()).Find(x => x.Id == id);

        if (employee == null)
            return TypedResults.NotFound("Employee id not exist.");

        return Results.Ok(employee);
    }

    public static async Task<IResult> GetEmployeeByNameAndBithdateInterval(string name, DateTime startDate, DateTime endDate, EmployeeDatabase database)
    {
        var employees = (await database.Employees.ToListAsync()).Where(
        x => x.FirstName == name
        && x.Birthdate >= startDate
        && x.Birthdate <= endDate).ToList();

        return Results.Ok(employees);
    }

    public static async Task<IResult> GetAllEmployeeByBossId(int id, EmployeeDatabase database)
    {
        var employees = (await database.Employees.ToListAsync()).Where(
        x => x?.BossId == id).ToList();

        return Results.Ok(employees);
    }

    public static async Task<IResult> GetEmployeeCountAverageByRole(string role, EmployeeDatabase database)
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

    public static async Task<IResult> AddEmployee(EmployeeDTO employeeDTO, EmployeeDatabase database)
    {
        if (employeeDTO != null)
        {
            Employee employee = new()
            {
                FirstName = employeeDTO.FirstName,
                LastName = employeeDTO.LastName,
                Birthdate = employeeDTO.Birthdate,
                EmploymentDate = employeeDTO.EmploymentDate,
                BossId = employeeDTO.BossId,
                HomeAddress = employeeDTO.HomeAddress,
                CurrentSalary = employeeDTO.CurrentSalary,
                Role = employeeDTO.Role
            };
            employee.Boss = await database.Employees.FindAsync(employeeDTO.BossId);
            await database.Employees.AddAsync(employee);
            await database.SaveChangesAsync();

            return TypedResults.Created($"employees/{employee.Id}", employee);
        }
        return TypedResults.Problem("Add employee fail");
    }

    public static async Task<IResult> UpdateEmployee(int id, EmployeeDTO employee, EmployeeDatabase database)
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

    public static async Task<IResult> UpdateEmployeeSalary(int id, decimal salary, EmployeeDatabase database)
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

    public static async Task<IResult> DeleteEmployee(int id, EmployeeDatabase database)
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

