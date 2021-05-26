using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Xml;

namespace MirrorIndexHelper
{
    class PdfDownloader
    {
        private static event Progress onProgress;
        public static event Progress OnProgress
        {
            add { onProgress += value; }
            remove { onProgress -= value; }
        }

        //static public void DownloadAll(string tablename)
        //{
        //    //string tablename = "tmp_mirror29_201804_2009_test2Del20181109";
        //    //string logPath = Directory.GetCurrentDirectory() + "\\PDFlog.txt";
        //    bool downOrNot = false;
        //    int nowCount = 0;
        //    if (File.Exists(logPath))
        //    {
        //        readAlreadyDownLog();
        //        downOrNot = true;
        //        nowCount = al.Count;
        //    }

        //    string pdfSavePath = System.IO.Path.Combine(Common.GetAssemblyPath(), tablename, "Origin");
        //    if (!System.IO.Directory.Exists(pdfSavePath))
        //        System.IO.Directory.CreateDirectory(pdfSavePath);
        //    SqlTool mssql = new global::SqlTool(System.Configuration.ConfigurationSettings.AppSettings["dbconnectionstring"]);
        //    DocumentPicker.FullTextTransmiter picker = new DocumentPicker.FullTextTransmiter();
        //    Dictionary<string, string> dicUid2Docid = new Dictionary<string, string>();
        //    bool errorFlag = false;
        //    try
        //    {
        //        string sql = "select count(0) from " + tablename + " where uniqueid is not null";
        //        int count = Convert.ToInt32(mssql.Scaler(sql));
        //        sql = "select f_doc_id,uniqueid from " + tablename + " where uniqueid is not null";
        //        IDataReader reader = mssql.DoReader(sql);
        //        int ipgs = 0;
        //        if (downOrNot)
        //        {
        //            ipgs = nowCount;
        //        }

        //        while (reader.Read())
        //        {
        //            //ipgs++;
        //            if (null != onProgress && ipgs % 20 == 0)
        //            {
        //                onProgress(count, ipgs);
        //            }
        //            //DocumentPicker.PickUpResult pr = picker.PickupFile(Convert.ToString(reader["uniqueid"]));
        //            string fDocId = Convert.ToString(reader["f_doc_id"]);
        //            //if (pr.IsFinded && ! pr.NeedScan)//下载PDF
        //            //{

        //            ///20181030 新增 判断是否已经下载过

        //            bool alreadyDown = true;
        //            if (downOrNot)
        //            {
        //                alreadyDown = judgeDownLoadOrNot(fDocId);
        //            }
        //            if (alreadyDown)
        //            {
        //                try    
        //                {
        //                    do
        //                    {
        //                        //20201120  获取PDF接口变换 需要加密uniqueid 
        //                        string encryptStr = AES.Encrypt(Convert.ToString(reader["uniqueid"]));

        //                        //20201120  结束位置


        //                        DocumentPicker.PickUpResult pr = picker.PickupFile(Convert.ToString(reader["uniqueid"]));
        //                        if (pr.IsFinded && !pr.NeedScan)//下载PDF

        //                        {

        //                            System.IO.File.WriteAllBytes(System.IO.Path.Combine(pdfSavePath, fDocId + ".pdf"), pr.Content);

        //                            ipgs++;   ///计数放在下载后，实际下载量
        //                            writeLog(fDocId);
        //                            errorFlag = false;

        //                            //20201119  测试下载的PDF
        //                            iTextSharp.text.pdf.PdfReader pdfReader = null;
        //                            try//仓储中的部分PDF是损坏的
        //                            {
        //                                pdfReader = new iTextSharp.text.pdf.PdfReader(pdfSavePath+"\\"+ fDocId + ".pdf");
        //                                pdfReader.Close();

        //                            }
        //                            catch
        //                            {

        //                                string debugStop20201119 = string.Empty;
        //                            }
        //                            finally
        //                            {

