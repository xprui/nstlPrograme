using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MirrorIndexHelper
{
    class PDFWatermark
    {
        /// <summary>
        /// PDF加密、打水印
        /// </summary>
        /// <param name="SetWaterMark">是否打水印</param>
        /// <param name="markstring">水印文字</param>
        /// <param name="pdfInput">PDF源文件</param>
        /// <param name="pdfOutput">加密、打水印后的PDF</param>
        /// <param name="userPassword">打开PDF时要输入的密码</param>
        /// <param name="ownerPassword">控制PDF的主密码</param>
        /// <param name="permissions">限制PDF操作的权限</param>
        public static void EncryptPDF(bool SetWaterMark, string markstring, string pdfInput, string pdfOutput,string userPassword,string ownerPassword,int permissions)
        {
            string watermarkedFile = pdfOutput;
            string outputPath = Path.GetDirectoryName(pdfOutput);
            if (!string.IsNullOrEmpty(outputPath) && !Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            // Creating watermark on a separate layer
            // Creating iTextSharp.text.pdf.PdfReader object to read the Existing PDF Document
  

            PdfReader reader = new PdfReader(pdfInput);
            FileStream fs = null;
            //PdfStamper stamper = null;
            using ( fs = new FileStream(watermarkedFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                // Creating iTextSharp.text.pdf.PdfStamper object to write Data from iTextSharp.text.pdf.PdfReader object to FileStream object
                PdfStamper stamper = new PdfStamper(reader, fs);

                //加密
                stamper.SetEncryption(Encoding.UTF8.GetBytes(userPassword),Encoding.UTF8.GetBytes(ownerPassword), permissions, PdfWriter.STRENGTH128BITS);
                // Getting total number of pages of the Existing Document
                if (SetWaterMark)
                {
                    int pageCount = reader.NumberOfPages;

                    // Create New Layer for Watermark
                    //PdfLayer layer = new PdfLayer("WatermarkLayer", stamper.Writer);
                    BaseFont font = BaseFont.CreateFont(System.Environment.GetEnvironmentVariable("windir") + @"\Fonts\simhei.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    // Loop through each Page
                    for (int i = 1; i <= pageCount; i++)
                    {
                        //提取文本
                        iTextSharp.text.pdf.parser.ITextExtractionStrategy strategy = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                        string text = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, i, strategy);
                        // Getting the Page Size
                        iTextSharp.text.Rectangle rect = reader.GetPageSize(i);
                        PdfContentByte cb;
                        if (text.Length > 100)
                        {
                            // Get the ContentByte object
                            cb = stamper.GetUnderContent(i);//在内容下方加水印
                        }
                        else//没有文字的页，水印浮在上面
                        {
                            cb = stamper.GetOverContent(i);//在内容上方加水印
                        }
                        // Tell the cb that the next commands should be "bound" to this new layer
                        //cb.BeginLayer(layer);
                        cb.SetFontAndSize(font, 50);
                        PdfGState gState = new PdfGState();
                        gState.FillOpacity = 0.15f;//透明度
                        cb.SetGState(gState);

                        cb.SetColorFill(SpotColor.DARK_GRAY);
                        cb.BeginText();
                        cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, markstring, rect.Width / 2, rect.Height / 2, 45f);
                        cb.EndText();
                        // Close the layer
                        //cb.EndLayer();    
                    }
                }
                stamper.Close();
                stamper.Dispose();
            }
            reader.Close();
        }

        private static void judgePDFsafe(PdfStamper stamper, PdfReader reader, FileStream fs)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// PDF解密（仅去除打开密码，权限限制编辑文档以保护水印）
        /// </summary>
        /// <param name="pdf">要解密的PDF</param>
        /// <param name="userPassword">打开密码</param>
        /// <param name="ownerPassword">权限密码</param>
        static public void PdfDeCode(string pdf,string userPassword,string ownerPassword)
        {
            PdfReader reader = new PdfReader(File.ReadAllBytes(pdf), Encoding.UTF8.GetBytes(ownerPassword));
            using (MemoryStream memStream = new MemoryStream())
            {
                PdfStamper stamper = new PdfStamper(reader, memStream);
                stamper.Close();
                reader.Close();
                File.WriteAllBytes(pdf, memStream.ToArray());
            }
            reader = new PdfReader(File.ReadAllBytes(pdf));
            FileStream fs = new FileStream(pdf, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            PdfStamper encryptor = new PdfStamper(reader,fs);
            encryptor.SetEncryption(null, Encoding.UTF8.GetBytes(ownerPassword), -9, PdfWriter.STRENGTH128BITS);
            encryptor.Close();
            encryptor.Dispose();
            fs.Close();
            fs.Dispose();
            reader.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPdf"></param>
        /// <param name="tPdf"></param>
        /// <param name="userPassword"></param>
        /// <param name="ownerPassword"></param>
        static public void PdfDeCodeTo(string sPdf,string tPdf,string userPassword,string ownerPassword)
        {
            PdfReader reader = new PdfReader(File.ReadAllBytes(sPdf), Encoding.UTF8.GetBytes(ownerPassword));
            using (MemoryStream memStream = new MemoryStream())
            {
                PdfStamper stamper = new PdfStamper(reader, memStream);
                stamper.Close();
                reader.Close();
                File.WriteAllBytes(tPdf, memStream.ToArray());
            }
            reader = new PdfReader(File.ReadAllBytes(tPdf));
            FileStream fs = new FileStream(tPdf, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            PdfStamper encryptor = new PdfStamper(reader, fs);
            encryptor.SetEncryption(null, Encoding.UTF8.GetBytes(ownerPassword), -9, PdfWriter.STRENGTH128BITS);
            encryptor.Close();
            encryptor.Dispose();
            fs.Close();
            fs.Dispose();
            reader.Close();
        }


    }

    
}
