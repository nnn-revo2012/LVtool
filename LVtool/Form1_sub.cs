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
using System.Xml.XPath;
using System.Xml.Linq;
using System.Diagnostics;

namespace LVtool
{
    public partial class Form1 : Form
    {

        //ファイルの存在＆コードチェック
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
                }

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
                return -1;
            }
            return result;

        }

        //UTF-8 BOM無で読み書きする
        private bool FileCopy(string SFile, string DFile)
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
                        line = line.Replace("&#x0;", "");
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

        //UTF-8 BOM無で読み書きする
        private bool FileConvert(string SFile, string DFile, bool flag)
        {

            var enc = new System.Text.UTF8Encoding(false);

            try
            {
                if (!File.Exists(SFile))
                {
                    return false;
                }

                var xdoc = XDocument.Load(SFile);
                var plu = xdoc.XPathSelectElements("Setting/Plugins/PluginInfo");
                var fav = xdoc.XPathSelectElements("Setting/Favorites/Favorite");
                var bla = xdoc.XPathSelectElements("Setting/BlackLists/BlackList");
                var vie = xdoc.XPathSelectElements("Setting/ViewLog/PerformerLog");

                var ttt  = "プラグイン: " + plu.Count() + "\r\n";
                    ttt += "お気に入り: " + fav.Count() + "\r\n";
                    ttt += "ブラックリスト: " + bla.Count() + "\r\n";
                    ttt += "ログ: " + vie.Count() + "\r\n";
                Debug.WriteLine(ttt);

                foreach (var elm in plu)
                {
                    foreach (var rpl in ReplaceWords2.ToList())
                    {
                        if (elm.Element("Site").Value == rpl[0])
                        {
                            if (plu.Where(c => (c.Element("Site").Value == rpl[1])).Count() > 0)
                            {
                                Debug.WriteLine(rpl[1] + " が存在するので削除");
                                elm.Remove();
                            }
                            else
                            {
                                Debug.WriteLine(rpl[1] + " はないので変換");
                                elm.Element("Site").SetValue(rpl[1]);
                                elm.Element("Alias").SetValue(rpl[1]);
                            }
                        }
                    }
                }

                foreach (var elm in fav)
                {
                    foreach (var rpl in ReplaceWords2.ToList())
                    {
                        if (elm.Element("Site").Value == rpl[0])
                        {
                            var id = elm.Element("ID").Value;
                            if (fav.Where(c => (c.Element("Site").Value == rpl[1] && c.Element("ID").Value == id)).Count() > 0)
                            {
                                Debug.WriteLine(rpl[1] + ": " + id + " が存在するので削除");
                                elm.Remove();
                            }
                            else
                            {
                                Debug.WriteLine(rpl[1] + ": " + id + " はないので変換");
                                elm.Element("Site").SetValue(rpl[1]);
                            }
                        }
                    }
                }

                foreach (var elm in vie)
                {
                    foreach (var rpl in ReplaceWords2.ToList())
                    {
                        if (elm.Element("Site").Value == rpl[0])
                        {
                            var id = elm.Element("ID").Value;
                            if (vie.Where(c => (c.Element("Site").Value == rpl[1] && c.Element("ID").Value == id)).Count() > 0)
                            {
                                Debug.WriteLine(rpl[1] + ": " + id + " が存在するので削除");
                                elm.Remove();
                            }
                            else
                            {
                                Debug.WriteLine(rpl[1] + ": " + id + " はないので変換");
                                elm.Element("Site").SetValue(rpl[1]);
                            }
                        }
                    }
                }

                xdoc.Save(DFile);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
                return false;
            }
            return true;

        }

        //文字列を変換する
        private string ReplaceExport(string str, int mode)
        {
            var line = str;

            if (mode == 1) //びわっぽい→しんびわ
            {
                foreach (var item in ReplaceWords)
                    line = line.Replace("//(" + item[0] + "):", "//(" + item[1] + "):");
            }
            else if (mode == 2) //しんびわ→びわっぽい
            {
                foreach (var item in ReplaceWords)
                    line = line.Replace("//(" + item[1] + "):", "//(" + item[0] + "):");
            }

            if (mode >= 2) //しんびわ→びわっぽい びわっぽい→びわっぽい
            {
                foreach (var item in ReplaceWords2)
                    line = line.Replace("//(" + item[0] + "):", "//(" + item[1] + "):");
            }

            return line;
        }

        //Shift-JISで読み書きする
        private bool FileCopySJIS(string SFile, string DFile, int mode)
        {

            var enc = System.Text.Encoding.GetEncoding("shift_jis");
            if (mode <= 0 || mode > 3) return false;

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
                        line = ReplaceExport(line, mode);
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

    }
}