        //                            }
        //                            //20201119 测试结束为止

        //                        }
        //                        else
        //                        {
        //                            DataUtils.UpdateDBData("delete " + tablename + " where f_doc_id='" + fDocId + "'");
        //                            errorFlag = false;
        //                        }
        //                        //System.IO.File.WriteAllBytes(System.IO.Path.Combine(pdfSavePath, Convert.ToString(reader["f_doc_id"]) + ".pdf"), pr.Content);
        //                        //writeLog(Convert.ToString(reader["f_doc_id"]));
        //                    } while (errorFlag);
        //                }
        //                catch (Exception edo)
        //                {
        //                    errorFlag = true;
        //                }
        //            }
        //            //else//删除无效数据
        //            //{
        //            //    //DataUtils.UpdateDBData("delete " + tablename + " where f_doc_id='" + Convert.ToString(reader["f_doc_id"]) + "'");
        //            //    DataUtils.UpdateDBData("delete " + tablename + " where f_doc_id='" + fDocId + "'");
        //            //}
        //        }
        //        onProgress(count, ipgs);
        //    }
        //    catch (Exception err)
        //    {
        //        //errorFlag = true;
        //        //throw err;
        //    }
        //    finally
        //    {
        //        mssql.Dispose();
        //    }


        //    MessageBox.Show("完成");
        //}


        /// <summary>
        /// 废掉20201120
        /// </summary>
        /// <param name="tablename"></param>
        //static public void DownloadAll(string tablename)
        //{
        //    //string tablename = "tmp_mirror29_201804_2009_test2Del20181109";
        //    //string logPath = Directory.GetCurrentDirectory() + "\\PDFlog.txt";
        //    bool downOrNot = false;
        //    int nowCount = 0;
        //    if (File.Exists(logPath))
        //    {
        //        readAlreadyDownLog();
        //        downOrNot = true;
        //        nowCount = al.Count;
        //    }

        //    string pdfSavePath = System.IO.Path.Combine(Common.GetAssemblyPath(), tablename, "Origin");
        //    if (!System.IO.Directory.Exists(pdfSavePath))
        //        System.IO.Directory.CreateDirectory(pdfSavePath);
        //    SqlTool mssql = new global::SqlTool(System.Configuration.ConfigurationSettings.AppSettings["dbconnectionstring"]);
        //    DocumentPicker.FullTextTransmiter picker = new DocumentPicker.FullTextTransmiter();
        //    Dictionary<string, string> dicUid2Docid = new Dictionary<string, string>();
        //    bool errorFlag = false;
        //    try
        //    {
        //        string sql = "select count(0) from " + tablename + " where uniqueid is not null";
        //        int count = Convert.ToInt32(mssql.Scaler(sql));
        //        sql = "select f_doc_id,uniqueid from " + tablename + " where uniqueid is not null";
        //        IDataReader reader = mssql.DoReader(sql);
        //        int ipgs = 0;
        //        if (downOrNot)
        //        {
        //            ipgs = nowCount;
        //        }

        //        while (reader.Read())
        //        {
        //            //ipgs++;
        //            if (null != onProgress && ipgs % 20 == 0)
        //            {
        //                onProgress(count, ipgs);
        //            }
        //            //DocumentPicker.PickUpResult pr = picker.PickupFile(Convert.ToString(reader["uniqueid"]));
        //            string fDocId = Convert.ToString(reader["f_doc_id"]);
        //            //if (pr.IsFinded && ! pr.NeedScan)//下载PDF
        //            //{

        //            ///20181030 新增 判断是否已经下载过

        //            bool alreadyDown = true;
        //            if (downOrNot)
        //            {
        //                alreadyDown = judgeDownLoadOrNot(fDocId);
        //            }
        //            if (alreadyDown)
        //            {
        //                //20201120  获取PDF接口变换 需要加密uniqueid 
        //                string encryptStr = AES.Encrypt(Convert.ToString(reader["uniqueid"]));

