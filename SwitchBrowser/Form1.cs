using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SwitchBrowser
{
    public partial class Form1 : Form
    {
        private string ini_path = @"..\MainModel.ini";
        private FileInfo ini_info = null;

        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        private string read_key()
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString("Settings", "BrowserSelect", "OS", temp, 255, ini_info.FullName);
            return temp.ToString();
        }

        private void write_key(string str)
        {
            long i = WritePrivateProfileString("Settings", "BrowserSelect", str, ini_info.FullName);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ini_info = new FileInfo(ini_path);
            if (!File.Exists(ini_info.FullName))
            {
                MessageBox.Show("未找到配置文件！");
                Application.Exit();
            }
            try
            {
                init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        private void init()
        {
            string temp = read_key();
            if (temp == "Application")
            {
                radioButton1.Checked = true;
            }
            else if (temp == "OS")
            {
                radioButton2.Checked = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (radioButton1.Checked)
                {
                    write_key("Application");
                }
                else if (radioButton2.Checked)
                {
                    write_key("OS");
                }
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
