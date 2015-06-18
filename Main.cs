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
        int MostSp;
        
        //臒l
        float level = 0.6f;
        float level2 = 0.6f;
        //�]��
        int score1 = 0;//�ǂ��\��
        int score2 = 0;//�L�т��ǂ�
        double speed;
        double speed2;
        double speed3;

        double len;
        double len2;
        double len3;
        //�\�����̋���
        double startlen;
        double startlen2;
        double startlen3;
        //�X�g���[�g���̋���
        double KeepLen = 0;
        double KeepLen2= 0;
        double KeepLen3= 0;
        //�\�����猝������̋���
        double alldistance;
        double alldistance2;
        double alldistance3;

        bool flag = false;
        bool flag2 = false;
        bool flagM = false;
        bool flagSE = true;
        bool Keyflage = false;
        bool Fflag = true;
        bool Cameraflag = true;
        bool Ma = true;
        bool Pflag = false;
        bool Pflag2 = false;
        bool Pflag3 = false;
        bool Countflag = true;
        bool Pchange = true;
       

        Joint keepWristR;
        Joint keepShoulderR;
        Joint startWristR;
        Joint startShoulderR;
        Joint startWristL;
        Joint startShoulderL;
        Joint startElbowR;
        Joint startElbowL;

        double[] Px = new double[6];
        double[] Py = new double[6];
        double[] Pz = new double[6];
        double[] Px2 = new double[6];
        double[] Py2 = new double[6];
        double[] Pz2 = new double[6];
        double[] Px3 = new double[6];
        double[] Py3 = new double[6];
        double[] Pz3 = new double[6];




        SoundPlayer[] BGM = new SoundPlayer[3];
        Uri[] SE = new Uri[6];

        String ID = null;
        

        Stopwatch stopwatch = new Stopwatch();
        Stopwatch Keeptime = new Stopwatch();
        Stopwatch Panchtime = new Stopwatch();
        Stopwatch IDtime = new Stopwatch();
        Stopwatch Setime = new Stopwatch();
        Stopwatch SetPtime = new Stopwatch();
        double timeGet;
        double timeGet2;
        double timeGet3;
        //SubWindow sw = new SubWindow();


        BitmapImage[] Bimage = new BitmapImage[10];
        BitmapImage[] Simage = new BitmapImage[5];

        // �t�@�C�����J���_�C�A���O
        Microsoft.Win32.OpenFileDialog ofDialog =
            new Microsoft.Win32.OpenFileDialog();
        double lenR;
        double lenL;
       
       

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

        

       
        // Kinect���X�^�[�g������
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
                            SceneChange();

                            if (Keyboard.IsKeyDown(Key.S) && Keyflage == false)
                            {
                                savePos();
                                Keyflage = true;
                            }

                            if (Fflag == true)
                            {
                                Save1.ReadFile(ref Px,ref Py,ref Pz);
                                Save1.ReadFile2(ref Px2, ref Py2, ref Pz2);
                                Save1.ReadFile3(ref Px3,ref Py3,ref Pz3);
                                //content���Ǘ����Ă�N���X����Ăяo��
                                ContentManager.BGMmanager(ref BGM);
                                
                                ContentManager.SEManager(ref SE);

                                ContentManager.ImageManager(ref Bimage,ref Simage);

                                Under.Source = Bimage[6];
                                Under2.Source = Bimage[6];
                                Under3.Source = Bimage[6];
                                Hit.Source = Bimage[8];
                                Fflag = false;
                            }

                            stopwatch.Start();
                           
                            
                            
#region ��ʑJ��
                            switch (fase)
                            {
                                    
#region �^�C�g��
                                case 0:

                                    if (flagM == false)
                                    {
                                        BGM[0].PlayLooping();
                                        //MusicB.Source = new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Music\No_Escape.wav");
                                        //MusicB.Play();
                                        flagM = true;
                                    }
                                   
    
                                    
                                    //�摜��\������
                                    FaseM.Source = Bimage[0];

                                    //Scene.Content = stopwatch.Elapsed;
                                    if (Keyboard.IsKeyDown(Key.Enter) && stopwatch.ElapsedMilliseconds>1500)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        fase = 1;
                                    }
                                    break;
