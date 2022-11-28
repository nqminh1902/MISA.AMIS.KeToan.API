using MISA.AMIS.KeToan.Common.Entities;
using MISA.AMIS.KeToan.Common;
using MISA.AMIS.KeToan.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MISA.AMIS.KeToan.Common.Entities.Attribute;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace MISA.AMIS.KeToan.BL
{
    public class BaseBL<T> : IBaseBL<T>

    {
        #region Field

        private IBaseDL<T> _baseDL;

        #endregion

        #region Constructor

        public BaseBL(IBaseDL<T> baseDL)
        {
            _baseDL = baseDL;
        }

        #endregion

        #region Method
        /// <summary>
        /// Lấy danh sách tất cả bản ghi
        /// </summary>
        /// <returns>Danh sách tất cả bản ghi</returns>
        /// CreatedBy: Nguyễn Quang Minh (11/11/2022)
        public IEnumerable<T> GetAllRecords()
        {
            return _baseDL.GetAllRecords();
        }

        /// <summary>
        /// Lấy thông tin của 1 bản ghi theo ID
        /// </summary>
        /// <param name="recordID"> Id của bản ghi </param>
        /// <returns>Trả về thông tin của bản ghi</returns>
        /// CreatedBy: Nguyễn Quang Minh (11/11/2022)
        public T GetRecordByID(Guid recordID)
        {
            return _baseDL.GetRecordByID(recordID);
        }

        /// <summary>
        /// Thêm mới một bản ghi
        /// </summary>
        /// <param name="record">Đối tượng cẩn thêm mới</param>
        /// <returns>ID của đối tượng vừa thêm mới</returns>
        /// CreatedBy: Nguyễn Quang Minh (25/11/2022)
        public ServiceResponse InsertRecord(T record)
        {
            // Gọi đến hàm để validate dữ liệu
            var validateResult = ValidateRequestData(true, record);
            

            // Nếu Fail thì trả về lỗi
            if (!validateResult.Success)
            {
                return new ServiceResponse()
                {
                    Data = validateResult.Data,
                    Success = validateResult.Success,
                };
            }
            else
            {
                // Thực hiện gọi làm thêm bản ghi và trả về kết quả
                var employeeID = _baseDL.InsertRecord(record);
                return new ServiceResponse()
                {
                    Data = validateResult.Data,
                    Success = validateResult.Success,
                    ID = employeeID
                };

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
            return _baseDL.DeleteRecord(recordID);
        }

        /// <summary>
        /// Sửa thông tin 1 bản ghi theo ID
        /// </summary>
        /// <param name="recordID">ID của bản ghi muốn sửa</param>
        /// <param name="record">Đối tượng bản ghi muốn sửa</param>
        /// <returns>ID của bản ghi đã sửa</returns>
        /// CreateBy: Nguyễn Quang Minh (12/11/2022)
        public ServiceResponse UpdateRecord(Guid recordID, T record)
        {
            // Gọi đến hàm để validate dữ liệu
            var validateResult = ValidateRequestData(false, record);

            // Nếu Fail thì trả về lỗi
            if (!validateResult.Success)
            {
                return new ServiceResponse()
                {
                    Data = validateResult.Data,
                    Success = validateResult.Success,
                };
            }
            else
            {
                // Thực hiện gọi hàm sửa bản ghi và trả về kết quả
                var employeeIDUpdate = _baseDL.UpdateRecord(recordID, record);
                return new ServiceResponse()
                {
                    Data = validateResult.Data,
                    Success = validateResult.Success,
                    ID = employeeIDUpdate
                };
            }
        }
        #endregion

        /// <summary>
        /// Validate dữ liệu
        /// </summary>
        /// <param name="differentiate">Tham số để phân biệt giữa update và insert</param>
        /// <param name="employee">Đối tượng nhân viên cần validate</param>
        /// <returns>Trả về kết quả validate có thành công hay không</returns>
        private ServiceResponse ValidateRequestData(bool differentiate, T record)
        {
            // Lấy danh sách thuộc tính(property) của class
            var properties = typeof(T).GetProperties();

            // Duyệt từng thuộc tính 
            var validateFailures = new List<string>();


            foreach (var property in properties)
            {
                // Lấy được giá trị thuộc tính đó
                var propertyValue = property.GetValue(record);

                var requiredAttribute = (IsNullOrEmpty?)Attribute.GetCustomAttribute(property, typeof(IsNullOrEmpty));

                var checkEmail = (IsEmail?)Attribute.GetCustomAttribute(property, typeof(IsEmail));

                var checkDuplicate = (IsDuplicate?)Attribute.GetCustomAttribute(property, typeof(IsDuplicate));

                var checkDateOfBirth = (IsValidDateOfBirth?)Attribute.GetCustomAttribute(property, typeof(IsValidDateOfBirth));

                var checkIsNumber = (IsNumber?)Attribute.GetCustomAttribute(property, typeof(IsNumber));

                var checkIsNotNumber = (IsNotNumber?)Attribute.GetCustomAttribute(property, typeof(IsNotNumber));

                var checkValidCode = (IsValidCode?)Attribute.GetCustomAttribute(property, typeof(IsValidCode));

                // Kiểm tra porperty có required không và giá trị từ frondend có null không
                if (requiredAttribute != null && string.IsNullOrEmpty(propertyValue?.ToString()))
                {
                    validateFailures.Add(requiredAttribute.ErrorMessage);
                }

                
                // Kiểm tra property có lớn hơn 20 ký tự và kết thúc bằng số không
                if (checkValidCode != null && propertyValue?.ToString().Length > 20)
                {
                    validateFailures.Add(checkValidCode.ErrorMessage);
                }

                // Kiểm tra property có IsEmail không và có đúng định dạng email không
                if (checkEmail != null && propertyValue?.ToString() != null && propertyValue.ToString()?.Trim() != "")
                {
                    bool isEmail = IsValidEmail(propertyValue.ToString());
                    if (!isEmail)
                    {
                        validateFailures.Add(checkEmail.ErrorMessage);

                    }
                }

                // Kiểm tra xem property không phải là số nếu là số thì lỗi
                if(checkIsNotNumber != null && int.TryParse(propertyValue.ToString(), out int value))
                {
                    validateFailures.Add(checkIsNotNumber.ErrorMessage);
                }

                // Kiểm tra xem property có phải là số không
                if(checkIsNumber != null && !string.IsNullOrEmpty(propertyValue.ToString().Trim()))
                {
                    if (!int.TryParse(propertyValue.ToString(), out int m))
                    {
                    validateFailures.Add(checkIsNumber.ErrorMessage);
                    }
                }

                //Kiểm tra property có IsDuplicate không và có bị trùng mã nhân viên không 
                //differentiate: true là insert, false là update
                if (checkDuplicate != null && differentiate && propertyValue?.ToString().Length < 20)
                {
                    bool check = _baseDL.CheckCodeExist(propertyValue.ToString());

                    if (check)
                    {
                        validateFailures.Add(checkDuplicate.ErrorMessage);
                    }

                }

                if (checkDateOfBirth != null)
                {
                    var dateValue = propertyValue as DateTime? ?? new DateTime();

                    //alter this as needed. I am doing the date comparison if the value is not null

                    if (dateValue.Date > DateTime.Now.Date)
                    {
                        validateFailures.Add(checkDateOfBirth.ErrorMessage);
                    }
                }
            }

            if (validateFailures.Count > 0)
            {
                return new ServiceResponse()
                {
                    Success = false,
                    Data = validateFailures
                };
            }
            return new ServiceResponse()
            {
                Success = true,
            };
        }

        /// <summary>
        /// Check định dạng email
        /// </summary>
        /// <param name="email">Dữ liệu chuỗi email đầu vào</param>
        /// <returns>Nếu đúng thì trả về true, ngược lại false</returns>
        private static bool IsValidEmail(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (email.Trim().EndsWith("."))
            {
                return false;
            }
            if (match.Success)
            {
                return true;
            }
            return false;
        }

        protected virtual bool ValidateCustom(T entity)
        {

            return true;
        }
    }
}
