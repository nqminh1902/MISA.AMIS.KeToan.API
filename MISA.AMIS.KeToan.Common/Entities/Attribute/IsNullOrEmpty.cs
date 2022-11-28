using MISA.AMIS.KeToan.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.KeToan.Common
{
    /// <summary>
    /// Class trả về ErrorMessage dữ liệu bắt buộc nhập
    /// </summary>
    public class IsNullOrEmpty : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
                return new ValidationResult(this.ErrorMessageResourceName);      
        }
    }

}
