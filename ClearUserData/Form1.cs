using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ClearUserData
{
    public partial class Form1 : Form
    {
        static string data_path = @"..\data";
        static string exe_path = @"..\MainModel.exe";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DirectoryInfo data_info = new DirectoryInfo(data_path);
            if (!check_file_avaliable(data_info))
            {
                MessageBox.Show("数据占用中，请先关闭客户端！");
            }
            else if (!delete_files(data_info))
            {
                MessageBox.Show("删除失败，用户目录下可能仍有数据残留", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (MessageBox.Show("数据清除成功！是否重新登录？", "成功", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                {
                    FileInfo info = new FileInfo(exe_path);
                    try
                    {
                        ProcessStartInfo process = new ProcessStartInfo(info.FullName);
                        process.WorkingDirectory = info.DirectoryName;
                        Process.Start(process);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                Application.Exit();
            }
        }

        private bool check_file_avaliable(DirectoryInfo info)
        {
            if (!info.Exists) return true;
            foreach (FileInfo file in info.GetFiles())
            {
                if (!file.Exists) continue;
                FileStream fs = null;
                try
                {
                    fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch
                {
                    if (fs != null) fs.Close();
                    return false;
                }
                finally
                {
                    if (fs != null) fs.Close();
                }
            }
            foreach (DirectoryInfo dir in info.GetDirectories())
            {
                if (!check_file_avaliable(dir)) return false;
            }
            return true;
        }

        private bool delete_files(DirectoryInfo info)
        {
            if (!info.Exists) return true;
            foreach (FileInfo file in info.GetFiles())
            {
                if (!file.Exists) continue;
                try
                {
                    File.Delete(file.FullName);
                }
                catch { return false; }
            }
            foreach (DirectoryInfo dir in info.GetDirectories())
            {
                if (dir.Name.ToLower() == "icon") continue;
                if (!delete_files(dir)) return false;
                if (dir.GetDirectories().Length == 0)
                {
                    try
                    {
                        Directory.Delete(dir.FullName);
                    }
                    catch { return false; }
                }
            }
            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
