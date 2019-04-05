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

        private int ConvertMode = 1;
        // 1:お気に入り変換/設定ファイル修正 2:設定ファイル(DMMサイトID)修正

        private readonly string[][] ReplaceWords =
            {
            new[] {"AngelLive", "angel-live"},
            new[] {"ChatPia", "chatpia"},
            new[] {"DXlive", "dxlive"},
            new[] {"caribbeancomgirl", "caribbeancomgirl(DX)"},
            new[] {"KanjukuLive", "kanjukulive"},
            new[] {"DMM(w)", "dmm(w)"},
            new[] {"DMM(m)", "dmm(m)"},
            new[] {"DMM(o)", "dmm(o)"},
            new[] {"DMM(a)", "dmm"},
            new[] {"DMMw", "dmm(w)"},
            new[] {"DMMm", "dmm(m)"},
            new[] {"DMMo", "dmm(o)"},
            new[] {"DMMa", "dmm"},
            new[] {"Jewel", "j-live"},
            new[] {  "Madamu", "madamu"}
            //new[] {"", ""}
            };

        private readonly string[][] ReplaceWords2 =
            {
            new[] {"DMM(w)", "DMMw"},
            new[] {"DMM(m)", "DMMm"},
            new[] {"DMM(o)", "DMMo"},
            new[] {"DMM(a)", "DMMa"}
            };


        public Form1()
        {
            InitializeComponent();

        }

        //最初
        private void work(string filename)
        {
            var kcode = -1;
            try
            {

                kcode = FileCheck(filename);
                if (ConvertMode == 2)
                {
                    if (kcode == 1)
                    {
                        // DMM(a) -> DMMa 変換処理
                        ConvertSetting(filename);
                    }
                    else
                    {
                        MessageBox.Show("設定ファイルのみ変換します。\r\n変換を中止しました。",
                           "中止",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Error);
                    }
                }
                else if (ConvertMode == 1)
                {
                    if (kcode == 1)
                    {
                        // 設定ファイル修正
                        ReplaceSetting(filename);
                    }
                    else
                    {
                        // お気に入り変換
                        ReplaceFavorite(filename);
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        //設定ファイル修正(&#x0;のみ修正)
        private void ReplaceSetting(string filename)
        {
            var result = false;

            //修正先のファイル名を作成する
            var newfile = Path.Combine(Path.GetDirectoryName(filename), Path.GetRandomFileName());

            try
            {
                //ファイルをコピー
                result = FileCopy(filename, newfile);
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
        private void ReplaceFavorite(string filename)
        {
            var result = false;
            var mode = -1;

            var newfile = Path.Combine(Path.GetDirectoryName(filename), Path.GetRandomFileName());

            try
            {
                //変換するファイルの種類を確認する
                using (var fo2 = new Form2())
                {
                    fo2.ShowDialog();
                    if (fo2.DialogResult == DialogResult.OK)
                    {
                        mode = fo2.select_mode;
                        result = FileCopySJIS(filename, newfile, mode);
                    }
                    else if (fo2.DialogResult == DialogResult.Cancel)
                    {
                        result = false;
                    }
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

        //2:設定ファイル(DMMサイトID)修正
        private void ConvertSetting(string filename)
        {
            var result = false;

            //修正先のファイル名を作成する
            var newfile = Path.Combine(Path.GetDirectoryName(filename), Path.GetRandomFileName());

            try
            {
                result = FileConvert(filename, newfile, checkBox1.Checked);
                result = false;
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

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            try
            {
                for (int i = 0; i < files.Length; i++)
                {
                        work(files[i]); 
                }
            }
            catch (Exception)
            {
                MessageBox.Show("変換を中止しました。",
                   "中止",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
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

        private void お気に入りインポート修正ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConvertMode = 2; //2:設定ファイル(DMMサイトID)修正

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
