/*
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Drawing {
    /// <summary>
    /// 图像瓦片化。
    /// </summary>
    public class ImageTile {

        #region fields
        private float _zoom;
        private System.Drawing.Size _blockSize;
        private System.Drawing.SizeF _offset;
        private System.Drawing.SizeF _offset_orignal;
        #endregion

        #region properties
        /// <summary>
        /// 获取或设置缩放等级（1表示原始比例）。
        /// </summary>
        public float Zoom {
            get { return _zoom; }
            set { _zoom = value; }
        }
        /// <summary>
        /// 获取或设置块大小。
        /// </summary>
        public System.Drawing.Size BlockSize {
            get { return _blockSize; }
            set { _blockSize = value; }
        }
        /// <summary>
        /// 获取或设置偏移坐标（坐标系为缩放后的）。
        /// </summary>
        public System.Drawing.SizeF Offset {
            get { return _offset; }
            set {
                _offset = value;
                if (value.IsEmpty || _zoom == 0F) {
                    _offset_orignal = value;
                    return;
                }
                if (_zoom > 0) {
                    _offset_orignal = new System.Drawing.SizeF(value.Width / _zoom, value.Height / _zoom);
                } else {
                    _offset_orignal = new System.Drawing.SizeF(value.Width * _zoom, value.Height * _zoom);
                }
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建ImageTile实例。
        /// </summary>
        /// <param name="blockWith">块的宽度。</param>
        /// <param name="blockHeight">块的高度。</param>
        /// <param name="zoom">缩放等级（1表示原始比例）。</param>
        public ImageTile(int blockWith, int blockHeight, float zoom)
            : this(new System.Drawing.Size(blockWith, blockHeight), zoom) {
        }
        /// <summary>
        /// 创建ImageTile实例。
        /// </summary>
        /// <param name="blockSize">块大小。</param>
        /// <param name="zoom">缩放等级（1表示原始比例）。</param>
        public ImageTile(System.Drawing.Size blockSize, float zoom) {
            _zoom = zoom;
            _blockSize = blockSize;
            _offset = System.Drawing.SizeF.Empty;
            _offset_orignal = _offset;
        }
        #endregion

        #region methods

        #region Tile
        /// <summary>
        /// 瓦片化。
        /// </summary>
        /// <param name="file">图像文件（不存在返回空白列表）。</param>
        /// <returns>返回块列表。</returns>
        public System.Collections.Generic.List<TileImageBlock> Tile(string file) {
            System.Drawing.Image image = null;
            if (!string.IsNullOrEmpty(file) && System.IO.File.Exists(file)) {
                using (var stream = System.IO.File.OpenRead(file)) {
                    image = System.Drawing.Image.FromStream(stream);
                }
            }
            var list = Tile(image);
            if (image != null)
                image.Dispose();
            return list;
        }
        /// <summary>
        /// 瓦片化。
        /// </summary>
        /// <param name="image">图像（为空时返回空白列表）。</param>
        /// <returns>返回块列表。</returns>
        public System.Collections.Generic.List<TileImageBlock> Tile(System.Drawing.Image image) {
            var list = new System.Collections.Generic.List<TileImageBlock>();
            if (image == null || _zoom == 0)
                return list;
            float zoom_width=image.Width;
            float zoom_height=image.Height;
            float block_src_width_max = _blockSize.Width;
            float block_src_height_max = _blockSize.Height;
            if (_zoom > 0) {
                zoom_width = zoom_width * _zoom;
                zoom_height = zoom_height * _zoom;
                block_src_width_max = block_src_width_max / _zoom;
                block_src_height_max = block_src_height_max / _zoom;
            } else {
                zoom_width = System.Math.Abs(zoom_width / _zoom);
                zoom_height = System.Math.Abs(zoom_height / _zoom);
                block_src_width_max = System.Math.Abs(block_src_width_max * _zoom);
                block_src_height_max = System.Math.Abs(block_src_height_max * _zoom);
            }
            if (!_offset.IsEmpty) {
                zoom_width += _offset.Width;
                zoom_height += _offset.Height;
            }

            float x_max = (float)System.Math.Ceiling( zoom_width / _blockSize.Width);
            float y_max = (float)System.Math.Ceiling( zoom_height / _blockSize.Height);
            
            for (int x = 0; x < x_max; x++) {
                for (int y = 0; y < y_max; y++) {
                    var item = Tile(image, x, y, block_src_width_max, block_src_height_max);
                    if (item == null) {
                        break;
                    }
                    list.Add(item);
                }
            }

            return list;
        }
        /// <summary>
        /// 瓦片化单块）。
        /// </summary>
        /// <param name="file">图像文件（不存在返回空白列表）。</param>
        /// <param name="x">块坐标X（块大小在图像上的位置）。</param>
        /// <param name="y">块坐标Y（块大小在图像上的位置）。</param>
        /// <returns>返回对应的块。</returns>
        public TileImageBlock Tile(string file, int x, int y) {
            System.Drawing.Image image = null;
            if (!string.IsNullOrEmpty(file) && System.IO.File.Exists(file)) {
                using (var stream = System.IO.File.OpenRead(file)) {
                    image = System.Drawing.Image.FromStream(stream);
                }
            }
            var block = Tile(image, x, y);
            if (image != null)
                image.Dispose();
            return block;
        }

        /// <summary>
        /// 瓦片化（单块）。
        /// </summary>
        /// <param name="image">图像（为空时返回空白列表）。</param>
        /// <param name="x">块坐标X（块大小在图像上的位置）。</param>
        /// <param name="y">块坐标Y（块大小在图像上的位置）。</param>
        /// <returns>返回对应的块。</returns>
        public TileImageBlock Tile(System.Drawing.Image image, int x, int y) {
            if (image == null || _zoom == 0 || x < 0 || y < 0)
                return null;
            float block_src_width_max = _blockSize.Width;
            float block_src_height_max = _blockSize.Height;
            if (_zoom > 0) {
                block_src_width_max = block_src_width_max / _zoom;
                block_src_height_max = block_src_height_max / _zoom;
            } else {
                block_src_width_max = System.Math.Abs(block_src_width_max * _zoom);
                block_src_height_max = System.Math.Abs(block_src_height_max * _zoom);
            }

            return Tile(image, x, y, block_src_width_max, block_src_height_max);
        }
        TileImageBlock Tile(System.Drawing.Image image, int x, int y, float block_width_src_max, float block_height_src_max) {
            float block_x_src = x * block_width_src_max;
            float block_y_src = y * block_height_src_max;
            const float block_x = 0F;
            const float block_y = 0F;
            //偏移：x,y 与实际有差别
            if (!_offset_orignal.IsEmpty) {
                block_x_src += _offset_orignal.Width;
                block_y_src += _offset_orignal.Height;
                //if(x==0)
                //    block_x += _offset.Width;
                //if(y==0)
                //    block_y += _offset.Height;
            }
            //块不能超出有效范围
            if (block_x_src > image.Width - 1 || block_y_src > image.Height - 1)
                return null;

            float block_width = _blockSize.Width;
            float block_width_src = image.Width - block_x_src;
            if (block_width_src > block_width_src_max) {
                block_width_src = block_width_src_max;
            } else {
                if (_zoom > 0) {
                    block_width = block_width_src * _zoom;
                } else {
                    block_width = System.Math.Abs(block_width_src / _zoom);
                }
            }

            float block_height = _blockSize.Height;
            float block_height_src = image.Height - block_y_src;
            if (block_height_src > block_height_src_max) {
                block_height_src = block_height_src_max;
            } else {
                if (_zoom > 0) {
                    block_height = block_height_src * _zoom;
                } else {
                    block_height = System.Math.Abs(block_height_src / _zoom);
                }
            }

            var image_block = new System.Drawing.Bitmap(_blockSize.Width, _blockSize.Height, image.PixelFormat);
            using (var g = System.Drawing.Graphics.FromImage(image_block)) {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                g.DrawImage(image, new System.Drawing.RectangleF(block_x, block_y, block_width, block_height), new System.Drawing.RectangleF(block_x_src, block_y_src, block_width_src, block_height_src), System.Drawing.GraphicsUnit.Pixel);
            }
            return new TileImageBlock(_zoom, _blockSize, x, y, (int)block_x_src, (int)block_y_src, image_block);
        }
        #endregion

        #endregion
    }
    /// <summary>
    /// 图像瓦片块。
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("[{Zoom}]{X},{Y} {Width}x{Height}")]
    public class TileImageBlock : System.IDisposable {

        #region fields
        private System.Drawing.Size _blockSize;
        private float _zoom;
        private int _x;
        private int _y;
        private int _imageX;
        private int _imageY;
        private int _width;
        private int _height;
        private System.Drawing.Image _image;

        #endregion

        #region properties
        /// <summary>
        /// 获取缩放等级（1表示原始比例）。
        /// </summary>
        public float Zoom { get { return _zoom; } }
        /// <summary>
        /// 获取块预设大小。
        /// </summary>
        public System.Drawing.Size BlockSize { get { return _blockSize; } }
        /// <summary>
        /// 获取块坐标X（块大小在图像上的位置）。
        /// </summary>
        public int X { get { return _x; } }
        /// <summary>
        /// 获取块坐标Y（块大小在图像上的位置）。
        /// </summary>
        public int Y { get { return _y; } }
        /// <summary>
        /// 获取图像坐标X。
        /// </summary>
        public int ImageX { get { return _imageX; } }
        /// <summary>
        /// 获取图像坐标Y。
        /// </summary>
        public int ImageY { get { return _imageY; } }
        /// <summary>
        /// 获取块的宽度。
        /// </summary>
        public int Width { get { return _width; } }
        /// <summary>
        /// 获取块的高度。
        /// </summary>
        public int Height { get { return _height; } }
        /// <summary>
        /// 获取块的图像。
        /// </summary>
        public System.Drawing.Image Image { get { return _image; } }

        #endregion

        #region ctor
        /// <summary>
        /// 创建TileImageBlock实例。
        /// </summary>
        /// <param name="zoom">缩放等级（1表示原始比例）。</param>
        /// <param name="blockSize">预设大小。</param>
        /// <param name="x">块坐标X（块大小在图像上的位置）。</param>
        /// <param name="y">块坐标Y（块大小在图像上的位置）。</param>
        /// <param name="imageX">图像坐标X。</param>
        /// <param name="imageY">图像坐标Y。</param>
        /// <param name="image">块的图像。</param>
        public TileImageBlock(float zoom, System.Drawing.Size blockSize, int x, int y,int imageX, int imageY, System.Drawing.Image image) {
            _zoom = zoom;
            _blockSize = blockSize;
            _x = x;
            _y = y;
            _imageX = imageX;
            _imageY = imageY;
            _image = image;
            if (image != null) {
                _width = image.Width;
                _height = image.Height;
            }
        }
        #endregion

        #region methods

        #region Save
        /// <summary>
        /// 保存到文件。
        /// </summary>
        /// <param name="path">保存位置。</param>
        /// <param name="prefix">保存前辍。</param>
        /// <returns>返回保存的文件路径。</returns>
        public string Save(string path, string prefix) {
            if (_image == null)
                return null;
            string file = System.IO.Path.Combine(path, string.Format("{0}_{1}x{2}_{3}_{4}_{5}.png", prefix, _blockSize.Width, _blockSize.Height, _zoom, _x, _y));
            return Save(file) ? file : null;
        }
        /// <summary>
        /// 保存到文件。
        /// </summary>
        /// <param name="file">文件路径。</param>
        /// <returns>返回是否成功。</returns>
        public bool Save(string file) {
            if (string.IsNullOrEmpty(file))
                return false;
            using (var stream = new System.IO.MemoryStream()) {
                _image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                if (System.IO.File.Exists(file))
                    AppHelper.DeleteFile(file);
                else
                    AppHelper.CreateDirectory(System.IO.Path.GetDirectoryName(file), false);
                System.IO.File.WriteAllBytes(file, stream.ToArray());
            }
            return System.IO.File.Exists(file);
        }
        #endregion
        #region Dispose
        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        public void Dispose() {
            if (_image != null) {
                _image.Dispose();
                _image = null;
            }
        }
        #endregion

        #endregion
    }

}
