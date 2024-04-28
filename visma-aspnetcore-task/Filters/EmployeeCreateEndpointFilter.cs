

using System.Reflection;

class EmployeeCreateEndPointFilter : IEndpointFilter
{
    protected readonly ILogger Logger;

    public EmployeeCreateEndPointFilter(ILoggerFactory logger)
    {
        Logger = logger.CreateLogger<EmployeeCreateEndPointFilter>();
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
          EndpointFilterDelegate next)
    {

        EmployeeDTO? employee = context.GetArgument<EmployeeDTO>(0);
        EmployeeDatabase? database = context.GetArgument<EmployeeDatabase>(1);
        List<string> errorList = new List<string>();

        if (employee.GetType().GetProperties().Any(x => x.PropertyType.Name == "String" ? string.IsNullOrEmpty(x.GetValue(employee) as string) : x.GetValue(employee) == null && x.Name != "BossId"))
        {
            return TypedResults.BadRequest("All properties, except for Boss are required");
        }

        if (employee.Role.ToUpper() == "CEO")
        {
            if (database.Employees.Any(x => x.Role.ToUpper() == "CEO")) 
                errorList.Add("There can be only 1 employee with CEO role");

            if(employee.BossId != null)
                 errorList.Add("CEO role has no boss");
        }

        if (employee.BossId == null)
        {
            if (employee.Role.ToUpper() == "CEO")
            {
                
            }
            else
            {
                errorList.Add("Only the CEO role has no boss");
            }
        }
        else
        {
            if(database.Employees.Find(employee.BossId)==null)
            {
                errorList.Add("Boss Id not exist");
            }
        }

        if(employee.FirstName.Length>50 || employee.LastName.Length >50)
        {
            errorList.Add("FirstName and LastName cannot be longer than 50 characters");
        }

        if (employee.FirstName == employee.LastName)
        {
            errorList.Add("FirstName != LastName");
        }

        if (employee.Birthdate <= DateTime.Now.AddYears(-70) ||
            employee.Birthdate >= DateTime.Now.AddYears(-18))
        {
            errorList.Add("Employee must be at least 18 years old and not older than 70 years");
        }

        if (employee.EmploymentDate <= new DateTime(2000, 1, 1))
        {
            errorList.Add("EmploymentDate cannot be earlier than 2000-01-01");
        }

        if (employee.EmploymentDate > DateTime.Now)
        {
            errorList.Add("EmploymentDate cannot be a future date");
        }

        if(employee.CurrentSalary<0)
        {
            errorList.Add("Current salary must be non-negative");
        }

        if(errorList.Count > 0)
        {
            Logger.LogError("Failed to create employee: \n"+string.Join('\n',errorList));
            return Results.BadRequest(errorList);
        }

        return next(context);
    }
}