/************************************************************
IP���ݿ⡢�ֻ������ز�ѯ�������Դ���루C#��
  Author: rssn
  Email : rssn@163.com
  QQ    : 126027268
  Blog  : http://blog.csdn.net/rssn_net/
 ************************************************************/
using System;
using System.IO;


namespace AtomLab.Utility
{
    /// <summary>
    /// IpLocator��
    /// </summary>
    public class IpLocator
    {
        // ���ķ�����IP����
        public static IpLocation GetIpLocation(string fn, string ips)
        {
            if (!File.Exists(fn))
            {
                throw new Exception("�ļ�������!");
            }
            FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader fp = new BinaryReader(fs);
            //���ļ�ͷ,��ȡ��ĩ��¼ƫ����
            int fo = fp.ReadInt32();
            int lo = fp.ReadInt32();
            //IPֵ
            uint ipv = IpStringToInt(ips);
            // ��ȡIP������¼ƫ��ֵ
            int rcOffset = getIndexOffset(fs, fp, fo, lo, ipv);
            fs.Seek(rcOffset, System.IO.SeekOrigin.Begin);

            IpLocation ipl;
            if (rcOffset >= 0)
            {
                fs.Seek(rcOffset, System.IO.SeekOrigin.Begin);
                //��ȡ��ͷIPֵ
                ipl.IpStart = fp.ReadUInt32();
                //ת����¼��
                fs.Seek(ReadInt24(fp), System.IO.SeekOrigin.Begin);
                //��ȡ��βIPֵ
                ipl.IpEnd = fp.ReadUInt32();
                ipl.Country = GetString(fs, fp);
                ipl.City = GetString(fs, fp);
            }
            else
            {
                //û�ҵ�
                ipl.IpStart = 0;
                ipl.IpEnd = 0;
                ipl.Country = "δ֪����";
                ipl.City = "δ֪��ַ";
            }
            ipl.All = ipl.Country + " " + ipl.City;
            fp.Close();
            fs.Close();
            return ipl;
        }

        // ��������: ���á����ַ�������������, ��λIP������¼λ��
        private static int getIndexOffset(FileStream fs, BinaryReader fp, int _fo, int _lo, uint ipv)
        {
            int fo = _fo, lo = _lo;
            int mo;    //�м�ƫ����
            uint mv;    //�м�ֵ
            uint fv, lv; //�߽�ֵ
            uint llv;   //�߽�ĩĩֵ
            fs.Seek(fo, System.IO.SeekOrigin.Begin);
            fv = fp.ReadUInt32();
            fs.Seek(lo, System.IO.SeekOrigin.Begin);
            lv = fp.ReadUInt32();
            //��ʱ������,ĩ��¼��ƫ����
            mo = ReadInt24(fp);
            fs.Seek(mo, System.IO.SeekOrigin.Begin);
            llv = fp.ReadUInt32();
            //�߽��⴦��
            if (ipv < fv)
                return -1;
            else if (ipv > llv)
                return -1;
            //ʹ��"���ַ�"ȷ����¼ƫ����
            do
            {
                mo = fo + (lo - fo) / 7 / 2 * 7;
                fs.Seek(mo, System.IO.SeekOrigin.Begin);
                mv = fp.ReadUInt32();
                if (ipv >= mv)
                    fo = mo;
                else
                    lo = mo;
                if (lo - fo == 7)
                    mo = lo = fo;
            } while (fo != lo);
            return mo;
        }

        // �ַ�����ֵ���ж�
        public static bool IsNumeric(string s)
        {
            if (s != null && System.Text.RegularExpressions.Regex.IsMatch(s, @"^-?\d+$"))
                return true;
            else
                return false;
        }
        // IP�ַ���->������ֵ
        public static uint IpStringToInt(string IpString)
        {
            uint Ipv = 0;
            string[] IpStringArray = IpString.Split('.');
            int i;
            uint Ipi;
            for (i = 0; i < 4 && i < IpStringArray.Length; i++)
            {
                if (IsNumeric(IpStringArray[i]))
                {
                    Ipi = (uint)Math.Abs(Convert.ToInt32(IpStringArray[i]));
                    if (Ipi > 255) Ipi = 255;
                    Ipv += Ipi << (3 - i) * 8;
                }
            }
            return Ipv;
        }
        // ������ֵ->IP�ַ���
        public static string IntToIpString(uint Ipv)
        {
            string IpString = "";
            IpString += (Ipv >> 24) + "." + ((Ipv & 0x00FF0000) >> 16) + "." + ((Ipv & 0x0000FF00) >> 8) + "." + (Ipv & 0x000000FF);
            return IpString;
        }
        // ��ȡ�ַ���
        private static string ReadString(BinaryReader fp)
        {
            byte[] TempByteArray = new byte[128];
            int i = 0;
            do
            {
                TempByteArray[i] = fp.ReadByte();
            } while (TempByteArray[i++] != '\0' && i < 128);
            return System.Text.Encoding.Default.GetString(TempByteArray).TrimEnd('\0');
        }
        // ��ȡ���ֽڵ�����
        private static int ReadInt24(BinaryReader fp)
        {
            if (fp == null) return -1;
            int ret = 0;
            ret |= (int)fp.ReadByte();
            ret |= (int)fp.ReadByte() << 8 & 0xFF00;
            ret |= (int)fp.ReadByte() << 16 & 0xFF0000;
            return ret;
        }
        // ��ȡIP���ڵ��ַ���
        private static string GetString(FileStream fs, BinaryReader fp)
        {
            byte Tag;
            int Offset;
            Tag = fp.ReadByte();
            if (Tag == 0x01)		// �ض���ģʽ1: ������Ϣ�������Ϣ����
            {
                Offset = ReadInt24(fp);
                fs.Seek(Offset, System.IO.SeekOrigin.Begin);
                return GetString(fs, fp);
            }
            else if (Tag == 0x02)	// �ض���ģʽ2: ������Ϣû���������Ϣ����
            {
                Offset = ReadInt24(fp);
                int TmpOffset = (int)fs.Position;
                fs.Seek(Offset, System.IO.SeekOrigin.Begin);
                string TmpString = GetString(fs, fp);
                fs.Seek(TmpOffset, System.IO.SeekOrigin.Begin);
                return TmpString;
            }
            else	// ���ض���: ���ģʽ
            {
                fs.Seek(-1, System.IO.SeekOrigin.Current);
                return ReadString(fp);
            }
        }

        public static string GetIpLocation(string ip)
        {
            return GetIpLocation(AppDomain.CurrentDomain.BaseDirectory + "App_data\\QQWry.Dat", ip).All;
        }
    }

    // IP��ѯ����ṹ
    public struct IpLocation
    {
        public uint IpStart;
        public uint IpEnd;
        public string Country;
        public string City;
        public string All;
    }
}