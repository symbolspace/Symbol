/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System.IO;

#pragma warning disable 1591

namespace Symbol.Encryption {
    public class BinaryEncryptionHelper {

        #region types
        /// <summary>
        /// 加密器
        /// </summary>
        public class Encryption {

            #region fields
            private byte[] _password;
            private int _tJ;
            private int _tJ2;
            private int _tJ3;
            private bool _empty;
            #endregion
            /// <summary>
            /// 是否为空白密码，为true时表示不处理加/解密操作。
            /// </summary>
            public bool IsEmptyPassword { get { return _empty; } }

            #region ctor
            /// <summary>
            /// 创建加密器的实例
            /// </summary>
            /// <param name="password">密码</param>
            /// <param name="empty">是否为空白密码，为true时表示不处理加/解密操作。</param>
            public Encryption(byte[] password,bool empty=false) {
                _password = password;
                _empty = empty;
                Reset();
            }
            #endregion

            #region methods

            #region Reset
            /// <summary>
            /// 重置密码DNA
            /// </summary>
            public void Reset(){
                _tJ = 0;
                _tJ2 = _password.Length - 1;
                _tJ3 = 0;
            }
            #endregion

            #region Next
            private byte Next(){
                if (_tJ > _tJ2) {
                    _tJ3--;
                    if (_tJ3 == -1)
                        _tJ3 = _tJ2;
                    _tJ = _tJ3;
                }
                byte result= _password[_tJ];
                _tJ++;
                return result;
            }
            #endregion

            #region Execute
            /// <summary>
            /// 执行加密/解密处理
            /// </summary>
            /// <param name="buffer">需要处理的数据</param>
            /// <param name="offset">起始位置，0表示从头开始。</param>
            /// <param name="count">处理的长度，-1表示剩余的。</param>
            public void Execute(byte[] buffer, int offset = 0, int count = -1) {
                if (_empty)
                    return;
                if (count == -1)
                    count = buffer.Length;
                for (int i = offset; i < count; i++) {
                    buffer[i] = (byte)(buffer[i] ^ Next());
                }
            }
            #endregion

            #endregion
        }
        #endregion

        #region methods