#endregion

#region �����i�T�v�j
                                case 1:

                                    
                                    //�摜��\������
                                    FaseM.Source = Bimage[1];
                                    //Scene.Content = stopwatch.Elapsed;
                                    if (Keyboard.IsKeyDown(Key.Enter) && stopwatch.ElapsedMilliseconds > 1500)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        fase = 2;

                                    }
                                    break;
#endregion

#region �����i�Q�[�����[���j
                                case 2:

                                    
                                    //�摜��\������
                                    FaseM.Source = Bimage[2];

                                    
                                    if (Keyboard.IsKeyDown(Key.Enter) && stopwatch.ElapsedMilliseconds > 1500)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        flagSE = true;

                                        fase = 3;

                                    }
                                    break;
#endregion

#region �`���[�g���A��
                                case 3:

                                  
                                    //�摜��\������
                                    FaseM.Source = Bimage[3];
                                    distanceR(keepWristR, keepShoulderR);
                                    distanceL(startWristL,startShoulderL);
                                   /* if (Panchtime.ElapsedMilliseconds > 300)
                                    {
                                        Pflag = true;
                                        Panchtime.Stop();
                                        Panchtime.Reset();
                                    }*/

                                   
                                    if (Keyboard.IsKeyDown(Key.C) && Cameraflag == true)
                                    {

                                        SsSave();
                                        Cameraflag = false;
                                    }

                                    if (flagSE == true)
                                    {
                                        MusicB.Source = SE[1];
                                        MusicA.Source = SE[4];
                                        MusicC.Source = SE[0];
                                        flagSE = false;
                                    }
                                    if (Dot(startWristR, startElbowR, Px[0], Py[0], Pz[0], Px[1], Py[1], Pz[1]) >= level &&
                                        Dot(startElbowR, startShoulderR, Px[1], Py[1], Pz[1], Px[2], Py[2], Pz[2]) >= level &&
                                        Dot(startWristL, startElbowL, Px[3], Py[3], Pz[3], Px[4], Py[4], Pz[4]) >= level &&
                                        Dot(startElbowL, startShoulderL, Px[4], Py[4], Pz[4], Px[5], Py[5], Pz[5]) >= level)
                                    {
                                        Pchange = false;
                                        //�\���̉�
                                        MusicB.Stop();
                                        if (!Pflag)
                                        {
                                            MusicA.Play();
                                            Setime.Start();

                                            Pflag = true;
                                        }
                                            
                                        //SsSave();
                                    }


                                    if (lenR > 35 && Pchange == false && startWristR.Position.Y > startElbowR.Position.Y && startElbowR.Position.Y < startShoulderR.Position.Y * 1.2)
                                        
                                    {
                                       // MusicB.Play();
                                            //�U���̉�
                                        MusicA.Stop();
                                            if (!Pflag3)
                                            {
                                                MusicB.Play();
                                                SetPtime.Start();

                                                Pflag3 = true;
                                            }
                                            
                                          
                                     
                                        //SsSave();
                                    }

                                    if (lenL > 35 && Pchange == false && startWristL.Position.Y > startElbowL.Position.Y  && startElbowL.Position.Y < startShoulderL.Position.Y * 1.2)
                                    {

                                        //�U���̉�
                                        MusicA.Stop();
                                        if (!Pflag2)
                                        {
                                            MusicC.Play();
                                            SetPtime.Start();
                                            Pflag2 = true;
                                        }

                                        //SsSave();
                                    }

                                    if (Setime.ElapsedMilliseconds > 400)
                                    {
                                        Pflag = false;
                                       
                                        
                                        MusicA.Stop();
                                       
                                        Setime.Stop();
                                        Setime.Reset();
                                    }

                                 if(SetPtime.ElapsedMilliseconds > 800){
                                     Pflag2 = false;
                                     Pflag3 = false;
                                     
                                     MusicB.Stop();
                                     MusicC.Stop();
                                     SetPtime.Stop();
                                     SetPtime.Reset();
                                 }
                                   

        


                                    if (Keyboard.IsKeyDown(Key.Enter) && stopwatch.ElapsedMilliseconds > 1500)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        
                                        
                                        flagM = false;
                                        fase = 4;

                                    }


                                    // Scene.Content = stopwatch.Elapsed;

                                    break;
