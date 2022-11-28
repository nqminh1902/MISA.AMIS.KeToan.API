using MISA.AMIS.KeToan.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.KeToan.DL
{
    public interface IEmployeeDL : IBaseDL<Employee>
    {
        /// <summary>
        /// Danh sách bản ghi theo tìm kiếm và phân trang
        /// </summary>
        /// <param name="keyword">Từ cần tìm kiếm</param>
        /// <param name="pageSize">Số bản ghi trên 1 trang</param>
        /// <param name="pageNumber">Vị trí của bản ghi bắt đầu</param>
        /// <returns></returns>
        public PaginResult GetEmployeeByFilterAndPaging(string keyword, int pageSize, int pageNumber);    

        /// <summary>
        /// Lấy mã nhân viên mới nhất
        /// </summary>
        /// <returns>Mã nhân viên mới nhất</returns>
        public string GetNewCode();

        /// <summary>
        /// Xóa hàng loạt bản ghi theo ID
        /// </summary>
        /// <param name="listEmployeeID">Danh sách ID</param>
        /// <returns>Danh sách ID xóa thành công</returns>
        /// CreateBy: Nguyễn Quang Minh (15/11/2022)
        public ListEmployeeID DeleteMultipleEmployee(ListEmployeeID listEmployeeID);


    }
}
