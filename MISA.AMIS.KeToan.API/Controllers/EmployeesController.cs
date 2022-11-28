using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Spreadsheet;
using Fingers10.ExcelExport.ActionResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.AMIS.KeToan.API.Controllers;
using MISA.AMIS.KeToan.BL;
using MISA.AMIS.KeToan.Common;
using MISA.AMIS.KeToan.Common.Entities;
using MISA.AMIS.KeToan.Common.Enums;
using MySqlConnector;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Runtime.Versioning;
//using Color = DocumentFormat.OpenXml.Office2010.Excel.Color;

namespace MISA.AMIS.KeToan.API.Controllers
{
    //Atribute nằm trong ngoặc vuông
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : BaseController<Employee>
    {

        #region Field
        private IEmployeeBL _employeeBL;
        #endregion

        #region Constructor
        public EmployeesController(IEmployeeBL employeeBL) : base(employeeBL)
        {
             _employeeBL = employeeBL;
        }
        #endregion

        #region Method

        /// <summary>
        /// Xóa danh sách nhân viên theo ID
        /// </summary>
        /// <param name="listEmployeeID">danh sách ID nhân viên</param>
        /// <returns>Danh sách nhân viên đã xóa</returns>
        [HttpPost("deleteBatch")]
        public IActionResult DeleteMultipleEmployee([FromBody] ListEmployeeID listEmployeeID)
        {
            try
            {
                var ListID = _employeeBL.DeleteMultipleEmployee(listEmployeeID);

                if (ListID != null)
                {
                    return StatusCode(StatusCodes.Status200OK, ListID);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    ErrorCode = 2,
                    DevMsg = Resources.DevMsg_DeleteMultipleFail,
                    UserMsg = Resources.UserMsg_DeleteMultipleFail,
                    MoreInfo = Resources.MoreInfo_Exception,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
                {
                    ErrorCode = AMISErrorCode.Exception,
                    DevMsg = Resources.DevMsg_Exception,
                    UserMsg = Resources.UserMsg_Exception,
                    MoreInfo = Resources.MoreInfo_Exception,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

        /// <summary>
        /// API lấy danh sách nhân viên theo bộ lọc và phân trang
        /// </summary>
        /// <param name="keyword">Từ khóa muốn tìm kiếm</param>
        /// <param name="limit">Số bản ghi muốn lấy</param>
        /// <param name="offset">Vị trí của bản ghi bắt đầu lấy</param>
        /// <param name="departmentID">ID của phòng ban muốn lọc</param>
        /// <returns>Danh sách nhân viên và tổng số bản ghi</returns>
        /// CreateBy: Nguyễn Quang Minh (03/11/2022)
        [HttpGet("filter")]
        public IActionResult GetEmployeeByFilterAndPaging([FromQuery] string? keyword, [FromQuery] int pageSize = 25, [FromQuery] int pageNumber = 1)
        {
            try
            {

                var multipleResult = _employeeBL.GetEmployeeByFilterAndPaging(keyword, pageSize, pageNumber);

                if (multipleResult != null)
                {
                    return StatusCode(StatusCodes.Status200OK, multipleResult);
                }
                // Thất bại
                return StatusCode(StatusCodes.Status200OK, new PaginResult());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
                {
                    ErrorCode = AMISErrorCode.Exception,
                    DevMsg = Resources.DevMsg_Exception,
                    UserMsg = Resources.UserMsg_Exception,
                    MoreInfo = Resources.MoreInfo_Exception,
                    TraceId = HttpContext.TraceIdentifier
                });
            }

        }

        /// <summary>
        /// Lấy mã nhân viên mới nhất
        /// </summary>
        /// <returns>Trả về mã nhân viên mới nhất</returns>
        [HttpGet("NewEmployeeCode")]
        public IActionResult GetNewCode()
        {
            try
            {
                var newCode = _employeeBL.GetNewCode();
                if (newCode != "")
                {
                    return StatusCode(StatusCodes.Status200OK, newCode);
                }
                return StatusCode(StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
                {
                    ErrorCode = AMISErrorCode.Exception,
                    DevMsg = Resources.DevMsg_Exception,
                    UserMsg = Resources.UserMsg_Exception,
                    MoreInfo = Resources.MoreInfo_Exception,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

        /// <summary>
        /// Xuất ra danh sách nhân viên ra file excel
        /// </summary>
        /// <returns>File excel danh sách nhân viên</returns>
        [HttpPost("exportExcel")]
        public IActionResult ExportExcelFile()
        {
            try
            {
                // Lấy dữ liệu từ database
                var listEmployee = _employeeBL.GetAllRecords();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                if (listEmployee != null)
                {
                    // Khởi tạo vùng nhớ
                    var stream = new MemoryStream();
                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        // Tạo và đặt tên cho sheet
                        var worksheet = xlPackage.Workbook.Worksheets.Add("Sheet1");
                        const int startRow = 4;
                        var row = startRow;
                        var stt = 1;

                        // Khởi tạo title
                        worksheet.Cells["A1"].Value = "DANH SÁCH NHÂN VIÊN";
                        using (var r = worksheet.Cells["A1:I1"])
                        {
                            // Merge hàng A1:I1
                            r.Merge = true;
                            // Cho chữ căn giữa
                            r.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous;
                            // Set phông chữ
                            r.Style.Font.SetFromFont("Arial", 16, true);
                            
                        }

                        // Set chiều rộng cột excel
                        worksheet.Column(1).Width = 8;
                        worksheet.Column(2).Width = 15;
                        worksheet.Column(3).Width = 30;
                        worksheet.Column(4).Width = 10;
                        worksheet.Column(5).Width = 15;
                        worksheet.Column(6).Width = 30;
                        worksheet.Column(7).Width = 30;
                        worksheet.Column(8).Width = 15;
                        worksheet.Column(9).Width = 30;

                        // Gán tên header cho từng cột khi xuất ra file excel
                        worksheet.Cells["A3"].Value = "STT";
                        worksheet.Cells["B3"].Value = "Mã nhân viên";
                        worksheet.Cells["C3"].Value = "Tên nhân viên";
                        worksheet.Cells["D3"].Value = "Giới tính";
                        worksheet.Cells["E3"].Value = "Ngày sinh";
                        worksheet.Cells["F3"].Value = "Chức danh";
                        worksheet.Cells["G3"].Value = "Tên đơn vị";
                        worksheet.Cells["H3"].Value = "Số tài khoản";
                        worksheet.Cells["I3"].Value = "Tên ngân hàng";


                        var modelRows = listEmployee.Count() + 3;
                        string modelRange = "A4:I" + modelRows.ToString();
                        using (var range = worksheet.Cells[modelRange])
                        {
                            // Set phông chữ
                            range.Style.Font.SetFromFont("Arial", 10, false);

                            // Set Border
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        }

                        // Lấy range vào tạo format cho range đó ở đây là từ A3 tới I3
                        using (var range = worksheet.Cells["A3:I3"])
                        {
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous;
                            // Set PatternType
                            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                            // Set Font cho text  trong Range hiện tại
                            range.Style.Font.SetFromFont("Arial", 10, true);
                            // Set Border
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        }

                        // Truyền dữ liệu vào excel
                        row = 4;
                        foreach (var employee in listEmployee)
                        {
                            worksheet.Cells[row, 1].Value = stt;
                            worksheet.Cells[row, 2].Value = employee.EmployeeCode;
                            worksheet.Cells[row, 3].Value = employee.EmployeeName;
                            worksheet.Cells[row, 4].Value = employee.Gender == Gender.Male ? "Nam" : employee.Gender == Gender.Female ? "Nữ" : employee.Gender == Gender.Orther ? "Khác" : "";
                            worksheet.Cells[row, 5].Value = employee.DateOfBirth;
                            worksheet.Cells[row, 5].Style.Numberformat.Format = "dd/mm/yyyy";
                            worksheet.Cells[row, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous;
                            worksheet.Cells[row, 6].Value = employee.JobPositionName;
                            worksheet.Cells[row, 7].Value = employee.DepartmentName;
                            worksheet.Cells[row, 8].Value = employee.BankAccountNumber;
                            worksheet.Cells[row, 9].Value = employee.BankName;
                            
                            row++;
                            stt++;
                        }
                        // Lưu spreadsheet mới
                        xlPackage.Save();
                    }

                    stream.Position = 0;
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "users.xlsx");
                }
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
                {
                    ErrorCode = AMISErrorCode.Exception,
                    DevMsg = Resources.DevMsg_Exception,
                    UserMsg = Resources.UserMsg_Exception,
                    MoreInfo = Resources.MoreInfo_Exception,
                    TraceId = HttpContext.TraceIdentifier
                });
            }

        }
        #endregion
    }
}

