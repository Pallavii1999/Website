using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string division)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "StudentData.xlsx");
            var students = new List<Student>();

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Excel file not found.");
            }

            ExcelPackage.License.SetNonCommercialPersonal("Demo");

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets["Sheet3"];
                if (worksheet == null || worksheet.Dimension == null)
                {
                    return View(students);
                }

                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var divCode = worksheet.Cells[row, 26].Text;
                    var divName = GetDivisionName(divCode);

                    if (string.IsNullOrEmpty(division) || divName == division)
                    {
                        students.Add(new Student
                        {
                            Name = worksheet.Cells[row, 2].Text,
                            Mob1 = worksheet.Cells[row, 18].Text,
                            Mob2 = worksheet.Cells[row, 19].Text,
                            Gender = worksheet.Cells[row, 24].Text == "1" ? "Male" : "Female",
                            PsYear = worksheet.Cells[row, 25].Text,
                            Division = divName
                        });
                    }
                }
            }

            ViewBag.SelectedDivision = division;
            return View(students);
        }

        public IActionResult Edit(string name)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "StudentData.xlsx");
            ExcelPackage.License.SetNonCommercialPersonal("Pallavi");

            var student = new Student();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets["Sheet3"];
                if (worksheet == null || worksheet.Dimension == null)
                {
                    return NotFound("Sheet not found.");
                }

                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var currentName = worksheet.Cells[row, 2].Text;
                    if (currentName.Trim().Equals(name.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        student.Name = currentName;
                        student.Mob1 = worksheet.Cells[row, 18].Text;
                        student.Mob2 = worksheet.Cells[row, 19].Text;
                        student.Gender = worksheet.Cells[row, 24].Text == "1" ? "Male" : "Female";
                        student.PsYear = worksheet.Cells[row, 25].Text;
                        student.Division = GetDivisionName(worksheet.Cells[row, 26].Text);
                        break;
                    }
                }
            }

            return View(student);
        }

        [HttpPost]
        public IActionResult Edit(Student student)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "StudentData.xlsx");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Excel file not found.");
            }

            ExcelPackage.License.SetNonCommercialPersonal("Pallavi");

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets["Sheet3"];
                if (worksheet == null || worksheet.Dimension == null)
                {
                    return NotFound("Sheet not found.");
                }

                int rowCount = worksheet.Dimension.Rows;
                bool updated = false;

                for (int row = 2; row <= rowCount; row++)
                {
                    var currentName = worksheet.Cells[row, 2].Text;
                    if (currentName.Trim().Equals(student.Name.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        worksheet.Cells[row, 18].Value = student.Mob1;
                        worksheet.Cells[row, 19].Value = student.Mob2;
                        worksheet.Cells[row, 24].Value = student.Gender == "Male" ? "1" : "0";
                        worksheet.Cells[row, 25].Value = student.PsYear;
                        worksheet.Cells[row, 26].Value = GetDivisionCode(student.Division);
                        updated = true;
                        break;
                    }
                }

                if (updated)
                {
                    package.SaveAs(new FileInfo(filePath));
                    TempData["Message"] = $"Student '{student.Name}' updated successfully!";
                }
                else
                {
                    TempData["Message"] = $"Student '{student.Name}' not found in Excel.";
                }
            }

            return RedirectToAction("Index");
        }

        private string GetDivisionName(string code)
        {
            return code switch
            {
                "1" => "A",
                "2" => "B",
                "3" => "C",
                "4" => "D",
                "5" => "E",
                "6" => "F",
                _ => "Unknown"
            };
        }

        private string GetDivisionCode(string name)
        {
            return name switch
            {
                "A" => "1",
                "B" => "2",
                "C" => "3",
                "D" => "4",
                "E" => "5",
                "F" => "6",
                _ => "0"
            };
        }
    }
}
