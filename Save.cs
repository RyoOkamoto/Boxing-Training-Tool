#Save.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BoxingSensor
{
    public class Save1
    {


        //CSVファイルにポーズ情報書き込む
        public static void PosSave(double num, double num2, double num3)
        {
            StreamWriter sw;
            FileInfo fi;
            fi = new FileInfo(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\BoxingSensor\/data.csv");
            sw = fi.AppendText();
            sw.WriteLine(num + "," + num2 + "," + num3);
            sw.Flush();
            sw.Close();
        }
