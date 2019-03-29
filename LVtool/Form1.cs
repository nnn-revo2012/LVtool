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
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            for (int i = 0; i < files.Length; i++)
            {
                work(files[i]);
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

        private void work(string filename)
        {
            var kcode = -1;
            var result = false;

            try
            {

                //変換先のファイル名を作成する
                var newfile = Path.Combine(Path.GetDirectoryName(filename), Path.GetRandomFileName());

                //ファイルをコピー
                kcode = FileCheck(filename);
                if (kcode == -1)
                {
                    MessageBox.Show("変換を中止しました。",
                       "中止",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
                    return;
                }

                //ファイルをコピー
                if (kcode == 2)
                {
                    result = FileCopySJIS(filename, newfile, checkBox1.Checked);
                }
                else
                {
                    result = FileCopy(filename, newfile, checkBox1.Checked);
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
                    var msg = (kcode == 2) ? "シフトJIS" : "UTF-8";
                    msg += "で変換しました。\r\n\r\n"
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

        private int FileCheck(string SFile)
        {
            var enc = new System.Text.UTF8Encoding(false);
            var result = -1;
            DialogResult result2;

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
                        //確認する
                        result2 = MessageBox.Show("お気に入りのインポート／エクスポートファイルですか？",
                        "確認",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                        if (result2 == DialogResult.Yes)
                        {
                            //ShiftJIS
                            result2 = MessageBox.Show("シフトJISで読み込みます。",
                                "情報",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            result = 2;
                        }
                        else if (result2 == DialogResult.No)
                        { // utf-8
                            result2 = MessageBox.Show("UTF-8で読み込みます。",
                                "情報",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            result = 1;
                        }
                        else if (result2 == DialogResult.Cancel)
                        {
                            //中止
                            result = -1;
                        }
                    }
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
