using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LVtool
{
    public partial class Form1 : Form
    {

        int ConvertMode = 1; // 1:お気に入り変換 2:設定ファイル修正

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            for (int i = 0; i < files.Length; i++)
            {
                if (ConvertMode == 2)
                    work(files[i]); //設定ファイル修正
                else
                    work2(files[i]); //お気に入り変換
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        //設定ファイル修正
        private void work(string filename)
        {
            var result = false;

            //修正先のファイル名を作成する
            var newfile = Path.Combine(Path.GetDirectoryName(filename), Path.GetRandomFileName());

            try
            {
                if (FileCheck(filename) == 1)
                {
                    //ファイルをコピー
                    result = FileCopy(filename, newfile, checkBox1.Checked);
                }
                else
                {
                    MessageBox.Show("設定ファイルのみ変換します。\r\n変換を中止しました。",
                       "中止",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
                    result = false;
                }

                if (result == true)
                {
                    //元ファイルをリネーム
                    var renamefile = filename + ".bak";
                    if (!File.Exists(renamefile))
                    {
                        File.Move(filename, renamefile);

                    }

                    //ファイルを元ファイルにリネーム
                    if (!File.Exists(filename))
                    {
                        File.Move(newfile, filename);

                    }

                    var msg = "設定ファイルを変換しました。\r\n\r\n"
                        + "変換したファイルが文字化けしていたら、\r\n"
                        + renamefile + " を元のファイルにリネームしてください。";      
                    MessageBox.Show(msg,
                        "変換終了",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                }
                else
                {
                    MessageBox.Show("変換を中止しました。",
                       "中止",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
                }

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        //お気に入り変換
        private void work2(string filename)
        {
            var result = false;
            DialogResult result2;

            var newfile = Path.Combine(Path.GetDirectoryName(filename), Path.GetRandomFileName());

            try
            {
                var ret = FileCheck(filename);
                if (ret == 1)
                {
                    checkBox1.Checked = false;
                    work(filename);
                    return;
                }
                if (ret != -1)
                {
                    //確認する
                    result2 = MessageBox.Show("お気に入りのインポート／エクスポートファイルですか？",
                        "確認",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                    if (result2 == DialogResult.Yes)
                    {
                        result = FileCopySJIS(filename, newfile, false);
                    }
                    else if (result2 == DialogResult.No)
                    {
                        //中止
                        result = false;
                    }
                }
                else
                {
                    MessageBox.Show("変換を中止しました。",
                           "中止",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Error);
                    result = false;
                }

                if (result == true)
                {
                    //新しいファイルに日付を加える
                    var renamefile = newfile + "_";
                    if (!File.Exists(renamefile))
                    {
                        File.Move(newfile, renamefile);

                    }
                    var msg = "お気に入りファイルを変換しました。";
                    MessageBox.Show(msg,
                        "変換終了",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                }
                else
                {
                    MessageBox.Show("変換を中止しました。",
                       "中止",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
                }

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
        private int FileCheck(string SFile)
        {
            var enc = new System.Text.UTF8Encoding(false);
            var result = -1;

            try
            {
                if (!File.Exists(SFile))
                {
                    return result;
                }
                using (var sr = new StreamReader(SFile, enc))
                {
                    string line = sr.ReadLine();
                    result = line.IndexOf("utf-8");
                    if (result > -1)
                    {
                        //UTF-8 として処理する
                        result = 1;
                    }
                    else
                    {
                        result = 2;
                    }
                    /*
                  */
                }

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
                return -1;
            }
            return result;

        }

        //文字列を変換する
        private string ReplaceWords(string str, bool flag)
        {
            var line = str.Replace("&#x0;", "");

            if (flag == true)
            {
                line = line.Replace("DMM(a)", "DMMa");
                line = line.Replace("DMM(o)", "DMMo");
                line = line.Replace("DMM(m)", "DMMm");
                line = line.Replace("DMM(w)", "DMMw");
            }

            return line;
        }

        //UTF-8 BOM無で読み書きする
        private bool FileCopy(string SFile, string DFile, bool flag)
        {

            var enc = new System.Text.UTF8Encoding(false);

            try
            {
                if (!File.Exists(SFile))
                {
                    return false;
                }
                using (var sr = new StreamReader(SFile, enc))
                using (var sw = new StreamWriter(DFile, true, enc))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null) // 1行ずつ読み出し。
                    {
                        line = ReplaceWords(line, flag);
                        sw.WriteLine(line);
                    }
                }

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
                return false;
            }
            return true;

        }

        //Shift-JISで読み書きする
        private bool FileCopySJIS(string SFile, string DFile, bool flag)
        {

            var enc = System.Text.Encoding.GetEncoding("shift_jis");

            try
            {
                if (!File.Exists(SFile))
                {
                    return false;
                }
                using (var sr = new StreamReader(SFile, enc))
                using (var sw = new StreamWriter(DFile, true, enc))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null) // 1行ずつ読み出し。
                    {
                        line = ReplaceWords(line, flag);
                        sw.WriteLine(line);
                    }
                }

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
                return false;
            }
            return true;

        }

        private void お気に入りインポート修正ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConvertMode = 2; // 1:お気に入り変換 2:設定ファイル修正
            this.textBox2.Visible = true;
            this.groupBox1.Visible = true;
            this.checkBox1.Visible = true;

        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
