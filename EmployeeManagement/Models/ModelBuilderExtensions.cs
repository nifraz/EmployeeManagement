using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasData(
                    new Employee()
                    {
                        Id = 1,
                        Name = "Nifraz Navahz",
                        Email = "nifraz@live.com",
                        Department = Dept.IT
                    },
                    new Employee()
                    {
                        Id = 2,
                        Name = "Kasun Karindra",
                        Email = "kasuna@gmail.com",
                        Department = Dept.HR
                    },
                    new Employee()
                    {
                        Id = 3,
                        Name = "Hasanthi Kumarasinghe",
                        Email = "hasa23@yahoo.com",
                        Department = Dept.None
                    }
                );
        }
    }
}