        //                //20201120  结束位置
        //                string pdfTarPath = System.IO.Path.Combine(pdfSavePath, fDocId + ".pdf");
        //                string xml= GetPDF(encryptStr, pdfTarPath);

        //                XmlDocument xd = new XmlDocument();
        //                xd.LoadXml(xml);
        //                bool isFinded = Convert.ToBoolean( xd.SelectSingleNode("//PickUpResult/IsFinded").InnerText);
        //                bool needScan = Convert.ToBoolean(xd.SelectSingleNode("//PickUpResult/NeedScan").InnerText);
        //                if (isFinded && !needScan)//下载PDF
        //                {
        //                    string contentStr = xd.SelectSingleNode("//PickUpResult/Content").InnerText;



        //                    //byte[] byteArray = System.Text.Encoding.Default.GetBytes(xd.SelectSingleNode("//PickUpResult/Content").InnerText);
        //                    //byte[] byteArray2 = xd.SelectSingleNode("//PickUpResult/Content").InnerText ;
        //                    //System.IO.File.WriteAllBytes(pdfTarPath, byteArray);
        //                }


        //                //20201119  测试下载的PDF
        //                iTextSharp.text.pdf.PdfReader pdfReader = null;
        //                try//仓储中的部分PDF是损坏的
        //                {
        //                    pdfReader = new iTextSharp.text.pdf.PdfReader(pdfSavePath + "\\" + fDocId + ".pdf");
        //                    pdfReader.Close();
        //                    ///计数放在下载后，实际下载量
        //                    writeLog(fDocId);
        //                    ipgs++;
        //                }
        //                catch
        //                {

        //                    string debugStop20201119 = string.Empty;
        //                }
        //                finally
        //                {

        //                }
        //                //20201119 测试结束为止

        //                try
        //                {
        //                    do
        //                    {


        //                        DocumentPicker.PickUpResult pr = picker.PickupFile(Convert.ToString(reader["uniqueid"]));
        //                        if (pr.IsFinded && !pr.NeedScan)//下载PDF

        //                        {

        //                            System.IO.File.WriteAllBytes(System.IO.Path.Combine(pdfSavePath, fDocId + ".pdf"), pr.Content);


        //                            errorFlag = false;



        //                        }
        //                        else
        //                        {
        //                            DataUtils.UpdateDBData("delete " + tablename + " where f_doc_id='" + fDocId + "'");
        //                            errorFlag = false;
        //                        }
        //                        //System.IO.File.WriteAllBytes(System.IO.Path.Combine(pdfSavePath, Convert.ToString(reader["f_doc_id"]) + ".pdf"), pr.Content);
        //                        //writeLog(Convert.ToString(reader["f_doc_id"]));
        //                    } while (errorFlag);
        //                }
        //                catch (Exception edo)
        //                {
        //                    errorFlag = true;
        //                }
        //            }
        //            //else//删除无效数据
        //            //{
        //            //    //DataUtils.UpdateDBData("delete " + tablename + " where f_doc_id='" + Convert.ToString(reader["f_doc_id"]) + "'");
        //            //    DataUtils.UpdateDBData("delete " + tablename + " where f_doc_id='" + fDocId + "'");
        //            //}
        //        }
        //        onProgress(count, ipgs);
        //    }
        //    catch (Exception err)
        //    {
        //        //errorFlag = true;
        //        //throw err;
        //    }
        //    finally
        //    {
        //        mssql.Dispose();
        //    }


        //    MessageBox.Show("完成");
        //}



