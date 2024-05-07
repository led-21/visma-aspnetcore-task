using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using visma_aspnetcore_task.Interfaces;
using visma_aspnetcore_task.Services;

public static class EmployeesEndpoints
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

    public static async Task<IResult> GetAllEmployee(IEmployeeServices employeeServices)
    {
        return await employeeServices.GetAllEmployeeAsync();
    }

    public static async Task<IResult> GetEmployeeById(int id, IEmployeeServices employeeServices)
    {
        return await employeeServices.GetEmployeeByIdAsync(id);
    }

    public static async Task<IResult> GetEmployeeByNameAndBithdateInterval(string name, DateTime startDate, DateTime endDate, IEmployeeServices employeeServices)
    {
        return await employeeServices.GetEmployeeByNameAndBithdateIntervalAsync(name, startDate, endDate);
    }

    public static async Task<IResult> GetAllEmployeeByBossId(int id, IEmployeeServices employeeServices)
    {
        return await employeeServices.GetAllEmployeeByBossIdAsync(id);
    }

    public static async Task<IResult> GetEmployeeCountAverageByRole(string role, IEmployeeServices employeeServices)
    {
        return await employeeServices.GetEmployeeCountAverageByRoleAsync(role);
    }

    public static async Task<IResult> AddEmployee(EmployeeDTO employeeDto, IEmployeeServices employeeServices)
    {
        return await employeeServices.AddEmployeeAsync(employeeDto);
    }

    public static async Task<IResult> UpdateEmployee(int id, EmployeeDTO employee, IEmployeeServices employeeServices)
    {
        return await employeeServices.UpdateEmployeeAsync(id, employee);
    }

    public static async Task<IResult> UpdateEmployeeSalary(int id, decimal salary, IEmployeeServices employeeServices)
    {
        return await employeeServices.UpdateEmployeeSalaryAsync(id, salary);
    }

    public static async Task<IResult> DeleteEmployee(int id, IEmployeeServices employeeServices)
    {
        return await employeeServices.DeleteEmployeeAsync(id);
    }
}

