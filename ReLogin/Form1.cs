using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ReLogin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            main_proc();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        static string exe_path = @"..\MainModel.exe";
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
                    else if (temp == DialogResult.Cancel) Application.Exit();
                }
            }
            Login();
            Application.Exit();
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
