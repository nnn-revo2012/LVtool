using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Media;

namespace LVtool.Utils
{
    public class Util
    {

        //アプリケーションの場所をGet
        public static string GetApplicationDirectory()
        {
            //アプリケーションの場所
            var tmp = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(tmp);
        }

        //ファイル名の後ろに日付時間を付加する
        public static string GetLogfile(string dir, string filename)
        {
            var tmp = Path.GetFileNameWithoutExtension(filename) 
                + "_" + System.DateTime.Now.ToString("yyMMdd_HHmmss")
                + Path.GetExtension(filename);
            return Path.Combine(dir, tmp);
        }

        //ログ出力
        public static void WriteLog(string logfile, string mes)
        {
            System.IO.File.AppendAllText(logfile, mes + "\r\n");
        }

    }
}
