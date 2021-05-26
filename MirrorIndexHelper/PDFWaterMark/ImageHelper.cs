using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace PDF2Image
{
    class WatermarkParameter
    {
        SizeF _size;

        public SizeF Size
        {
            get { return _size; }
            set { _size = value; }
        }
        Font _font;

        public Font Font
        {
            get { return _font; }
            set { _font = value; }
        }
    }

    class ImageHelper
    {
        /// <summary>
        /// 水印在页面位置及对应字体大小
        /// </summary>
        static Dictionary<string, WatermarkParameter> _dicWatermarkSize = new Dictionary<string, WatermarkParameter>();
        /// <summary>
        /// 图片加水平垂直居中，顺时针45度旋转的文字水印
        /// </summary>
        /// <param name="bmp">要加水印的图片</param>
        /// <param name="waterstring">水印文字</param>
        /// <returns></returns>
        static public  Bitmap BitmapAddMarkstring(Bitmap bmp, string waterstring)
        {
            Graphics g = Graphics.FromImage(bmp);
            int fs = 200;
            Font f = new Font("黑体", fs);
            SizeF size;// = g.MeasureString(waterstring,f);
            WatermarkParameter wp;
            if (_dicWatermarkSize.ContainsKey(waterstring))
                wp = _dicWatermarkSize[waterstring];
            else
            {
                do
                {
                    f.Dispose();
                    f = new Font("黑体", fs);
                    size = g.MeasureString(waterstring, f);
                    fs--;
                }
                while (size.Width > bmp.Width + (int)(0.293*size.Width)/2);
                wp = new WatermarkParameter();
                wp.Font = f;
                wp.Size = size;
                _dicWatermarkSize.Add(waterstring, wp);
            }
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TranslateTransform((bmp.Width - (int)(wp.Size.Width * 0.707)) / 2, (bmp.Height - (int)(wp.Size.Height*0.293)) / 4);
            Brush bush = new SolidBrush(Color.FromArgb(85, 180, 180, 180));
            g.RotateTransform(45);
            g.DrawString(waterstring, wp.Font, bush, 0, 0);
            g.ResetTransform();
            g.Dispose();
            return bmp;
        }
    }
}
