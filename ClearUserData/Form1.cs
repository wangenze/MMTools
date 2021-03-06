﻿using System;
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
            this.Hide();
            main_proc();

            DirectoryInfo data_info = new DirectoryInfo(data_path);
            if (!check_file_avaliable(data_info))
            {
                MessageBox.Show("数据占用中，请先关闭客户端！");
                this.Show();
            }
            else if (!delete_files(data_info))
            {
                MessageBox.Show("删除失败，用户目录下可能仍有数据残留", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Show();
            }
            else
            {
                if (MessageBox.Show("数据清除成功！是否重新登录？", "成功", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                {
                    Login();
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

        static int count = 0;
        void main_proc()
        {
            Process[] main_model_procs = Process.GetProcessesByName("MainModel");
            FileInfo info = new FileInfo(exe_path);
            List<Process> processes = new List<Process>();
            foreach (Process process in main_model_procs)
            {
                if (process.MainModule.FileName == info.FullName)
                {
                    processes.Add(process);
                }
            }
            if (processes.Count > 0)
            {
                CloseProcesses(processes);
                while (WaitForExits(processes, 15000))
                {
                    DialogResult temp = MessageBox.Show("客户端仍在退出中，请选择：\r\n“是”继续等待，“否”强制结束，“取消”取消登陆", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    if (temp == DialogResult.No) CloseProcesses(processes, true);
                    else if (temp == DialogResult.Cancel) return;
                }
            }
        }

        void Login()
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

        void CloseProcesses(List<Process> processes, bool force = false)
        {
            if (processes == null || processes.Count == 0) return;
            foreach (Process process in processes)
            {
                if (process != null && !process.HasExited)
                {
                    if (force) process.Kill();
                    else process.CloseMainWindow();
                }
            }
        }

        bool WaitForExits(List<Process> processes, int milliseconds = int.MaxValue)
        {
            if (processes == null || processes.Count == 0) return true;
            SetCount(processes);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (Process process in processes)
            {
                if (process == null) break;
                if (process.HasExited) processes.Remove(process);
                else
                {
                    long time_left = milliseconds - stopwatch.ElapsedMilliseconds;
                    if (time_left > 0) process.WaitForExit((int)time_left);
                }
            }
            stopwatch.Stop();
            if (count < processes.Count) return false;
            return true;
        }

        void SetCount(List<Process> processes)
        {
            count = 0;
            if (processes == null || processes.Count == 0) return;
            foreach (Process process in processes)
            {
                process.Exited += new EventHandler(OnExit);
            }
        }

        void OnExit(object sender, EventArgs e)
        {
            count++;
        }
    }
}
