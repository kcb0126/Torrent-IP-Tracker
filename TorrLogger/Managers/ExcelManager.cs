using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorrLogger.ViewModels;

namespace TorrLogger.Managers
{
    class ExcelManager
    {
        public static ExcelManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new ExcelManager();
                }
                return _instance;
            }
        }

        public void SaveClientViewModels(ObservableCollection<ClientViewModel> models, string excelFileName)
        {
            Microsoft.Office.Interop.Excel.Application oXL;
            Microsoft.Office.Interop.Excel._Workbook oWB;
            Microsoft.Office.Interop.Excel._Worksheet oSheet;
            Microsoft.Office.Interop.Excel.Range oRng;
            object misValue = System.Reflection.Missing.Value;
            try
            {
                //Start Excel and get Application object.
                oXL = new Microsoft.Office.Interop.Excel.Application();
                oXL.Visible = false;

                //Get a new workbook.
                oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));
                oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

                //Add table headers going cell by cell.
                oSheet.Cells[1, 1] = "No";
                oSheet.Cells[1, 2] = "IP Address";
                oSheet.Cells[1, 3] = "Port";
                oSheet.Cells[1, 4] = "Client";
                oSheet.Cells[1, 5] = "Start Date";
                oSheet.Cells[1, 6] = "Start Time";
                oSheet.Cells[1, 7] = "End Date";
                oSheet.Cells[1, 8] = "End Time";
                oSheet.Cells[1, 9] = "Title";
                oSheet.Cells[1, 10] = "File Hash";
                oSheet.Cells[1, 11] = "Country";
                oSheet.Cells[1, 12] = "ISP";

                //Format A1:F1 as bold, vertical alignment = center.
                oSheet.get_Range("A1", "K1").Font.Bold = true;
                oSheet.get_Range("A1", "K1").VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

                for(int i = 0; i < models.Count; i++)
                {
                    oSheet.Cells[i + 2, 1] = models[i].No;
                    oSheet.Cells[i + 2, 2] = models[i].IpAddress;
                    oSheet.Cells[i + 2, 3] = models[i].Port;
                    oSheet.Cells[i + 2, 4] = models[i].Client;
                    oSheet.Cells[i + 2, 5] = "=\"" + models[i].Date + "\"";
                    oSheet.Cells[i + 2, 6] = models[i].Time;
                    oSheet.Cells[i + 2, 7] = "=\"" + models[i].EndDate + "\"";
                    oSheet.Cells[i + 2, 8] = models[i].EndTime;
                    oSheet.Cells[i + 2, 9] = models[i].Title;
                    oSheet.Cells[i + 2, 10] = models[i].FileHash;
                    oSheet.Cells[i + 2, 11] = models[i].Country;
                    oSheet.Cells[i + 2, 12] = models[i].ISP;
                }

                //AutoFit columns A:H.
                oRng = oSheet.UsedRange;
                oRng.EntireColumn.AutoFit();

                oXL.Visible = false;
                oXL.UserControl = false;
                oWB.SaveAs(excelFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                    false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                oWB.Close();
            }
            catch
            {
                int notused = 0;
            }
        }

        private static ExcelManager _instance = null;
    }
}
