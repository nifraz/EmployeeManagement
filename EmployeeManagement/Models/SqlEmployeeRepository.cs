using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class SqlEmployeeRepository : IEmployeeRepository
    {
        private readonly EmDbContext dbContext;

        public SqlEmployeeRepository(EmDbContext dbContext)
        {
            this.dbContext = dbContext;
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
