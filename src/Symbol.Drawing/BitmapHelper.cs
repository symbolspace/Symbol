/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Symbol.Drawing {
    /// <summary>
    /// Bitmap辅助类。
    /// </summary>
    public static class BitmapHelper {

        #region methods

        #region Binarization
        /// <summary>
        /// 二值化，将图像变成纯黑白
        /// </summary>
        /// <param name="image">需要调整的图像</param>
        /// <param name="tile">阀值，1-255</param>
        public unsafe static void Binarization(Bitmap image, byte tile) {
            int w = image.Width;
            int h = image.Height;
            //将灰度二位数组变成二值图像;
            BitmapData dataDest = image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            byte* p = (byte*)dataDest.Scan0;
            int offset = dataDest.Stride - w * 3;
            int max = tile * 3;
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    p[0] = p[1] = p[2] = (byte)((p[0] + p[1] + p[2]) > max ? 255 : 0);
                    p += 3;
                }
                p += offset;
            }
            image.UnlockBits(dataDest);
        }
        #endregion
        #region Light
        /// <summary>
        /// 亮度调节
        /// </summary>
        /// <param name="image">需要调整的图像</param>
        /// <param name="light">亮度,-255~255。</param>
        public static void Light(Bitmap image, int light) {
            int w = image.Width; int h = image.Height;
            int pix = 0;
            BitmapData data = image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe {
                byte* p = (byte*)data.Scan0;
                int offset = data.Stride - w * 3;
                for (int y = 0; y < h; y++) {
                    for (int x = 0; x < w; x++) {
                        for (int i = 0; i < 3; i++) {
                            pix = p[i] + light;
                            if (light < 0) {
                                p[i] = (byte)Math.Max(0, pix);
                            } else if (light > 0) {
                                p[i] = (byte)Math.Min(255, pix);
                            }

                        }
                        p += 3;
                    }
                    p += offset;
                }

            }
            image.UnlockBits(data);
        }
        #endregion
        #region Contrast
        /// <summary>
        /// 对比度调节
        /// </summary>
        /// <param name="image">需要调整的图像</param>
        /// <param name="contrast">对比度,-100~100。</param>
        public static void Contrast(Bitmap image, int contrast) {
            double pix = 0, cts = (100.0D + contrast) / 100.0D;
            cts *= cts;
            int w = image.Width, h = image.Height;
            BitmapData data = image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            unsafe {
                byte* p = (byte*)data.Scan0;
                int offset = data.Stride - w * 3;
                for (int y = 0; y < h; y++) {
                    for (int x = 0; x < w; x++) {
                        for (int i = 0; i < 3; i++) {
                            pix = ((p[i] / 255.0 - 0.5) * cts + 0.5) * 255;
                            if (pix < 0) {
                                pix = 0;
                            } else if (pix > 255) {
                                pix = 255;
                            }
                            p[i] = (byte)pix;
                        }
                        p += 3;
                    }
                    p += offset;
                }
            }

            image.UnlockBits(data);
        }
        #endregion
        #region Grayscale
        /// <summary>
        /// 灰度化，将图像变成灰色的
        /// </summary>
        /// <param name="image">需要调整的图像</param>
        public unsafe static void Grayscale(Bitmap image) {
            int w = image.Width; int h = image.Height;
            BitmapData data = image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //将原图变成灰度图像;
            byte* p = (byte*)data.Scan0;
            //byte[,] vSource = new byte[w, h];
            int offset = data.Stride - w * 3;

            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    p[0] = p[1] = p[2] = (byte)(((int)p[0] + (int)p[1] + (int)p[2]) / 3);
                    p += 3;
                }
                p += offset;
            }
            image.UnlockBits(data);
        }
        #endregion
        #region Inverse
        /// <summary>
        /// 反色
        /// </summary>
        /// <param name="image">需要处理的图像</param>
        public unsafe static void Inverse(Bitmap image) {
            int w = image.Width; int h = image.Height;
            BitmapData data = image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //将原图变成灰度图像;
            byte* p = (byte*)data.Scan0;
            int offset = data.Stride - w * 3;
            const byte max = 255;
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    p[0] = (byte)(max - p[0]);
                    p[1] = (byte)(max - p[1]);
                    p[2] = (byte)(max - p[2]);

                    p += 3;
                }
                p += offset;
            }
            image.UnlockBits(data);
        }
        #endregion


        #region ClearBackColor
        /// <summary>
        /// 清除背景色
        /// </summary>
        /// <param name="image">需要处理的图像</param>
        /// <param name="replaceColor">替换为目标颜色</param>
        public unsafe static void ClearBackColor(Bitmap image, System.Drawing.Color replaceColor) {
            System.Collections.Generic.Dictionary<int, ColorCount> colors = new System.Collections.Generic.Dictionary<int, ColorCount>();
            ColorCount maxC = null;

            int w = image.Width; int h = image.Height;
            BitmapData data = image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            byte* p = (byte*)data.Scan0;
            byte* pOri = p;
            int offset = data.Stride - w * 3;

            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    int a = ColorHelper.ToArgb(p[0], p[1], p[2]);
                    ColorCount c = null;
                    if (colors.TryGetValue(a, out c)) {
                        c.Points.Add(new Point(x, y));
                    } else {
                        c = new ColorCount() { Color = a };
                        c.Points.Add(new Point(x, y));
                        colors.Add(a, c);
                    }
                    if (maxC == null || maxC.Points.Count < c.Points.Count) {
                        maxC = c;
                    }
                    p += 3;
                }
                p += offset;
            }
            p = pOri;
            if (maxC != null) {
                foreach (Point item in maxC.Points) {
                    p = pOri + item.X * 3 + item.Y * w * 3 + item.Y * offset;
                    p[0] = replaceColor.R;
                    p[1] = replaceColor.G;
                    p[2] = replaceColor.B;
                }
            }

            image.UnlockBits(data);
        }
        /// <summary>
        /// 清除背景色
        /// </summary>
        /// <param name="image">需要处理的图像</param>
        /// <param name="backColor">需要清除的颜色</param>
        /// <param name="replaceColor">替换为目标颜色</param>
        public unsafe static void ClearBackColor(Bitmap image, System.Drawing.Color backColor, System.Drawing.Color replaceColor) {
            int backColorArgb = ColorHelper.ToArgb(backColor.R, backColor.G, backColor.B);

            int w = image.Width; int h = image.Height;
            BitmapData data = image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            byte* p = (byte*)data.Scan0;
            int offset = data.Stride - w * 3;

            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    int a = ColorHelper.ToArgb(p[0], p[1], p[2]);
                    if (a == backColorArgb) {
                        p[0] = replaceColor.R;
                        p[1] = replaceColor.G;
                        p[2] = replaceColor.B;
                    }
                    p += 3;
                }
                p += offset;
            }
            image.UnlockBits(data);

        }
        class ColorCount {
            public int Color;
            public System.Collections.Generic.List<Point> Points;
            public ColorCount() {
                Points = new System.Collections.Generic.List<Point>();
            }
        }
        #endregion
        #region ClearBorder
        /// <summary>
        /// 清除图像边框（边缘清除为白色，不会对图像大小有影响）
        /// </summary>
        /// <param name="image">需要处理的图像</param>
        /// <param name="all">所有边框宽度</param>
        public static void ClearBorder(Bitmap image, int all) {
            ClearBorder(image, all, all, all, all);
        }
        /// <summary>
        /// 清除图像边框（边缘清除为白色，不会对图像大小有影响）
        /// </summary>
        /// <param name="image">需要处理的图像</param>
        /// <param name="left">左边宽度</param>
        /// <param name="top">上边宽度</param>
        /// <param name="right">右边宽度</param>
        /// <param name="bottom">下边宽度</param>
        public unsafe static void ClearBorder(Bitmap image, int left, int top, int right, int bottom) {
            int w = image.Width; int h = image.Height;
            BitmapData data = image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //将原图变成灰度图像;
            byte* p = (byte*)data.Scan0;
            byte* p2 = p;
            //byte[,] vSource = new byte[w, h];
            int offset = data.Stride - w * 3;

            int x = 0;
            int y = 0;
            int x1 = 0;
            int x2 = w - 1;
            if (left > 0) {
                x1 = left;
                for (x = 0; x < left; x++) {
                    for (y = 0; y < h; y++) {
                        p2 = p + x * 3 + y * w * 3 + y * offset;
                        p2[0] = p2[1] = p2[2] = 255;
                    }
                }
            }
            if (right > 0) {
                x2 = w - right;
                for (x = w - right; x < w; x++) {
                    for (y = 0; y < h; y++) {
                        p2 = p + x * 3 + y * w * 3 + y * offset;
                        p2[0] = p2[1] = p2[2] = 255;
                    }
                }
            }
            if (top > 0) {
                for (x = x1; x < x2; x++) {
                    for (y = 0; y < top; y++) {
                        p2 = p + x * 3 + y * w * 3 + y * offset;
                        p2[0] = p2[1] = p2[2] = 255;
                    }
                }
            }
            if (bottom > 0) {
                for (x = x1; x < x2; x++) {
                    for (y = h - bottom; y < h; y++) {
                        p2 = p + x * 3 + y * w * 3 + y * offset;
                        p2[0] = p2[1] = p2[2] = 255;
                    }
                }
            }
            image.UnlockBits(data);
        }
        #endregion
        #region ClearNoise
        /// <summary>
        /// 清除杂点
        /// </summary>
        /// <param name="image">需要调整的图像</param>
        /// <param name="count">杂点数量,1-9。</param>
        public static void ClearNoise(Bitmap image, int count) {
            int w = image.Width, h = image.Height;
            System.Drawing.Color emptyColor = System.Drawing.Color.White;
            int emptyColorValue = emptyColor.ToArgb();
            //for (int y = 0; y < h; y++) {
            //    for (int x = 0; x < w; x++) {

            //        if (image.GetPixel(x, y).ToArgb() != emptyColorValue) {
            //            int[] cl = GetBitmapColors3x3_Argb(image, x, y, emptyColorValue);
            //            int n0 =
            //                (cl[0] != emptyColorValue ? 1 : 0) +
            //                (cl[1] != emptyColorValue ? 1 : 0) +
            //                (cl[2] != emptyColorValue ? 1 : 0) +
            //                (cl[3] != emptyColorValue ? 1 : 0) +
            //                (cl[5] != emptyColorValue ? 1 : 0) +
            //                (cl[6] != emptyColorValue ? 1 : 0) +
            //                (cl[7] != emptyColorValue ? 1 : 0) +
            //                (cl[8] != emptyColorValue ? 1 : 0) + 1;

            //            if (n0 <= count) {
            //                image.SetPixel(x, y, emptyColor);
            //            }
            //        }

            //    }// for x
            //}// for y


            BitmapData data = image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe {
                byte* p = (byte*)data.Scan0;
                byte* p2 = p;
                int offset = data.Stride - w * 3;
                for (int x = 0; x < w; x++) {
                    for (int y = 0; y < h; y++) {
                        p2 = p + x * 3 + y * w * 3 + y * offset;
                        int argb = ColorHelper.ToArgb(p2[0], p2[1], p2[2]);
                        if (argb == emptyColorValue)
                            continue;
                        int[] cl = Symbol.Drawing.BitmapHelper.GetBitmapColors3x3_Argb(p, offset, w, h, x, y, emptyColorValue);
                        int n0 =
                            (cl[0] != emptyColorValue ? 1 : 0) +
                            (cl[1] != emptyColorValue ? 1 : 0) +
                            (cl[2] != emptyColorValue ? 1 : 0) +
                            (cl[3] != emptyColorValue ? 1 : 0) +
                            (cl[5] != emptyColorValue ? 1 : 0) +
                            (cl[6] != emptyColorValue ? 1 : 0) +
                            (cl[7] != emptyColorValue ? 1 : 0) +
                            (cl[8] != emptyColorValue ? 1 : 0) + 1;

                        if (n0 <= count) {
                            //image.SetPixel(x, y, emptyColor);
                            p2[0] = emptyColor.R;
                            p2[1] = emptyColor.G;
                            p2[2] = emptyColor.B;
                        }

                        //p += 3;
                    }
                    //p += offset;
                }

            }
            image.UnlockBits(data);
        }
        #endregion
        #region MonochromeHSB8
        /// <summary>
        /// 单色Hsb 8色
        /// </summary>
        /// <param name="image">需要处理的图像</param>
        public static unsafe void MonochromeHSB8(Bitmap image) {
            int w = image.Width; int h = image.Height;
            BitmapData data = image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            byte* p = (byte*)data.Scan0;
            int offset = data.Stride - w * 3;
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    HsbColor hsb = HsbColor.FromRGB(p[0], p[1], p[2]);


                    if (hsb.Brightness < 0.35F) {
                        //黑色
                        p[0] = p[1] = p[2] = 0;
                        //灰度 0
                    } else if (hsb.Saturation < 0.15F && hsb.Brightness > 0.75F) {
                        //白色
                        p[0] = p[1] = p[2] = 255;
                        //灰度 255
                    } else if (hsb.Saturation < 0.15F && 0.35F < hsb.Brightness && hsb.Brightness < 0.75F) {
                        //灰色
                        p[0] = p[1] = p[2] = 128;
                        //灰度 128
                    } else if (hsb.Brightness >= 0.35F && hsb.Saturation >= 0.15F) {
                        //彩色

                        if ((hsb.Hue >= 0 && hsb.Hue < 15) || (hsb.Hue >= 340 && hsb.Hue < 360)) {
                            //红色相近
                            p[0] = 255; p[1] = 0; p[2] = 0;
                            //灰度 40
                        } else if (hsb.Hue >= 15 && hsb.Hue < 75) {
                            //黄色相近
                            p[0] = 255; p[1] = 255; p[2] = 0;
                            //灰度 80
                        } else if (hsb.Hue >= 75 && hsb.Hue < 150) {
                            //绿色相近
                            p[0] = 0; p[1] = 255; p[2] = 0;
                            //灰度 120
                        } else if (hsb.Hue >= 150 && hsb.Hue < 185) {
                            //青色相近
                            p[0] = 0; p[1] = 255; p[2] = 255;
                            //灰度 160
                        } else if (hsb.Hue >= 185 && hsb.Hue < 270) {
                            //蓝色相近
                            p[0] = 0; p[1] = 0; p[2] = 255;
                            //灰度 200
                        } else if (hsb.Hue >= 270 && hsb.Hue < 340) {
                            //洋红
                            p[0] = 255; p[1] = 0; p[2] = 255;
                            //灰度 220
                        } else {
                            // * 紫色
                            p[0] = 128; p[1] = 0; p[2] = 128;
                            //灰度 180
                        }

                    }
                    p += 3;
                }
                p += offset;
            }
            image.UnlockBits(data);
        }
        #region c code
        /*

 //  黑白
            //黑色
            if(V<0.35)          
            {
                ((uchar*)(dstColorSegByColorGray->imageData + y*dstColorSegByColorGray->widthStep))[x]
                    = 0;  //灰度
                ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels]
                    = 0;  //B
                ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+1]
                    = 0;  //G
                ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+2]
                    = 0;  //R
            }          
            //白色
            if(S<0.15 && V>0.75)
            {
                ((uchar*)(dstColorSegByColorGray->imageData + y*dstColorSegByColorGray->widthStep))[x]
                    = 255;  //灰度
                ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels]
                    = 255;  //B
                ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+1]
                    = 255;  //G
                ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+2]
                    = 255;  //R
            }
             
            //灰色
            if(S<0.15 && 0.35<V && V<0.75)
            {
                ((uchar*)(dstColorSegByColorGray->imageData + y*dstColorSegByColorGray->widthStep))[x]
                    = 128;  //灰度
                ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels]
                    = 128;  //B
                ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+1]
                    = 128;  //G
                ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+2]
                    = 128;  //R
            }
 
            //  彩色
            if(V>=0.35 && S>=0.15)
            {
                //红色相近
                if((H>=0 && H<15) || (H>=340 && H<360))
                {
                    ((uchar*)(dstColorSegByColorGray->imageData + y*dstColorSegByColorGray->widthStep))[x]
                        = 40;  //灰度
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels]
                        = 0;  //B
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+1]
                        = 0;  //G
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+2]
                        = 255;  //R                
                }
                //黄色相近
                else if(H>=15 && H<75)
                {
                    ((uchar*)(dstColorSegByColorGray->imageData + y*dstColorSegByColorGray->widthStep))[x]
                        = 80;  //灰度
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels]
                        = 0;  //B
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+1]
                        = 255;  //G
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+2]
                        = 255;  //R
                }
                //绿色相近
                else if(H>=75 && H<150)
                {
                    ((uchar*)(dstColorSegByColorGray->imageData + y*dstColorSegByColorGray->widthStep))[x]
                        = 120;  //灰度
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels]
                        = 0;  //B
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+1]
                        = 255;  //G
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+2]
                        = 0;  //R                  
                }
                ///青色相近
                else if(H>=150 && H<185)
                {
                    ((uchar*)(dstColorSegByColorGray->imageData + y*dstColorSegByColorGray->widthStep))[x]
                        = 160;  //灰度
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels]
                        = 255;  //B
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+1]
                        = 255;  //G
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+2]
                        = 0;  //R
                }//蓝色相近
                else if(H>=185 && H<270)
                {
                    ((uchar*)(dstColorSegByColorGray->imageData + y*dstColorSegByColorGray->widthStep))[x]
                        = 200;  //灰度
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels]
                        = 255;  //B
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+1]
                        = 0;  //G
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+2]
                        = 0;  //R                  
                }//洋红：270-340
                else if(H>=270 && H<340)
                {
                    ((uchar*)(dstColorSegByColorGray->imageData + y*dstColorSegByColorGray->widthStep))[x]
                        = 220;  //灰度
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels]
                        = 255;  //B
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+1]
                        = 0;  //G
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+2]
                        = 255;  //R                
                }
                else
                {
                    ((uchar*)(dstColorSegByColorGray->imageData + y*dstColorSegByColorGray->widthStep))[x]
                        = 180;  //灰度
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels]
                        = 128;  //B  //紫色Purple
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+1]
                        = 0;  //G
                    ((uchar*)(dstColorSegByColor->imageData + y*dstColorSegByColor->widthStep))[x*dstColorSegByColor->nChannels+2]
                        = 128;  //R
                }*/
        #endregion
        #endregion


        #region Thinning
        /// <summary>
        /// 细化图像（需要二值化图像）
        /// </summary>
        /// <param name="image">需要处理的二值化图像。</param>
        public unsafe static void Thinning(Bitmap image) {
            int w = image.Width, h = image.Height;
            sbyte[] vArray = new sbyte[w * h];

            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    vArray[w * h - (x + y * w) - 1] = (sbyte)(image.GetPixel(x, y).R == 0 ? 1 : 0);
                }// end of x
            }// end of y
            vArray = ThinnerRosenfeld(vArray, w, h);
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    image.SetPixel(x, y, vArray[w * h - (x + y * w) - 1] == 0 ? Color.White : Color.Black);
                }//for x
            }//for y
        }

        /// <summary>
        /// 图像细化主函数,细化图像
        /// </summary>
        /// <param name="image">始终为一个一给图像数组(倒序式)</param>
        /// <param name="lx">图像宽</param>
        /// <param name="ly">图像高</param>
        /// <returns>返回细化后的数组</returns>
        private unsafe static sbyte[] ThinnerRosenfeld(sbyte[] image, long lx, long ly) {
            sbyte[] f;
            sbyte[] g;
            sbyte[] n = new sbyte[10];
            sbyte[] a = new sbyte[] { 0, -1, 1, 0, 0 };
            sbyte[] b = new sbyte[] { 0, 0, 0, 1, -1 };
            sbyte nrnd, cond, n48, n26, n24, n46, n68, n82, n123, n345, n567, n781;
            short k, shori;
            long i, j;
            long ii, jj, kk, kk1, kk2, kk3, size;
            size = ly * lx;

            //g = (sbyte *)malloc(size);
            g = new sbyte[size];
            if (g == null) {
                Console.WriteLine("error in alocating mmeory!\n");
                return null;
            }

            f = image;
            //f=
            for (kk = 0L; kk < size; kk++) {
                g[kk] = f[kk];
            }

            do {
                shori = 0;
                for (k = 1; k <= 4; k++) {
                    for (i = 1; i < ly - 1; i++) {
                        ii = i + a[k];

                        for (j = 1; j < lx - 1; j++) {
                            kk = i * lx + j;

                            if (f[kk] == 0)
                                continue;

                            jj = j + b[k];
                            kk1 = ii * lx + jj;

                            if (f[kk1] != 0)
                                continue;

                            kk1 = kk - lx - 1;
                            kk2 = kk1 + 1;
                            kk3 = kk2 + 1;
                            n[3] = f[kk1];
                            n[2] = f[kk2];
                            n[1] = f[kk3];
                            kk1 = kk - 1;
                            kk3 = kk + 1;
                            n[4] = f[kk1];
                            n[8] = f[kk3];
                            kk1 = kk + lx - 1;
                            kk2 = kk1 + 1;
                            kk3 = kk2 + 1;
                            n[5] = f[kk1];
                            n[6] = f[kk2];
                            n[7] = f[kk3];

                            nrnd = (sbyte)(n[1] + n[2] + n[3] + n[4]
                                + n[5] + n[6] + n[7] + n[8]);
                            if (nrnd <= 1)
                                continue;

                            cond = 0;
                            n48 = (sbyte)(n[4] + n[8]);
                            n26 = (sbyte)(n[2] + n[6]);
                            n24 = (sbyte)(n[2] + n[4]);
                            n46 = (sbyte)(n[4] + n[6]);
                            n68 = (sbyte)(n[6] + n[8]);
                            n82 = (sbyte)(n[8] + n[2]);
                            n123 = (sbyte)(n[1] + n[2] + n[3]);
                            n345 = (sbyte)(n[3] + n[4] + n[5]);
                            n567 = (sbyte)(n[5] + n[6] + n[7]);
                            n781 = (sbyte)(n[7] + n[8] + n[1]);

                            if (n[2] == 1 && n48 == 0 && n567 > 0) {
                                if (cond == 0)
                                    continue;
                                g[kk] = 0;
                                shori = 1;
                                continue;
                            }

                            if (n[6] == 1 && n48 == 0 && n123 > 0) {
                                if (cond == 0)
                                    continue;
                                g[kk] = 0;
                                shori = 1;
                                continue;
                            }

                            if (n[8] == 1 && n26 == 0 && n345 > 0) {
                                if (cond == 0)
                                    continue;
                                g[kk] = 0;
                                shori = 1;
                                continue;
                            }

                            if (n[4] == 1 && n26 == 0 && n781 > 0) {
                                if (cond == 0)
                                    continue;
                                g[kk] = 0;
                                shori = 1;
                                continue;
                            }

                            if (n[5] == 1 && n46 == 0) {
                                if (cond == 0)
                                    continue;
                                g[kk] = 0;
                                shori = 1;
                                continue;
                            }

                            if (n[7] == 1 && n68 == 0) {
                                if (cond == 0)
                                    continue;
                                g[kk] = 0;
                                shori = 1;
                                continue;
                            }

                            if (n[1] == 1 && n82 == 0) {
                                if (cond == 0)
                                    continue;
                                g[kk] = 0;
                                shori = 1;
                                continue;
                            }

                            if (n[3] == 1 && n24 == 0) {
                                if (cond == 0)
                                    continue;
                                g[kk] = 0;
                                shori = 1;
                                continue;
                            }

                            cond = 1;
                            if (cond == 0)
                                continue;
                            g[kk] = 0;
                            shori = 1;
                        }
                    }

                    for (i = 0; i < ly; i++) {
                        for (j = 0; j < lx; j++) {
                            kk = i * lx + j;
                            f[kk] = g[kk];
                        }
                    }
                }
            } while (shori != 0);

            //free(g);
            return f;
        }

        #endregion
        #region XStrikethrough
        /// <summary>
        /// 横向删除线
        /// </summary>
        /// <param name="image">需要处理的图像</param>
        /// <param name="span">跨度（百分比小数）</param>
        /// <param name="continuousDegree">连续度（像素）</param>
        /// <param name="backColor">背景色</param>
        public unsafe static void XStrikethrough(Bitmap image, float span, int continuousDegree, System.Drawing.Color backColor) {
            int backColorArgb = ColorHelper.ToArgb(backColor.R, backColor.G, backColor.B);


            int w = image.Width; int h = image.Height;
            BitmapData data = image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            byte* p = (byte*)data.Scan0;
            byte* pOri = p;
            int offset = data.Stride - w * 3;

            ImagePointToIndexHandler pointToIndexHandler = (p1, p2) => {
                if (p1 < 0 || p2 < 0 || p1 >= w || p2 >= h)
                    return -1;
                return p1 * 3 + p2 * w * 3 + p2 * offset;
            };
            System.Collections.Generic.Dictionary<int, XStrikethroughLine> lines = new System.Collections.Generic.Dictionary<int, XStrikethroughLine>();
            for (int x = 0; x < w; x++) {
                for (int y = 0; y < h; y++) {
                    int index = pointToIndexHandler(x, y);
                    p = pOri + index;
                    int a = ColorHelper.ToArgb(p[0], p[1], p[2]);
                    if (a == backColorArgb)
                        continue;
                    XStrikethroughLine line = null;
                    if (x > 0) {
                        var o = LinqHelper.FirstOrDefault(
                            LinqHelper.ThenBy(
                                LinqHelper.OrderBy(
                                    LinqHelper.Where(
                                        LinqHelper.Select(
                                            lines.Values,
                                            p1 => new { p1, c = p1.Continuous(x, y, continuousDegree) }),
                                        p1 => p1.c != int.MinValue),
                                    p1 => p1.c),
                                p1 => p1.p1.Y)
                            );
                        if (o != null)
                            line = o.p1;
                    }
                    XStrikethroughLinePoint point = new XStrikethroughLinePoint() {
                        X = x,
                        Y = y,
                        Index = index
                    };
                    if (y > 0) {
                        int topPIndex = pointToIndexHandler(x, y - 1);
                        byte* topP = pOri + topPIndex;
                        point.TopPoint = new ColorScanStridePoint() {
                            X = x,
                            Y = y - 1,
                            Index = topPIndex,
                            R = topP[0],
                            G = topP[1],
                            B = topP[2],
                            Color = ColorHelper.ToArgb(topP[0], topP[1], topP[2]),
                        };
                    }
                    if (y < h - 1) {
                        int bottomPIndex = pointToIndexHandler(x, y + 1);
                        byte* bottomP = pOri + bottomPIndex;
                        point.BottomPoint = new ColorScanStridePoint() {
                            X = x,
                            Y = y + 1,
                            Index = bottomPIndex,
                            R = bottomP[0],
                            G = bottomP[1],
                            B = bottomP[2],
                            Color = ColorHelper.ToArgb(bottomP[0], bottomP[1], bottomP[2]),
                        };
                    }
                    if (line == null) {
                        line = new XStrikethroughLine() {
                            X = x,
                            Y = y,
                            Color = a,
                            R = p[0],
                            G = p[1],
                            B = p[2],
                            Width = 1,
                            Height = 1,
                            Right = x + 1,
                            Bottom = y + 1,
                            Index = index,
                            ImagePointToIndexHandler = pointToIndexHandler,
                        };
                        line.Points.Add(index, point);
                        lines.Add(index, line);
                    } else {
                        if (x > line.X) {
                            line.Width = x - line.X + 1;
                            line.Right = x;
                        }
                        if (y > line.Bottom) {
                            line.Height = y - line.Y + 1;
                            line.Bottom = y;
                        }
                        line.Points.Add(index, point);
                    }
                }
            }
            System.Collections.Generic.IEnumerable<XStrikethroughLine> q =
                 LinqHelper.Select(
                     LinqHelper.ThenBy(
                         LinqHelper.OrderByDescending(
                             LinqHelper.Where(
                                 LinqHelper.Select(
                                     lines.Values,
                                     p1 => new { p1, c = ((float)p1.Width) / w }),
                                 p1 => p1.c >= span),
                             p1 => p1.c),
                         p1 => p1.p1.X),
                     p1 => p1.p1);
            foreach (var item in q) {
                foreach (XStrikethroughLinePoint point in item.Points.Values) {
                    p = pOri + point.Index;
                    XStrikethrough_SetColor(p, pOri, point.X, point.Y, backColorArgb, pointToIndexHandler);
                    //ColorScanStridePoint colorPoint = point.TopPoint ?? point.BottomPoint;
                    ////p[0] = 255;
                    ////p[1] = 0; p[2] = 0;
                    //p[0] = colorPoint.R;
                    //p[1] = colorPoint.G;
                    //p[2] = colorPoint.B;
                    ////p[0] = p[1] = p[2] = 255;
                }
            }

            image.UnlockBits(data);

        }
        static unsafe void XStrikethrough_SetColor(byte* p, byte* pOri, int x, int y, int backColorArgb, ImagePointToIndexHandler pointToIndexHandler) {
            int pArgb = ColorHelper.ToArgb(p[0], p[1], p[2]);
            int top = pointToIndexHandler(x, y - 1);

            if (top != -1) {
                byte* p2 = pOri + top;
                int a = ColorHelper.ToArgb(p2[0], p2[1], p2[2]);
                if (a != backColorArgb && a != pArgb) {
                    p[0] = p2[0];
                    p[1] = p2[1];
                    p[2] = p2[2];
                    return;
                }
            }



            int bottom = pointToIndexHandler(x, y + 1);
            if (bottom != -1) {
                byte* p2 = pOri + bottom;
                int a = ColorHelper.ToArgb(p2[0], p2[1], p2[2]);
                if (a != backColorArgb && a != pArgb) {
                    p[0] = p2[0];
                    p[1] = p2[1];
                    p[2] = p2[2];
                    return;
                }
            }
            int left = pointToIndexHandler(x - 1, y);
            bool isEmpty = true;
            if (left != -1) {
                byte* p2 = pOri + left;
                int a = ColorHelper.ToArgb(p2[0], p2[1], p2[2]);
                if (a != backColorArgb && a != pArgb) {
                    isEmpty = false;
                    //p[0] = p2[0];
                    //p[1] = p2[1];
                    //p[2] = p2[2];
                    return;
                }
            }
            if (isEmpty) {
                p[0] = p[1] = p[2] = 255;
            }
        }
        delegate int ImagePointToIndexHandler(int x, int y);
        [System.Diagnostics.DebuggerDisplay("({X},{Y}),{Points.Count}")]
        class XStrikethroughLine : ColorScanStridePoint {
            public System.Collections.Generic.Dictionary<int, XStrikethroughLinePoint> Points;

            public int Width;
            public int Height;
            public int Right;
            public int Bottom;

            public ImagePointToIndexHandler ImagePointToIndexHandler;


            public XStrikethroughLine() {
                Points = new System.Collections.Generic.Dictionary<int, XStrikethroughLinePoint>();
            }

            public int Continuous(int x, int y, int degree) {
                if (Points.Count == 0)
                    return int.MinValue;
                int xStart = x - 1;
                int yStart = y;

                if (xStart < 0)
                    return int.MinValue;
                //现在先只实现左侧横向的，不考虑圆形辐射
                degree++;
                for (int i = 0; i < degree; i++) {
                    if (Find(xStart - i, y) != null)
                        return i * degree - 1;
                    if (Find(xStart - i, y - 1) != null)
                        return i * degree;
                    if (Find(xStart - i, y + 1) != null)
                        return i * degree + 1;
                }
                return int.MinValue;
            }
            public XStrikethroughLinePoint Find(int index) {
                XStrikethroughLinePoint result = null;
                if (Points.TryGetValue(index, out result))
                    return result;
                return null;
            }
            public XStrikethroughLinePoint Find(int x, int y) {
                int index = ImagePointToIndexHandler(x, y);
                return Find(index);
            }
        }
        class XStrikethroughLinePoint : ScanStridePoint {
            public ColorScanStridePoint TopPoint;
            public ColorScanStridePoint BottomPoint;
        }//nearby
        class ScanStridePoint {
            public int Index;
            public int X;
            public int Y;
        }
        //public 
        class ColorScanStridePoint : ScanStridePoint {
            public int Color;
            public byte R;
            public byte G;
            public byte B;

        }
        #endregion

        #region GetBitmapColors3x3
        private static readonly int[][] _pos3x3_finds = new int[][] { 
                     //left|top               top            right|top
                     //  0                     1                 2
            new int[]{-1,-1},      new int[]{ 0,-1},  new int[]{ 1,-1 },
                     //left                 center             right
                     //  3                     4                 5
            new int[]{ 1, 0},      new int[]{ 0, 0},  new int[]{ 1, 0 },
                     //left|bottom          bottom           right|bottom
                     //  6                     7                 8
            new int[]{-1, 1},      new int[]{ 0, 1},  new int[]{ 1, 1 },
        };
        /// <summary>
        /// 获取图像中指定坐标周围的颜色。
        /// </summary>
        /// <param name="image">目标图像。</param>
        /// <param name="x">当前坐标X。</param>
        /// <param name="y">当前坐标Y。</param>
        /// <param name="emptyColor">空白颜色，当坐标在边缘时，用来补上空缺坐标。</param>
        /// <returns>返回一个颜色数组。</returns>
        public static System.Drawing.Color[] GetBitmapColors3x3(
            System.Drawing.Bitmap image, 
            int x, int y, System.Drawing.Color emptyColor) {
            Color[] result = new Color[_pos3x3_finds.Length];
            if (image == null) {
                for (int i = 0; i < result.Length; i++) {
                    result[i] = emptyColor;
                }
                return result;
            }

            int w = image.Width - 1, h = image.Height - 1;
            for (int i = 0; i < result.Length; i++) {
                int x2 = x + _pos3x3_finds[i][0];
                int y2 = y + _pos3x3_finds[i][1];
                result[i] = (x2 >= 0 && y2 >= 0 && x2 < w && y2 < h) ? image.GetPixel(x2, y2) : emptyColor;
            }
            return result;
        }
        /// <summary>
        /// 获取图像中指定坐标周围的颜色。
        /// </summary>
        /// <param name="image">目标图像。</param>
        /// <param name="x">当前坐标X。</param>
        /// <param name="y">当前坐标Y。</param>
        /// <param name="emptyColorArgb">空白颜色的Argb值，当坐标在边缘时，用来补上空缺坐标。</param>
        /// <returns>返回一个颜色数组。</returns>
        public static int[] GetBitmapColors3x3_Argb(

            System.Drawing.Bitmap image, int x, int y, int emptyColorArgb) {
            int[] result = new int[_pos3x3_finds.Length];
            if (image == null) {
                for (int i = 0; i < result.Length; i++) {
                    result[i] = emptyColorArgb;
                }
                return result;
            }

            int w = image.Width - 1, h = image.Height - 1;
            for (int i = 0; i < result.Length; i++) {
                int x2 = x + _pos3x3_finds[i][0];
                int y2 = y + _pos3x3_finds[i][1];
                result[i] = (x2 >= 0 && y2 >= 0 && x2 < w && y2 < h) ? image.GetPixel(x2, y2).ToArgb() : emptyColorArgb;
            }
            return result;
        }
        /// <summary>
        /// 获取图像中指定坐标周围的颜色。
        /// </summary>
        /// <param name="imageDataScan">目标图像内存地址。</param>
        /// <param name="offset">图像扫描偏移量。</param>
        /// <param name="width">图像的宽度。</param>
        /// <param name="height">图像高度。</param>
        /// <param name="x">当前坐标X。</param>
        /// <param name="y">当前坐标Y。</param>
        /// <param name="emptyColorArgb">空白颜色的Argb值，当坐标在边缘时，用来补上空缺坐标。</param>
        /// <returns>返回一个颜色数组。</returns>
        public unsafe static int[] GetBitmapColors3x3_Argb(byte* imageDataScan, int offset, int width, int height, int x, int y, int emptyColorArgb) {
            int[] result = new int[_pos3x3_finds.Length];
            int w = width - 1, h = height - 1;
            for (int i = 0; i < result.Length; i++) {
                int x2 = x + _pos3x3_finds[i][0];
                int y2 = y + _pos3x3_finds[i][1];
                if (x2 >= 0 && y2 >= 0 && x2 < w && y2 < h) {
                    byte* scan = imageDataScan + x2 * 3 + y2 * width * 3 + y2 * offset;
                    result[i] = ColorHelper.ToArgb(scan[0], scan[1], scan[2]);
                } else {
                    result[i] = emptyColorArgb;
                }
            }
            return result;
        }

        #endregion


        #endregion
    }

}