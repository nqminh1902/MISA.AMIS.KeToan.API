namespace MISA.AMIS.KeToan.Common.Entities
{
    /// <summary>
    /// Kết quả trả về của API danh sách nhân viên theo bộ lọc và phân trang
    /// </summary>
    public class PaginResult
    {
        
        /// <summary>
        /// Danh sách nhân viên
        /// </summary>
        public List<Employee>? Data { get; set; }

        /// <summary>
        ///  tổng số bản ghi
        /// </summary>
        public long TotalCount { get; set; }
    }
}