        static public void DownloadAll(string tablename)
        {
            //string tablename = "tmp_mirror29_201804_2009_test2Del20181109";
            //string logPath = Directory.GetCurrentDirectory() + "\\PDFlog.txt";
            bool downOrNot = false;
            int nowCount = 0;
            if (File.Exists(logPath))
            {
                readAlreadyDownLog();
                downOrNot = true;
                nowCount = al.Count;
            }

            string pdfSavePath = System.IO.Path.Combine(Common.GetAssemblyPath(), tablename, "Origin");
            if (!System.IO.Directory.Exists(pdfSavePath))
                System.IO.Directory.CreateDirectory(pdfSavePath);
            SqlTool mssql = new global::SqlTool(System.Configuration.ConfigurationSettings.AppSettings["dbconnectionstring"]);
            DocumentPicker.FullTextTransmiter picker = new DocumentPicker.FullTextTransmiter();
            Dictionary<string, string> dicUid2Docid = new Dictionary<string, string>();
            bool errorFlag = false;
            try
            {
                string sql = "select count(0) from " + tablename + " where uniqueid is not null";
                int count = Convert.ToInt32(mssql.Scaler(sql));
                sql = "select f_doc_id,uniqueid from " + tablename + " where uniqueid is not null";
                IDataReader reader = mssql.DoReader(sql);
                int ipgs = 0;
                if (downOrNot)
                {
                    ipgs = nowCount;
                }

                while (reader.Read())
                {
                    //ipgs++;
                    if (null != onProgress && ipgs % 20 == 0)
                    {
                        onProgress(count, ipgs);
                    }
                    //DocumentPicker.PickUpResult pr = picker.PickupFile(Convert.ToString(reader["uniqueid"]));
                    string fDocId = Convert.ToString(reader["f_doc_id"]);
                    //if (pr.IsFinded && ! pr.NeedScan)//下载PDF
                    //{

                    ///20181030 新增 判断是否已经下载过

                    bool alreadyDown = true;
                    if (downOrNot)
                    {
                        alreadyDown = judgeDownLoadOrNot(fDocId);
                    }
                    if (alreadyDown)
                    {
                        try
                        {
                            do
                            {
                                //20201120  获取PDF接口变换 需要加密uniqueid 
                                string encryptStr = AES.Encrypt(Convert.ToString(reader["uniqueid"]));
                                string pdfSP = System.IO.Path.Combine(pdfSavePath, fDocId + ".pdf");
                                bool downSuccess= GetPDFInfo(encryptStr, pdfSP);
                                //20201120  结束位置
                                if (downSuccess)
                                {
                                    ipgs++;   ///计数放在下载后，实际下载量
                                    writeLog(fDocId);
                                   errorFlag = false;
                                }
                                else {
                                    DataUtils.UpdateDBData("delete " + tablename + " where f_doc_id='" + fDocId + "'");
                                    errorFlag = false;
                                }

                                
                            } while (errorFlag);
                        }
                        catch (Exception edo)
                        {
                            errorFlag = true;
                        }
                    }
                    //else//删除无效数据
                    //{
                    //    //DataUtils.UpdateDBData("delete " + tablename + " where f_doc_id='" + Convert.ToString(reader["f_doc_id"]) + "'");
                    //    DataUtils.UpdateDBData("delete " + tablename + " where f_doc_id='" + fDocId + "'");
                    //}
                }
                onProgress(count, ipgs);
            }
            catch (Exception err)
            {
                //errorFlag = true;
                //throw err;
            }
            finally
            {
                mssql.Dispose();
            }


            MessageBox.Show("完成");
        }




        static string logPath = Directory.GetCurrentDirectory() + "\\PDFlog.txt";
        /// <summary>
        /// 记录PDF --测试用
        /// </summary>
        /// <param name="info"></param>
        public static void writeLog(string info)
        {

            //string logPath = Directory.GetCurrentDirectory()+"\\PDFlog.txt";
            string[] infor = { info, };
            if (!File.Exists(logPath))
            {

                File.Create(logPath).Dispose();
                File.WriteAllLines(logPath, infor);
            }
            else
            {

                File.AppendAllLines(logPath, infor);
            }

        }


