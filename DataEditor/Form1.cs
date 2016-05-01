using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DataEditor
{
    public partial class Form1 : Form
    {
        DataTable dt = new DataTable();
        JArray ja = null;
        string file_name = "";
        public Form1()
        {
            InitializeComponent();
            dataGridView1.DataSource = dt;
            dt.Columns.Add("Name");
            dt.Columns.Add("Value");
            dt.Columns.Add("Remark");
            
            foreach (DataGridViewColumn dgvc in dataGridView1.Columns)
            {
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dataGridView1.Columns[0].ReadOnly = true;
            foreach (DataGridViewColumn dgvc in dataGridView1.Columns)
            {
                dgvc.MinimumWidth = 100;
                dgvc.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (e.Control && e.KeyCode == Keys.V)
            {
                dgvPaste(ref dt, ref dgv);
            }
            //else if (e.Control && e.KeyCode == Keys.X)
            //{
            //    dgvCut(ref dgv);
            //}
            else if (e.Control && e.KeyCode == Keys.C)
            {
                dgvCopy(ref dgv);
            }
            //else if (e.KeyCode == Keys.Delete)
            //{
            //    dgvDel(ref dgv);
            //}
        }

        //private void Paste(DataGridView dgv)
        //{
        //    string text = Clipboard.GetText();
        //    if (string.IsNullOrEmpty(text))
        //    {
        //        return;
        //    }
        //    string[] lines = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        //    foreach (string line in lines)
        //    {
        //        string[] strs = line.Split(new char[] { '\t' });
        //        dgv.Rows.Add(strs);
        //    }
        //}



        /// <summary>
        /// DataGridView复制
        /// </summary>
        /// <param name="dgv">DataGridView实例</param>
        private void dgvCopy(ref DataGridView dgv)
        {
            if (dgv.GetCellCount(DataGridViewElementStates.Selected) > 0)
            {
                try
                {
                    Clipboard.SetDataObject(dgv.GetClipboardContent());
                }
                catch (Exception MyEx)
                {
                    MessageBox.Show(MyEx.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// DataGridView剪切
        /// </summary>
        /// <param name="dgv">DataGridView实例</param>
        private void dgvCut(ref DataGridView dgv)
        {
            dgvCopy(ref dgv);
            try
            {
                int k = dgv.SelectedRows.Count;
                if (k == dgv.Rows.Count)
                    k--;
                for (int i = k; i >= 1; i--)
                {
                    dgv.Rows.RemoveAt(dgv.SelectedRows[i - 1].Index);
                }
            }
            catch (Exception MyEx)
            {
                MessageBox.Show(MyEx.Message);
            }

        }


        /// <summary>
        /// DataGridView删除
        /// </summary>
        /// <param name="dgv">DataGridView实例</param>
        private void dgvDel(ref DataGridView dgv)
        {
            try
            {
                int k = dgv.SelectedRows.Count;

                if (MessageBox.Show("确实要删除这" + Convert.ToString(k) + "项吗？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                { }
                else
                {
                    if (k == dgv.Rows.Count)
                        k--;
                    for (int i = k; i >= 1; i--)
                    {
                        dgv.Rows.RemoveAt(dgv.SelectedRows[i - 1].Index);
                    }
                }
            }
            catch (Exception MyEx)
            {
                MessageBox.Show(MyEx.Message);
            }

        }

        /// <summary>
        /// DataGridView粘贴
        /// </summary>
        /// <param name="dt">DataGridView数据源</param>
        /// <param name="dgv">DataGridView实例</param>
        public void dgvPaste(ref DataTable dt, ref DataGridView dgv)
        {
            try
            {
                int cRowIndex = dgv.CurrentCell.RowIndex;
                int cColIndex = dgv.CurrentCell.ColumnIndex;
                //最后一行为新行
                int rowCount = dgv.Rows.Count;
                int colCount = dgv.ColumnCount;
                //获取剪贴板内容
                string pasteText = Clipboard.GetText();
                //判断是否有字符存在
                if (string.IsNullOrEmpty(pasteText))
                    return;
                //以换行符分割的数组
                string[] lines = pasteText.Trim().Split('\n');
                int txtLength = lines.Length;
                DataRow row;
                //判断是修改还是添加,如果dgv中行数减当前行号大于要粘贴的行数，直接修改
                if (rowCount - cRowIndex > txtLength)
                {
                    for (int j = cRowIndex; j < cRowIndex + txtLength; j++)
                    {
                        //以制表符分割的数组
                        string[] vals = lines[j - cRowIndex].Split('\t');
                        //判断要粘贴的列数与dgv中列数减当前列号的大小，取最小值
                        int minColLength = vals.Length > colCount - cColIndex ? colCount - cColIndex : vals.Length;
                        row = dt.Rows[j];
                        for (int i = 0; i < minColLength; i++)
                        {
                            if (dgv.Columns[i + cColIndex].ReadOnly)
                            {
                                continue;
                            }
                            row[i + cColIndex] = vals[i];
                        }
                    }
                }
                //否则先修改后添加
                else
                {
                    //修改
                    for (int j = cRowIndex; j < rowCount; j++)
                    {
                        string[] vals = lines[j - cRowIndex].Split('\t');
                        int minColLength = vals.Length > colCount - cColIndex ? colCount - cColIndex : vals.Length;
                        row = dt.Rows[j];
                        for (int i = 0; i < minColLength; i++)
                        {
                            if (dgv.Columns[i + cColIndex].ReadOnly)
                            {
                                continue;
                            }
                            row[i + cColIndex] = vals[i];
                        }
                    }
                    //添加
                    //for (int j = rowCount; j < cRowIndex + txtLength; j++)
                    //{
                    //    string[] vals = lines[j - cRowIndex].Split('\t');
                    //    int minColLength = vals.Length > colCount - cColIndex ? colCount - cColIndex : vals.Length;
                    //    //新行
                    //    row = dt.NewRow();
                    //    for (int i = 0; i < minColLength; i++)
                    //    {
                    //        row[i + cColIndex] = vals[i];
                    //    }
                    //    //添加到dgv数据源中
                    //    dt.Rows.Add(row);
                    //}
                    if (cRowIndex == rowCount)
                        dgv.Rows.RemoveAt(dt.Rows.Count);
                }
            }
            catch (Exception MyEx)
            {
                MessageBox.Show(MyEx.Message);
            }
        }

        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                file_name = openFileDialog1.FileName;
                FileInfo file_info = new FileInfo(file_name);
                toolStripStatusLabel1.Text = file_info.FullName;
                dt.Clear();
                LoadFile();
            }
        }

        private void LoadFile()
        {
            string json_file = File.ReadAllText(file_name);
            if (ja != null)
            {
                ja.Clear();
            }
            ja = JsonConvert.DeserializeObject<JArray>(json_file);
            foreach (JToken jt in ja)
            {
                dt.Rows.Add(jt["name"], jt["value"], jt["remark"]);
            }
        }

        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(file_name))
            {
                MessageBox.Show("请先打开数据文件!");
                return;
            }
            else
            {
                try
                {
                    SaveFile();
                    MessageBox.Show("保存完成");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SaveFile()
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string value = dt.Rows[i][1].ToString();
                if (ja[i]["value"] != null)
                {
                    ja[i]["value"] = value;
                }
                string remark = dt.Rows[i][2].ToString();
                if (ja[i]["remark"] != null)
                {
                    ja[i]["remark"] = remark;
                }
            }
            string json_file = JsonConvert.SerializeObject(ja);
            File.WriteAllText(file_name, json_file);
        }

        private void 另存为AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(file_name))
            {
                MessageBox.Show("请先打开数据文件!");
                return;
            }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                file_name = saveFileDialog1.FileName;
                FileInfo file_info = new FileInfo(file_name);
                toolStripStatusLabel1.Text = file_info.FullName;
                SaveFile();
                MessageBox.Show("保存完成");
            }
        }

        private void 关闭CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            file_name = "";
            toolStripStatusLabel1.Text = "";
            dt.Clear();
            if (ja != null)
            {
                ja.Clear();
            }
        }
    }
}
