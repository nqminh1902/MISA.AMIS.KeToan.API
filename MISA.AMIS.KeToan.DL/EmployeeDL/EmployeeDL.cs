using MISA.AMIS.KeToan.Common.Entities;
using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MISA.AMIS.KeToan.Common;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace MISA.AMIS.KeToan.DL
{

    public class EmployeeDL : BaseDL<Employee>, IEmployeeDL
    {


        /// <summary>
        /// Xóa hàng loạt bản ghi theo ID
        /// </summary>
        /// <param name="listEmployeeID">Danh sách ID</param>
        /// <returns>Danh sách ID xóa thành công</returns>
        /// CreateBy: Nguyễn Quang Minh (15/11/2022)
        public ListEmployeeID DeleteMultipleEmployee(ListEmployeeID listEmployeeID)
        {

            MySqlTransaction transaction = null;

            var ids = new List<Guid>();

            foreach (Guid id in listEmployeeID.EmployeeIDs)
            {
                ids.Add(id);
            }

            var str = string.Join("','", ids);

            //Chuẩn bị câu lệnh SQL
            string sql = $" DELETE FROM employee WHERE EmployeeID IN ('{str}');";

            int numberOfRowsAffected = 0;

            // Khời tạo kết nối tới DB MySQL
            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                mySqlConnection.Open();
                try
                {
                    transaction = mySqlConnection.BeginTransaction();
                    //Thực hiện gọi vào DB
                    numberOfRowsAffected = mySqlConnection.Execute(sql, transaction: transaction);
                    if (numberOfRowsAffected == listEmployeeID.EmployeeIDs.Count)
                    {
                        transaction.Commit();

                    }
                    else
                    {

                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
                finally
                {
                    mySqlConnection.Close();
                }
            }
            //Xử lý kết quả trả về

            //Thành công: Trả về Id nhân viên thêm thành công
            if (numberOfRowsAffected > 0)
            {
                return listEmployeeID;
            }
            return null;
        }

        /// <summary>
        /// Danh sách bản ghi theo tìm kiếm và phân trang
        /// </summary>
        /// <param name="keyword">Từ cần tìm kiếm</param>
        /// <param name="pageSize">Số bản ghi trên 1 trang</param>
        /// <param name="pageNumber">Vị trí của bản ghi bắt đầu</param>
        /// <returns></returns>
        public PaginResult GetEmployeeByFilterAndPaging(string? keyword, int pageSize, int pageNumber)
        {
            //Chuẩn bị câu lệnh sql
            string storeProcedureName = Procedure.GET_BY_FILTER_PAGING;

            // Chuẩn bị tham số đầu vào

            var parameters = new DynamicParameters();

            parameters.Add("v_offset", (pageNumber - 1) * pageSize);
            parameters.Add("v_limit", pageSize);
            parameters.Add("v_soft", "ModifiedDate DESC");

            var lstCondition = new List<string>();
            string searchClause = "";
            if (keyword != null)
            {
                lstCondition.Add($"EmployeeCode LIKE '%{keyword}%'");
                lstCondition.Add($"EmployeeName LIKE '%{keyword}%'");
                lstCondition.Add($"PhoneNumber LIKE '%{keyword}%'");
            }
            if (lstCondition.Count > 0)
            {
                searchClause = string.Join(" OR ", lstCondition);
            }
            parameters.Add("v_search", searchClause);

            // Khời tạo kết nối tới DB MySQL
            using (var mysqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                var multipleResult = mysqlConnection.QueryMultiple(storeProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                if (multipleResult != null)
                {
                    var listEmployee = multipleResult.Read<Employee>().ToList();
                    var totalCount = multipleResult.Read<long>().Single();
                    return new PaginResult()
                    {
                        Data = listEmployee,
                        TotalCount = totalCount,
                    };
                }
            }
            return new PaginResult();
        }

        /// <summary>
        /// Lấy mã nhân viên mới nhất
        /// </summary>
        /// <returns>Mã nhân viên mới nhất</returns>
        public string GetNewCode()
        {
            // Khời tạo kết nối tới DB MySQL

            // Chuẩn bị câu lệnh SQL
            string storeProcedureName = Procedure.GET_NEW_EMPLOYEE_CODE;

            // Thực hiên gọi vào DB
            string newID = "";
            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                var newIdNumber = mySqlConnection.QueryFirstOrDefault<double>(storeProcedureName, commandType: System.Data.CommandType.StoredProcedure);
                newID = $"NV{newIdNumber}";
            }

            return newID;
        }
    }
}
