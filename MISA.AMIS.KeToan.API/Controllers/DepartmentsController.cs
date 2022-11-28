using Dapper;
using Microsoft.AspNetCore.Mvc;
using MISA.AMIS.KeToan.BL;
using MISA.AMIS.KeToan.Common.Entities;
using MySqlConnector;

namespace MISA.AMIS.KeToan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : BaseController<Department>
    {

        #region Constructor

        public DepartmentsController(IBaseBL<Department> baseBL) : base(baseBL)
        {

        }

        #endregion

        /*/// <summary>
        /// Lấy tất cả bản ghi
        /// </summary>
        /// <returns>Trả về tất cả các bản ghi</returns>
        [HttpGet]
        public IActionResult GetAllDepartment()
        {
            try
            {
                // Khời tạo kết nối tới DB MySQL
                string connectionString = "Server=localhost;Port=3309;Database=misa.web09.nsdh.nqminh;Uid=root;Pwd=minh2001;";
                var mySqlConnection = new MySqlConnection(connectionString);

                // Chuẩn bị câu lệnh SQL
                string storeProcedureName = "Proc_department_GetAll";

                // Thực hiên gọi vào DB
                var department = mySqlConnection.Query<Department>(storeProcedureName, commandType: System.Data.CommandType.StoredProcedure);

                // Xử lý trả về

                // Thành công: Trả về dữ liệu cho FE
                if (department != null)
                {
                    return StatusCode(StatusCodes.Status200OK, department);
                }
                // Thất bại
                return StatusCode(StatusCodes.Status200OK, new List<Department>());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {

                    ErrorCode = 1,
                    DevMsg = "Catch an exception",
                    UserMsg = "Lấy danh sách nhân viên thất bại",
                    MoreInfo = "https://openapi.misa.com.vn/errorcode/e001",
                    TraceId = HttpContext.TraceIdentifier


                });

            }*/
        

    }
}
