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

using LVtool.Utils;

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
                        //その他として処理する
                        result = 2;
                    }
                }

            }
            catch (Exception Ex)
            {
                return -1;
            }
            return result;

        }

        //設定ファイルの &#x0; を消去 (UTF-8 BOM無)
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

        //設定ファイルのsiteid DMM(a) -> DMMa に変換する (UTF-8)
        private bool FileConvert(string SFile, string DFile, string logfile)
        {

            var enc = new System.Text.UTF8Encoding(false);

            try
            {
                if (!File.Exists(SFile))
                {
                    return false;
                }

                Util.WriteLog(logfile, "***** 変換開始 *****");
                var xdoc = XDocument.Load(SFile);
                var plu = xdoc.XPathSelectElements("Setting/Plugins/PluginInfo");
                var fav = xdoc.XPathSelectElements("Setting/Favorites/Favorite");
                var bla = xdoc.XPathSelectElements("Setting/BlackLists/BlackList");
                var vie = xdoc.XPathSelectElements("Setting/ViewLog/PerformerLog");

                Util.WriteLog(logfile, "プラグイン: " + plu.Count() + "件");
                //foreach (var elm in plu)
                for (int i = plu.Count()-1; i>=0; i--)
                {
                    var elm = plu.ToArray()[i];
                    foreach (var rpl in ReplaceWords2.ToList())
                    {
                        if (elm.Element("Site").Value == rpl[0])
                        {
                            if (plu.Where(c => (c.Element("Site").Value == rpl[1])).Count() > 0)
                            {
                                Util.WriteLog(logfile, rpl[0] + ": " + rpl[1] + "が存在するので削除");
                                elm.Remove();
                            }
                            else
                            {
                                Util.WriteLog(logfile, rpl[0] + ": " + rpl[1] + "に変換");
                                elm.Element("Site").SetValue(rpl[1]);
                                elm.Element("Alias").SetValue(rpl[1]);
                            }
                        }
                    }
                }

                Util.WriteLog(logfile, "お気に入り: " + fav.Count() + "件");
                //foreach (var elm in fav)
                for (int i = fav.Count() - 1; i >= 0; i--)
                {
                    var elm = fav.ToArray()[i];
                    foreach (var rpl in ReplaceWords2.ToList())
                    {
                        if (elm.Element("Site").Value == rpl[0])
                        {
                            var id = elm.Element("ID").Value;
                            if (fav.Where(c => (c.Element("Site").Value == rpl[1] && c.Element("ID").Value == id)).Count() > 0)
                            {
                                Util.WriteLog(logfile, rpl[0] + " " + id + ": " + rpl[1] + " " + id + "が存在するので削除");
                                elm.Remove();
                            }
                            else
                            {
                                Util.WriteLog(logfile, rpl[0] + " " + id + ": " + rpl[1] + " " + id + "に変換");
                                elm.Element("Site").SetValue(rpl[1]);
                            }
                        }
                    }
                }

                Util.WriteLog(logfile, "ブラックリスト: " + bla.Count() + "件");
                //foreach (var elm in bla)
                for (int i = bla.Count() - 1; i >= 0; i--)
                {
                    var elm = bla.ToArray()[i];
                    foreach (var rpl in ReplaceWords2.ToList())
                    {
                        if (elm.Element("Site").Value == rpl[0])
                        {
                            var id = elm.Element("ID").Value;
                            if (bla.Where(c => (c.Element("Site").Value == rpl[1] && c.Element("ID").Value == id)).Count() > 0)
                            {
                                Util.WriteLog(logfile, rpl[0] + " " + id + ": " + rpl[1] + " " + id + "が存在するので削除");
                                elm.Remove();
                            }
                            else
                            {
                                Util.WriteLog(logfile, rpl[0] + " " + id + ": " + rpl[1] + " " + id + "に変換");
                                elm.Element("Site").SetValue(rpl[1]);
                            }
                        }
                    }
                }

                Util.WriteLog(logfile, "履歴: " + vie.Count() + "件");
                //foreach (var elm in vie)
                for (int i = vie.Count() - 1; i >= 0; i--)
                {
                    var elm = vie.ToArray()[i];
                    foreach (var rpl in ReplaceWords2.ToList())
                    {
                        if (elm.Element("Site").Value == rpl[0])
                        {
                            var id = elm.Element("ID").Value;
                            if (vie.Where(c => (c.Element("Site").Value == rpl[1] && c.Element("ID").Value == id)).Count() > 0)
                            {
                                Util.WriteLog(logfile, rpl[0] + " " + id + ": " + rpl[1] + " " + id + "が存在するので削除");
                                elm.Remove();
                            }
                            else
                            {
                                Util.WriteLog(logfile, rpl[0] + " " + id + ": " + rpl[1] + " " + id + "に変換");
                                elm.Element("Site").SetValue(rpl[1]);
                            }
                        }
                    }
                }

                xdoc.Save(DFile);
                Util.WriteLog(logfile, "***** 変換終了 *****");

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
                return false;
            }
            return true;

        }

        //siteid文字列を変換する
        private string ReplaceExport(string str, int mode)
        {
            var line = str;

            try
            {
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
            }
            catch (Exception)
            {
                return str;
            }

            return line;
        }

        //お気に入りインポート／エクスポートファイルのsiteidを変換する (Shift-JIS)
        private bool FileCopySJIS(string SFile, string DFile, int mode)
        {

            var enc = System.Text.Encoding.GetEncoding("shift_jis");

            try
            {
                if (mode <= 0 || mode > 3) return false;
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
