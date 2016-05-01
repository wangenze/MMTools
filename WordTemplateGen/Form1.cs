using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Word = Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WordTemplateGen
{
    public partial class Form1 : Form
    {
        //[StructLayout(LayoutKind.Sequential)]
        //public struct COPYDATASTRUCT
        //{
        //    public IntPtr dwData;
        //    public int cbData;
        //    [MarshalAs(UnmanagedType.LPStr)]
        //    public string lpData;
        //}

        //[DllImport("User32.dll")]
        //public static extern int SendMessage(IntPtr hwnd, int msg, int wParam, ref COPYDATASTRUCT IParam);
        //[DllImport("User32.dll")]
        //public static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int nMaxCount);

        //const int WM_COPYDATA = 0x004A;

        public Form1()
        {
            InitializeComponent();
        }

        //IntPtr wordWindow = IntPtr.Zero;

        Word.Application oWord = null;
        Word.Document oDoc = null;

        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    open_word();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void 关闭CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                close_Word();
            }
            catch (Exception /*ex*/)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (oDoc != null)
            {
                oWord.Selection.TypeText(textBox1.Text);
            }
        }


        private void open_word()
        {
            if (oWord == null)
            {
                oWord = new Word.Application();
                oWord.Visible = true;
            }
            if (oDoc != null)
            {
                oDoc.Close(false);
            }
            oDoc = oWord.Documents.Open(openFileDialog1.FileName);
        }

        private void close_Word()
        {
            try
            {
                if (oDoc != null)
                {
                    oDoc.Close(false);
                    Marshal.ReleaseComObject(oDoc);
                    oDoc = null;
                }
                if (oWord != null)
                {
                    oWord.Quit(false);
                    Marshal.ReleaseComObject(oWord);
                    oWord = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

        private void 加载属性列表LToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void load_json(string filename)
        {

        }
    }
}
