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
    /// MainWindow.xaml �̑��ݍ�p���W�b�N
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly int Bgr32BytesPerPixel = PixelFormats.Bgr32.BitsPerPixel / 8;
        int i = 1;
        int fase = 0;
        int fasecopy = 0;

        double speed;
        double len;
        //�\�����̋���
        double startlen;
        //�X�g���[�g���̋���
        double KeepLen = 0;
        //�\�����猝������̋���
        double alldistance;
        bool flag = false;

        Joint keepWristR;
        Joint keepShoulderR;
        Joint startWristR;
        Joint startShoulderR;


        Stopwatch stopwatch = new Stopwatch();
        Stopwatch Keeptime = new Stopwatch();
        double timeGet;
        //SubWindow sw = new SubWindow();

        // �ǂݍ��񂾉摜��ۑ����邽�߂̕ϐ�
        BitmapImage bmpImage;
        BitmapImage bmpImage2;
        // �t�@�C�����J���_�C�A���O
        Microsoft.Win32.OpenFileDialog ofDialog =
            new Microsoft.Win32.OpenFileDialog();
       

        public MainWindow()
        {
            
            try
            {
                InitializeComponent();

                // Kinect���ڑ�����Ă��邩�ǂ���
                if (KinectSensor.KinectSensors.Count == 0)
                {
                    throw new Exception("�ڑ����Ă�������");
                }
                StartKinect(KinectSensor.KinectSensors[0]);
                //sw.Show();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

           
                
             
        }

        

       
        //Kinect���X�^�[�g������
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


        //Kinect���X�g�b�v������
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
                           
                            
                            //�����ɂ�肽������
                            
                            WRMeasure(kinect, depthFrame, skeltonFrame);
                            //content���Ǘ����Ă�N���X����Ăяo��
                            /*ContentManager.BGMmanager();
                            ContentManager.SEManager();
                            ContentManager.ImageManager();*/

                            stopwatch.Start();

                            //��ʑJ��
                            switch (fase)
                            {
                                case 0:
                                    if (fasecopy != fase)
                                    {
                                        
                                    }
                                    //Scene.Content = "�^�C�g��";
                                    //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\BoxingBegin.png"));
                                    //�摜��\������
                                    FaseM.Source = bmpImage;
                                   
                                    //Scene.Content = stopwatch.Elapsed;
                                    if (stopwatch.ElapsedTicks >= 18000000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        fase = 1;
                                    }
                                    break;

                                case 1:
                                    
                                   // Scene.Content = "�\����I�I";
                                    //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\BoxingExplain.png"));
                                    //�摜��\������
                                    FaseM.Source = bmpImage;
                                    //Scene.Content = stopwatch.Elapsed;
                                    if (stopwatch.ElapsedTicks >= 18000000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        fase = 2;
                                    
                                    }
                                    break;

                                case 2:

                                    //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\Ready.png"));
                                    //�摜��\������
                                    FaseM.Source = bmpImage;
                                    Scene.Content = stopwatch.Elapsed;
                                    if (stopwatch.ElapsedTicks >= 18000000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        flag = true;
                                        
                                        fase = 3;

                                    }
                                    break;

                                case 3:


                                    Scene.Content = Keeptime.Elapsed;
                                    if (flag == true)
                                    {
                                        Keeptime = new Stopwatch();
                                        timeGet = 0;
                                        flag = false;
                                    }
                                    

                                    //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\Count.png"));
                                    //�摜��\������
                                    FaseM.Source = bmpImage;
                                    if (stopwatch.ElapsedMilliseconds >= 1000)
                                    {
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g3.png"));
                                        //�摜��\������
                                        Count.Source = bmpImage2;
                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 2000)
                                    {
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g2.png"));
                                        //�摜��\������
                                        Count.Source = bmpImage2;
                                        distanceS(startWristR, startShoulderR);

                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 3000)
                                    {
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g1.png"));
                                        //�摜��\������
                                        Count.Source = bmpImage2;
                                        

                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 4000)
                                    {
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                        bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g0.png"));
                                        //�摜��\������
                                        Count.Source = bmpImage2;
                                        
                                       
                                        distance(keepWristR, keepShoulderR);
                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 5000)
                                    {
                                        Count.Source = null;
                                    }
                                   // Scene.Content = stopwatch.Elapsed;
                                    if (stopwatch.ElapsedTicks >= 18000000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        

                                        fase = 4;

                                    }
                                    break;

                                case 4:
                                    //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                    bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\Result.png"));
                                    //�摜��\������
                                    FaseM.Source = bmpImage;
                                    alldistance = (KeepLen - startlen);
                                    dis.Content = "����"+ alldistance +"cm";
                                    Ti.Content = "����" + timeGet + "ms";
                                    speed = (alldistance / timeGet)*10;
                                    Sp.Content = "���x" + speed + "m/s";
                                    

                                    Scene.Content = Keeptime.Elapsed;
                                    if (Keyboard.IsKeyDown(Key.Enter))
                                    {
                                        Save1.PosSave(KeepLen, timeGet, speed);
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

        //�E���`�E���܂ł̏��擾
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


        //�E���`�E���܂ł̏�����
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
        
        //�ő勗�����o
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
                
            }

            if (timeGet > 0 )
            {
                
                Keeptime.Stop();
                
            }


        }

        //�����\���������o
        public void distanceS(Joint WristR, Joint ShoulderR)
        {


            startlen = (Math.Sqrt((WristR.Position.X - ShoulderR.Position.X) * (WristR.Position.X - ShoulderR.Position.X) +
                                     (WristR.Position.Y - ShoulderR.Position.Y) * (WristR.Position.Y - ShoulderR.Position.Y) +
                                     (WristR.Position.Z - ShoulderR.Position.Z) * (WristR.Position.Z - ShoulderR.Position.Z))) * 100;
            

        }
        //�X�N���[���V���b�g�ۑ�
        public void SsSave()
        {

            //�A�N�e�B�u�E�B���h�E���X�N���[���L���v�`������ꍇ
            System.Windows.Forms.SendKeys.SendWait("%{PRTSC}");

            //�S��ʂ��X�N���[���L���v�`������ꍇ
            //System.Windows.Forms.SendKeys.SendWait("^{PRTSC}");

            IDataObject dobj = Clipboard.GetDataObject();
            if (dobj.GetDataPresent(DataFormats.Bitmap) == true)
            {
                System.Windows.Interop.InteropBitmap ibmp
                  = (System.Windows.Interop.InteropBitmap)dobj.GetData(DataFormats.Bitmap);

                PngBitmapEncoder enc = new PngBitmapEncoder();
                //JpegBitmapEncoder enc = new JpegBitmapEncoder();//JPEG�t�@�C���ŕۑ�����ꍇ
                enc.Frames.Add(BitmapFrame.Create(ibmp));
                System.IO.FileStream fs = new System.IO.FileStream(@"C:\BoxScreen\" + i + ".png",
                  System.IO.FileMode.Create, System.IO.FileAccess.Write);
                i++;
                enc.Save(fs);
                fs.Close();
                fs.Dispose();
            }

        }
       

        //�{�^���N���b�N�ŃX�N���[���V���b�g�ۑ�
        private void �ۑ�_Click(object sender, RoutedEventArgs e)
        {

            for (i = 1; i <= 30; i++)
            {
                //�A�N�e�B�u�E�B���h�E���X�N���[���L���v�`������ꍇ
                System.Windows.Forms.SendKeys.SendWait("%{PRTSC}");

                //�S��ʂ��X�N���[���L���v�`������ꍇ
                //System.Windows.Forms.SendKeys.SendWait("^{PRTSC}");

                IDataObject dobj = Clipboard.GetDataObject();
                if (dobj.GetDataPresent(DataFormats.Bitmap) == true)
                {
                    System.Windows.Interop.InteropBitmap ibmp
                      = (System.Windows.Interop.InteropBitmap)dobj.GetData(DataFormats.Bitmap);

                    PngBitmapEncoder enc = new PngBitmapEncoder();
                    //JpegBitmapEncoder enc = new JpegBitmapEncoder();//JPEG�t�@�C���ŕۑ�����ꍇ
                    enc.Frames.Add(BitmapFrame.Create(ibmp));
                   
                        System.IO.FileStream fs = new System.IO.FileStream(@"C:\BoxScreen\" + i + ".png",
                          System.IO.FileMode.Create, System.IO.FileAccess.Write);

                        enc.Save(fs);
                        fs.Close();
                        fs.Dispose();
                    }
                }
            
        }

        //A�L�[�ŃX�N���[���ۑ�
        private void Key_Click()
        {

            if (Keyboard.IsKeyDown(Key.A) == true)
            {


                //A�L�[�����������̏���

                //�A�N�e�B�u�E�B���h�E���X�N���[���L���v�`������ꍇ
                System.Windows.Forms.SendKeys.SendWait("%{PRTSC}");

                //�S��ʂ��X�N���[���L���v�`������ꍇ
                //System.Windows.Forms.SendKeys.SendWait("^{PRTSC}");

                IDataObject dobj = Clipboard.GetDataObject();
                if (dobj.GetDataPresent(DataFormats.Bitmap) == true)
                {
                    System.Windows.Interop.InteropBitmap ibmp
                      = (System.Windows.Interop.InteropBitmap)dobj.GetData(DataFormats.Bitmap);

                    PngBitmapEncoder enc = new PngBitmapEncoder();
                    //JpegBitmapEncoder enc = new JpegBitmapEncoder();//JPEG�t�@�C���ŕۑ�����ꍇ
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

       

        


    }

















}
