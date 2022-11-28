using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.KeToan.Common
{
    /// <summary>
    /// Class trả về ErrorMessage nếu trùng mã nhân viên
    /// </summary>
    public class IsDuplicate : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            return new ValidationResult(this.ErrorMessage);
        }
    }
}