#endregion

#region 1��ڂ��܂�
                                case 4:
                                    
                                    if (flagM == false)
                                    {
                                        
                                        BGM[0].Stop();
                                        BGM[1].PlayLooping();
                                        Cameraflag = true;
                                        flagM = true;
                                    }
                                   
                                    //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                   // bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\Count.png"));
                                    //�摜��\������

                                    FaseM.Source = Bimage[4];
                                    if (Dot(startWristR, startElbowR, Px[0], Py[0], Pz[0], Px[1], Py[1], Pz[1]) >= level &&
                                       Dot(startElbowR, startShoulderR, Px[1], Py[1], Pz[1], Px[2], Py[2], Pz[2]) >= level &&
                                       Dot(startWristL, startElbowL, Px[3], Py[3], Pz[3], Px[4], Py[4], Pz[4]) >= level &&
                                       Dot(startElbowL, startShoulderL, Px[4], Py[4], Pz[4], Px[5], Py[5], Pz[5]) >= level && Countflag == true)
                                    {
                                        //�\���J�E���g
                                        score1 += 1;
                                        Countflag = false;
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 2000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        flagM = false;
                                        Countflag = true;
                                        flag = true;
                                        flag2 = true;
                                        fase = 5;

                                    }


                                    break;
#endregion

#region 1��ڃJ�E���g�_�E����
                                //�R�C�Q�C�P

                                case 5:

                                    if (Ma == true)
                                    {
                                        MusicA.Source = SE[3];
                                        Ma = false;
                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 1000)
                                    {
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                       // bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g3.png"));
                                        //�摜��\������
                                        Count.Source = Simage[3];
                                       
                                        MusicA.Play();
                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 2000)
                                    {
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                       // bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g2.png"));
                                        //�摜��\������
                                        Count.Source = Simage[2];
                                       

                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 3000)
                                    {
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                       // bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g1.png"));
                                        //�摜��\������
                                        Count.Source = Simage[1];


                                    }


                                    if (stopwatch.ElapsedMilliseconds >= 3500)
                                    {
                                        stopwatch.Stop();
                                        distanceS(startWristR, startShoulderR);
                                        stopwatch.Reset();
                                        Ma = true;
                                        MusicA.Stop();
                                        MusicB.Stop();
                                        Pflag3 = false;
                                        fase = 6;

                                    }
                                    break;
#endregion

#region 1��ڃp���`
                                case 6:
                                    //distanceR(keepWristR, keepShoulderR);
                                    //Scene.Content = Keeptime.Elapsed;
                                    if (flag == true)
                                    {
                                        MusicB.Source = SE[2];
                                        Keeptime = new Stopwatch();
                                        timeGet = 0;
                                        flag = false;
                                        Countflag = true;
                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 500)
                                    {
                                        
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                        //bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g0.png"));
                                        //�摜��\������
                                        if (flag2 == true)
                                        {
                                            Count.Source = null;
                                            FaseM.Source = Bimage[7];
                                            flag2 = false;
                                        }

                                        distance(keepWristR, keepShoulderR);
                                    }

                                    if (len > 35  && startWristR.Position.Y > startElbowR.Position.Y && startElbowR.Position.Y < startShoulderR.Position.Y * 1.2 )
                                    {

                                        if (Countflag == true)
                                        {
                                            //�X�g���[�N�J�E���g
                                            score1 += 1;
                                            Countflag = false;
                                        }
                                        // MusicB.Play();
                                        //�U���̉�
                                       
                                        if (!Pflag3)
                                        {
                                            MusicB.Play();
                                            Pflag3 = true;
                                        }
                                        //SsSave();
                                    }
                               

                                   

                                   
                                    if (stopwatch.ElapsedMilliseconds >= 6000)
                                    {
                                        FaseM.Source = null;
                                        Countflag = true;
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        MusicB.Stop();
                                        fase = 7;

                                    }

                                    break;
