using Atlantic.Data.Models;
using Atlantic.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Entities;
using OfficeOpenXml;

namespace Atlantic.Api.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BulkEntryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BulkEntryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles"));
                }
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var data = new List<Dictionary<string, object>>();
                var cards = new List<VisaCard>();
                var existingCardCounter = 0;

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    for (int row = 2; row <= rowCount; row++) // Start from row 2 to skip the header
                    {
                        var dateParts = worksheet.Cells[row, 5].Text.Split("/");
                        var card = new VisaCard
                        {
                            SerialNumber = worksheet.Cells[row, 2].Text,
                            Barcode = worksheet.Cells[row, 3].Text,
                            LastDigits = worksheet.Cells[row, 4].Text,
                            Expiry = (new DateTime(Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[0]), 1)).AddMonths(1).AddMinutes(-1),
                        };
                        if (!DB.Queryable<VisaCard>().Any(x => x.SerialNumber == card.SerialNumber))
                        {
                            cards.Add(card);
                        }
                        else
                        {
                            existingCardCounter++;
                        }
                        var rowData = new Dictionary<string, object>();
                        for (int col = 1; col <= colCount; col++)
                        {
                            rowData[worksheet.Cells[1, col].Text] = worksheet.Cells[row, col].Text;
                        }
                        data.Add(rowData);
                    }
                }

                if (cards.Count > 0)
                {
                    await cards.SaveAsync();
                }

                return Ok(cards);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
