using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LandDistribution
{
    public partial class Form1 : Form
    {
        Dictionary<int, int> Groups;
        Dictionary<int, int> Lands;

        public Form1()
        {
            InitializeComponent();
            Groups = new Dictionary<int, int>();
            Lands = new Dictionary<int, int>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fname = "";
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Excel File Dialog";
            fdlg.Filter = "Excel Files|*.xls;*.xlsx";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                fname = fdlg.FileName;
            }

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(fname);
            Microsoft.Office.Interop.Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Microsoft.Office.Interop.Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            // dt.Column = colCount;  
            dataGridView1.ColumnCount = colCount; // Sould be 4
            dataGridView1.RowCount = rowCount;

            for (int i = 1; i <= rowCount; i++)
            {
                // Add data to dictionaries
                if (i > 1)
                {
                    if (xlRange.Cells[i, 1].Value != null && xlRange.Cells[i, 2].Value != null)
                    {
                        int groupId = int.Parse(xlRange.Cells[i, 1].Value?.ToString());
                        double groupPercent = double.Parse(xlRange.Cells[i, 2].Value?.ToString());
                        Groups.Add(groupId, Convert.ToInt32(groupPercent * 100 * 1000));
                    }

                    if (xlRange.Cells[i, 3].Value != null && xlRange.Cells[i, 4].Value != null)
                    {
                        int landId = int.Parse(xlRange.Cells[i, 3].Value?.ToString());
                        double landPercent = double.Parse(xlRange.Cells[i, 4].Value?.ToString());
                        Lands.Add(landId, Convert.ToInt32(landPercent * 100 * 1000));
                    }
                }

                for (int j = 1; j <= colCount; j++)
                {
                    //write the value to the Grid  
                    if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                    {
                        dataGridView1.Rows[i - 1].Cells[j - 1].Value = xlRange.Cells[i, j].Value2.ToString();
                    }
                
                    // Console.Write(xlRange.Cells[i, j].Value2.ToString() + "\t");  
                    //add useful things here!     
                }
            }

            //cleanup  
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:  
            //  never use two dots, all COM objects must be referenced and released individually  
            //  ex: [somthing].[something].[something] is bad  

            //release com objects to fully kill excel process from running in the background  
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release  
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release  
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";

            int variance = Convert.ToInt32(numericUpDown1.Value * 1000);

            // Key - GroupID
            // Value - List of LandID
            SortedDictionary<int, List<int>> result = BL.CalculateLands(Groups, Lands, variance);

            foreach (KeyValuePair<int, List<int>> entry in result)
            {
                textBox1.AppendText($"Group {entry.Key} (Original Percentage: {Groups[entry.Key]/1000.0}%):" + Environment.NewLine);
                textBox1.AppendText($"Lands: {string.Join(", ", entry.Value)}" + Environment.NewLine);
                textBox1.AppendText($"Total percentage got: {Math.Round(entry.Value.Select(e => Lands[e]/1000.0).Sum(), 2)}%" + Environment.NewLine);
                textBox1.AppendText($"============================" + Environment.NewLine);
            }           
        }
    }
}