#endregion

#region 2��ڂ��܂�
                                case 7:
                                    //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                    //bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\Count.png"));
                                    //�摜��\������
                                    FaseM.Source = Bimage[4];
                                    if (Dot(startWristR, startElbowR, Px[0], Py[0], Pz[0], Px[1], Py[1], Pz[1]) >= level &&
                                      Dot(startElbowR, startShoulderR, Px[1], Py[1], Pz[1], Px[2], Py[2], Pz[2]) >= level &&
                                      Dot(startWristL, startElbowL, Px[3], Py[3], Pz[3], Px[4], Py[4], Pz[4]) >= level &&
                                      Dot(startElbowL, startShoulderL, Px[4], Py[4], Pz[4], Px[5], Py[5], Pz[5]) >= level && Countflag == true)
                                    {
                                        //�\���J�E���g
                                        score1 += 1;
                                        Countflag = false;
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 2000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        Countflag = true;
                                        Pflag3 = false;
                                        fase = 8;

                                    }

                                    break;
#endregion

#region 2��ڃJ�E���g�_�E����
                                //�R�C�Q�C�P
                                case 8:
                                    if (Ma == true)
                                    {
                                        MusicA.Source = SE[3];
                                        Ma = false;
                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 1000)
                                    {
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                        //bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g3.png"));
                                        //�摜��\������
                                        Count.Source = Simage[3];
                                        MusicA.Play();
                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 2000)
                                    {
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                        //bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g2.png"));
                                        //�摜��\������
                                        Count.Source = Simage[2];
                                        

                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 3000)
                                    {
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                        //bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g1.png"));
                                        //�摜��\������
                                        Count.Source = Simage[1];


                                    }


                                    if (stopwatch.ElapsedMilliseconds >= 3500)
                                    {
                                        stopwatch.Stop();
                                        distanceS2(startWristR, startShoulderR);
                                        stopwatch.Reset();
                                        MusicA.Stop();
                                        Ma = true;
                                        flag = true;
                                        flag2 = true;
                                        fase = 9;

                                    }

                                    break;
#endregion

#region 2��ڃp���`
                                //�p���`
                                case 9:
                                    //distanceR(keepWristR, keepShoulderR);
                                    //Scene.Content = Keeptime.Elapsed;
                                    if (flag == true)
                                    {
                                        MusicB.Source = SE[2];
                                        Keeptime = new Stopwatch();
                                        timeGet2 = 0;
                                        flag = false;
                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 500)
                                    {
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                        //bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g0.png"));
                                        //�摜��\������
                                       
                                        if (flag2 == true)
                                        {
                                            Count.Source = null;
                                            FaseM.Source = Bimage[7];
                                            flag2 = false;
                                        }

                                        distance2(keepWristR, keepShoulderR);
                                    }
                                  
                                    if (len2 > 35  && startWristR.Position.Y > startElbowR.Position.Y && startElbowR.Position.Y < startShoulderR.Position.Y * 1.2)
                                    {

                                        if (Countflag == true)
                                        {
                                            //�X�g���[�N�J�E���g
                                            score1 += 1;
                                            Countflag = false;
                                        }
                                        // MusicB.Play();
                                        //�U���̉�
                                        
                                        if (!Pflag3)
                                        {
                                            MusicB.Play();
                                            Pflag3 = true;
                                        }

                                        //SsSave();
                                    }
                                   

                                  

                                  

                                    if (stopwatch.ElapsedMilliseconds >= 6000)
                                    {
                                        FaseM.Source = null;
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        Countflag = true;
                                        fase = 10;

                                    }
                                    break;
