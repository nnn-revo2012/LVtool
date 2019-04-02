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

        //文字列を変換する
        private string ReplaceExport(string str, int mode)
        {
            var line = str;

            if (mode == 1) //びわっぽい→しんびわ
            {
                foreach (var item in ReplaceWords)
                    line = line.Replace(item[0], item[1]);
            }
            else if (mode == 2) //しんびわ→びわっぽい
            {
                foreach (var item in ReplaceWords)
                    line = line.Replace(item[1], item[0]);
            }

            if (mode >= 2) //しんびわ→びわっぽい びわっぽい→びわっぽい
            {
                foreach (var item in ReplaceWords2)
                    line = line.Replace(item[0], item[1]);
            }

            return line;
        }


        //文字列を変換する
        private string ReplaceConfig(string str, bool flag)
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
                        line = ReplaceConfig(line, flag);
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
