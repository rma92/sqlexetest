using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace SqliteExeTest1
{
    public partial class Form1 : Form
    {
        private string _szSqlExe;
        public Form1()
        {
            InitializeComponent();
            //load the config file
            string szJson = File.ReadAllText("config.json");
            JObject joJsonObject = JObject.Parse(szJson);
            _szSqlExe = "spatialite.exe";
            _szSqlExe = joJsonObject["SqlExe"]?.ToString();
        }

        string[] RunSqlString(string szSql = "select sqlite_version();")
        {
            string szOutput = "";
            string szError = "";
            try
            {
                // Path to spatialite.exe and its arguments
                string spatialiteExePath = _szSqlExe;
                //string spatialiteExePath = "sqlite3.exe";
                string arguments = "-echo -csv";

                // Create a process start info
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = spatialiteExePath,
                    Arguments = arguments,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    //CreateNoWindow = true
                    CreateNoWindow = false
                };

                // Create a new process and start it
                Process process = new Process
                {
                    StartInfo = startInfo
                };
                process.Start();
                
                process.StandardInput.Write(szSql);
                process.StandardInput.Close();

                // Read the standard output and standard error
                szOutput = process.StandardOutput.ReadToEnd();
                szError = process.StandardError.ReadToEnd();

                // Wait for the process to exit
                process.WaitForExit();

                // Check for any errors
                if (!string.IsNullOrEmpty(szError))
                {
                    Console.WriteLine("Error: " + szError);
                }
                Console.WriteLine("Output: " + szOutput);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return new string[]{szOutput, szError};
        }

        private void buttonRunSql_Click_1(object sender, EventArgs e)
        {
            string[] aOut = RunSqlString(this.textBoxSql.Text);
            textBox1.Text = aOut[0];
            textBox2.Text = aOut[1];
        }
    }
}