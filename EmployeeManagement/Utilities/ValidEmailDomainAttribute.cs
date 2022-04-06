using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Utilities
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private readonly string domain;

        public ValidEmailDomainAttribute(string domain)
        {
            this.domain = domain;
        }

        public override bool IsValid(object value)
        {
            var parts = value.ToString().Split("@");
            if (parts.Length < 2)
            {
                return false;
            }
            return parts[1].ToLowerInvariant() == domain.ToLowerInvariant();
        }
    }
}
