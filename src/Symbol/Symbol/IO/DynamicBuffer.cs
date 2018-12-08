/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.IO {
    /// <summary>
    /// 动态缓冲区，自动扩充
    /// </summary>
    public class DynamicBuffer :IDisposable{

        #region fields
        private byte[] _buffer;
        private int _count;
        private int _size;
        private bool _resize;

        #endregion

        #region properties

        #region this[int index]
        /// <summary>
        /// 获取或设置缓冲区指定位置的值。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte this[int index] {
            get {
                var buffer = ThreadHelper.InterlockedGet(ref _buffer);
                return buffer == null ? byte.MinValue : buffer[index];
            }
            set {
                var buffer = ThreadHelper.InterlockedGet(ref _buffer);
                if(buffer!=null)
                    buffer[index] = value;
            }
        }
        #endregion

        /// <summary>
        /// 获取是否采用重新调整数组大小方式。
        /// </summary>
        public bool Resize {
            get {
                return _resize;
            }
        }

        /// <summary>
        /// 获取当前数据缓冲区。
        /// </summary>
        public byte[] Buffer {
            get {
                return ThreadHelper.InterlockedGet(ref _buffer);
            }
            private set {
                System.Threading.Interlocked.Exchange(ref _buffer, value);
            }
        }
        /// <summary>
        /// 获取当前已写入缓冲区数据长度（字节数）。
        /// </summary>
        public int Count {
            get {
                return System.Threading.Interlocked.CompareExchange(ref _count, 0, -1);
            }
            private set {
                System.Threading.Interlocked.Exchange(ref _count, value);
            }
        }
        /// <summary>
        /// 获取缓冲区剩余长度（字节数）。
        /// </summary>
        public int ReserveCount {
            get {
                var buffer = Buffer;
                int count = Count;
                if (buffer == null)
                    return 0;
                return buffer.Length - count;
            }
        }
        /// <summary>
        /// 获取或设置缓冲区的大小。
        /// </summary>
        public int Size {
            get {
                return System.Threading.Interlocked.CompareExchange(ref _size, 0, -1);
            }
            set {
                if (value < 1)
                    return;
                var size = Size;
                var count = Count;
                if (value <= size)
                    return;
                var buffer = Buffer;
                if (buffer != null) {
                    if (_resize) {
                        Array.Resize(ref buffer, value);
                    } else {
                        byte[] newBuffer = new byte[value];
                        System.Buffer.BlockCopy(buffer, 0, newBuffer, 0, count); //复制以前的数据
                                                                                 //替换
                        Buffer = newBuffer;
                    }
                }
                System.Threading.Interlocked.CompareExchange(ref _size, value, size);
            }
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建DynamicBuffer实例（10M）。
        /// </summary>
        public DynamicBuffer() : this(10485760) {

        }
        /// <summary>
        /// 创建DynamicBuffer实例（10M）。
        /// </summary>
        /// <param name="resize">采用重新调整数组大小方式。</param>
        public DynamicBuffer(bool resize) : this(10485760, resize) {

        }
        /// <summary>
        /// 创建DynamicBuffer实例。
        /// </summary>
        /// <param name="size">缓冲区大小。</param>
        public DynamicBuffer(int size)
            :this(size,false){
        }
        /// <summary>
        /// 创建DynamicBuffer实例。
        /// </summary>
        /// <param name="size">缓冲区大小。</param>
        /// <param name="resize">采用重新调整数组大小方式。</param>
        public DynamicBuffer(int size, bool resize) {
            _count = 0;
            _size = size;
            _buffer = new byte[size];
            _resize = resize;
        }
        /// <summary>
        /// 创建DynamicBuffer实例。
        /// </summary>
        /// <param name="data">缓冲数据。</param>
        public DynamicBuffer(byte[] data) {
            _resize = false;
            if (data == null) {
                _count = 0;
                _size = 8192;
                _buffer = new byte[8192];
            } else {
                _count = data.Length;
                _size = data.Length;
                _buffer = data;
            }
        }
        #endregion

        #region methods

        #region Clear
        /// <summary>
        /// 清理缓冲区所有数据（仅放弃数据，无实际操作）
        /// </summary>
        public void Clear() {
            Count = 0;
        }
        /// <summary>
        /// 从缓冲区左边开始，清理指定大小的数据区域
        /// </summary>
        /// <param name="count">超过可用数据大小将完全清理（仅放弃数据，无实际操作），否则后面的数据自动往缓冲区左侧移动。</param>
        public void Clear(int count) //清理指定大小的数据
        {
            if (count < 1)
                return;
            var this_count = Count;

            if (count >= this_count) //如果需要清理的数据大于现有数据大小，则全部清理
            {
                Count = 0;
                //_count = 0;
            } else {
                var buffer = Buffer;
                if (buffer != null) {
                    for (int i = 0; i < this_count - count; i++) //否则后面的数据往前移
                    {
                        buffer[i] = buffer[count + i];
                    }
                }
                Count = this_count - count;
            }
        }
        #endregion

        #region TryRead

        //#region TryReadBoolean
        ///// <summary>
        ///// 尝试读取bool
        ///// </summary>
        ///// <param name="value">输出值</param>
        ///// <returns>返回是否成功</returns>
        //public bool TryReadBoolean(out bool value) {
        //    if (Count >= 1) {
        //        value = BitConverter.ToBoolean(Buffer, 0);
        //        //Clear(1);
        //        return true;
        //    }
        //    value = false;
        //    return false;
        //}
        //#endregion
        //#region TryReadChar
        ///// <summary>
        ///// 尝试读取char
        ///// </summary>
        ///// <param name="value">输出值</param>
        ///// <returns>返回是否成功</returns>
        //public bool TryReadChar(out char value) {
        //    if (_count >= 2) {
        //        value = BitConverter.ToChar(_buffer, 0);
        //        //Clear(2);
        //        return true;
        //    }
        //    value = char.MinValue;
        //    return false;
        //}
        //#endregion
        //#region TryReadInt16
        ///// <summary>
        ///// 尝试读取short
        ///// </summary>
        ///// <param name="value">输出值</param>
        ///// <returns>返回是否成功</returns>
        //public bool TryReadInt16(out short value) {
        //    if (_count >= 2) {
        //        value = BitConverter.ToInt16(_buffer, 0);
        //        //Clear(2);
        //        return true;
        //    }
        //    value = -1;
        //    return false;
        //}
        //#endregion
        //#region TryReadUInt16
        ///// <summary>
        ///// 尝试读取ushort
        ///// </summary>
        ///// <param name="value">输出值</param>
        ///// <returns>返回是否成功</returns>
        //public bool TryReadUInt16(out ushort value) {
        //    if (_count >= 2) {
        //        value = BitConverter.ToUInt16(_buffer, 0);
        //        //Clear(2);
        //        return true;
        //    }
        //    value = 0;
        //    return false;
        //}
        //#endregion
        //#region TryReadInt32
        ///// <summary>
        ///// 尝试读取int
        ///// </summary>
        ///// <param name="value">输出值</param>
        ///// <returns>返回是否成功</returns>
        //public bool TryReadInt32(out int value) {
        //    if (_count >= 4) {
        //        value = BitConverter.ToInt32(_buffer, 0);
        //        //Clear(4);
        //        return true;
        //    }
        //    value = -1;
        //    return false;
        //}
        //#endregion
        //#region TryReadUInt32
        ///// <summary>
        ///// 尝试读取uint
        ///// </summary>
        ///// <param name="value">输出值</param>
        ///// <returns>返回是否成功</returns>
        //public bool TryReadUInt32(out uint value) {
        //    if (_count >= 4) {
        //        value = BitConverter.ToUInt32(_buffer, 0);
        //        //Clear(4);
        //        return true;
        //    }
        //    value = 0;
        //    return false;
        //}
        //#endregion
        //#region TryReadInt64
        ///// <summary>
        ///// 尝试读取long
        ///// </summary>
        ///// <param name="value">输出值</param>
        ///// <returns>返回是否成功</returns>
        //public bool TryReadInt64(out long value) {
        //    if (_count >= 8) {
        //        value = BitConverter.ToInt64(_buffer, 0);
        //        //Clear(8);
        //        return true;
        //    }
        //    value = -1L;
        //    return false;
        //}
        //#endregion
        //#region TryReadUInt64
        ///// <summary>
        ///// 尝试读取long
        ///// </summary>
        ///// <param name="value">输出值</param>
        ///// <returns>返回是否成功</returns>
        //public bool TryReadUInt64(out ulong value) {
        //    if (_count >= 8) {
        //        value = BitConverter.ToUInt64(_buffer, 0);
        //        //Clear(8);
        //        return true;
        //    }
        //    value = 0UL;
        //    return false;
        //}
        //#endregion
        //#region TryReadDouble
        ///// <summary>
        ///// 尝试读取double
        ///// </summary>
        ///// <param name="value">输出值</param>
        ///// <returns>返回是否成功</returns>
        //public bool TryReadDouble(out double value) {
        //    if (_count >= 8) {
        //        value = BitConverter.ToDouble(_buffer, 0);
        //        //Clear(8);
        //        return true;
        //    }
        //    value = -1L;
        //    return false;
        //}
        //#endregion
        //#region TryReadSingle
        ///// <summary>
        ///// 尝试读取float
        ///// </summary>
        ///// <param name="value">输出值</param>
        ///// <returns>返回是否成功</returns>
        //public bool TryReadSingle(out float value) {
        //    if (_count >= 4) {
        //        value = BitConverter.ToSingle(_buffer, 0);
        //        //Clear(4);
        //        return true;
        //    }
        //    value = 0UL;
        //    return false;
        //}
        //#endregion
        //#region TryReadString
        ///// <summary>
        ///// 尝试读取float
        ///// </summary>
        ///// <param name="value">输出值</param>
        ///// <param name="length">读取长度（字节）。</param>
        ///// <param name="encoding">编码。</param>
        ///// <returns>返回是否成功</returns>
        //public bool TryReadString(out string value,int length, System.Text.Encoding encoding) {
        //    value = null;
        //    if (encoding == null)
        //        return false;
        //    if (_count >= length) {
        //        value = encoding.GetString(_buffer, 0, length);
        //        //Clear(length);
        //        return true;
        //    }
        //    return false;
        //}
        //#endregion
        #endregion

        #region Write
        /// <summary>
        /// 写入缓冲数据
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        public DynamicBuffer Write(char value) {
            byte[] buffer = BitConverter.GetBytes(value);
            return Write(buffer);
        }
        /// <summary>
        /// 写入缓冲数据
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        public DynamicBuffer Write(float value) {
            byte[] buffer = BitConverter.GetBytes(value);
            return Write(buffer);
        }
        /// <summary>
        /// 写入缓冲数据
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        public DynamicBuffer Write(double value) {
            byte[] buffer = BitConverter.GetBytes(value);
            return Write(buffer);
        }
        /// <summary>
        /// 写入缓冲数据
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        public DynamicBuffer Write(bool value) {
            byte[] buffer = BitConverter.GetBytes(value);
            return Write(buffer);
        }
        /// <summary>
        /// 写入缓冲数据（不转换字节顺序）
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        public DynamicBuffer Write(short value) {
            return Write(value, false);
        }
        /// <summary>
        /// 写入缓冲数据
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        /// <param name="convert">是否采用网络通用字节顺序，.NET默认是从左开始</param>
        public DynamicBuffer Write(short value, bool convert) {
            if (convert) {
                value = System.Net.IPAddress.HostToNetworkOrder(value); //NET是小头结构，网络字节是大头结构，需要客户端和服务器约定好
            }
            byte[] buffer = BitConverter.GetBytes(value);
            return Write(buffer);
        }
        /// <summary>
        /// 写入缓冲数据
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        public DynamicBuffer Write(ushort value) {
            byte[] buffer = BitConverter.GetBytes(value);
            return Write(buffer);
        }
        /// <summary>
        /// 写入缓冲数据（不转换字节顺序）
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        public DynamicBuffer Write(int value) {
            return Write(value, false);
        }
        /// <summary>
        /// 写入缓冲数据
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        /// <param name="convert">是否采用网络通用字节顺序，.NET默认是从左开始</param>
        public DynamicBuffer Write(int value, bool convert) {
            if (convert) {
                value = System.Net.IPAddress.HostToNetworkOrder(value); //NET是小头结构，网络字节是大头结构，需要客户端和服务器约定好
            }
            byte[] buffer = BitConverter.GetBytes(value);
            return Write(buffer);
        }
        /// <summary>
        /// 写入缓冲数据
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        public DynamicBuffer Write(uint value) {
            byte[] buffer = BitConverter.GetBytes(value);
            return Write(buffer);
        }
        /// <summary>
        /// 写入缓冲数据（不转换字节顺序）
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        public DynamicBuffer Write(long value) {
            return Write(value, false);
        }
        /// <summary>
        /// 写入缓冲数据
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        /// <param name="convert">是否采用网络通用字节顺序，.NET默认是从左开始</param>
        public DynamicBuffer Write(long value, bool convert) {
            if (convert) {
                value = System.Net.IPAddress.HostToNetworkOrder(value); //NET是小头结构，网络字节是大头结构，需要客户端和服务器约定好
            }
            byte[] buffer = BitConverter.GetBytes(value);
            return Write(buffer);
        }
        /// <summary>
        /// 写入缓冲数据
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        public DynamicBuffer Write(ulong value) {
            byte[] buffer = BitConverter.GetBytes(value);
            return Write(buffer);
        }

        #region string
        /// <summary>
        /// 写入缓冲数据（自动忽略空或空文本,UTF-8编码）
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        public DynamicBuffer Write(string value) {
            if (string.IsNullOrEmpty(value))
                return this;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(value);
            return Write(buffer);
        }
        /// <summary>
        /// 写入缓冲数据（自动忽略空或空文本）
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        /// <param name="encoding">字符编码，为空时自动采用utf-8</param>
        public DynamicBuffer Write(string value, System.Text.Encoding encoding) {
            if (string.IsNullOrEmpty(value))
                return this;
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            byte[] buffer = encoding.GetBytes(value);
            return Write(buffer);
        }
        #endregion
        #region buffer
        /// <summary>
        /// 写入缓冲数据
        /// </summary>
        /// <param name="value">需要写入的数据</param>
        public DynamicBuffer Write(byte value) {
            return Write(new byte[] { value });
        }
        /// <summary>
        /// 写入缓冲数据（自动扩充缓冲区，全部）
        /// </summary>
        /// <param name="buffer">需要写入的数据</param>
        public DynamicBuffer Write(byte[] buffer) {
            return Write(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// 写入缓冲数据（自动扩充缓冲区，部分）
        /// </summary>
        /// <param name="buffer">需要写入的数据</param>
        /// <param name="offset">起始位置，从0开始</param>
        /// <param name="count">写入数量，最大不能超过bufffer.Length</param>
        public DynamicBuffer Write(byte[] buffer, int offset, int count) {
            var this_buffer = Buffer;
            if (this_buffer == null)
                return this;

            var this_count = Count;
            var reserveCount = ReserveCount;
            if (reserveCount < count) {
                Size = this_buffer.Length + count - reserveCount;
                this_buffer = Buffer;
            }
            Count = this_count + count;
            System.Buffer.BlockCopy(buffer, offset, this_buffer, this_count, count);
            return this;
        }

        /// <summary>
        /// 写入缓冲数据
        /// </summary>
        /// <param name="stream">需要写入的数据</param>
        /// <returns></returns>
        public DynamicBuffer Write(System.IO.Stream stream) {
            if (stream==null ||  !stream.CanRead)
                return this;
            byte[] buffer = new byte[8192];
            int count = 0;
            while ((count = stream.Read(buffer, 0, buffer.Length)) > 0) {
                Write(buffer, 0, count);
            }
            return this;
        }
        #endregion


        #endregion

        #region CopyTo
        /// <summary>
        /// 复制到目标数组。
        /// </summary>
        /// <param name="destinationArray"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="count"></param>
        /// <returns>返回成功复制的数量。</returns>
        public int Copy(byte[] destinationArray, int destinationIndex, int sourceIndex, int count) {
            var this_count = Count;
            var this_buffer = Buffer;
            if (this_buffer == null)
                return -1;
            if (
                       destinationArray == null                         //目标为空
                    || this_count == 0                                      //可用为0
                    || count < 1                                        //复制数为0
                    || sourceIndex < 0                                  //源小于0
                    || sourceIndex > count - 1                          //超出可用范围
                    || destinationIndex < 0                             //目标位置小于0
                    || destinationArray.Length == 0                     //目标没有足够空间
                    || destinationIndex > destinationArray.Length - 1   //超出目标有效位置
                )
                return 0;
            //求出目标可有空间
            int maxCount = destinationArray.Length - destinationIndex;
            //求出源可用空间
            int maxCount2 = this_count - sourceIndex;
            //求出实际可用空间
            maxCount = System.Math.Min(maxCount, maxCount2);
            //求出实际需要的空间
            count = System.Math.Min(count, maxCount);

            System.Buffer.BlockCopy(this_buffer, sourceIndex, destinationArray, destinationIndex, count);
            return count;

        }
        #endregion
        #region ToArray
        /// <summary>
        /// 将缓冲区中的数据输出为一个新的byte[]（不清空缓冲区）。
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray() {
            return ToArray(false);
        }
        /// <summary>
        /// 将缓冲区中的数据输出为一个新的byte[]。
        /// </summary>
        /// <param name="clear">是否在输出完后清空缓冲区</param>
        /// <returns></returns>
        public byte[] ToArray(bool clear) {
            var this_count = Count;
            var this_buffer = Buffer;
            if (this_buffer == null)
                this_count = 0;

            byte[] buffer = new byte[this_count];
            if (this_count > 0) {
                System.Buffer.BlockCopy(this_buffer, 0, buffer, 0, this_count);
            }
            if (clear) {
                Clear();
            }
            return buffer;
        }

        /// <summary>
        /// 释放对象占用的资源。
        /// </summary>
        public void Dispose() {
            System.Threading.Interlocked.Exchange(ref _buffer, null);
        }
        #endregion

        #endregion

    }
}