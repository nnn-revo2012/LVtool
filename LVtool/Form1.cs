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

        private int ConvertMode = 1; // 1:お気に入り変換 2:設定ファイル修正

        private readonly string[][] ReplaceWords =
            {
            new[] { "//("+ "AngelLive" +")", "//("+"angel-live"+")" },
            new[] { "//("+ "ChatPia" +")", "//("+"chatpia"+")" },
            new[] { "//("+ "DXlive" +")", "//("+ "dxlive" +")" },
            new[] { "//("+ "caribbeancomgirl" +")", "//("+ "caribbeancomgirl(DX)" +")" },
            new[] { "//("+ "KanjukuLive" + ")", "//("+ "kanjukulive" +")" },
            new[] { "//("+ "DMM(w)" +")", "//("+ "dmm(w)" +")" },
            new[] { "//("+ "DMM(m)" +")", "//("+ "dmm(m)" +")" },
            new[] { "//("+ "DMM(o)" +")", "//("+ "dmm(o)" +")" },
            new[] { "//("+ "DMM(a)" +")", "//("+ "dmm" +")" },
            new[] { "//("+ "DMMw" +")", "//("+ "dmm(w)" +")" },
            new[] { "//("+ "DMMm" +")", "//("+ "dmm(m)" +")" },
            new[] { "//("+ "DMMo" +")", "//("+ "dmm(o)" +")" },
            new[] { "//("+ "DMMa" +")", "//("+ "dmm" +")" },
            new[] { "//("+ "Jewel" +")", "//("+ "j-live" +")" },
            new[] { "//("+ "Madamu" +")", "//("+ "madamu" +")" }
            //new[] { "//("+ "" +")", "//("+ "" +")" }
            };

        private readonly string[][] ReplaceWords2 =
            {
            new[] { "//("+ "DMM(w)" +")", "//("+ "DMMw" +")" },
            new[] { "//("+ "DMM(m)" +")", "//("+ "DMMm" +")" },
            new[] { "//("+ "DMM(o)" +")", "//("+ "DMMo" +")" },
            new[] { "//("+ "DMM(a)" +")", "//("+ "DMMa" +")" }
            };

        public Form1()
        {
            InitializeComponent();

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
            var mode = -1;

            var newfile = Path.Combine(Path.GetDirectoryName(filename), Path.GetRandomFileName());

            try
            {
                var ret = FileCheck(filename);
                if (ret == 1)
                {
                    //設定ファイル修正へ
                    checkBox1.Checked = false;
                    work(filename);
                    return;
                }
                if (ret != -1)
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
