using MISA.AMIS.KeToan.Common;
using MISA.AMIS.KeToan.Common.Entities;
using MISA.AMIS.KeToan.DL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MISA.AMIS.KeToan.BL
{
    public class EmployeeBL : BaseBL<Employee>, IEmployeeBL
    {
        #region Field
        private IEmployeeDL _employeeDL;
        #endregion

        #region Constructor
        public EmployeeBL(IEmployeeDL employeeDL) : base(employeeDL)
        {
            _employeeDL = employeeDL;
        }
        #endregion

        #region Method
        /// <summary>
        /// Danh sách bản ghi theo tìm kiếm và phân trang
        /// </summary>
        /// <param name="keyword">Từ cần tìm kiếm</param>
        /// <param name="pageSize">Số bản ghi trên 1 trang</param>
        /// <param name="pageNumber">Vị trí của bản ghi bắt đầu</param>
        /// <returns></returns>
        public PaginResult GetEmployeeByFilterAndPaging(string? keyword, int pageSize, int pageNumber)
        {
            return _employeeDL.GetEmployeeByFilterAndPaging(keyword, pageSize, pageNumber);
        }

        /// <summary>
        /// Lấy mã nhân viên mới nhất
        /// </summary>
        /// <returns>Mã nhân viên mới nhất</returns>
        public string GetNewCode()
        {
            return _employeeDL.GetNewCode();
        }

        /// <summary>
        /// Xóa hàng loạt bản ghi theo ID
        /// </summary>
        /// <param name="listEmployeeID">Danh sách ID</param>
        /// <returns>Danh sách ID xóa thành công</returns>
        /// CreateBy: Nguyễn Quang Minh (15/11/2022)
        public ListEmployeeID DeleteMultipleEmployee(ListEmployeeID listEmployeeID)
        {
            return _employeeDL.DeleteMultipleEmployee(listEmployeeID);
        }

        #endregion
        protected override bool ValidateCustom(Employee entity)
        {
            return base.ValidateCustom(entity);
        }
    }
}

