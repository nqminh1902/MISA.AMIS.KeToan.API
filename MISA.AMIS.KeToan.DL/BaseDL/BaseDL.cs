using Dapper;
using MISA.AMIS.KeToan.Common;
using MISA.AMIS.KeToan.Common.Entities;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.KeToan.DL
{
    public class BaseDL<T> : IBaseDL<T>
    {
        // Rules: Khởi tạo muộn nhất có thể. Giải phóng sớm nhất có thể

        /// <summary>
        /// Lấy danh sách tất cả nhân viên
        /// </summary>
        /// <returns>Danh sách tất cả nhân viên</returns>
        /// CreatedBy: Nguyễn Quang Minh (11/11/2022)
        public IEnumerable<T> GetAllRecords()
        {
            // Chuẩn bị câu lệnh SQL
            string storeProcedureName = String.Format(Procedure.GET_ALL, typeof(T).Name);

            // Thực hiên gọi vào DB
            var records = new List<T>();

            // Khời tạo kết nối tới DB MySQL
            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                records = (List<T>)mySqlConnection.Query<T>(storeProcedureName, commandType: System.Data.CommandType.StoredProcedure);

            }      
            return records;
            
        }

        /// <summary>
        /// Lấy thông tin của 1 bản ghi theo ID
        /// </summary>
        /// <param name="recordID"> Id của bản ghi </param>
        /// <returns>Trả về thông tin của bản ghi</returns>
        /// CreatedBy: Nguyễn Quang Minh (11/11/2022)
        public T GetRecordByID(Guid recordID)
        {

            // Chuẩn bị câu lệnh SQL
            string storeProcedureName = String.Format(Procedure.GET_BY_ID, typeof(T).Name);

            var parameters = new DynamicParameters();
            parameters.Add($"{typeof(T).Name}ID", recordID);


            // Khời tạo kết nối tới DB MySQL
            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                // Thực hiên gọi vào DB
                var record = mySqlConnection.QueryFirstOrDefault<T>(storeProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
                // Xử lý trả về

                // Thành công: Trả về dữ liệu cho FE
                return record;
            }

        }

        /// <summary>
        /// Lấy mã bản ghi để kiểm tra có bị trùng không
        /// </summary>
        /// <param name="recordCode"> Mã của bản ghi</param>
        /// <returns>Trả về true or false</returns>
        public bool CheckCodeExist(string recordCode)
        {
            // Chuẩn bị câu lệnh SQL
            string storeProcedureName = Procedure.GET_CODE_BY_CODE;

            // Chuẩn bị tham số đầu vào
            var parameters = new DynamicParameters();

            parameters.Add("v_EmployeeCode", recordCode);

            // Khời tạo kết nối tới DB MySQL
            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                //Thực hiện gọi vào DB
                var code = mySqlConnection.QueryFirstOrDefault(storeProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                if (code != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Thêm mới một bản ghi
        /// </summary>
        /// <param name="record">Đối tượng cẩn thêm mới</param>
        /// <returns>ID của đối tượng vừa thêm mới</returns>
        /// CreatedBy: Nguyễn Quang Minh (25/11/2022)
        public Guid InsertRecord(T record)
        {
            // Chuẩn bị câu lệnh sql
            string storeProcedureName = String.Format(Procedure.INSERT_RECORD, typeof(T).Name);

            var parameters = new DynamicParameters();

            // Lấy rả các Prop của đối tượng
            var properties = record?.GetType().GetProperties();

            // Khởi tạo biến lưu giá trị
            object propValue;

            // Tạo id mới
            var newRecordID = Guid.NewGuid();

            foreach (var prop in properties)
            {
                // LẤy ra attribute KeyAttribute
                var keyAtrribute = Attribute.GetCustomAttribute(prop, typeof(KeyAttribute));
                // Nếu có thì lưu id mới
                if (keyAtrribute != null)
                {
                    propValue = newRecordID;
                }
                // Lưu giá trị của đối tượng
                else
                {
                    propValue = prop.GetValue(record);
                }

                // Chuyền vào parameter
                parameters.Add($"v_{prop.Name}", propValue);
            }
            // Khởi tạo kết nối tới DB MySQL
            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                mySqlConnection.Open();
                // Bắt đầu transaction 
                var transaction = mySqlConnection.BeginTransaction();
                // Thực hiện gọi vào DB
                var result = mySqlConnection.Execute(storeProcedureName, parameters, transaction, commandType: System.Data.CommandType.StoredProcedure);
                // return kết quả
                if (result > 0)
                {
                    transaction.Commit();
                    mySqlConnection.Close();
                    return newRecordID;
                }
                // Rollback về lại ban đầu
                else
                {
                    transaction.Rollback();
                    mySqlConnection.Close();
                    return newRecordID;
                }
            }

        }

        /// <summary>
        /// Sửa thông tin 1 bản ghi theo ID
        /// </summary>
        /// <param name="recordID">ID của bản ghi muốn sửa</param>
        /// <param name="record">Đối tượng bản ghi muốn sửa</param>
        /// <returns>ID của bản ghi đã sửa</returns>
        /// CreateBy: Nguyễn Quang Minh (12/11/2022)
        public Guid UpdateRecord(Guid recordID, T record)
        {
            // Chuẩn bị câu lệnh sql
            string storeProcedureName = String.Format(Procedure.UPDATE_RECORD, typeof(T).Name);

            var parameters = new DynamicParameters();

            // Lấy rả các Prop của đối tượng
            var properties = record?.GetType().GetProperties();

            // Khởi tạo biến lưu giá trị
            object propValue;

            foreach (var prop in properties)
            {
                // Lưu giá trị của đối tượng
                propValue = prop.GetValue(record);

                // Chuyền vào parameter
                parameters.Add($"v_{prop.Name}", propValue);
            }
            // Khởi tạo kết nối tới DB MySQL
            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                mySqlConnection.Open();
                // Bắt đầu transaction 
                var transaction = mySqlConnection.BeginTransaction();
                // Thực hiện gọi vào DB
                var result = mySqlConnection.Execute(storeProcedureName, parameters, transaction, commandType: System.Data.CommandType.StoredProcedure);
                if (result > 0)
                {
                    transaction.Commit();
                    mySqlConnection.Close();
                    return recordID;
                }
                else
                {
                    transaction.Rollback();
                    mySqlConnection.Close();
                    return recordID;
                }
            }
        }

        /// <summary>
        /// Xóa 1 bản ghi theo ID
        /// </summary>
        /// <param name="recordID">ID của bản ghi muốn xóa</param>
        /// <returns>ID của bản ghi đã bị xóa</returns>
        /// CreateBy: Nguyễn Quang Minh (12/11/2022)
        public Guid DeleteRecord(Guid recordID)
        {

            // Chuẩn bị câu lệnh SQL
            string storeProcedureName = String.Format(Procedure.DELETE, typeof(T).Name);

            var parameters = new DynamicParameters();
            parameters.Add($"{typeof(T).Name}ID", recordID);


            int numberOfRowsAffected = 0;
            // Khời tạo kết nối tới DB MySQL
            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                numberOfRowsAffected = mySqlConnection.Execute(storeProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

            }

            //Thành công: Trả về Id nhân viên thêm thành công
            if (numberOfRowsAffected > 0)
            {
                return recordID;
            }
            return new Guid();
        }

       
    }
}