#endregion

#region 3��ڂ��܂�
                                //3��ڂ��܂�
                                case 10:
                                    //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                    //bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\Count.png"));
                                    //�摜��\������
                                    FaseM.Source = Bimage[4];
                                    if (Dot(startWristR, startElbowR, Px[0], Py[0], Pz[0], Px[1], Py[1], Pz[1]) >= level &&
                                      Dot(startElbowR, startShoulderR, Px[1], Py[1], Pz[1], Px[2], Py[2], Pz[2]) >= level &&
                                      Dot(startWristL, startElbowL, Px[3], Py[3], Pz[3], Px[4], Py[4], Pz[4]) >= level &&
                                      Dot(startElbowL, startShoulderL, Px[4], Py[4], Pz[4], Px[5], Py[5], Pz[5]) >= level && Countflag == true)
                                    {
                                        //�\���J�E���g
                                        score1 += 1;
                                        Countflag = false;
                                    }


                                    if (stopwatch.ElapsedMilliseconds >= 2000)
                                    {
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        Countflag = true;
                                        Pflag3 = false;
                                        fase = 11;

                                    }

                                    break;
#endregion

#region 3��ڃJ�E���g�_�E����
                                //�R�C�Q�C�P
                                case 11:
                                    if (Ma == true)
                                    {
                                        MusicA.Source = SE[3];
                                        Ma = false;
                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 1000)
                                    {
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                        //bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g3.png"));
                                        //�摜��\������
                                        Count.Source = Simage[3];
                                        MusicA.Play();
                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 2000)
                                    {
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                        //bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g2.png"));
                                        //�摜��\������
                                        Count.Source = Simage[2];
                                        

                                    }
                                    if (stopwatch.ElapsedMilliseconds >= 3000)
                                    {
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                        //bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g1.png"));
                                        //�摜��\������
                                        Count.Source = Simage[1];


                                    }


                                    if (stopwatch.ElapsedMilliseconds >= 3500)
                                    {

                                        stopwatch.Stop();
                                        distanceS3(startWristR, startShoulderR);
                                        stopwatch.Reset();
                                        MusicA.Stop();
                                        Ma = true;
                                        flag = false;
                                        flag2 = true;
                                        fase = 12;

                                    }

                                    break;
#endregion

#region 3��ڃp���`
                                //�p���`
                                case 12:
                                    //distanceR(keepWristR, keepShoulderR);
                                    //Scene.Content = Keeptime.Elapsed;
                                    if (flag == true)
                                    {
                                        MusicA.Stop();
                                        MusicB.Source = SE[2];
                                        Keeptime = new Stopwatch();
                                        timeGet3 = 0;
                                        flag = false;
                                    }

                                   

                                    if (stopwatch.ElapsedMilliseconds >= 500)
                                    {
                                        //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                        //bmpImage2 = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\�J�E���g0.png"));
                                        //�摜��\������


                                        if (flag2 == true)
                                        {
                                            Count.Source = null;
                                            FaseM.Source = Bimage[7];
                                            flag2 = false;
                                        }

                                        distance3(keepWristR, keepShoulderR);
                                    }
                                 
                                    if (len3 > 35 && startWristR.Position.Y > startElbowR.Position.Y && startElbowR.Position.Y < startShoulderR.Position.Y * 1.2)
                                    {

                                        if (Countflag == true)
                                        {
                                            //�X�g���[�N�J�E���g
                                            score1 += 1;
                                            Countflag = false;
                                        }
                                        // MusicB.Play();
                                        //�U���̉�
                                        
                                        if (!Pflag3)
                                        {
                                            MusicB.Play();
                                            Pflag3 = true;
                                        }

                                        //SsSave();
                                    }

                                    if (stopwatch.ElapsedMilliseconds >= 6000)
                                    {
                                        FaseM.Source = null;
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        flagM = false;
                                        BGM[1].Stop();
                                        Countflag = true;
                                        fase = 13;

                                    }

                                    break;
