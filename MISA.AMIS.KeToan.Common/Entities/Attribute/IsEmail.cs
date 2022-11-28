using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MISA.AMIS.KeToan.Common
{
    /// <summary>
    /// Class trả về thông báo lỗi Email
    /// </summary>
    public class IsEmail : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            return new ValidationResult(this.ErrorMessage);
        }
    }
}
