/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace Agrishare.Core.Utils
{
    /// <summary>
    /// Base class to import data from an Excel spreadsheet
    /// </summary>
    public class ImportExcel
    {
        /// <summary>
        /// Excel file
        /// </summary>
        public Entities.File ExcelFile;

        private int rowCount = 0;
        private int columnCount = 0;

        private int progressCounter = 0;
        private int totalCounter = 0;

        public bool Process(out string Feedback)
        {
            Feedback = string.Empty;
            var success = false;

            string path = string.Empty;
            ExcelPackage package = null;
            ExcelWorksheet workSheet = null;

            try
            {
                path = Entities.File.CDNAbsolutePath + ExcelFile.Name + ExcelFile.Extension;
                package = new ExcelPackage(new FileInfo(path));
                workSheet = package.Workbook.Worksheets[1];
            }
            catch (Exception ex)
            {
                package?.Dispose();
                Feedback = $"{ex.Message} {ex.StackTrace}";
                return false;
            }

            try
            {
                rowCount = workSheet.Dimension.Rows;
                columnCount = workSheet.Dimension.Columns;

                var range = workSheet.Cells[1, 1, rowCount, columnCount];

                var rowsToProcess = rowCount - 1;
                totalCounter = columnCount * rowsToProcess * 2;

                var startRow = 2;

                PreValidation();

                // validate cells
                for (var row = startRow; row <= rowCount; row++)
                {
                    // ignore empty row
                    if (IsEmptyRow(range, row))
                        continue;

                    for (var col = 1; col <= columnCount; col++)
                    {
                        var error = string.Empty;
                        var cell = GetCellValue(range, row, col);
                        if (!ValidateCell(col, cell, out error))
                            throw new Exception($"{CellReference(row, col)} {error}");
                    }
                }

                PostValidation();
                PreExtraction();

                // extract data
                for (var row = startRow; row <= rowCount; row++)
                {
                    // ignore empty row
                    if (IsEmptyRow(range, row))
                        continue;

                    // convert columns to a List
                    var columns = new List<string>();
                    for (var col = 1; col <= columnCount; col++)
                    {
                        var cell = GetCellValue(range, row, col);
                        columns.Add(cell);
                        progressCounter++;
                    }

                    ExtractRow(columns);
                }

                PostExtraction();

                // all done
                Feedback = $"Extracted {rowCount} rows";
                success = true;

            }
            catch (Exception ex)
            {
                // whoops
                Feedback = ex.Message;
            }
            finally
            {
                // cleanup
                package?.Dispose();
                try { File.Delete(path); }
                catch { }
            }

            return success;

        }

        /// <summary>
        /// Method to run before row data is validate
        /// </summary>
        public virtual void PreValidation()
        {

        }

        /// <summary>
        /// Virtual method that is used to validate the row data
        /// </summary>
        public virtual bool ValidateCell(int Column, string Data, out string Error)
        {
            Error = "Method not implemented";
            return false;
        }

        /// <summary>
        /// Method to run after row data has been validated successfully
        /// </summary>
        public virtual void PostValidation()
        {

        }

        /// <summary>
        /// Method to run before row data is extracted
        /// </summary>
        public virtual void PreExtraction()
        {

        }
        
        /// <summary>
        /// Virtual method that is used to extract row data
        /// </summary>
        public virtual void ExtractRow(List<string> Row)
        {

        }

        /// <summary>
        /// Method to run after row data has been extracted successfully
        /// </summary>
        public virtual void PostExtraction()
        {

        }

        private bool IsEmptyRow(ExcelRange Range, int Row)
        {
            for (var col = 1; col <= columnCount; col++)
            {
                var cellValue = GetCellValue(Range, Row, col);
                if (!string.IsNullOrEmpty(cellValue))
                    return false;
            }
            return true;
        }

        private string GetCellValue(ExcelRange Range, int Row, int Column)
        {
            var cell = Range[Row, Column];
            return cell.Text;
        }

        private string CellReference(int Row, int Column)
        {
            string[] Columns = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            try
            {
                return Columns[Column - 1] + (Row).ToString();
            }
            catch
            {
                return "Row " + (Row).ToString() + ", Column " + (Column).ToString();
            }
        }

        #region Validation helpers

        /// <summary>
        /// Check if data is empty
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public string Required(string Data)
        {
            if (string.IsNullOrEmpty(Data))
                return "Required field. ";
            return string.Empty;
        }

        /// <summary>
        /// Check if data is too long
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public string MaxLength(string Data, int Length)
        {
            if (Data.Length > Length)
                return $"Maximum {Length} characters. ";
            return string.Empty;
        }

        /// <summary>
        /// Check if data is an integer
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public string Integer(string Data)
        {
            try
            {
                int.Parse(Data);
                return string.Empty;
            }
            catch
            {
                return "Number required. ";
            }
        }

        /// <summary>
        /// Check if data is an float
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public string Float(string Data)
        {
            try
            {
                float.Parse(Data);
                return string.Empty;
            }
            catch
            {
                return "Number required. ";
            }
        }

        /// <summary>
        /// Check if data is an Decimal
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public string Decimal(string Data)
        {
            try
            {
                decimal.Parse(Data);
                return string.Empty;
            }
            catch
            {
                return "Number required. ";
            }
        }

        /// <summary>
        /// Check if data is a valid date
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public string Date(string Data)
        {
            try
            {
                DateTime.Parse(Data);
                return string.Empty;
            }
            catch
            {
                return "Number required. ";
            }
        }

        #endregion

    }
}
