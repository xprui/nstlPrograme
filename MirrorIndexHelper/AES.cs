﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace MirrorIndexHelper
{
    class AES
    {
        private static readonly string Default_AES_Key = "istic@311";
        private static byte[] Keys = { 0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F };

        /// <summary>
        /// 对称加密算法AES RijndaelManaged加密(RijndaelManaged（AES）算法是块式加密算法)
        /// </summary>
        /// <param name="encryptString">待加密字符串</param>
        /// <returns>加密结果字符串</returns>
        public static string Encrypt(string encryptString)
        {
            return Encrypt(encryptString, Default_AES_Key);
        }

        /// <summary>
        /// 对称加密算法AES RijndaelManaged加密(RijndaelManaged（AES）算法是块式加密算法)
        /// </summary>
        /// <param name="encryptString">待加密字符串</param>
        /// <param name="encryptKey">加密密钥，须半角字符</param>
        /// <returns>加密结果字符串</returns>
        public static string Encrypt(string encryptString, string encryptKey)
        {
            encryptKey = GetSubString(encryptKey, 32, "");
            encryptKey = encryptKey.PadRight(32, ' ');

            RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Key = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 32));
            rijndaelProvider.IV = Keys;
            ICryptoTransform rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

            byte[] inputData = Encoding.UTF8.GetBytes(encryptString);
            byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);

            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// 对称加密算法AES RijndaelManaged解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
        public static string Decrypt(string decryptString)
        {
            return Decrypt(decryptString, Default_AES_Key);
        }

        /// <summary>
        /// 对称加密算法AES RijndaelManaged解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串,失败返回空</returns>
        public static string Decrypt(string decryptString, string decryptKey)
        {
            try
            {
                decryptKey = GetSubString(decryptKey, 32, "");
                decryptKey = decryptKey.PadRight(32, ' ');

                RijndaelManaged rijndaelProvider = new RijndaelManaged();
                rijndaelProvider.Key = Encoding.UTF8.GetBytes(decryptKey);
                rijndaelProvider.IV = Keys;
                ICryptoTransform rijndaelDecrypt = rijndaelProvider.CreateDecryptor();

                byte[] inputData = Convert.FromBase64String(decryptString);
                byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);

                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (Exception err)
            {
                var msg = err.Message;
                return string.Empty;
            }
        }

        /// <summary>
        /// 按字节长度(按字节,一个汉字为2个字节)取得某字符串的一部分
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <param name="length">所取字符串字节长度</param>
        /// <param name="tailString">附加字符串(当字符串不够长时，尾部所添加的字符串，一般为"...")</param>
        /// <returns>某字符串的一部分</returns>
        private static string GetSubString(string sourceString, int length, string tailString)
        {
            return GetSubString(sourceString, 0, length, tailString);
        }

        /// <summary>
        /// 按字节长度(按字节,一个汉字为2个字节)取得某字符串的一部分
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <param name="startIndex">索引位置，以0开始</param>
        /// <param name="length">所取字符串字节长度</param>
        /// <param name="tailString">附加字符串(当字符串不够长时，尾部所添加的字符串，一般为"...")</param>
        /// <returns>某字符串的一部分</returns>
        private static string GetSubString(string sourceString, int startIndex, int length, string tailString)
        {
            string myResult = sourceString;

            //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
            if (System.Text.RegularExpressions.Regex.IsMatch(sourceString, "[\u0800-\u4e00]+") ||
                System.Text.RegularExpressions.Regex.IsMatch(sourceString, "[\xAC00-\xD7A3]+"))
            {
                //当截取的起始位置超出字段串长度时
                if (startIndex >= sourceString.Length)
                {
                    return string.Empty;
                }
                else
                {
                    return sourceString.Substring(startIndex,
                                                   ((length + startIndex) > sourceString.Length) ? (sourceString.Length - startIndex) : length);
                }
            }

            //中文字符，如"中国人民abcd123"
            if (length <= 0)
            {
                return string.Empty;
            }
            byte[] bytesSource = Encoding.Default.GetBytes(sourceString);

            //当字符串长度大于起始位置
            if (bytesSource.Length > startIndex)
            {
                int endIndex = bytesSource.Length;

                //当要截取的长度在字符串的有效长度范围内
                if (bytesSource.Length > (startIndex + length))
                {
                    endIndex = length + startIndex;
                }
                else
                {   //当不在有效范围内时,只取到字符串的结尾
                    length = bytesSource.Length - startIndex;
                    tailString = "";
                }

                int[] anResultFlag = new int[length];
                int nFlag = 0;
                //字节大于127为双字节字符
                for (int i = startIndex; i < endIndex; i++)
                {
                    if (bytesSource[i] > 127)
                    {
                        nFlag++;
                        if (nFlag == 3)
                        {
                            nFlag = 1;
                        }
                    }
                    else
                    {
                        nFlag = 0;
                    }
                    anResultFlag[i] = nFlag;
                }
                //最后一个字节为双字节字符的一半
                if ((bytesSource[endIndex - 1] > 127) && (anResultFlag[length - 1] == 1))
                {
                    length = length + 1;
                }

                byte[] bsResult = new byte[length];
                Array.Copy(bytesSource, startIndex, bsResult, 0, length);
                myResult = Encoding.Default.GetString(bsResult);
                myResult = myResult + tailString;

                return myResult;
            }

            return string.Empty;

        }

    }
}
