using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UnitTests.Helpers;
using visma_aspnetcore_task.Services;
using Xunit.Abstractions;

namespace UnitTests
{
    public class EmployeesUnitTests
    {
        private readonly EmployeeServices _employeeServices;
        private readonly EmployeeDatabase _mockDatabase;
        private readonly ITestOutputHelper output;

        public EmployeesUnitTests(ITestOutputHelper output)
        {
            //Test Output
            this.output = output;
            
            //Memory Cache
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();
            
            //Arrange
            _mockDatabase = new MockDb().CreateDbContext();
            _employeeServices = new(_mockDatabase, memoryCache);

            Employee employeeCEO = new Employee
            {
                FirstName = "Carol", LastName = "Lee", Birthdate = new DateTime(1993, 2, 8), Boss = null, BossId = null,
                CurrentSalary = 5000, EmploymentDate = new DateTime(2021, 6, 18), HomeAddress = "789 Oak Rd, Village",
                Role = "CEO"
            };

            _mockDatabase.Employees.Add(employeeCEO);

            _mockDatabase.Employees.Add(new Employee
            {
                FirstName = "Bob", LastName = "Johnson", Birthdate = new DateTime(1985, 9, 20), Boss = employeeCEO,
                BossId = employeeCEO.Id, CurrentSalary = 3000, EmploymentDate = new DateTime(2023, 8, 5),
                HomeAddress = "456 Elm Ave, Town", Role = "Software Engineer"
            });

            _mockDatabase.SaveChanges();
        }

        [Fact]
        public async Task TestMockDatabase()
        {
            //Assert
            Assert.True(_mockDatabase.Employees.Count() == 2);
            Assert.IsType<List<Employee>>(_mockDatabase.Employees.ToList());
            Assert.Equal("Carol", (await _mockDatabase.Employees.FindAsync(1))?.FirstName);
        }

        [Fact]
        public async Task GetAllEmployeesFromMockDatabaseReturnResultsOk()
        {
            //Act
            var result = await EmployeesEndpoints.GetAllEmployee(_employeeServices);

            //Assert
            Assert.IsType<Ok<List<Employee>>>(result);

            //Output
            output.WriteLine("Employee List");
            foreach (var employee in ((Ok<List<Employee>>)result).Value!)
            {
                output.WriteLine("Id: {0}, Name:{1} {2}", employee.Id, employee.FirstName, employee.LastName);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetEmployeeByIdFromMockDatabaseReturnResultsOk(int id)
        {
            //Act
            var result = await EmployeesEndpoints.GetEmployeeById(id, _employeeServices);

            //Assert
            Assert.IsType<Ok<Employee>>(result);

            //Output
            var employee = ((Ok<Employee>)result).Value!;
            output.WriteLine("Id: {0}, Name:{1} {2}", employee.Id, employee.FirstName, employee.LastName);
        }

        [Fact]
        public async Task GetEmployeesByNameAndBithdateIntervalFromMockDatabaseReturnResultsOk()
        {
            //Act
            var result = await EmployeesEndpoints.GetEmployeeByNameAndBithdateInterval("Gabi",
                DateTime.Now.AddYears(-20), DateTime.Now, _employeeServices);

            //Assert
            Assert.IsType<Ok<List<Employee>>>(result);
        }

        [Fact]
        public async Task GetEmployeesByRoleFromMockDatabaseReturnNotFound()
        {
            //Act
            var result = await EmployeesEndpoints.GetEmployeeCountAverageByRole("MasterChef", _employeeServices);

            //Assert
            Assert.IsAssignableFrom<NotFound<string>>(result);
        }

        [Fact]
        public async Task GetEmployeesByBossIdFromMockDatabaseReturnResultsOk()
        {
            //Act
            var result = await EmployeesEndpoints.GetAllEmployeeByBossId(1, _employeeServices);

            //Assert
            Assert.IsType<Ok<List<Employee>>>(result);
        }


        [Fact]
        public async Task CreateEmployeeInMockDatabase()
        {
            //Arrange
            EmployeeDTO employeeDto = new EmployeeDTO
            {
                FirstName = "Alice", LastName = "Smith", Birthdate = DateTime.Now.AddYears(-22), BossId = 1,
                CurrentSalary = 5000, EmploymentDate = DateTime.Now.AddYears(-1), HomeAddress = "123 Main St, City",
                Role = "Software Engineer"
            };

            //Act
            var result = await EmployeesEndpoints.AddEmployee(employeeDto, _employeeServices);

            //Assert
            Assert.IsType<Created<Employee>>(result);

            //Output
            output.WriteLine("Employee List");
            foreach (var employee in _mockDatabase.Employees.ToList())
            {
                output.WriteLine("Id: {0}, Name: {1} {2}, Address: {3}", employee.Id, employee.FirstName,
                    employee.LastName, employee.HomeAddress);
            }
        }

        [Fact]
        public async Task UpdateEmployeeFromMockDatabase()
        {
            var employeeToUpdate = _mockDatabase.Employees.Find(1);
            EmployeeDTO employeeDTO = new EmployeeDTO()
            {
                FirstName = employeeToUpdate!.FirstName,
                LastName = employeeToUpdate.LastName,
                Birthdate = employeeToUpdate.Birthdate,
                BossId = employeeToUpdate.BossId,
                HomeAddress = "New home address",
                CurrentSalary = employeeToUpdate.CurrentSalary,
                Role = employeeToUpdate.Role
            };

            //Act
            var result = await EmployeesEndpoints.UpdateEmployee(employeeToUpdate.Id, employeeDTO, _employeeServices);

            //Assert
            Assert.IsType<NoContent>(result);

            //Output
            output.WriteLine("Employee List");
            foreach (var employee in _mockDatabase.Employees.ToList())
            {
                output.WriteLine("Id: {0}, Name: {1}, Address: {2}", employee.Id, employee.FirstName,
                    employee.HomeAddress);
            }
        }

        [Fact]
        public async Task DeleteEmployeeFromMockDatabase()
        {
            //Before delete
            output.WriteLine("Before Delete Id 2");
            foreach (var employee in _mockDatabase.Employees.ToList())
            {
                output.WriteLine("Id: {0}, Name: {1} {2}", employee.Id, employee.FirstName, employee.LastName);
            }

            //Act
            var result = await EmployeesEndpoints.DeleteEmployee(2, _employeeServices);

            //Assert
            Assert.IsType<NoContent>(result);

            //Output after delete
            output.WriteLine("After Delete Id 2");
            foreach (var employee in _mockDatabase.Employees.ToList())
            {
                output.WriteLine("Id: {0}, Name: {1} {2}", employee.Id, employee.FirstName, employee.LastName);
            }
        }
    }
}