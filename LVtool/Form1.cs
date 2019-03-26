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
            try
            {

                //変換先のファイル名を作成する
                var newfile = Path.Combine(Path.GetDirectoryName(filename), Path.GetRandomFileName());

                //ファイルをコピー
                if (FileCopy(filename, newfile, checkBox1.Checked))
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

                    MessageBox.Show("変換しました。");
                }
                else
                {
                    MessageBox.Show("変換を中止しました。");
                }

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private bool FileCopy(string SFile, string DFile, bool flag)
        {
            var enc = new System.Text.UTF8Encoding(false);

            //一時ファイル番号大きい方から読み込み
            //ファイル書き出す
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
                        line = line.Replace("&#x0;", "");
                        if (flag == true)
                        {
                            line = line.Replace("DMM(a)", "DMMa");
                            line = line.Replace("DMM(o)", "DMMo");
                            line = line.Replace("DMM(m)", "DMMm");
                            line = line.Replace("DMM(w)", "DMMw");

                        }
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
