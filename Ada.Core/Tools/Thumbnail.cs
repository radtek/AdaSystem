using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Tools
{
    public class Thumbnail
    {

        /// <summary>
        /// 制作缩略图
        /// </summary>
        /// <param name="fileName">文件名(绝对路径)</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public static string MakeThumbnailImageToBase64(string fileName, int maxWidth = 120, int maxHeight = 120)
        {
            //判断文件是否存在，是否是图片
            if (!IsImage(fileName))
            {
                return string.Empty;
            }
            byte[] imageBytes = File.ReadAllBytes(fileName);
            Image img = Image.FromStream(new MemoryStream(imageBytes));
            Size newSize = ResizeImage(img.Width, img.Height, maxWidth, maxHeight);
            string base64String;
            using (Image displayImage = new Bitmap(img, newSize))
            {
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        displayImage.Save(ms, ImageFormat.Png);
                        var bytes = ms.GetBuffer();
                        base64String = "data:image/png;base64," + Convert.ToBase64String(bytes);
                    }
                }
                finally
                {
                    img.Dispose();
                }
            }
            return base64String;
        }
        /// <summary>
        /// 计算新尺寸
        /// </summary>
        /// <param name="width">原始宽度</param>
        /// <param name="height">原始高度</param>
        /// <param name="maxWidth">最大新宽度</param>
        /// <param name="maxHeight">最大新高度</param>
        /// <returns></returns>
        private static Size ResizeImage(int width, int height, int maxWidth, int maxHeight)
        {
            //此次2012-02-05修改过=================
            if (maxWidth <= 0)
                maxWidth = width;
            if (maxHeight <= 0)
                maxHeight = height;
            //以上2012-02-05修改过=================
            decimal MAX_WIDTH = maxWidth;
            decimal MAX_HEIGHT = maxHeight;
            var aspectRatio = MAX_WIDTH / MAX_HEIGHT;

            int newWidth, newHeight;
            decimal originalWidth = width;
            decimal originalHeight = (decimal)height;

            if (originalWidth > MAX_WIDTH || originalHeight > MAX_HEIGHT)
            {
                decimal factor;
                // determine the largest factor 
                if (originalWidth / originalHeight > aspectRatio)
                {
                    factor = originalWidth / MAX_WIDTH;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
                else
                {
                    factor = originalHeight / MAX_HEIGHT;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }
            return new Size(newWidth, newHeight);
        }
        private static bool IsImage(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }
            try
            {
                Image.FromFile(fileName);
            }
            catch
            {
                return false;
            }
            return true;

        }
    }
}
