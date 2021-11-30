using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Drawing;

namespace ExportToExcel.App
{
    class ExcelFacade
    {
        //Excel.Application excel_app = null;
        //Excel.Workbook excel_book = null;
        //Excel.Worksheet excel_sheet = null;
        ////Excel.Range excel_range = null;

        //Structure.Model _model = null;
        //string _folder = "";

        //public ExcelFacade(Structure.Model model, string folder)
        //{
        //    _model = model;
        //    this._folder = folder;

        //    excel_app = new Excel.Application();
        //    excel_app.ScreenUpdating = false;
        //    excel_app.Visible = false;

        //    //string file_template = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"\Plugins\ExportToExcel\Resources" + Path.DirectorySeparatorChar + "reports_template.xlsx";
        //    string file_template = @"D:\reports_template.xlsx";

        //    excel_app.Workbooks.Open(file_template,
        //        Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //        Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //        Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //        Type.Missing, Type.Missing);

        //    excel_book = excel_app.Workbooks.Item[1];
        //    excel_sheet = excel_book.Worksheets[1];

        //    // Call run method
        //    Run();
        //}

        //// Run
        //private void Run()
        //{
        //    int count_row = 6;
        //    int count_link = 5;
        //    int position = 1;

        //    // Loop RootFolders
        //    foreach (var root_item in _model.Folders)
        //    {
        //        // string
        //        string DateMeeting = root_item.NameFolder; // D

        //        // Loop TitleFolders
        //        foreach (var title_item in root_item.Folders)
        //        {
        //            // string
        //            string name_title = title_item.NameFolder; // E

        //            // Loop Viewpoints
        //            foreach (var viewpoint_item in title_item.ViewPoints)
        //            {
        //                // position A
        //                string NumberCollision = viewpoint_item.NumberCollision; // B
        //                string Description = viewpoint_item.Description; // C 
        //                string Discipline = viewpoint_item.Discipline; // F

        //                #region Add in excel

        //                // номер п/п
        //                var A = excel_sheet.Range["A" + count_row];
        //                A.Value = position;
        //                A.HorizontalAlignment = Excel.Constants.xlCenter;
        //                A.VerticalAlignment = Excel.Constants.xlCenter;
        //                A.Font.Bold = true;

                        
        //                // Номер коллизии (соответствует номеру точки обзора в NWD)
        //                var B = excel_sheet.Range["B" + count_row];
        //                B.Value = NumberCollision;
        //                B.HorizontalAlignment = Excel.Constants.xlCenter;
        //                B.VerticalAlignment = Excel.Constants.xlCenter;
        //                B.Font.Bold = true;


        //                // Комментарий коллизии (текст описания точки обзора в NWD)
        //                var C = excel_sheet.Range["C" + count_row];
        //                excel_sheet.Hyperlinks.Add(C, viewpoint_item.Image, Type.Missing, Type.Missing, Description);

        //                var hyper_link = excel_sheet.Hyperlinks.Item[count_row - count_link];
        //                hyper_link.Range.WrapText = true;
                        
        //                hyper_link.Range.VerticalAlignment = Excel.Constants.xlCenter;
        //                hyper_link.Range.Font.ColorIndex = 51;
        //                hyper_link.Range.Font.Underline = false;
        //                hyper_link.Range.RowHeight = hyper_link.Range.RowHeight + 18;


        //                // Дата проверки
        //                var D = excel_sheet.Range["D" + count_row];
        //                D.Value = DateMeeting;
        //                D.HorizontalAlignment = Excel.Constants.xlCenter;
        //                D.VerticalAlignment = Excel.Constants.xlCenter;


        //                // Титул
        //                var E = excel_sheet.Range["E" + count_row];
        //                E.Value = name_title;
        //                E.HorizontalAlignment = Excel.Constants.xlCenter;
        //                E.VerticalAlignment = Excel.Constants.xlCenter;
        //                E.Font.Bold = true;


        //                // Дисциплина 
        //                var F = excel_sheet.Range["F" + count_row];
        //                F.Value = Discipline;
        //                F.HorizontalAlignment = Excel.Constants.xlCenter;
        //                F.VerticalAlignment = Excel.Constants.xlCenter;
        //                F.Font.Bold = true;


        //                // Статус коллизии
        //                try
        //                {
        //                    if(viewpoint_item.Comments != null && viewpoint_item.Comments.Count != 0)
        //                    {
        //                        var H = excel_sheet.Range["H" + count_row];
        //                        SetStatus(viewpoint_item.Comments[0], H);
        //                    }
                                
        //                }
        //                catch(Exception ex)
        //                {

        //                }
                        
        //                #endregion

        //                count_row++;
        //                position++;
        //            }
        //        }

        //        excel_sheet.Range["A" + count_row].EntireRow.Interior.ColorIndex = 15;
        //        excel_sheet.Range["A" + count_row].RowHeight = 20;


        //        count_link++;
        //        count_row++;
        //    }

        //    string save_path = _folder + Path.DirectorySeparatorChar;
        //    string file_name = "Отчет_по_" + Path.GetFileNameWithoutExtension(_model.NameProject) + _model.CurrentDate.ToString("_dd_MM_yyyy") + ".xlsx";

        //    if (!Directory.Exists(save_path))
        //        Directory.CreateDirectory(save_path);

        //    if (File.Exists(save_path + file_name))
        //        File.Delete(save_path + file_name);

        //    excel_book.SaveAs(save_path + file_name);
            
        //    excel_app.ScreenUpdating = true;
        //    excel_app.Visible = true;
        //}

        //// Set status
        //private void SetStatus(Structure.Comment comment, Excel.Range range)
        //{
        //    switch (comment.Status)
        //    {
        //        case "Активный":
        //            range.Value = "Активный";
        //            range.Interior.Color = Color.FromArgb(0, 146, 208, 80);
        //            break;

        //        case "Утверждено":
        //            range.Value = "Утверждено";
        //            range.Interior.Color = Color.FromArgb(0, 146, 209, 80);
        //            break;

        //        case "Новый":
        //            range.Value = "Новый";
        //            range.Interior.Color = Color.FromArgb(0, 196, 229, 159);
        //            break;

        //        case "Исправлено":
        //            range.Value = "Исправлено";
        //            range.Interior.Color = Color.FromArgb(0, 255, 255, 0);
        //            break;
        //    }
            
        //    range.HorizontalAlignment = Excel.Constants.xlCenter;
        //    range.VerticalAlignment = Excel.Constants.xlCenter;
        //    range.Font.Bold = true;
        //}

        //// Set border
        //private void SetBorder(Excel.Range range)
        //{
        //    var Excelcells = range;
        //    Microsoft.Office.Interop.Excel.XlBordersIndex BorderIndex;

        //    BorderIndex = Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft;
        //    Excelcells.Borders[BorderIndex].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
        //    Excelcells.Borders[BorderIndex].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
        //    Excelcells.Borders[BorderIndex].ColorIndex = 0;


        //    BorderIndex = Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop;
        //    Excelcells.Borders[BorderIndex].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
        //    Excelcells.Borders[BorderIndex].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
        //    Excelcells.Borders[BorderIndex].ColorIndex = 0;


        //    BorderIndex = Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom;
        //    Excelcells.Borders[BorderIndex].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
        //    Excelcells.Borders[BorderIndex].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
        //    Excelcells.Borders[BorderIndex].ColorIndex = 0;

        //    BorderIndex = Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight;
        //    Excelcells.Borders[BorderIndex].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
        //    Excelcells.Borders[BorderIndex].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
        //    Excelcells.Borders[BorderIndex].ColorIndex = 0;
        //}
    }
}
