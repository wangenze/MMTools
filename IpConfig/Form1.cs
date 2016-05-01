using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace IpConfig
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private char[] spliter = { ':' };
        private string ini_path = @"..\MainModel.ini";
        private FileInfo ini_info = null;
        private string default_ip = "";
        private string default_port = "";
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        private string read_key(string key, string defaultVal)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString("Settings", key, defaultVal, temp, 255, ini_info.FullName);
            return temp.ToString();
        }

        private void write_key(string key, string str)
        {
            long i = WritePrivateProfileString("Settings", key, str, ini_info.FullName);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string url in Properties.URLs.Default.URLStrings)
            {
                if (string.IsNullOrEmpty(url))
                {
                    continue;
                }
                string[] str = url.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                if (str.Length < 1)
                {
                    continue;
                }
                string ip = str[0].Trim();
                string port = "";
                if (str.Length >= 2)
                {
                    port = str[1].Trim();
                }
                ListViewItem new_item = new ListViewItem(ip);
                new_item.SubItems.Add(port);
                listView1.Items.Add(new_item);
            }
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
            default_ip = read_key("serverUrl", "localhost");
            default_port = read_key("serverPort", "80");
            textBox1.Text = default_ip.Trim();
            textBox2.Text = default_port.Trim();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }
            ListViewItem item = listView1.SelectedItems[0];
            string ip = item.Text;
            string port = "";
            if (item.SubItems.Count > 1)
            {
                port = item.SubItems[1].Text;
            }
            if (string.IsNullOrEmpty(port))
            {
                port = "80";
            }
            textBox1.Text = ip;
            textBox2.Text = port;
            set_config(ip, port);
        }

        private void set_config(string ip, string port)
        {
            write_key("serverUrl", ip);
            write_key("serverPort", port);
            string server = ip;
            if (port != "80")
            {
                server = server + ":" + port;
            }
            write_key("URL", @"http://" + server);
            write_key("wsdlUrl", @"http://" + server + @"/MainModel/services/IDataService");
            write_key("dbDownLoadUrl", @"http://" + server + @"/MainModel/genericFile/downloadFile.mm?nodeId=");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            set_config(default_ip, default_port);
            init();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv.SelectedItems.Count == 0)
            {
                return;
            }
            ListViewItem item = lv.SelectedItems[0];
            string ip = item.Text;
            string port = "";
            if (item.SubItems.Count > 1)
            {
                port = item.SubItems[1].Text;
            }
            if (string.IsNullOrEmpty(port))
            {
                port = "80";
            }
            textBox1.Text = ip;
            textBox2.Text = port;
            set_config(ip, port);
        }
    }
}
