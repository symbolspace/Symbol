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