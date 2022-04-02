using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class SqlEmployeeRepository : IEmployeeRepository
    {
        private readonly EmDbContext dbContext;
        private readonly ILogger<SqlEmployeeRepository> logger;

        public SqlEmployeeRepository(EmDbContext dbContext, ILogger<SqlEmployeeRepository> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public Employee Add(Employee employee)
        {
            dbContext.Employees.Add(employee);
            dbContext.SaveChanges();
            return employee;
        }

        public Employee Delete(int id)
        {
            var employee = dbContext.Employees.Find(id);
            if (employee != default)
            {
                dbContext.Employees.Remove(employee);
                dbContext.SaveChanges();
            }
            return employee;
        }

        public Employee Get(int id)
        {
            logger.LogTrace("Trace Log");
            logger.LogDebug("Debug Log");
            logger.LogInformation("Information Log");
            logger.LogWarning("Warning Log");
            logger.LogError("Error Log");
            logger.LogCritical("Critical Log");

            return dbContext.Employees.Find(id);
        }

        public IEnumerable<Employee> GetAll()
        {
            return dbContext.Employees;
        }

        public Employee Update(Employee updatedEmployee)
        {
            var entry = dbContext.Employees.Attach(updatedEmployee);
            entry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            dbContext.SaveChanges();
            return entry.Entity;
        }
    }
}