#endregion

#region ���ʉ��
                                //���茋��
                                case 13:
                                    if (flagM == false)
                                    {
                                        flag = true;
                                        flag2 = true;
                                        
                                        BGM[0].PlayLooping();
                                        flagM = true;
                                    }
                                    
                                    //�t�@�C������BitmapImage�Ƀf�[�^��ǂݍ���
                                    //bmpImage = new BitmapImage(new Uri(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\Picture\Result.png"));
                                    //�摜��\������
                                   
                                    FaseM.Source = Bimage[5];
                                    alldistance = (KeepLen - startlen);
                                    speed = (alldistance / timeGet) * 10 *3.6;
                                    alldistance2 = (KeepLen2 - startlen2);
                                    speed2 = (alldistance2 / timeGet2) * 10 *3.6;
                                    alldistance3 = (KeepLen3 - startlen3);
                                    speed3 = (alldistance3 / timeGet3) * 10 *3.6;
                                    if (timeGet == 0)
                                    {
                                        speed = 0;

                                    }
                                    if (timeGet2 == 0)
                                    {
                                        speed2 = 0;

                                    }
                                    if (timeGet3 == 0)
                                    {
                                        speed3 = 0;

                                    }

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
                                    if (speed3 == speed && speed3 == speed2)
                                    {
                                        MostSp = 0;
                                    }

                                    if (MostSp == 1)
                                    {
                                        dis.Content = "����" + ToRoundDown(alldistance, 2) + "cm " + " ����" + timeGet + "ms " + " ���x" +ToRoundDown(speed, 2) + "km/h";
                                        Ti.Content = "�X�g���[�N " + score1 + "/6�_ �����قǗǂ��t�H�[������I";
                                        Sp.Content = "�L�� " + score2 + "�_ �����قǐL�т̂���p���`����I";
                                    }

                                    if (MostSp == 2)
                                    {
                                        dis.Content = "����" + ToRoundDown(alldistance2, 3) + "cm " + " ����" + timeGet2 + "ms " + " ���x" +ToRoundDown(speed2,2) + "km/h";

                                        Ti.Content = "�t�H�[�� " + score1 + "/6�_ �����قǗǂ��t�H�[������I";
                                        Sp.Content = "�L�� " + score2 + "�_ �����قǐL�т̂���p���`����I";
                                    }

                                    if (MostSp == 3)
                                    {
                                        dis.Content = "����" + ToRoundDown(alldistance3, 3) + "cm " + " ����" + timeGet3 + "ms " + " ���x" +ToRoundDown(speed3,2) + "km/h";
                                        Ti.Content = "�t�H�[�� " + score1 + "/6�_ �����قǗǂ��t�H�[������I";
                                        Sp.Content = "�L�� " + score2 + "�_ �����قǐL�т̂���p���`����I";
                                    }

                                    if (MostSp == 0)
                                    {
                                        dis.Content = "����" + ToRoundDown(alldistance3, 3) + "cm " + " �~�X" + timeGet3 + "ms " + " �~�X" +ToRoundDown(speed3,2) + "km/h";
                                        Ti.Content = "�t�H�[�� " + score1;
                                        Sp.Content = "�L�� " + score2;
                                    }

                                    if (fase != 13)
                                    {
                                        dis.Content = null;
                                        Ti.Content = null;
                                        Sp.Content = null;
                                        flagM = false;
                                    }
                                    

                                    if (Keyboard.IsKeyDown(Key.Enter) && stopwatch.ElapsedMilliseconds > 1500)
                                    {

                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        
                                        
                                        fase = 14;

                                    }
                                    break;
#endregion

