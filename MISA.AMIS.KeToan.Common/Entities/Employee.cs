
using Fingers10.ExcelExport.Attributes;
using MISA.AMIS.KeToan.Common.Entities.Attribute;
using MISA.AMIS.KeToan.Common.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace MISA.AMIS.KeToan.Common.Entities
{
    /// <summary>
    /// Nhân viên
    /// </summary>
    public class Employee : BaseEntities
    {
        /// <summary>
        /// ID nhân viên
        /// </summary>
        [KeyAttribute]
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Mã nhân viên
        /// </summary>
        [IsNullOrEmpty(ErrorMessage = "Mã nhân viên không được để trống")]
        [IsDuplicate(ErrorMessage = "Mã nhân viên bị trùng")]
        [IsValidCode(ErrorMessage = "Mã nhân viên nhỏ hơn 20 ký tự")]
        public string EmployeeCode { get; set; }

        /// <summary>
        /// Tên nhân viên
        /// </summary>      
        [IsNullOrEmpty(ErrorMessage = "Tên không được để trống")]
        [IsNotNumber(ErrorMessage = "Tên không hợp lệ, phải là chữ")]
        public string EmployeeName { get; set; }

        /// <summary>
        /// Giới tính
        /// </summary>
        public Gender? Gender { get; set; } //enum

        /// <summary>
        /// Ngày sinh
        /// </summary>
        [IsValidDateOfBirth(ErrorMessage = "Ngày sinh không vượt quá ngày hiện tại")]
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// ID đơn vị nhân viên
        /// </summary>
        [IsNullOrEmpty(ErrorMessage = "Đơn vị không được để trống")]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Tên đơn vị nhân viên
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Tên chức danh nhân viên
        /// </summary>
        public string JobPositionName { get; set; }

        /// <summary>
        /// Tên Vai trò
        /// </summary>
        public int EmployeeRole { get; set; }

        /// <summary>
        /// Số chứng minh nhân dân
        /// </summary>
        public string IdentityNumber { get; set; }

        /// <summary>
        /// Ngày cấp
        /// </summary>
        public DateTime? IdentityDate { get; set; }

        /// <summary>
        /// Nơi cấp
        /// </summary>
        public string IdentityPlace { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Điện thoại di động
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Điện thoại cố định
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [IsEmail(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        /// <summary>
        /// Tài khoản ngân hàng
        /// </summary>
        public string BankAccountNumber { get; set; }

        /// <summary>
        /// Tên ngân hàng
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// Tên chi nhánh
        /// </summary>
        public string BankBranchName { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public string? CreateBy { get; set; }

        /// <summary>
        /// Ngày sửa
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Người sửa gần nhất
        /// </summary>
        public string? ModifiedBy { get; set; }
    }
}
