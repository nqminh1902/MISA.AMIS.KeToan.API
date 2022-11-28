using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.KeToan.Common
{
    public class KeyAttribute:ValidationAttribute
    {
        /// <summary>
        /// Class trả về ErrorMessage nhập sai ngày sinh
        /// </summary>

            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                return ValidationResult.Success;
            }
    }
}
