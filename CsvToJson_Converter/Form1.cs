using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;

namespace CsvToJson_Converter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_convert_Click(object sender, EventArgs e)
        {
            if (txt_in.Text != "")
                ConverCsvToJson();
        }

        /// <summary>
        /// Main function of program
        /// </summary>
        private void ConverCsvToJson()
        {
            string line = "";
            string jsonLine = "";

            string lBracket = "[";
            string rBracket = "]";
            string lineBreak = "\r\n";

            string[] columns;

            List<String> csvList = new List<string>();
            List<String> jsonList = new List<string>();

            // Text area to List
            txt_in.Text = txt_in.Text.Replace(";", ",");
            csvList = txt_in.Lines.ToList();

            // Extract columns
            columns = SplitByComma(csvList[0]);

            // Add first caracter of Json.
            jsonList.Add(lBracket + lineBreak);

            // Go over list of lines
            for (int i = 1; i < csvList.Count; i++)
            {

                // Extract line
                line = csvList[i].ToString();

                // Convert line to Json line
                jsonLine = ConverToJsonLine(columns, line);

                if (i != csvList.Count - 1)
                {
                    // Take off last comma
                    jsonLine = jsonLine + "," + lineBreak;
                }

                // Add Json Line to Json List
                jsonList.Add(jsonLine);
            }

            // Add last caracter of Json.
            jsonList.Add(lineBreak + rBracket);

            // Json List to text area
            txt_out.Text = "";
            foreach (string elem in jsonList)
            {
                txt_out.Text += elem;
            }
        }

        /// <summary>
        /// Function to conver a csv line onto a Json object (an string with curly brackets and spaces or break lines)
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private string ConverToJsonLine(string[] columns, string line)
        {
            string[] jsonValues;
            String jsonObject = "";

            if (HasQuotes(line))
            {
                jsonValues = ExtractValuesWithQuotes(line);
            }
            else
            {
                jsonValues = ExtractValuesWithoutQuotes(line);
            }

            jsonObject = ValuesToJsonObject(columns, jsonValues);

            return jsonObject;
        }

        /// <summary>
        /// Funtion to extract values when line has not a composed value with commas
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string[] ExtractValuesWithoutQuotes(string line)
        {
            return SplitByComma(line);
        }

        /// <summary>
        /// Funtion to extract values when line has a composed value with commas
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string[] ExtractValuesWithQuotes(string line)
        {
            string auxLine = line;
            string[] auxArray;
            ArrayList newArray = new ArrayList(); 
            Queue queueLines = new Queue();

            int i = 0;
            foreach (Match match in Regex.Matches(line, "\"([^\"]*)\""))
            {
                auxLine = auxLine.Replace(match.ToString(), "Cell" + i);
                queueLines.Enqueue(match.ToString());
                i++;
            }

            auxArray = SplitByComma(auxLine.ToString());

            int x = 0;
            foreach(var elem in auxArray)
            {
                if (elem.Contains("Cell")) {
                    newArray.Add(queueLines.Dequeue().ToString().Replace("\"", ""));
                } else {
                    newArray.Add(elem.ToString());
                }

            }

            return (String[]) newArray.ToArray(typeof(string));
        }

        /// <summary>
        /// Function to set up each json object between curly brackets 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private string ValuesToJsonObject(string[] columns, string[] values)
        {
            string jsonProp = "";
            string lCurly = "    {";
            string rCurly = "    }";
            string lineBreak = "\r\n";

            jsonProp = lCurly + lineBreak;

            for (int i = 0; i < columns.Length; i++)
            {
                jsonProp = jsonProp + ConcatColumnAndValue(columns[i].ToString(), values[i].ToString());

                if (i == columns.Length - 1)
                {
                    jsonProp = TakeOffLastComma(jsonProp);
                }

                jsonProp = jsonProp + lineBreak;
            }

            jsonProp = TakeOffLastComma(jsonProp);


            jsonProp = jsonProp + lineBreak + rCurly;

            return jsonProp;
        }

        private string ConcatColumnAndValue(string col, string val)
        {
            if (IsNumeric(val))
                return "        \"" + col.Trim() + "\"" + ": " + val + ",";
                else
                return "        \"" + col.Trim() + "\"" + ": " + "\"" + val + "\",";
        }

        /// <summary>
        /// Function to know if line has va composed value with commas. 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool HasQuotes(string line)
        {
            return line.Contains('"');
        }

        /// <summary>
        /// Funtión to take off the last comma in a line (when is final of jason object)
        /// </summary>
        /// <param name="jsonLine"></param>
        /// <returns></returns>
        private string TakeOffLastComma(string jsonLine)
        {
            return jsonLine.Substring(0, jsonLine.Length - 1);
        }

        /// <summary>
        /// Function to split values from a line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string[] SplitByComma(string line)
        {
            return line.Split(',');
        }
     
        /// <summary>
        /// Procedure to clear text areas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Clear_Click(object sender, EventArgs e)
        {
            txt_in.Text = "";
            txt_out.Text = "";
        }

        /// <summary>
        /// Functio to get if string could be a number
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsNumeric(string value)
        {
            bool result = int.TryParse(value, out int i) || double.TryParse(value, out double x);
            return result;
        }
    }
}
