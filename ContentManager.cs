＃ContentManager.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace BoxingSensor
{
    class ContentManager
    {

        public static void ImageManager(ref BitmapImage[] Bimage ,BitmapImage[] Simage)
        {
            //画像保存する配列
            BitmapImage[] bmpImage = new BitmapImage[10];
            BitmapImage[] subImage = new BitmapImage[10];

            //ファイルパスを指定するもの
            bmpImage[0] = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\title.png"));//タイトル
            bmpImage[1] = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\explain1.png"));//説明（概要）
            bmpImage[2] = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\explain2.png"));//説明（ルール）

            bmpImage[3] = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\tutorial.png"));//チュートリアル
            bmpImage[4] = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\sets.png"));//構え
            bmpImage[5] = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\result.png"));//結果
            bmpImage[6] = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\UnderLine.png"));//UnderLine
            bmpImage[7] = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\shoot.png"));//放て
            bmpImage[8] = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\punchimage.png"));

            subImage[0] = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント0.png"));
            subImage[1] = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント1.png"));
            subImage[2] = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント2.png"));
            subImage[3] = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント3.png"));
            //ここに追加6/16
            //String rootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //String Resouce="\\Visual Studio 2013\\Projects\\BoxingSensor\\";

            
            Bimage[0] = bmpImage[0];
            Bimage[1] = bmpImage[1];
            Bimage[2] = bmpImage[2];
            Bimage[3] = bmpImage[3];
            Bimage[4] = bmpImage[4];
            Bimage[5] = bmpImage[5];
            Bimage[6] = bmpImage[6];
            Bimage[7] = bmpImage[7];
            Bimage[8] = bmpImage[8];

            Simage[0] = subImage[0];
            Simage[1] = subImage[1];
            Simage[2] = subImage[2];
            Simage[3] = subImage[3];

            //Bimage[1] = new BitmapImage(new Uri(rootPath + Resouce + "DSC_0296.JPG"));//タイトル
            //Bimage[2] = new BitmapImage(new Uri(rootPath + Resouce + "DSC_0293.JPG"));//概要説明
            //Bimage[3] = new BitmapImage(new Uri(rootPath + Resouce + "DSC_0316.JPG"));//ルール説明
            //Bimage[4] = new BitmapImage(new Uri(rootPath + Resouce + "DSC_0299.JPG"));//チュートリアル
            //Bimage[5] = new BitmapImage(new Uri(rootPath + Resouce + "DSC_0302.JPG"));//構えをする
            //Bimage[6] = new BitmapImage(new Uri(rootPath + Resouce + "DSC_0305.JPG"));//結果画面




        }
       
        public static void BGMmanager(ref SoundPlayer[] bgm)
        {
            //BGMを管理する配列
            SoundPlayer[] BGM = new SoundPlayer[3];

            //ファイルパスを指定するもの
            BGM[0] = new SoundPlayer(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Music\game_image8.wav");
            BGM[1] = new SoundPlayer(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Music\No_Escape.wav");

            
            bgm[0] = BGM[0];
            bgm[1] = BGM[1];
        }

        public static void SEManager(ref Uri[] se)
        {
            //SEを管理する配列
            Uri[] SE = new Uri[6];

            //ファイルパスを指定するもの
            SE[0] = new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Music\パンチ小.wav");
            SE[1] = new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Music\パンチ中.wav");
            SE[2] = new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Music\パンチ大.wav");

            SE[3] = new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Music\Countdown01-2.wav");
            SE[4] = new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Music\setup1.wav");
            SE[5] = new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Music\camera1.wav");


            //ここに追加

            se[0] = SE[0];
            se[1] = SE[1];
            se[2] = SE[2];
            se[3] = SE[3];
            se[4] = SE[4];
            se[5] = SE[5];
        }









    }
}

