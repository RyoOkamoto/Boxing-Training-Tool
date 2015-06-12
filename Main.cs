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
        int fasecopy = 0;

        double speed;
        double len;
        double KeepLen = 0;
        Joint keepWristR;
        Joint keepShoulderR;

        Stopwatch stopwatch = new Stopwatch();
        Stopwatch Keeptime = new Stopwatch();
        double timeGet;
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


            //Key_Click();
           







        }

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
                            stopwatch.Start();
                            switch (fase)
                            {
                                case 0:
                                    if (fasecopy != fase)
                                    {
                                        
                                    }
                                    //Scene.Content = "タイトル";
                                    //ファイルからBitmapImageにデータを読み込む
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\BoxingBegin.png"));
                                    //画像を表示する
                                    FaseM.Source = bmpImage;
                                   
                                    Scene.Content = stopwatch.Elapsed;
                                    if (stopwatch.ElapsedTicks >= 18000000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        fase = 1;
                                    }
                                    break;

                                case 1:
                                    
                                   // Scene.Content = "構えろ！！";
                                    //ファイルからBitmapImageにデータを読み込む
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\BoxingExplain.png"));
                                    //画像を表示する
                                    FaseM.Source = bmpImage;
                                    Scene.Content = stopwatch.Elapsed;
                                    if (stopwatch.ElapsedTicks >= 18000000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        fase = 2;
                                    
                                    }
                                    break;

                                case 2:

                                    //ファイルからBitmapImageにデータを読み込む
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\Ready.png"));
                                    //画像を表示する
                                    FaseM.Source = bmpImage;
                                    Scene.Content = stopwatch.Elapsed;
                                    if (stopwatch.ElapsedTicks >= 18000000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        fase = 3;

                                    }
                                    break;

                                case 3:


                                    
                                    

                                    //ファイルからBitmapImageにデータを読み込む
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\Count.png"));
                                    //画像を表示する
                                    FaseM.Source = bmpImage;
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

                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 3000)
                                    {
                                        //ファイルからBitmapImageにデータを読み込む
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント1.png"));
                                        //画像を表示する
                                        Count.Source = bmpImage2;

                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 4000)
                                    {
                                        //ファイルからBitmapImageにデータを読み込む
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\カウント0.png"));
                                        //画像を表示する
                                        Count.Source = bmpImage2;
                                        Keeptime.Start();
                                        distance(keepWristR, keepShoulderR);
                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 5000)
                                    {
                                        Count.Source = null;
                                    }
                                    Scene.Content = stopwatch.Elapsed;
                                    if (stopwatch.ElapsedTicks >= 18000000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        Keeptime.Stop();

                                        fase = 4;

                                    }
                                    break;

                                case 4:
                                    //ファイルからBitmapImageにデータを読み込む
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\Result.png"));
                                    //画像を表示する
                                    FaseM.Source = bmpImage;
                                    dis.Content = "距離"+KeepLen+"cm";
                                    Ti.Content = "時間" + timeGet + "ms";
                                    speed = (KeepLen / timeGet)*10;
                                    Sp.Content = "速度" + speed + "m/s";
                                    

                                    Scene.Content = stopwatch.Elapsed;
                                    if (Keyboard.IsKeyDown(Key.Enter))
                                    {
                                        Save1.PosSave(KeepLen, timeGet, speed);
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        Keeptime.Reset();
                                        dis.Content = null;
                                        Ti.Content = null;
                                        Sp.Content = null;
                                        fase = 0;

                                    }
                                    break;

                                default: break;

                            }


                            if (Keyboard.IsKeyDown(Key.A))
                            {
                               // Save1.PosSave(1, 1, 1);
                            }

                            Key_Click();



                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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

        public void distance(Joint WristR, Joint ShoulderR)
        {
           

            len = (Math.Sqrt((WristR.Position.X - ShoulderR.Position.X) * (WristR.Position.X - ShoulderR.Position.X) +
                                     (WristR.Position.Y - ShoulderR.Position.Y) * (WristR.Position.Y - ShoulderR.Position.Y) +
                                     (WristR.Position.Z - ShoulderR.Position.Z) * (WristR.Position.Z - ShoulderR.Position.Z))) * 100;
            if (KeepLen < len)
            {
                KeepLen = len;
                timeGet = Keeptime.ElapsedMilliseconds;

            }

        }

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

        private void Key_Click()
        {

            if (Keyboard.IsKeyDown(Key.A) == true)
            {


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

        private void ScreenShot(object sener, KeyEventArgs e)
        {
            
        }

        


    }



}

