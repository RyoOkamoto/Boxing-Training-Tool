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

        public static void ImageManager()
        {
            //画像保存する配列
            BitmapImage[] bmpImage = new BitmapImage[10];
            BitmapImage[] subImage = new BitmapImage[10];

            //ファイルパスを指定するもの
            bmpImage[0] = new BitmapImage(new Uri(@""));
            //ここに追加

        }
       
        public static void BGMmanager(int num)
        {
            //BGMを管理する配列
            SoundPlayer[] BGM = new SoundPlayer[10];

            //ファイルパスを指定するもの
            BGM[0] = new SoundPlayer("");
            //ここに追加


        }

        public static void SEManager(int num)
        {
            //SEを管理する配列
            SoundPlayer[] SE = new SoundPlayer[10];

            //ファイルパスを指定するもの
            SE[0] = new SoundPlayer();
            //ここに追加



        }

