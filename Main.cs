#Main.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Diagnostics;
using System.Media;

namespace BoxingSensor
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly int Bgr32BytesPerPixel = PixelFormats.Bgr32.BitsPerPixel / 8;
        int i = 1;
        int fase = 0;
        int MostSp;
        
        //閾値
        float level = 0.8f;
        //評価
        int score1 = 0;//良い構え
        int score2 = 0;//伸びが良い
        double speed;
        double speed2;
        double speed3;

        double len;
        double len2;
        double len3;
        //構え時の距離
        double startlen;
        double startlen2;
        double startlen3;
        //ストレート時の距離
        double KeepLen = 0;
        double KeepLen2= 0;
        double KeepLen3= 0;
        //構えから拳を放つ時の距離
        double alldistance;
        double alldistance2;
        double alldistance3;

        bool flag = false;

        Joint keepWristR;
        Joint keepShoulderR;
        Joint startWristR;
        Joint startShoulderR;
        Joint startWristL;
        Joint startShoulderL;
        Joint startElbowR;
        Joint startElbowL;

        double[]Px;
        double[]Py;
        double[]Pz;
       /* double[] Px2;
        double[] Py2;
        double[] Pz2;*/


        String Sex = null;
        double height;

        Stopwatch stopwatch = new Stopwatch();
        Stopwatch Keeptime = new Stopwatch();
        double timeGet;
        double timeGet2;
        double timeGet3;
        //SubWindow sw = new SubWindow();

        // 読み込んだ画像を保存するための変数
        BitmapImage bmpImage;
        BitmapImage bmpImage2;
        // ファイルを開くダイアログ
        Microsoft.Win32.OpenFileDialog ofDialog =
            new Microsoft.Win32.OpenFileDialog();
       
       

        public MainWindow()
        {
            
            try
            {
                InitializeComponent();

                // Kinectが接続されているかどうか
                if (KinectSensor.KinectSensors.Count == 0)
                {
                    throw new Exception("接続してください");
                }
                StartKinect(KinectSensor.KinectSensors[0]);
                //sw.Show();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        

       
        //Kinectをスタートさせる
        private void StartKinect(KinectSensor kinect)
        {
            
            kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            kinect.SkeletonStream.Enable();

            kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinect_AllFramesReady);
            
            kinect.Start();
            
            Mode.Items.Clear();
            foreach (var range in Enum.GetValues(typeof(DepthRange)))
            {
                Mode.Items.Add(range.ToString());
            }
            Mode.SelectedIndex = 1;

        }


        //Kinectをストップさせる
        private void StopKinect(KinectSensor kinect)
        {
            if (kinect != null)
            {
                if (kinect.IsRunning)
                {
                    kinect.AllFramesReady -= kinect_AllFramesReady;

                    kinect.Stop();
                    kinect.Dispose();

                    imageRGB.Source = null;
                }
            }
        }

        void kinect_AllFramesReady(object sendor, AllFramesReadyEventArgs e)
        {
            try
            {
                KinectSensor kinect = sendor as KinectSensor;
                if (kinect == null)
                {
                    return;
                }

                using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
                {
                    if (colorFrame != null)
                    {
                        byte[] colorPixel = new byte[colorFrame.PixelDataLength];
                        colorFrame.CopyPixelDataTo(colorPixel);
                        imageRGB.Source = BitmapSource.Create(colorFrame.Width,
                            colorFrame.Height, 96, 96,
                            PixelFormats.Bgr32, null,
                            colorPixel, colorFrame.Width * colorFrame.BytesPerPixel);
                    }
                }

                using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                {
                    using (SkeletonFrame skeltonFrame = e.OpenSkeletonFrame())
                    {
                        if ((depthFrame != null) && (skeltonFrame != null))
                        {
                           
                            
                            //ここにやりたい処理
                            
                            WRMeasure(kinect, depthFrame, skeltonFrame);
                            SceneChange();
                            Key_Click();

                            //Save1.ReadFile(ref Px,Py,Pz);
                            //Save1.ReadFile2(ref Px2,Py2,Pz2);
                            //contentを管理してるクラスから呼び出す
                            /*ContentManager.BGMmanager();
                            ContentManager.SEManager();
                            ContentManager.ImageManager();*/

                            stopwatch.Start();

                            //画面遷移
                            switch (fase)
                            {
                                    
                                //タイトル
                                case 0:


                                    //ファイルからBitmapImageにデータを読み込む
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\BoxingBegin.png"));
                                    //画像を表示する
                                    FaseM.Source = bmpImage;

                                    //Scene.Content = stopwatch.Elapsed;
                                    if (Keyboard.IsKeyDown(Key.Enter))
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        fase = 1;
                                    }
                                    break;

                                //説明（概要）
                                case 1:

                                    // Scene.Content = "構えろ！！";
                                    //ファイルからBitmapImageにデータを読み込む
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\BoxingExplain.png"));
                                    //画像を表示する
                                    FaseM.Source = bmpImage;
                                    //Scene.Content = stopwatch.Elapsed;
                                    if (Keyboard.IsKeyDown(Key.Enter))
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        fase = 2;

                                    }
                                    break;

                                //説明（ゲームルール）
                                case 2:


                                    Scene.Content = stopwatch.Elapsed;
                                    if (Keyboard.IsKeyDown(Key.Enter))
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();


                                        fase = 3;

                                    }
                                    break;

                                //チュートリアル
                                case 3:

                                    //ファイルからBitmapImageにデータを読み込む
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\Ready.png"));
                                    //画像を表示する
                                    FaseM.Source = bmpImage;




                                    if (Dot(startWristR, startElbowR, Px[0], Py[0], Pz[0], Px[1], Py[1], Pz[1]) >= level &&
                                        Dot(startElbowR, startShoulderR, Px[1], Py[1], Pz[1], Px[2], Py[2], Pz[2]) >= level &&
                                        Dot(startWristL, startElbowL, Px[3], Py[3], Pz[3], Px[4], Py[4], Pz[4]) >= level &&
                                        Dot(startElbowL, startShoulderL, Px[4], Py[4], Pz[4], Px[5], Py[5], Pz[5]) >= level)
                                    {
                                        //構え成功の音


                                        SsSave();
                                    }

                                    if (Dot(startWristR, startElbowR, Px[0], Py[0], Pz[0], Px[1], Py[1], Pz[1]) >= level &&
                                     Dot(startElbowR, startShoulderR, Px[1], Py[1], Pz[1], Px[2], Py[2], Pz[2]) >= level)
                                    {
                                        //攻撃の音

                                        //SsSave();
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 6000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        flag = true;

                                        fase = 4;

                                    }


                                    // Scene.Content = stopwatch.Elapsed;

                                    break;

                                //かまえ
                                case 4:
                                    //ファイルからBitmapImageにデータを読み込む
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\Count.png"));
                                    //画像を表示する

                                    FaseM.Source = bmpImage;
                                    if (Dot(startWristR, startElbowR, Px[0], Py[0], Pz[0], Px[1], Py[1], Pz[1]) >= level &&
                                       Dot(startElbowR, startShoulderR, Px[1], Py[1], Pz[1], Px[2], Py[2], Pz[2]) >= level &&
                                       Dot(startWristL, startElbowL, Px[3], Py[3], Pz[3], Px[4], Py[4], Pz[4]) >= level &&
                                       Dot(startElbowL, startShoulderL, Px[4], Py[4], Pz[4], Px[5], Py[5], Pz[5]) >= level)
                                    {
                                        //構えカウント
                                        score1 += 1;
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 6000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        fase = 5;

                                    }


                                    break;


                                //３，２，１

                                case 5:
                                    if (stopwatch.ElapsedMilliseconds >= 1000)
                                    {
                                        //ファイルからBitmapImageにデータを読み込む
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント3.png"));
                                        //画像を表示する
                                        Count.Source = bmpImage2;
                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 2000)
                                    {
                                        //ファイルからBitmapImageにデータを読み込む
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント2.png"));
                                        //画像を表示する
                                        Count.Source = bmpImage2;
                                        distanceS(startWristR, startShoulderR);

                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 3000)
                                    {
                                        //ファイルからBitmapImageにデータを読み込む
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント1.png"));
                                        //画像を表示する
                                        Count.Source = bmpImage2;


                                    }


                                    if (stopwatch.ElapsedMilliseconds >= 3500)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();


                                        fase = 6;

                                    }
                                    break;

                                //パンチ
                                case 6:
                                    //Scene.Content = Keeptime.Elapsed;
                                    if (flag == true)
                                    {
                                        Keeptime = new Stopwatch();
                                        timeGet = 0;
                                        flag = false;
                                    }
                                    if (Dot(startWristR, startElbowR, Px[0], Py[0], Pz[0], Px[1], Py[1], Pz[1]) >= level &&
                                      Dot(startElbowR, startShoulderR, Px[1], Py[1], Pz[1], Px[2], Py[2], Pz[2]) >= level)
                                    {
                                        //ストロークカウント
                                        score1 += 1;
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 500)
                                    {
                                        //ファイルからBitmapImageにデータを読み込む
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント0.png"));
                                        //画像を表示する
                                        Count.Source = bmpImage2;


                                        distance(keepWristR, keepShoulderR);
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 1500)
                                    {
                                        Count.Source = null;
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 6000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        fase = 7;

                                    }

                                    break;

                                //2回目かまえ
                                case 7:
                                    //ファイルからBitmapImageにデータを読み込む
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\Count.png"));
                                    //画像を表示する

                                    if (Dot(startWristR, startElbowR, Px[0], Py[0], Pz[0], Px[1], Py[1], Pz[1]) >= level &&
                                      Dot(startElbowR, startShoulderR, Px[1], Py[1], Pz[1], Px[2], Py[2], Pz[2]) >= level &&
                                      Dot(startWristL, startElbowL, Px[3], Py[3], Pz[3], Px[4], Py[4], Pz[4]) >= level &&
                                      Dot(startElbowL, startShoulderL, Px[4], Py[4], Pz[4], Px[5], Py[5], Pz[5]) >= level)
                                    {
                                        //構えカウント
                                        score1 += 1;
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 6000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        fase = 8;

                                    }

                                    break;


                                //３，２，１
                                case 8:
                                    if (stopwatch.ElapsedMilliseconds >= 1000)
                                    {
                                        //ファイルからBitmapImageにデータを読み込む
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント3.png"));
                                        //画像を表示する
                                        Count.Source = bmpImage2;
                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 2000)
                                    {
                                        //ファイルからBitmapImageにデータを読み込む
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント2.png"));
                                        //画像を表示する
                                        Count.Source = bmpImage2;
                                        distanceS(startWristR, startShoulderR);

                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 3000)
                                    {
                                        //ファイルからBitmapImageにデータを読み込む
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント1.png"));
                                        //画像を表示する
                                        Count.Source = bmpImage2;


                                    }


                                    if (stopwatch.ElapsedMilliseconds >= 3500)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();


                                        fase = 9;

                                    }

                                    break;

                                //パンチ
                                case 9:
                                    //Scene.Content = Keeptime.Elapsed;
                                    if (flag == true)
                                    {
                                        Keeptime = new Stopwatch();
                                        timeGet2 = 0;
                                        flag = false;
                                    }

                                    if (Dot(startWristR, startElbowR, Px[0], Py[0], Pz[0], Px[1], Py[1], Pz[1]) >= level &&
                                      Dot(startElbowR, startShoulderR, Px[1], Py[1], Pz[1], Px[2], Py[2], Pz[2]) >= level)
                                    {
                                        //ストロークカウント
                                        score1 += 1;
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 500)
                                    {
                                        //ファイルからBitmapImageにデータを読み込む
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント0.png"));
                                        //画像を表示する
                                        Count.Source = bmpImage2;


                                        distance2(keepWristR, keepShoulderR);
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 1500)
                                    {
                                        Count.Source = null;
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 6000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        fase = 10;

                                    }
                                    break;

                                //3回目かまえ
                                case 10:
                                    //ファイルからBitmapImageにデータを読み込む
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\Count.png"));
                                    //画像を表示する

                                    if (Dot(startWristR, startElbowR, Px[0], Py[0], Pz[0], Px[1], Py[1], Pz[1]) >= level &&
                                      Dot(startElbowR, startShoulderR, Px[1], Py[1], Pz[1], Px[2], Py[2], Pz[2]) >= level &&
                                      Dot(startWristL, startElbowL, Px[3], Py[3], Pz[3], Px[4], Py[4], Pz[4]) >= level &&
                                      Dot(startElbowL, startShoulderL, Px[4], Py[4], Pz[4], Px[5], Py[5], Pz[5]) >= level)
                                    {
                                        //構えカウント
                                        score1 += 1;
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 6000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        fase = 11;

                                    }

                                    break;


                                //３，２，１
                                case 11:
                                    if (stopwatch.ElapsedMilliseconds >= 1000)
                                    {
                                        //ファイルからBitmapImageにデータを読み込む
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント3.png"));
                                        //画像を表示する
                                        Count.Source = bmpImage2;
                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 2000)
                                    {
                                        //ファイルからBitmapImageにデータを読み込む
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント2.png"));
                                        //画像を表示する
                                        Count.Source = bmpImage2;
                                        distanceS(startWristR, startShoulderR);

                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 3000)
                                    {
                                        //ファイルからBitmapImageにデータを読み込む
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント1.png"));
                                        //画像を表示する
                                        Count.Source = bmpImage2;


                                    }


                                    if (stopwatch.ElapsedMilliseconds >= 3500)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();


                                        fase = 12;

                                    }

                                    break;

                                //パンチ
                                case 12:

                                    //Scene.Content = Keeptime.Elapsed;
                                    if (flag == true)
                                    {
                                        Keeptime = new Stopwatch();
                                        timeGet3 = 0;
                                        flag = false;
                                    }

                                    if (Dot(startWristR, startElbowR, Px[0], Py[0], Pz[0], Px[1], Py[1], Pz[1]) >= level &&
                                      Dot(startElbowR, startShoulderR, Px[1], Py[1], Pz[1], Px[2], Py[2], Pz[2]) >= level)
                                    {
                                        //ストロークカウント
                                        score1 += 1;
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 500)
                                    {
                                        //ファイルからBitmapImageにデータを読み込む
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント0.png"));
                                        //画像を表示する
                                        Count.Source = bmpImage2;


                                        distance3(keepWristR, keepShoulderR);
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 1500)
                                    {
                                        Count.Source = null;
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 6000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        fase = 13;

                                    }

                                    break;

                                //測定結果
                                case 13:
                                    //ファイルからBitmapImageにデータを読み込む
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\Result.png"));
                                    //画像を表示する
                                    FaseM.Source = bmpImage;
                                    alldistance = (KeepLen - startlen);
                                    speed = (alldistance / timeGet) * 10;
                                    alldistance2 = (KeepLen2 - startlen2);
                                    speed2 = (alldistance2 / timeGet2) * 10;
                                    alldistance3 = (KeepLen3 - startlen3);
                                    speed3 = (alldistance3 / timeGet3) * 10;

                                    if (speed > speed2 && speed > speed3)
                                    {
                                        MostSp = 1;
                                    }
                                    if (speed2 > speed && speed2 > speed3)
                                    {
                                        MostSp = 2;
                                    }
                                    if (speed3 > speed && speed3 > speed2)
                                    {
                                        MostSp = 3;
                                    }

                                    if (MostSp == 1)
                                    {
                                        dis.Content = "距離" + ToRoundDown(alldistance, 3) + "cm " + " 時間" + timeGet + "ms " + " 速度" + speed + "m/s";
                                        Ti.Content = "ストローク" + score1;
                                        Sp.Content = "伸び" + score2;
                                    }

                                    if (MostSp == 2)
                                    {
                                        dis.Content = "距離" + ToRoundDown(alldistance2, 3) + "cm " + " 時間" + timeGet2 + "ms " + " 速度" + speed2 + "m/s";
         
                                      　Ti.Content = "ストローク" + score1;
                                        Sp.Content = "伸び" + score2;
                                    }

                                    if (MostSp == 3)
                                    {
                                        dis.Content = "距離" + ToRoundDown(alldistance3, 3) + "cm " + " 時間" + timeGet3 + "ms " + " 速度" + speed3 + "m/s";
                                        Ti.Content = "ストローク" + score1;
                                        Sp.Content = "伸び" + score2;
                                    }


                                    if (Keyboard.IsKeyDown(Key.Enter))
                                    {

                                        stopwatch.Stop();
                                        stopwatch.Reset();

                                        dis.Content = null;
                                        Ti.Content = null;
                                        Sp.Content = null;
                                        fase = 14;

                                    }
                                    break;

                                //身長と性別を入力
                                case 14:

                                    dis.Content = "性別をFかMで入力してください"+ Sex;
                                    Ti.Content = "身長を入力してください" + height;


                                    if (Keyboard.IsKeyDown(Key.Enter))
                                    {
                                        if (MostSp == 1)
                                        {
                                            Save1.DataSave(Sex, height, startlen, KeepLen, alldistance, timeGet, speed, score1, score2);
                                        }
                                        if (MostSp == 2)
                                        {
                                            Save1.DataSave(Sex, height, startlen2, KeepLen2, alldistance2, timeGet2, speed2, score1, score2);
                                        }
                                        if (MostSp == 3)
                                        {
                                            Save1.DataSave(Sex, height, startlen3, KeepLen3, alldistance3, timeGet3, speed3, score1, score2);
                                        }

                                        score1 = 0;
                                        score2 = 0;
                                        stopwatch.Stop();
                                        stopwatch.Reset();

                                        dis.Content = null;
                                        Ti.Content = null;
                                        Sp.Content = null;
                                        fase = 0;

                                    }
                                    break;

                                default: break;

                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //右肩～右手首までの情報取得
        public void WRMeasure(KinectSensor kinect, DepthImageFrame depthFrame, SkeletonFrame skeletonFrame)
        {
            ColorImageStream colorStream = kinect.ColorStream;
            DepthImageStream depthStream = kinect.DepthStream;

            Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
            skeletonFrame.CopySkeletonDataTo(skeletons);

            int playerIndex = 0;
            for (playerIndex = 0; playerIndex < skeletons.Length; playerIndex++)
            {
                if (skeletons[playerIndex].TrackingState == SkeletonTrackingState.Tracked)
                {
                    break;
                }
            }
            if (playerIndex == skeletons.Length)
            {
                return;
            }

            Skeleton skeleton = skeletons[playerIndex];
            playerIndex++;

            Joint WristR = skeleton.Joints[JointType.WristRight];
            Joint ShoulderR = skeleton.Joints[JointType.ShoulderRight];
            Joint ElbowR = skeleton.Joints[JointType.ElbowRight];

            

            if ((WristR.TrackingState != JointTrackingState.Tracked) ||
               (ShoulderR.TrackingState != JointTrackingState.Tracked) ||
               (ElbowR.TrackingState != JointTrackingState.Tracked))
            {
                return;
            }

            Joint WristL = skeleton.Joints[JointType.WristLeft];
            Joint ShoulderL = skeleton.Joints[JointType.ShoulderLeft];
            Joint ElbowL = skeleton.Joints[JointType.ElbowLeft];
            if ((WristL.TrackingState != JointTrackingState.Tracked) ||
               (ShoulderL.TrackingState != JointTrackingState.Tracked) ||
               (ElbowL.TrackingState != JointTrackingState.Tracked))
            {
                return;
            }

            keepWristR = WristR;
            keepShoulderR = ShoulderR;
            startWristR = WristR;
            startShoulderR = ShoulderR;
            startWristL = WristL;
            startShoulderL = ShoulderL;
            startElbowR = ElbowR;
            startElbowL = ElbowL;

            short[] depthPixel = new short[depthFrame.PixelDataLength];
            depthFrame.CopyPixelDataTo(depthPixel);

            ColorImagePoint[] colorPoint =
                new ColorImagePoint[depthFrame.PixelDataLength];
            kinect.MapDepthFrameToColorFrame(depthStream.Format, depthPixel,
                colorStream.Format, colorPoint);
            /*kinect.CoordinateMapper.MapDepthFrameToColorFrame(depthStream.Format,depthPixel,
                colorStream.Format, colorPoint);*/
            DrawMeasure(kinect, colorStream, WristR, ShoulderR);
            //DrawMeasureL(kinect, colorStream, WristL, ShoulderL);
        }


        //右肩～右手首までの情報可視化
        public void DrawMeasure(KinectSensor kinect, ColorImageStream colorStream, Joint WristR, Joint ShoulderR)
        {
           double lengthR = (Math.Sqrt((WristR.Position.X - ShoulderR.Position.X) * (WristR.Position.X - ShoulderR.Position.X) +
                                      (WristR.Position.Y - ShoulderR.Position.Y) * (WristR.Position.Y - ShoulderR.Position.Y) +
                                      (WristR.Position.Z - ShoulderR.Position.Z) * (WristR.Position.Z - ShoulderR.Position.Z))) * 100;

           ColorImagePoint WristRcolor = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(WristR.Position, colorStream.Format);
           ColorImagePoint ShoulderRcolor = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(ShoulderR.Position, colorStream.Format);
               // kinect.MapSkeletonPointToColor(ShoulderR.Position, colorStream.Format);
            Point WristRScalePoint = new Point(ScaleTo(WristRcolor.X, colorStream.FrameWidth, canvas1.Width),
                                               ScaleTo(WristRcolor.Y, colorStream.FrameHeight, canvas1.Height));

            Point ShoulderRScalePoint = new Point(ScaleTo(ShoulderRcolor.X, colorStream.FrameWidth, canvas1.Width),
                                               ScaleTo(ShoulderRcolor.Y, colorStream.FrameHeight, canvas1.Height));

            




            const int lineLength = 50;
            const int thickness = 10;
            canvas1.Children.Clear();

           /* canvas1.Children.Add(new Line()
            {
                Stroke = new SolidColorBrush(Colors.Red),
                X1 = WristRScalePoint.X,
                Y1 = WristRScalePoint.Y,
                X2 = WristRScalePoint.X + lineLength,
                Y2 = WristRScalePoint.Y,
                StrokeThickness = thickness,
            });
            
            canvas1.Children.Add(new Line()
            {
                Stroke = new SolidColorBrush(Colors.Red),
                X1 = ShoulderRScalePoint.X,
                Y1 = ShoulderRScalePoint.Y,
                X2 = ShoulderRScalePoint.X + lineLength,
                Y2 = ShoulderRScalePoint.Y,
                StrokeThickness = thickness,
            });*/

            canvas1.Children.Add(new Line()
            {
                Stroke = new SolidColorBrush(Colors.Red),
                X1 = WristRScalePoint.X,
                Y1 = WristRScalePoint.Y,
                X2 = ShoulderRScalePoint.X,
                Y2 = ShoulderRScalePoint.Y,
                StrokeThickness = thickness,
            });


            double Y = Math.Abs(WristRScalePoint.Y + ShoulderRScalePoint.Y) / 2;
            canvas1.Children.Add(new TextBlock()
            {
                Margin = new Thickness(WristRScalePoint.X + lineLength, Y, 0, 0),
                Text = lengthR.ToString(),
                Height = 36,
                Width = 100,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Colors.White),
            });

            /*long Time = TimeSpan.TicksPerMillisecond;

            canvas1.Children.Add(new TextBlock()
            {
                Margin = new Thickness(0,0, 0, 0),
                Text = Time.ToString(),
                Height = 36,
                Width = 60,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Colors.White),
            });*/


        }


       /* private void DrawMeasureL(KinectSensor kinect, ColorImageStream colorStream, Joint WristL, Joint ShoulderL)
        {
            double lengthL = Math.Abs((WristL.Position.X - ShoulderL.Position.X)*100);

            ColorImagePoint WristLcolor = kinect.MapSkeletonPointToColor(WristL.Position, colorStream.Format);
            ColorImagePoint ShoulderLcolor = kinect.MapSkeletonPointToColor(ShoulderL.Position, colorStream.Format);

            Point WristLScalePoint = new Point(ScaleTo(WristLcolor.X, colorStream.FrameWidth, canvas1.Width),
                                               ScaleTo(WristLcolor.Y, colorStream.FrameHeight, canvas1.Height));

            Point ShoulderLScalePoint = new Point(ScaleTo(ShoulderLcolor.X, colorStream.FrameWidth, canvas1.Width),
                                               ScaleTo(ShoulderLcolor.Y, colorStream.FrameHeight, canvas1.Height));






            const int lineLength = 50;
            const int thickness = 10;
            canvas1.Children.Clear();

         

            canvas1.Children.Add(new Line()
            {
                Stroke = new SolidColorBrush(Colors.Red),
                X1 = WristLScalePoint.X,
                Y1 = WristLScalePoint.Y,
                X2 = ShoulderLScalePoint.X,
                Y2 = ShoulderLScalePoint.Y,
                StrokeThickness = thickness,
            });


            double YL = Math.Abs(WristLScalePoint.Y + ShoulderLScalePoint.Y) / 2;
            canvas1.Children.Add(new TextBlock()
            {
                Margin = new Thickness(WristLScalePoint.X + lineLength, YL, 0, 0),
                Text = lengthL.ToString(),
                Height = 36,
                Width = 100,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Colors.White),
            });

            /*long Time = TimeSpan.TicksPerMillisecond;

            canvas1.Children.Add(new TextBlock()
            {
                Margin = new Thickness(0,0, 0, 0),
                Text = Time.ToString(),
                Height = 36,
                Width = 60,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Colors.White),
            });*/








        //}


        double ScaleTo(double value, double source, double dest)
        {
            return (value * dest) / source;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopKinect(KinectSensor.KinectSensors[0]);
        }
        
        //最大距離検出
        public void distance(Joint WristR, Joint ShoulderR)
        {
           
            

            len = (Math.Sqrt((WristR.Position.X - ShoulderR.Position.X) * (WristR.Position.X - ShoulderR.Position.X) +
                                     (WristR.Position.Y - ShoulderR.Position.Y) * (WristR.Position.Y - ShoulderR.Position.Y) +
                                     (WristR.Position.Z - ShoulderR.Position.Z) * (WristR.Position.Z - ShoulderR.Position.Z))) * 100;
            if (KeepLen < len)
            {
                KeepLen = len;
              
                Keeptime.Start();
                 
                timeGet = Keeptime.ElapsedMilliseconds;

                score2 += 1;
            }

            if (timeGet > 0 )
            {
                
                Keeptime.Stop();
                
            }

        }

        //最大距離検出2回目
        public void distance2(Joint WristR, Joint ShoulderR)
        {



            len2 = (Math.Sqrt((WristR.Position.X - ShoulderR.Position.X) * (WristR.Position.X - ShoulderR.Position.X) +
                                     (WristR.Position.Y - ShoulderR.Position.Y) * (WristR.Position.Y - ShoulderR.Position.Y) +
                                     (WristR.Position.Z - ShoulderR.Position.Z) * (WristR.Position.Z - ShoulderR.Position.Z))) * 100;
            if (KeepLen2 < len2)
            {
                KeepLen2 = len2;

                Keeptime.Start();

                timeGet2 = Keeptime.ElapsedMilliseconds;

                score2 += 1;
            }

            if (timeGet2 > 0)
            {

                Keeptime.Stop();

            }

        }

        //最大距離検出3回目
        public void distance3(Joint WristR, Joint ShoulderR)
        {



            len3 = (Math.Sqrt((WristR.Position.X - ShoulderR.Position.X) * (WristR.Position.X - ShoulderR.Position.X) +
                                     (WristR.Position.Y - ShoulderR.Position.Y) * (WristR.Position.Y - ShoulderR.Position.Y) +
                                     (WristR.Position.Z - ShoulderR.Position.Z) * (WristR.Position.Z - ShoulderR.Position.Z))) * 100;
            if (KeepLen3 < len3)
            {
                KeepLen3 = len3;

                Keeptime.Start();

                timeGet3 = Keeptime.ElapsedMilliseconds;

                score2 += 1;

            }

            if (timeGet3 > 0)
            {

                Keeptime.Stop();

            }

        }


        //初期構え距離検出
        public void distanceS(Joint WristR, Joint ShoulderR)
        {


            startlen = (Math.Sqrt((WristR.Position.X - ShoulderR.Position.X) * (WristR.Position.X - ShoulderR.Position.X) +
                                     (WristR.Position.Y - ShoulderR.Position.Y) * (WristR.Position.Y - ShoulderR.Position.Y) +
                                     (WristR.Position.Z - ShoulderR.Position.Z) * (WristR.Position.Z - ShoulderR.Position.Z))) * 100;
            

        }

        //初期構え距離検出2回目
        public void distanceS2(Joint WristR, Joint ShoulderR)
        {


            startlen2 = (Math.Sqrt((WristR.Position.X - ShoulderR.Position.X) * (WristR.Position.X - ShoulderR.Position.X) +
                                     (WristR.Position.Y - ShoulderR.Position.Y) * (WristR.Position.Y - ShoulderR.Position.Y) +
                                     (WristR.Position.Z - ShoulderR.Position.Z) * (WristR.Position.Z - ShoulderR.Position.Z))) * 100;


        }

        //初期構え距離検出3回目
        public void distanceS3(Joint WristR, Joint ShoulderR)
        {


            startlen3 = (Math.Sqrt((WristR.Position.X - ShoulderR.Position.X) * (WristR.Position.X - ShoulderR.Position.X) +
                                     (WristR.Position.Y - ShoulderR.Position.Y) * (WristR.Position.Y - ShoulderR.Position.Y) +
                                     (WristR.Position.Z - ShoulderR.Position.Z) * (WristR.Position.Z - ShoulderR.Position.Z))) * 100;
        }

        
        //内積比較
        public float Dot(Joint root,Joint tar,double rx,double ry,double rz,double tx,double ty,double tz)
        {

            float A1,A2,A3,B1,B2,B3;

            A1 = root.Position.X - tar.Position.X;
            A2 = root.Position.Y - tar.Position.Y;
            A3 = root.Position.Z - tar.Position.Z;

            B1 = (float)(rx - tx);
            B2 = (float)(ry - ty);
            B3 = (float)(rz - rz);

            float a, b, c;

            a = A1 * A1 + A2 * A2 + A3 * A3;
            b = B1 * B1 + B2 * B2 + B3 * B3;
            c = A1 * B1 + A2 * B2 + A3 * B3;

            return (float)(c / (Math.Sqrt(a) * Math.Sqrt(b)));
        }


        //スクリーンショット保存
        public void SsSave()
        {

            //アクティブウィンドウをスクリーンキャプチャする場合
            System.Windows.Forms.SendKeys.SendWait("%{PRTSC}");

            //全画面をスクリーンキャプチャする場合
            //System.Windows.Forms.SendKeys.SendWait("^{PRTSC}");

            IDataObject dobj = Clipboard.GetDataObject();
            if (dobj.GetDataPresent(DataFormats.Bitmap) == true)
            {
                System.Windows.Interop.InteropBitmap ibmp
                  = (System.Windows.Interop.InteropBitmap)dobj.GetData(DataFormats.Bitmap);

                PngBitmapEncoder enc = new PngBitmapEncoder();
                //JpegBitmapEncoder enc = new JpegBitmapEncoder();//JPEGファイルで保存する場合
                enc.Frames.Add(BitmapFrame.Create(ibmp));
                System.IO.FileStream fs = new System.IO.FileStream(@"C:\BoxScreen\" + i + ".png",
                  System.IO.FileMode.Create, System.IO.FileAccess.Write);
                i++;
                enc.Save(fs);
                fs.Close();
                fs.Dispose();
            }

        }
       

        //ボタンクリックでスクリーンショット保存
        private void 保存_Click(object sender, RoutedEventArgs e)
        {

            for (i = 1; i <= 30; i++)
            {
                //アクティブウィンドウをスクリーンキャプチャする場合
                System.Windows.Forms.SendKeys.SendWait("%{PRTSC}");

                //全画面をスクリーンキャプチャする場合
                //System.Windows.Forms.SendKeys.SendWait("^{PRTSC}");

                IDataObject dobj = Clipboard.GetDataObject();
                if (dobj.GetDataPresent(DataFormats.Bitmap) == true)
                {
                    System.Windows.Interop.InteropBitmap ibmp
                      = (System.Windows.Interop.InteropBitmap)dobj.GetData(DataFormats.Bitmap);

                    PngBitmapEncoder enc = new PngBitmapEncoder();
                    //JpegBitmapEncoder enc = new JpegBitmapEncoder();//JPEGファイルで保存する場合
                    enc.Frames.Add(BitmapFrame.Create(ibmp));
                   
                        System.IO.FileStream fs = new System.IO.FileStream(@"C:\BoxScreen\" + i + ".png",
                          System.IO.FileMode.Create, System.IO.FileAccess.Write);

                        enc.Save(fs);
                        fs.Close();
                        fs.Dispose();
                    }
                }
            
        }

        //Aキーでスクリーン保存
        private void Key_Click()
        {

            if (Keyboard.IsKeyDown(Key.A) == true)
            {

                Save1.PosSave1(startWristR.Position.X,startWristR.Position.Y,startWristR.Position.Z);//0
                Save1.PosSave1(startElbowR.Position.X, startElbowR.Position.Y, startElbowR.Position.Z);//1
                Save1.PosSave1(startShoulderR.Position.X, startShoulderR.Position.Y, startShoulderR.Position.Z);//2
                Save1.PosSave1(startWristL.Position.X, startWristL.Position.Y, startWristL.Position.Z);//3
                Save1.PosSave1(startElbowL.Position.X, startElbowL.Position.Y, startElbowL.Position.Z);//4
                Save1.PosSave1(startShoulderL.Position.X, startShoulderL.Position.Y, startShoulderL.Position.Z);//5



                //Aキーを押した時の処理

                //アクティブウィンドウをスクリーンキャプチャする場合
                System.Windows.Forms.SendKeys.SendWait("%{PRTSC}");

                //全画面をスクリーンキャプチャする場合
                //System.Windows.Forms.SendKeys.SendWait("^{PRTSC}");

                IDataObject dobj = Clipboard.GetDataObject();
                if (dobj.GetDataPresent(DataFormats.Bitmap) == true)
                {
                    System.Windows.Interop.InteropBitmap ibmp
                      = (System.Windows.Interop.InteropBitmap)dobj.GetData(DataFormats.Bitmap);

                    PngBitmapEncoder enc = new PngBitmapEncoder();
                    //JpegBitmapEncoder enc = new JpegBitmapEncoder();//JPEGファイルで保存する場合
                    enc.Frames.Add(BitmapFrame.Create(ibmp));
                    System.IO.FileStream fs = new System.IO.FileStream(@"C:\BoxScreen\" + i + ".png",
                      System.IO.FileMode.Create, System.IO.FileAccess.Write);
                    i++;
                    enc.Save(fs);
                    fs.Close();
                    fs.Dispose();
                }

            }
        }

        private void Mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        //小数点切捨て
        public static double ToRoundDown(double dValue, int iDigits)
        {
            double dCoef = Math.Pow(10, iDigits);

            return dValue > 0 ? Math.Floor(dValue * dCoef) / dCoef :
                                Math.Ceiling(dValue * dCoef) / dCoef;
        }
        //画面遷移
        public void SceneChange()
        {
            if (Keyboard.IsKeyDown(Key.NumPad0) == true)
            {
                fase = 0;
            }

            if (Keyboard.IsKeyDown(Key.D1) == true)
            {
                fase = 1;
            }

            if (Keyboard.IsKeyDown(Key.D2) == true)
            {
                fase = 2;
            }

            if (Keyboard.IsKeyDown(Key.D3) == true)
            {
                fase = 3;
            }

            if (Keyboard.IsKeyDown(Key.D4) == true)
            {
                fase = 4;
            }

            if (Keyboard.IsKeyDown(Key.D5) == true)
            {
                fase = 13;
            }

            if (Keyboard.IsKeyDown(Key.D6) == true)
            {
                fase = 14;
            }




        }
        


    }



}

