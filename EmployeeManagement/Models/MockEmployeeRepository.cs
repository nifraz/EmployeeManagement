using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private IList<Employee> _employees;

        public MockEmployeeRepository()
        {
            _employees = new List<Employee>()
            {
                new Employee() { Id = 1, Name = "Nifraz", Department = Dept.IT, Email = "nifraz@live.com"},
                new Employee() { Id = 2, Name = "Kasun", Department = Dept.HR, Email = "karindra@gmail.com"},
                new Employee() { Id = 3, Name = "Dimuthu", Department = Dept.Other, Email = "dimuwa@yahoo.com"},
            };
        }

        public Employee Add(Employee employee)
        {
            employee.Id = _employees.Max(e => e.Id) + 1;
            _employees.Add(employee);
            return employee;
        }

        public Employee Delete(int id)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);
            if (employee != default)
            {
                _employees.Remove(employee);
            }

            return employee;
        }

        public Employee Get(int id)
        {
            return _employees.FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<Employee> GetAll()
        {
            return _employees;
        }

        public Employee Update(Employee updatedEmployee)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == updatedEmployee.Id);
            if (employee != default)
            {
                employee.Name = updatedEmployee.Name;
                employee.Email = updatedEmployee.Email;
                employee.Department = updatedEmployee.Department;
            }
            return employee;
        }
    }
}
