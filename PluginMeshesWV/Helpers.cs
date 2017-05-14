﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PluginMeshesWV
{
    public static class Helpers
    {
        public static void RunShell(string file, string command)
        {
            Process process = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = file;
            startInfo.Arguments = command;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = Path.GetDirectoryName(file) + "\\";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        public static Bitmap LoadImageCopy(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Bitmap loaded = new Bitmap(fs);
            Bitmap result = (Bitmap)loaded.Clone();
            fs.Close();
            return result;
        }
 
        public class BinaryReader7Bit : BinaryReader
        {
            public BinaryReader7Bit(Stream stream) : base(stream) { }
            public new int Read7BitEncodedInt()
            {
                return base.Read7BitEncodedInt();
            }
        }

        public class BinaryWriter7Bit : BinaryWriter
        {
            public BinaryWriter7Bit(Stream stream) : base(stream) { }
            public new void Write7BitEncodedInt(int i)
            {
                base.Write7BitEncodedInt(i);
            }
        }

        public static void DeleteFileIfExist(string file)
        {
            if (File.Exists(file))
                File.Delete(file);
        }

        public static bool ByteArrayCompare(byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null || b1.Length != b2.Length)
                return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return false;
            return true;
        }

        public static void WriteInt(Stream s, int i)
        {
            s.Write(BitConverter.GetBytes(i), 0, 4);
        }

        public static void WriteUInt(Stream s, uint i)
        {
            s.Write(BitConverter.GetBytes(i), 0, 4);
        }

        public static void WriteShort(Stream s, short i)
        {
            s.Write(BitConverter.GetBytes(i), 0, 2);
        }

        public static void WriteUShort(Stream s, ushort i)
        {
            s.Write(BitConverter.GetBytes(i), 0, 2);
        }

        public static void WriteLEInt(Stream s, int i)
        {
            List<byte> t = new List<byte>(BitConverter.GetBytes(i));
            t.Reverse();
            s.Write(t.ToArray(), 0, 4);
        }

        public static void WriteLEUInt(Stream s, uint i)
        {
            List<byte> t = new List<byte>(BitConverter.GetBytes(i));
            t.Reverse();
            s.Write(t.ToArray(), 0, 4);
        }

        public static void WriteLEUShort(Stream s, ushort u)
        {
            byte[] buff = BitConverter.GetBytes(u);
            buff = buff.Reverse().ToArray();
            s.Write(buff, 0, 2);
        }

        public static int ReadInt(Stream s)
        {
            byte[] buff = new byte[4];
            s.Read(buff, 0, 4);
            return BitConverter.ToInt32(buff, 0);
        }

        public static uint ReadUInt(Stream s)
        {
            byte[] buff = new byte[4];
            s.Read(buff, 0, 4);
            return BitConverter.ToUInt32(buff, 0);
        }

        public static short ReadShort(Stream s)
        {
            byte[] buff = new byte[2];
            s.Read(buff, 0, 2);
            return BitConverter.ToInt16(buff, 0);
        }

        public static ushort ReadUShort(Stream s)
        {
            byte[] buff = new byte[2];
            s.Read(buff, 0, 2);
            return BitConverter.ToUInt16(buff, 0);
        }

        public static long ReadLong(Stream s)
        {
            byte[] buff = new byte[8];
            s.Read(buff, 0, 8);
            return BitConverter.ToInt64(buff, 0);
        }

        public static ulong ReadULong(Stream s)
        {
            byte[] buff = new byte[8];
            s.Read(buff, 0, 8);
            return BitConverter.ToUInt64(buff, 0);
        }

        public static float ReadFloat(Stream s)
        {
            byte[] buff = new byte[4];
            s.Read(buff, 0, 4);
            return BitConverter.ToSingle(buff, 0);
        }

        public static int ReadLEInt(Stream s)
        {
            byte[] buff = new byte[4];
            s.Read(buff, 0, 4);
            buff = buff.Reverse().ToArray();
            return BitConverter.ToInt32(buff, 0);
        }

        public static uint ReadLEUInt(Stream s)
        {
            byte[] buff = new byte[4];
            s.Read(buff, 0, 4);
            buff = buff.Reverse().ToArray();
            return BitConverter.ToUInt32(buff, 0);
        }

        public static short ReadLEShort(Stream s)
        {
            byte[] buff = new byte[2];
            s.Read(buff, 0, 2);
            buff = buff.Reverse().ToArray();
            return BitConverter.ToInt16(buff, 0);
        }

        public static ushort ReadLEUShort(Stream s)
        {
            byte[] buff = new byte[2];
            s.Read(buff, 0, 2);
            buff = buff.Reverse().ToArray();
            return BitConverter.ToUInt16(buff, 0);
        }

        public static byte[] ReadFull(Stream s, uint size)
        {
            byte[] buff = new byte[size];
            int totalread = 0;
            while ((totalread += s.Read(buff, totalread, (int)(size - totalread))) < size) ;
            return buff;
        }

        public static string ReadNullString(Stream s)
        {
            string res = "";
            byte b;
            while ((b = (byte)s.ReadByte()) > 0 && s.Position < s.Length) res += (char)b;
            return res;
        }

        public static string ReadStringPointer(Stream s)
        {
            string result = "";
            long offset = (long)ReadULong(s);
            long pos = s.Position;
            s.Seek(offset, 0);
            result = ReadNullString(s);
            s.Seek(pos, 0);
            return result;
        }

        public static void WriteNullString(Stream s, string t)
        {
            foreach (char c in t)
                s.WriteByte((byte)c);
            s.WriteByte(0);
        }

        public static ulong ReadLEB128(Stream s)
        {
            ulong result = 0;
            byte shift = 0;
            while (true)
            {
                int i = s.ReadByte();
                if (i == -1) return result;
                byte b = (byte)i;
                result |= (ulong)((b & 0x7f) << shift);
                if ((b >> 7) == 0)
                    return result;
                shift += 7;
            }
        }

        public static void WriteLEB128(Stream s, int value)
        {
            int temp = value;
            while (temp != 0)
            {
                int val = (temp & 0x7f);
                temp >>= 7;

                if (temp > 0)
                    val |= 0x80;

                s.WriteByte((byte)val);
            }
        }

        public static int HashFNV1(string StrToHash, int hashseed = 5381, int hashprime = 33)
        {
            int Hash = hashseed;
            for (int i = 0; i < StrToHash.Length; i++)
            {
                byte b = (byte)StrToHash[i];
                Hash = (int)(Hash * hashprime) ^ b;
            }
            return Hash;
        }

        public static bool MatchByteArray(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;
            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;
            return true;
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static byte[] StringAsByteArray(string str)
        {
            MemoryStream m = new MemoryStream();
            foreach (char c in str)
                m.WriteByte((byte)c);
            return m.ToArray();
        }

        public static string ByteArrayToHexString(byte[] data, int start = 0, int len = 0)
        {
            if (data == null)
                data = new byte[0];
            StringBuilder sb = new StringBuilder();
            if (start == 0)
                foreach (byte b in data)
                    sb.Append(b.ToString("X2"));
            else
                if (start > 0 && start + len <= data.Length)
                    for (int i = start; i < start + len; i++)
                        sb.Append(data[i].ToString("X2"));
                else
                    return "";
            return sb.ToString();
        }

        public static string ByteArrayAsString(byte[] data)
        {
            if (data == null)
                data = new byte[0];
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
                sb.Append((char)b);
            return sb.ToString();
        }

        public static string MakeTabs(int count)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
                sb.Append("  ");
            return sb.ToString();
        }

        public static string GetFileNameWithOutExtension(string path)
        {
            return Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);
        }

        public static TreeNode AddPath(TreeNode t, string path, char splitter = '/')
        {
            string[] parts = path.Split(splitter);
            TreeNode f = null;
            foreach (TreeNode c in t.Nodes)
                if (c.Text == parts[0].ToLower())
                {
                    f = c;
                    break;
                }
            if (f == null) 
            {
                f = new TreeNode(parts[0].ToLower());
                t.Nodes.Add(f);
            }
            if (parts.Length > 1)
            {
                string subpath = path.Substring(parts[0].Length + 1, path.Length - 1 - parts[0].Length);
                f = AddPath(f, subpath, splitter);
            }
            return t;
        }

        public static bool isDataEntryNode(TreeNode t)
        {
            if (t == null)
                return false;
            if (t.Text == "ROOT")
                return true;
            return isDataEntryNode(t.Parent);
        }
        
        public static string GetPathFromNode(TreeNode t, string seperator = "/")
        {
            string result = t.Text;
            if (t.Parent != null)
                result = GetPathFromNode(t.Parent, seperator) + seperator + result;
            return result;
        }

        public static void ExpandTreeByLevel(TreeNode node, int level)
        {
            node.Expand();
            if (level > 0)
                foreach (TreeNode t in node.Nodes)
                    ExpandTreeByLevel(t, level - 1);
        }

        public static void SelectNext(string text, TreeView tree)
        {
            text = text.ToLower();
            TreeNode t = tree.SelectedNode;
            if (t == null && tree.Nodes.Count != 0)
                t = tree.Nodes[0];
            while (true)
            {
                TreeNode t2 = FindNext(t, text);
                if (t2 != null)
                {
                    tree.SelectedNode = t2;
                    return;
                }
                else if (t.NextNode != null)
                    t = t.NextNode;
                else if (t.Parent != null && t.Parent.NextNode != null)
                    t = t.Parent.NextNode;
                else if (t.Parent != null && t.Parent.NextNode == null)
                    while (t.Parent != null)
                    {
                        t = t.Parent;
                        if (t.Parent != null && t.Parent.NextNode != null)
                        {
                            t = t.Parent.NextNode;
                            break;
                        }
                    }
                else
                    return;
                if (t.Text.Contains(text))
                {
                    tree.SelectedNode = t;
                    return;
                }
            }
        }

        public static TreeNode GetTopMostNode(TreeNode node)
        {
            if (node.Parent == null)
                return node;
            else
                return GetTopMostNode(node.Parent);
        }

        public static TreeNode FindNext(TreeNode t, string text)
        {
            foreach (TreeNode t2 in t.Nodes)
            {
                if (t2.Text.ToLower().Contains(text))
                    return t2;
                if (t2.Nodes.Count != 0)
                {
                    TreeNode t3 = FindNext(t2, text);
                    if (t3 != null)
                        return t3;
                }
            }
            return null;
        }
        
        public static string SkipSubFolder(string path, int start)
        {
            string[] parts = path.Split('\\');
            StringBuilder sb = new StringBuilder();
            for (int i = start; i < parts.Length - 1; i++)
                sb.Append(parts[i] + "\\");
            sb.Append(parts[parts.Length - 1]);
            return sb.ToString();
        }
    }

    public static class HalfUtils
    {
        private static readonly ushort[] FloatToHalfBaseTable;
        private static readonly byte[] FloatToHalfShiftTable;
        private static readonly int[] HalfToFloatExponentTable;
        private static readonly uint[] HalfToFloatMantissaTable;
        private static readonly uint[] HalfToFloatOffsetTable;

        static HalfUtils()
        {
            int num;
            HalfToFloatMantissaTable = new uint[0x800];
            HalfToFloatExponentTable = new int[0x40];
            HalfToFloatOffsetTable = new uint[0x40];
            FloatToHalfBaseTable = new ushort[0x200];
            FloatToHalfShiftTable = new byte[0x200];
            HalfToFloatMantissaTable[0] = 0;
            for (num = 1; num < 0x400; num++)
            {
                uint num2 = (uint)(num << 13);
                uint num3 = 0;
                while ((num2 & 0x800000) == 0)
                {
                    num3 -= 0x800000;
                    num2 = num2 << 1;
                }
                num2 &= 0xff7fffff;
                num3 += 0x38800000;
                HalfToFloatMantissaTable[num] = num2 | num3;
            }
            for (num = 0x400; num < 0x800; num++)
            {
                HalfToFloatMantissaTable[num] = (uint)(0x38000000 + ((num - 0x400) << 13));
            }
            HalfToFloatExponentTable[0] = 0;
            for (num = 1; num < 0x3f; num++)
            {
                if (num >= 0x1f)
                {
                    HalfToFloatExponentTable[num] = -2147483648 + ((num - 0x20) << 0x17);
                }
                else
                {
                    HalfToFloatExponentTable[num] = num << 0x17;
                }
            }
            HalfToFloatExponentTable[0x1f] = 0x47800000;
            HalfToFloatExponentTable[0x20] = -2147483648;
            HalfToFloatExponentTable[0x3f] = -947912704;
            HalfToFloatOffsetTable[0] = 0;
            for (num = 1; num < 0x40; num++)
            {
                HalfToFloatOffsetTable[num] = 0x400;
            }
            HalfToFloatOffsetTable[0x20] = 0;
            for (num = 0; num < 0x100; num++)
            {
                int num4 = num - 0x7f;
                if (num4 < -24)
                {
                    FloatToHalfBaseTable[num] = 0;
                    FloatToHalfBaseTable[num | 0x100] = 0x8000;
                    FloatToHalfShiftTable[num] = 0x18;
                    FloatToHalfShiftTable[num | 0x100] = 0x18;
                }
                else if (num4 < -14)
                {
                    FloatToHalfBaseTable[num] = (ushort)(((int)0x400) >> (-num4 - 14));
                    FloatToHalfBaseTable[num | 0x100] = (ushort)((((int)0x400) >> (-num4 - 14)) | 0x8000);
                    FloatToHalfShiftTable[num] = Convert.ToByte((int)(-num4 - 1));
                    FloatToHalfShiftTable[num | 0x100] = Convert.ToByte((int)(-num4 - 1));
                }
                else if (num4 <= 15)
                {
                    FloatToHalfBaseTable[num] = (ushort)((num4 + 15) << 10);
                    FloatToHalfBaseTable[num | 0x100] = (ushort)(((num4 + 15) << 10) | 0x8000);
                    FloatToHalfShiftTable[num] = 13;
                    FloatToHalfShiftTable[num | 0x100] = 13;
                }
                else if (num4 >= 0x80)
                {
                    FloatToHalfBaseTable[num] = 0x7c00;
                    FloatToHalfBaseTable[num | 0x100] = 0xfc00;
                    FloatToHalfShiftTable[num] = 13;
                    FloatToHalfShiftTable[num | 0x100] = 13;
                }
                else
                {
                    FloatToHalfBaseTable[num] = 0x7c00;
                    FloatToHalfBaseTable[num | 0x100] = 0xfc00;
                    FloatToHalfShiftTable[num] = 0x18;
                    FloatToHalfShiftTable[num | 0x100] = 0x18;
                }
            }
        }

        public static ushort Pack(float f)
        {
            FloatToUint num = new FloatToUint
            {
                floatValue = f
            };
            return (ushort)(FloatToHalfBaseTable[((int)(num.uintValue >> 0x17)) & 0x1ff] + ((num.uintValue & 0x7fffff) >> (FloatToHalfShiftTable[((int)(num.uintValue >> 0x17)) & 0x1ff] & 0x1f)));
        }

        public static float Unpack(ushort h)
        {
            FloatToUint num = new FloatToUint
            {
                uintValue = HalfToFloatMantissaTable[((int)HalfToFloatOffsetTable[h >> 10]) + (h & 0x3ff)] + ((uint)HalfToFloatExponentTable[h >> 10])
            };
            return num.floatValue;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatToUint
        {
            [FieldOffset(0)]
            public float floatValue;
            [FieldOffset(0)]
            public uint uintValue;
        }
    }
}
