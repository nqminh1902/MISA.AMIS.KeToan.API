namespace MISA.AMIS.KeToan.Common.Entities
{
    public class Department : BaseEntities
    {
        /// <summary>
        /// ID đơn vị
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Mã đơn vị
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// Tên đơn vị
        /// </summary>
        public string DepartmentName { get; set; }

    }
}