        #region BufferWaveXor
        /// <summary>
        /// 对二进制数据按指定的密码进行波形Xor处理。将处理后的数据再次调用它时，将会还原数据。
        /// </summary>
        /// <param name="buffer">需要处理的数据</param>
        /// <param name="password">密码（波形样本）</param>
        /// <returns>返回处理后的数据。</returns>
        public static byte[] BufferWaveXor(byte[] buffer, byte[] password) {
            byte[] result = new byte[buffer.Length];
            int tI = 0, tI2 = buffer.Length - 1, tJ = 0, tJ2 = password.Length - 1, tJ3 = 0;

            while (tI <= tI2) {
                if (tJ > tJ2) {
                    tJ3--;
                    if (tJ3 == -1)
                        tJ3 = tJ2;
                    tJ = tJ3;
                }
                result[tI] = (byte)(buffer[tI] ^ password[tJ]);
                tI++;
                tJ++;
            }
            return result;
        }
        #endregion
        #region BufferWaveXor
        /// <summary>
        /// 对二进制数据按指定的密码进行波形Xor处理。将处理后的数据再次调用它时，将会还原数据。
        /// </summary>
        /// <param name="buffer">需要处理的数据</param>
        /// <param name="password">密码（波形样本）</param>
        public static void BufferWaveXorBase(byte[] buffer, byte[] password) {
            int tI = 0, tI2 = buffer.Length - 1, tJ = 0, tJ2 = password.Length - 1, tJ3 = 0;

            while (tI <= tI2) {
                if (tJ > tJ2) {
                    tJ3--;
                    if (tJ3 == -1)
                        tJ3 = tJ2;
                    tJ = tJ3;
                }
                buffer[tI] = (byte)(buffer[tI] ^ password[tJ]);
                tI++;
                tJ++;
            }
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// 二进制加密流，只能写入或读取（具体取决于被包装的流），Seek等定位操作会报异常。
    /// </summary>
    /// <remarks>
    /// 对二进制数据按指定的密码进行波形Xor处理。将处理后的数据再次调用它时，将会还原数据。
    /// </remarks>
    public class BinaryEncryptionStream : Stream {
        private byte[] _password;
        private int _tJ;
        private int _tJ2;
        private int _tJ3;
        private Stream _stream;
        private bool _empty;
        [System.Serializable]
        public class PositionDNA {
            public int i;
            public int i2;
            public int i3;
            public byte[] password;
            public long index;
        }
        /// <summary>
        /// 当前加密位置，用于延迟处理。
        /// </summary>
        public PositionDNA CurrentPositionDNA {
            get {
                return new PositionDNA() {
                    i = _tJ,
                    i2 = _tJ2,
                    i3 = _tJ3,
                    password = _password,
                    index = _stream.CanSeek ? -1 : _stream.Position,
                };
            }
            set {
                if (value == null)
                    throw new System.ArgumentNullException("value", "不能设置为一个空位置");
                _tJ = value.i;
                _tJ2 = value.i2;
                _tJ3 = value.i3;
                _password = value.password;
                if (_stream.CanSeek)
                    _stream.Position = value.index;
            }
        }
        
        public override bool CanRead {
            get { return _stream.CanRead; }
        }
        public override bool CanSeek {
            get { return false; }
        }
        public override bool CanWrite {
            get { return _stream.CanWrite; }
        }
        public override long Length {
            get { return _stream.Length; }
        }
        public override long Position {
            get {
                return _stream.Position;
            }
            set {
                throw new System.NotImplementedException();
                //_stream.Position = value;
            }
        }

        /// <summary>
        /// 创建一个加密流
        /// </summary>
        /// <param name="stream">需要包装的流（数据载体）</param>
        /// <param name="password">密码</param>
        /// <param name="empty">是否为空白密码，为true时表示不处理加/解密操作。</param>
        public BinaryEncryptionStream(Stream stream, byte[] password,bool empty=false) {
            _stream = stream;
            _password = password;
            _tJ = 0;
            _tJ2 = password.Length - 1;
            _tJ3 = 0;
            _empty = empty;
        }

        public override void Flush() {
            _stream.Flush();
        }
        public override long Seek(long offset, SeekOrigin origin) {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value) {
            throw new System.NotImplementedException();
        }
        public int BaseRead(byte[] buffer, int offset, int count) {
            return _stream.Read(buffer, offset, count);
        }
        public override int Read(byte[] buffer, int offset, int count) {
            int result = _stream.Read(buffer, offset, count);
            if (_empty)
                return result;

            for (int i = 0; i < result; i++) {
                if (_tJ > _tJ2) {
                    _tJ3--;
                    if (_tJ3 == -1)
                        _tJ3 = _tJ2;
                    _tJ = _tJ3;
                }
                buffer[i] = (byte)(buffer[i] ^ _password[_tJ]);
                _tJ++;
            }

            return result;
        }
        public void BaseWrite(byte[] buffer, int offset, int count) {
            _stream.Write(buffer, offset, count);
        }
        public override void Write(byte[] buffer, int offset, int count) {
            if (_empty) {
                _stream.Write(buffer, offset, count);
                return;
            }
            for (int i = 0; i < count; i++) {
                if (_tJ > _tJ2) {
                    _tJ3--;
                    if (_tJ3 == -1)
                        _tJ3 = _tJ2;
                    _tJ = _tJ3;
                }
                _stream.WriteByte((byte)(buffer[i + offset] ^ _password[_tJ]));
                //buffer[i + offset] = (byte)(buffer[i + offset] ^ _password[_tJ]);
                _tJ++;
            }
            //_stream.Write(buffer, offset, count);
        }
#if !NETDNX
        public override void Close() {
            if (_stream != null) {
                //_stream.Close();
            }
            base.Close();
        }
#endif
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (_stream != null) {
                    //_stream.Close();
                    //_stream.Dispose();
                    _stream = null;
                    _password = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}

#pragma warning restore 1591