#region ID����͂���
                                //ID�����
                                case 14:

                                    IDtime.Start();
                                    if(IDtime.ElapsedMilliseconds > 100){
                                        IDcount();
                                        IDtime.Stop();
                                        IDtime.Reset();
                                    }

                                    dis.Content = "Thank you for playing";
                                    Ti.Content = "���x�����K���ď�肭�Ȃ낤�I�I";
                                    Sp.Content = "ID����͂��Ă������� " + ID;
                                    

                                    if ( Keyboard.IsKeyDown(Key.Enter) && stopwatch.ElapsedMilliseconds > 1500)
                                    {
                                       
                                            Save1.DataSave(ID, startlen, KeepLen, alldistance, timeGet, speed, score1, score2);
                                        
                                      
                                            Save1.DataSave(ID, startlen2, KeepLen2, alldistance2, timeGet2, speed2, score1, score2);
                                        
                                      
                                            Save1.DataSave(ID, startlen3, KeepLen3, alldistance3, timeGet3, speed3, score1, score2);
                                        
                                      

                                        score1 = 0;
                                        score2 = 0;
                                        stopwatch.Stop();
                                        stopwatch.Reset();
                                        IDtime.Stop();
                                        IDtime.Reset();
                                        dis.Content = null;
                                        Ti.Content = null;
                                        Sp.Content = null;
                                        fase = 0;

                                    }
                                    break;
#endregion

                                default: break;

                            }
