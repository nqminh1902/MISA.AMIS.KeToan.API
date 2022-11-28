using MISA.AMIS.KeToan.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.KeToan.Common.Entities
{
    public class ErrorResult
    {
        /// <summary>
        /// Mã lỗi
        /// </summary>
        public AMISErrorCode ErrorCode { get; set; }

        /// <summary>
        /// Message trả về cho dev hiểu
        /// </summary>
        public string DevMsg { get; set; }

        /// <summary>
        /// Message trả về cho người dùng hiểu
        /// </summary>
        public string UserMsg { get; set; }

        /// <summary>
        /// Thông tin chi tiết về lỗi
        /// </summary>
        public object MoreInfo { get; set; }

        /// <summary>
        /// ID dùng để trace lỗi khi log lại
        /// </summary>
        public string TraceId { get; set; }
    }
}