        ////////// 修改下载PDF -- 20180814 因出现下载超时中断
        static public void incrementDownPDF(string tablename)
        {
            string pdfSavePath = System.IO.Path.Combine(Common.GetAssemblyPath(), tablename, "Origin");
            //if (!System.IO.Directory.Exists(pdfSavePath))
            //    System.IO.Directory.CreateDirectory(pdfSavePath);

            ////readPDFLog 
            string PDFNameLogPath = Directory.GetCurrentDirectory() + "\\PDFNameLog.txt";
            FileStream fs = new FileStream(PDFNameLogPath, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            ArrayList alPDFName = new ArrayList();
            string line = string.Empty;
            while ((line = sr.ReadLine()) != null)
            {

                alPDFName.Add(line);
            }

            SqlTool mssql = new global::SqlTool(System.Configuration.ConfigurationSettings.AppSettings["dbconnectionstring"]);
            DocumentPicker.FullTextTransmiter picker = new DocumentPicker.FullTextTransmiter();
            Dictionary<string, string> dicUid2Docid = new Dictionary<string, string>();
            try
            {
                string sql = "select count(0) from " + tablename + " where uniqueid is not null";
                int count = Convert.ToInt32(mssql.Scaler(sql));
                sql = "select f_doc_id,uniqueid from " + tablename + " where uniqueid is not null";
                IDataReader reader = mssql.DoReader(sql);
                int ipgs = 0;
                while (reader.Read())
                {
                    ipgs++;
                    if (null != onProgress && ipgs % 20 == 0)
                    {
                        onProgress(count, ipgs);
                    }

                    ////
                    bool b = existsOrNot(alPDFName, Convert.ToString(reader["uniqueid"]));
                    if (!b)
                    {

                        DocumentPicker.PickUpResult pr = picker.PickupFile(Convert.ToString(reader["uniqueid"]));
                        if (pr.IsFinded && !pr.NeedScan)//下载PDF
                        {

                            System.IO.File.WriteAllBytes(System.IO.Path.Combine(pdfSavePath, Convert.ToString(reader["f_doc_id"]) + ".pdf"), pr.Content);
                            writeLog(Convert.ToString(reader["f_doc_id"]));
                        }
                        else//删除无效数据
                        {
                            DataUtils.UpdateDBData("delete " + tablename + " where f_doc_id='" + Convert.ToString(reader["f_doc_id"]) + "'");
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                mssql.Dispose();
            }



        }


        public static bool existsOrNot(ArrayList al, string uniqueid)
        {

            bool flag = false;
            foreach (string a in al)
            {

                if (a.Equals(uniqueid))
                {

                    flag = true;
                }

            }
            return flag;
        }



        static List<string> al = new List<string>();
        /// <summary>
        /// 判断是否已经下载过
        /// </summary>
        public static void readAlreadyDownLog()
        {

            //ArrayList al = new ArrayList();
            StreamReader sr = new StreamReader(logPath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                al.Add(line);
            }
            sr.Close();

            al = al.Distinct().ToList();
            //return al;
        }

        public static bool judgeDownLoadOrNot(string docId)
        {

            bool existsOrNot = true;
            //foreach (string di in al) {
            //    if (di == docId) {
            //        existsOrNot = false;
            //    }
            //}
            existsOrNot = !al.Contains(docId);
            return existsOrNot;
        }


        //20201120  新接口处理
        #region Post请求
        public static  string Post(string data, string uri)
        {
            //先根据用户请求的uri构造请求地址
            string serviceUrl = uri;//string.Format("{0}/{1}", this.BaseUri, uri);
            //创建Web访问对象
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            //把用户传过来的数据转成“UTF-8”的字节流
            byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);

            myRequest.Method = "POST";
            myRequest.ContentLength = buf.Length;
            myRequest.ContentType = "application/json";
            myRequest.MaximumAutomaticRedirections = 1;
            myRequest.AllowAutoRedirect = true;
            //发送请求
            Stream stream = myRequest.GetRequestStream();
            stream.Write(buf, 0, buf.Length);
            stream.Close();

            //获取接口返回值
            //通过Web访问对象获取响应内容
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            //通过响应内容流创建StreamReader对象，因为StreamReader更高级更快
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            //string returnXml = HttpUtility.UrlDecode(reader.ReadToEnd());//如果有编码问题就用这个方法
            string returnXml = reader.ReadToEnd();//利用StreamReader就可以从响应内容从头读到尾
            reader.Close();
            myResponse.Close();
            return returnXml;

        }

        public static void getXmlFromInterface() {
            string url = "http://168.160.16.105/isticopenservice/PDFTransmiter.asmx?op=PickUpFile";
            string data = "uniqueId=avZsPXtpiBzZMj93rawvKWD2W+NPWi3NyXjDMDNMqcg=&OnlyStream=false";
            try
            {
                string xml = Post(data, url);
                string debug3 = string.Empty;
            }
            catch (Exception exml) {
                string debug2 = string.Empty;
            }
            string debug = string.Empty;
        }

        public static bool  GetPDFInfo(string uid, string fn)
        {
            bool rV = false;
            var request = (HttpWebRequest)WebRequest.Create("http://168.160.16.105/isticopenservice/PDFTransmiter.asmx/PickUpFile");
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; Trident/4.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
            request.Method = "POST";
            //byte[] buff = Encoding.UTF8.GetBytes("uniqueID=" + System.Web.HttpUtility.UrlEncode(uid) + "&OnlyStream=true");
            byte[] buff = Encoding.UTF8.GetBytes("uniqueID=" + System.Web.HttpUtility.UrlEncode(uid) + "&OnlyStream=false");
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = buff.Length;
            Stream reqSteam = request.GetRequestStream();

          

            reqSteam.Write(buff, 0, buff.Length);
            reqSteam.Close();
            var res = (HttpWebResponse)request.GetResponse();
            Stream stream = res.GetResponseStream();

            StreamReader myStreamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(retString);
            bool isFinded = Convert.ToBoolean(xd.SelectSingleNode("//PickUpResult/IsFinded").InnerText);
            bool needScan = Convert.ToBoolean(xd.SelectSingleNode("//PickUpResult/NeedScan").InnerText);
            if (isFinded && !needScan)//下载PDF
            {

              rV=   GetPDF(uid, fn);
            }
            else {
                rV = false;
            }


            return rV;
        }

        public static bool GetPDF(string uid, string fn)
        {
            bool rV = false;
            var request = (HttpWebRequest)WebRequest.Create("http://168.160.16.105/isticopenservice/PDFTransmiter.asmx/PickUpFile");
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; Trident/4.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
            request.Method = "POST";
            byte[] buff = Encoding.UTF8.GetBytes("uniqueID=" + System.Web.HttpUtility.UrlEncode(uid) + "&OnlyStream=true");
            //byte[] buff = Encoding.UTF8.GetBytes("uniqueID=" + System.Web.HttpUtility.UrlEncode(uid) + "&OnlyStream=false");
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = buff.Length;
            Stream reqSteam = request.GetRequestStream();



            reqSteam.Write(buff, 0, buff.Length);
            reqSteam.Close();
            var res = (HttpWebResponse)request.GetResponse();
            Stream stream = res.GetResponseStream();


            int bufferSize = 2048;
            byte[] bytes = new byte[bufferSize];
            var fs = new FileStream(fn, FileMode.Create);
            try
            {
                int length = stream.Read(bytes, 0, bufferSize);
                while (length > 0)
                {
                    fs.Write(bytes, 0, length);
                    length = stream.Read(bytes, 0, bufferSize);
                }
                rV = true;

            }
            catch (Exception ex)
            {
                rV = false;
                //return;
            }
            finally
            {
                stream.Close();
                fs.Close();
                res.Close();
            }




            return rV;
        }


        #endregion

    }
}