#endregion

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
        

         //�������oR
        public void distanceR(Joint WristR, Joint ShoulderR)
        {



            lenR = (Math.Sqrt((WristR.Position.X - ShoulderR.Position.X) * (WristR.Position.X - ShoulderR.Position.X) +
                                     (WristR.Position.Y - ShoulderR.Position.Y) * (WristR.Position.Y - ShoulderR.Position.Y) +
                                     (WristR.Position.Z - ShoulderR.Position.Z) * (WristR.Position.Z - ShoulderR.Position.Z))) * 100;

        }

        //�������oL
        public void distanceL(Joint WristL, Joint ShoulderL)
        {



            lenL = (Math.Sqrt((WristL.Position.X - ShoulderL.Position.X) * (WristL.Position.X - ShoulderL.Position.X) +
                                     (WristL.Position.Y - ShoulderL.Position.Y) * (WristL.Position.Y - ShoulderL.Position.Y) +
                                     (WristL.Position.Z - ShoulderL.Position.Z) * (WristL.Position.Z - ShoulderL.Position.Z))) * 100;

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

                score2 += 1;
            }

            if (timeGet > 0 )
            {
                
                Keeptime.Stop();
                
            }

        }

        //�ő勗�����o2���
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

        //�ő勗�����o3���
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


        //�����\���������o
        public void distanceS(Joint WristR, Joint ShoulderR)
        {


            startlen = (Math.Sqrt((WristR.Position.X - ShoulderR.Position.X) * (WristR.Position.X - ShoulderR.Position.X) +
                                     (WristR.Position.Y - ShoulderR.Position.Y) * (WristR.Position.Y - ShoulderR.Position.Y) +
                                     (WristR.Position.Z - ShoulderR.Position.Z) * (WristR.Position.Z - ShoulderR.Position.Z))) * 100;
            

        }

        //�����\���������o2���
        public void distanceS2(Joint WristR, Joint ShoulderR)
        {


            startlen2 = (Math.Sqrt((WristR.Position.X - ShoulderR.Position.X) * (WristR.Position.X - ShoulderR.Position.X) +
                                     (WristR.Position.Y - ShoulderR.Position.Y) * (WristR.Position.Y - ShoulderR.Position.Y) +
                                     (WristR.Position.Z - ShoulderR.Position.Z) * (WristR.Position.Z - ShoulderR.Position.Z))) * 100;


        }

        //�����\���������o3���
        public void distanceS3(Joint WristR, Joint ShoulderR)
        {


            startlen3 = (Math.Sqrt((WristR.Position.X - ShoulderR.Position.X) * (WristR.Position.X - ShoulderR.Position.X) +
                                     (WristR.Position.Y - ShoulderR.Position.Y) * (WristR.Position.Y - ShoulderR.Position.Y) +
                                     (WristR.Position.Z - ShoulderR.Position.Z) * (WristR.Position.Z - ShoulderR.Position.Z))) * 100;
        }

        #region ���ϔ�r
        //���ϔ�r
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
        #endregion

        #region �X�N���[���ۑ�
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
        #endregion

        //�{�^���N���b�N�ŃX�N���[���V���b�g�ۑ�
       /* private void �ۑ�_Click(object sender, RoutedEventArgs e)
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
            
        }*/

        //�|�[�Y�Z�[�u�p
        private void savePos()
        {
            Save1.PosSave2(startWristR.Position.X, startWristR.Position.Y, startWristR.Position.Z);//0
            Save1.PosSave2(startElbowR.Position.X, startElbowR.Position.Y, startElbowR.Position.Z);//1
            Save1.PosSave2(startShoulderR.Position.X, startShoulderR.Position.Y, startShoulderR.Position.Z);//2
            Save1.PosSave2(startWristL.Position.X, startWristL.Position.Y, startWristL.Position.Z);//3
            Save1.PosSave2(startElbowL.Position.X, startElbowL.Position.Y, startElbowL.Position.Z);//4
            Save1.PosSave2(startShoulderL.Position.X, startShoulderL.Position.Y, startShoulderL.Position.Z);//5
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

        //�����_�؎̂�
        public static double ToRoundDown(double dValue, int iDigits)
        {
            double dCoef = Math.Pow(10, iDigits);

            return dValue > 0 ? Math.Floor(dValue * dCoef) / dCoef :
                                Math.Ceiling(dValue * dCoef) / dCoef;
        }
        //��ʑJ��
        public void SceneChange()
        {
            if (Keyboard.IsKeyDown(Key.NumPad0) == true)
            {
                fase = 0;
            }

            if (Keyboard.IsKeyDown(Key.NumPad1) == true)
            {
                fase = 1;
            }

            if (Keyboard.IsKeyDown(Key.NumPad2) == true)
            {
                fase = 2;
            }

            if (Keyboard.IsKeyDown(Key.NumPad3) == true)
            {
                fase = 3;
            }

            if (Keyboard.IsKeyDown(Key.NumPad4) == true)
            {
                fase = 4;
            }

            if (Keyboard.IsKeyDown(Key.NumPad5) == true)
            {
                fase = 13;
            }

            if (Keyboard.IsKeyDown(Key.NumPad6) == true)
            {
                fase = 14;
            }

            if (Keyboard.IsKeyDown(Key.Escape) == true)
            {
                Environment.Exit(0);

            }


        }
        #region IDcount
        private void IDcount()
        {
            if (Keyboard.IsKeyDown(Key.D1))
            {
                ID += "1";
            }

            if (Keyboard.IsKeyDown(Key.D2))
            {
                ID += "2";
            }

            if (Keyboard.IsKeyDown(Key.D3))
            {
                ID += "3";
            }

            if (Keyboard.IsKeyDown(Key.D4))
            {
                ID += "4";
            }

            if (Keyboard.IsKeyDown(Key.D5))
            {
                ID += "5";
            }

            if (Keyboard.IsKeyDown(Key.D6))
            {
                ID += "6";
            }

            if (Keyboard.IsKeyDown(Key.D7))
            {
                ID += "7";
            }

            if (Keyboard.IsKeyDown(Key.D8))
            {
                ID += "8";
            }

            if (Keyboard.IsKeyDown(Key.D9))
            {
                ID += "9";
            }

            if (Keyboard.IsKeyDown(Key.D0))
            {
                ID += "0";
            }

            if (Keyboard.IsKeyDown(Key.Back))
            {
                ID += " ";
            }

        #endregion



        }


    }

}
