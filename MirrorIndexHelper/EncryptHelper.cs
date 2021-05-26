using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using System.IO;
using System.Security.Cryptography;

namespace MirrorIndexHelper
{
    public class EncryptHelper
    {
        public EncryptHelper()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        #region PDF加密解密方法
        /// <summary>
        /// PDF加密 给PDF文件加密码
        /// </summary>
        /// <param name="pdfSrc">PDF文件路径</param>
        /// <param name="pdfDest">生成加密文件路径</param>
        /// <param name="strength"></param>
        /// <param name="userPassword">输入此密码不可进行修改存储和打印</param>
        /// <param name="ownerPassword">输入此密码拥有全部权限</param>
        /// <param name="permissions"></param>
        public static void EncodePDF(string pdfSrc, string pdfDest, bool strength,
            string userPassword, string ownerPassword, int permissions)
        {
            PdfReader reader = new PdfReader(pdfSrc);
            string pdfnewpath = pdfDest.Substring(0, pdfDest.LastIndexOf("\\"));
            if (!Directory.Exists(pdfnewpath))
                Directory.CreateDirectory(pdfnewpath);
            Stream os = (Stream)(new FileStream(pdfDest, FileMode.Create));
            PdfEncryptor.Encrypt(reader, os, strength, userPassword, ownerPassword, permissions);
            //permissions:
            //PdfWriter.AllowCopy;
            //PdfWriter.AllowModifyContents;
            
            //PdfEncryptor.Encrypt(reader, os, Encoding.Unicode.GetBytes(userPassword), Encoding.Unicode.GetBytes(ownerPassword), permissions, strength);
        }

        /// <summary>
        /// PDF解密 有密码的PDF文件生成为无密码的
        /// </summary>
        /// <param name="inputFile">PDF加密文件路径</param>
        /// <param name="outputFile">PDF去除密码后文件路径</param>
        /// <param name="ownerPassword">拥有全部权限的密码</param>
        public static void DecodePDF(string inputFile, string outputFile, string ownerPassword)
        {
            try
            {
                PdfReader reader = new PdfReader(inputFile, new ASCIIEncoding().GetBytes(ownerPassword));

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfStamper stamper = new PdfStamper(reader, memoryStream);
                    stamper.Close();
                    reader.Close();
                    if (!Directory.Exists(outputFile.Substring(0, outputFile.LastIndexOf("\\"))))
                        Directory.CreateDirectory(outputFile.Substring(0, outputFile.LastIndexOf("\\")));
                    File.WriteAllBytes(outputFile, memoryStream.ToArray());
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        #endregion

        #region 字符串加密解密方法
        //加密鑰匙
        private static byte[] DESKey = new byte[] { 11, 23, 93, 102, 72, 41, 18, 12 };
        //解密鑰匙
        private static byte[] DESIV = new byte[] { 75, 158, 46, 97, 78, 57, 17, 36 };
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Encode_String"></param>
        /// <returns></returns>
        public static string Encode(string Encode_String)
        {
            DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
            MemoryStream objMemoryStream = new MemoryStream();
            CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateEncryptor(DESKey, DESIV), CryptoStreamMode.Write);
            StreamWriter objStreamWriter = new StreamWriter(objCryptoStream);
            objStreamWriter.Write(Encode_String);
            objStreamWriter.Flush();
            objCryptoStream.FlushFinalBlock();
            objMemoryStream.Flush();
            return Convert.ToBase64String(objMemoryStream.GetBuffer(), 0, (int)objMemoryStream.Length);

        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Encode_String"></param>
        /// <returns></returns>
        public static string Decode(string Encode_String)
        {
            DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
            byte[] Input = Convert.FromBase64String(Encode_String);
            MemoryStream objMemoryStream = new MemoryStream(Input);
            CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateDecryptor(DESKey, DESIV), CryptoStreamMode.Read);
            StreamReader objStreamReader = new StreamReader(objCryptoStream);
            return objStreamReader.ReadToEnd();
        }
        #endregion

        #region 无用
        /// <summary>
        /// PDF解密 有密码的PDF文件生成为无密码的
        /// </summary>
        /// <param name="pdfSrc">PDF加密文件路径</param>
        /// <param name="pdfDest">PDF解密后文件路径</param>
        /// <param name="ownerPassword">拥有全部权限的密码</param>
        public static void DecodePDF1(string pdfSrc, string pdfDest, string ownerPassword)
        {

            PdfReader reader = new PdfReader(pdfSrc, Encoding.Default.GetBytes(ownerPassword));
            if (!Directory.Exists(pdfDest.Substring(0, pdfDest.LastIndexOf("\\"))))
                Directory.CreateDirectory(pdfDest.Substring(0, pdfDest.LastIndexOf("\\")));
            Stream os = (Stream)(new FileStream(pdfDest, FileMode.Create));


            PdfEncryptor.Encrypt(reader, os, null, null,
                PdfWriter.AllowAssembly | PdfWriter.AllowFillIn | PdfWriter.AllowScreenReaders | PdfWriter.AllowPrinting, false);
        }
        #endregion
    }
}
