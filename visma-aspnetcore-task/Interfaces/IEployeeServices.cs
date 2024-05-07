namespace visma_aspnetcore_task.Interfaces;

public interface IEmployeeServices
{
    public Task<IResult> GetAllEmployeeAsync();

    public Task<IResult> GetEmployeeByIdAsync(int id);

    public Task<IResult> GetEmployeeByNameAndBithdateIntervalAsync(string name, DateTime startDate, DateTime endDate);

    public Task<IResult> GetAllEmployeeByBossIdAsync(int id);

    public Task<IResult> GetEmployeeCountAverageByRoleAsync(string role);

    public Task<IResult> AddEmployeeAsync(EmployeeDTO? employeeDto);

    public Task<IResult> UpdateEmployeeAsync(int id, EmployeeDTO employee);

    public Task<IResult> UpdateEmployeeSalaryAsync(int id, decimal salary);

    public Task<IResult> DeleteEmployeeAsync(int id);
}