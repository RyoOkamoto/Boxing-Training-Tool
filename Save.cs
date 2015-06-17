#Save.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Kinect;

namespace BoxingSensor
{
    public class Save1
    {


        //CSV�t�@�C���Ƀf�[�^��񏑂�����
        public static void DataSave(String a, double num, double num2, double num3, double num4, double num5, double num6,double num7, double num8)
        {
            StreamWriter sw;
            FileInfo fi;
            fi = new FileInfo(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\BoxingSensor\/data.csv");
            sw = fi.AppendText();
            sw.WriteLine(a + "," + num + "," + num2 + "," + num3 + "," + num4 + "," + num5 + "," + num6 + "," + num7 + "," + num8);
            sw.Flush();
            sw.Close();
        }


        //CSV�t�@�C���Ƀ|�[�Y��񏑂����ށi�\���j
        public static void PosSave1(double num, double num2, double num3)
        {
            StreamWriter sw;
            FileInfo fi;
            fi = new FileInfo(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\BoxingSensor\/Pose1.csv");
            sw = fi.AppendText();
            sw.WriteLine(num + "," + num2 + "," + num3);
            sw.Flush();
            sw.Close();
        }

        //CSV�t�@�C���Ƀ|�[�Y��񏑂����ށi�E�X�g���[�g�j
        public static void PosSave2(double num, double num2, double num3)
        {
            StreamWriter sw;
            FileInfo fi;
            fi = new FileInfo(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\BoxingSensor\/Pose2.csv");
            sw = fi.AppendText();
            sw.WriteLine(num + "," + num2 + "," + num3);
            sw.Flush();
            sw.Close();
        }

        //CSV�t�@�C���Ƀ|�[�Y��񏑂����ށi���W���u�j
        public static void PosSave3(double num, double num2, double num3)
        {
            StreamWriter sw;
            FileInfo fi;
            fi = new FileInfo(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\BoxingSensor\/Pose3.csv");
            sw = fi.AppendText();
            sw.WriteLine(num + "," + num2 + "," + num3);
            sw.Flush();
            sw.Close();
        }




        //CSV�t�@�C������|�[�Y��3�_�̍��W�f�[�^��ǎ����,���̒l��Ԃ�.
        public static void ReadFile(ref double []x,double []y,double []z)
        {
           
                //�t�@�C���p�X�̐錾
            FileInfo fi = new FileInfo(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\BoxingSensor\/Pose1.csv");
                //�t�@�C���ǂݍ��ݏ���
                StreamReader sr = new StreamReader(fi.OpenRead());

                //��s���f�[�^��ǎ���Ċi�[
                for (int i = 0; i < 6; i++)
                {
                    String str = sr.ReadLine();
                    String[] str2 = str.Split(',');

                    x[i] = float.Parse(str2[0]);
                    y[i] = float.Parse(str2[1]);
                    z[i] = float.Parse(str2[2]);

                    
                }
                //�t�@�C���ǂݍ��݌㏈���i�N���[�Y�j
                sr.Close();
            
        }

        //CSV�t�@�C������|�[�Y��3�_�̍��W�f�[�^��ǎ����,���̒l��Ԃ�.
        public static void ReadFile2(ref double[] x, double[] y, double[] z)
        {

            //�t�@�C���p�X�̐錾
            FileInfo fi = new FileInfo(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\BoxingSensor\/Pose2.csv");
            //�t�@�C���ǂݍ��ݏ���
            StreamReader sr = new StreamReader(fi.OpenRead());

            //��s���f�[�^��ǎ���Ċi�[
            for (int i = 0; i < 6; i++)
            {
                String str = sr.ReadLine();
                String[] str2 = str.Split(',');

                x[i] = float.Parse(str2[0]);
                y[i] = float.Parse(str2[1]);
                z[i] = float.Parse(str2[2]);


            }
            //�t�@�C���ǂݍ��݌㏈���i�N���[�Y�j
            sr.Close();

        }

        //CSV�t�@�C������|�[�Y��3�_�̍��W�f�[�^��ǎ����,���̒l��Ԃ�.
        public static void ReadFile3(ref double[] x, double[] y, double[] z)
        {

            //�t�@�C���p�X�̐錾
            FileInfo fi = new FileInfo(@"C:\Users\labohp\Documents\Visual Studio 2013\Projects\BoxingSensor\BoxingSensor\/Pose3.csv");
            //�t�@�C���ǂݍ��ݏ���
            StreamReader sr = new StreamReader(fi.OpenRead());

            //��s���f�[�^��ǎ���Ċi�[
            for (int i = 0; i < 6; i++)
            {
                String str = sr.ReadLine();
                String[] str2 = str.Split(',');

                x[i] = float.Parse(str2[0]);
                y[i] = float.Parse(str2[1]);
                z[i] = float.Parse(str2[2]);


            }
            //�t�@�C���ǂݍ��݌㏈���i�N���[�Y�j
            sr.Close();

        }









       
    }
}
