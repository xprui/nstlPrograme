using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LuceneHelper
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Lucene.Net.Analysis.KTDictSeg.KTDictSegTokenizer tokenizer = new Lucene.Net.Analysis.KTDictSeg.KTDictSegTokenizer();
            List<string> result = tokenizer.Segment("Exif是可交换图像文件的缩写，是专门为数码相机的照片设定的，可以记录数码照片的属性和拍摄数据");
            DateTime dt = new DateTime(060821023900);
            Console.Write(string.Join("\r\n", result));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);  ///20181109 增加错误提示
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            MessageBox.Show(e.ExceptionObject.ToString());
        }
    }
}
