/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.IO.Packing {
    /// <summary>
    /// Buffer资源（头部[JSON]+数据块，采用Gzip压缩）。
    /// </summary>
    public class BufferResource : IDisposable {

        #region fields
        private System.Collections.Generic.Dictionary<string, Resource> _list_name;
        private System.Collections.Generic.List<Resource> _list;
        private int _version;
        #endregion

        #region properties
        /// <summary>
        /// 获取版本号（资源包采用的引擎版本，不同版本结构不相同）。
        /// </summary>
        public int Version {
            get { return _version; }
        }
        /// <summary>
        /// 获取当前资源数量。
        /// </summary>
        public int Count { get { return _list == null ? 0 : _list.Count; } }

        #region byte[] this[int index]
        /// <summary>
        /// 获取或设置指定索引的资源数据。
        /// </summary>
        /// <param name="index">从0开始的索引值，超出有效范围后自动忽略。</param>
        /// <returns>返回资源的数据，无效操作时直接为 new byte[0]。</returns>
        public byte[] this[int index] {
            get {
                if (_list == null || index < 0 || index > _list.Count - 1)
                    goto lb_Empty;

                Resource item = _list[index];
                if (item.Data == null)
                    goto lb_Empty;
                return item.Data;

                lb_Empty:
                return new byte[0];
            }
            set {
                Set(index, value);
            }
        }
        #endregion
        #region byte[] this[string name]
        /// <summary>
        /// 获取或设置指定名称的资源数据（不存在时自动追加）。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回资源的数据，无效操作时直接为 new byte[0]。</returns>
        public byte[] this[string name] {
            get {
                if (_list == null || _list.Count == 0 || string.IsNullOrEmpty(name))
                    goto lb_Empty;

                Resource item;
                if (!_list_name.TryGetValue(name, out item) || item.Data == null) {
                    goto lb_Empty;
                }
                return item.Data;

                lb_Empty:
                return new byte[0];
            }
            set {
                Set(name, value);
            }
        }
        #endregion

        #endregion

        #region ctor

        /// <summary>
        /// 创建BufferResource实例。
        /// </summary>
        public BufferResource() {
            _list = new System.Collections.Generic.List<Resource>();
            _list_name = new System.Collections.Generic.Dictionary<string, Resource>(StringComparer.OrdinalIgnoreCase);
            _version = 1;
        }
        /// <summary>
        /// 创建BufferResource实例。
        /// </summary>
        /// <param name="capacity">缓存大小。</param>
        public BufferResource(int capacity) {
            _list = new System.Collections.Generic.List<Resource>(capacity);
            _list_name = new System.Collections.Generic.Dictionary<string, Resource>(capacity, StringComparer.OrdinalIgnoreCase);
            _version = 1;
        }
        /// <summary>
        /// 创建BufferResource实例。
        /// </summary>
        /// <param name="data">从二进制中加载，自动忽略所有错误。</param>
        public BufferResource(byte[] data) : this() {
            Load(data);
        }
        /// <summary>
        /// 创建BufferResource实例。
        /// </summary>
        /// <param name="buffer">从缓冲区加载，自动忽略所有错误。</param>
        public BufferResource(Symbol.IO.DynamicBuffer buffer) : this() {
            Load(buffer.ToArray());
        }
        /// <summary>
        /// 创建BufferResource实例。
        /// </summary>
        /// <param name="file">从文件中加载，自动忽略所有错误。</param>
        public BufferResource(string file) : this() {
            Load(file);
        }
        /// <summary>
        /// 创建BufferResource实例。
        /// </summary>
        /// <param name="stream">从流中加载，自动忽略所有错误。</param>
        public BufferResource(System.IO.Stream stream) : this() {
            Load(stream);
        }

        #endregion

        #region methods

        #region Append
        void Append(Resource item) {
            _list.Add(item);
            _list_name.Add(item.Name, item);
        }
        #endregion

        #region FindIndex
        /// <summary>
        /// 查询指定名称的索引位置。
        /// </summary>
        /// <param name="name">名称，自动忽略错误。</param>
        /// <returns>返回从0开始的索引，未找到时返回-1。</returns>
        public int FindIndex(string name) {
            if (string.IsNullOrEmpty(name) || _list == null || _list.Count == 0)
                return -1;
            Resource item;
            if (!_list_name.TryGetValue(name, out item)) {
                return -1;
            }
            return _list.IndexOf(item);
        }
        #endregion
        #region FindName
        /// <summary>
        /// 查找指定索引的资源名称。
        /// </summary>
        /// <param name="index">从0开始的索引值，超出有效范围后自动忽略。</param>
        /// <returns>返回资源的名称，无效操作时返回null。</returns>
        public string FindName(int index) {
            if (_list == null || index < 0 || index > _list.Count - 1)
                return null;
            return _list[index].Name;
        }
        #endregion

        #region GetString
        /// <summary>
        /// 获取指定索引的资源数据 UTF-8编码。
        /// </summary>
        /// <param name="index">从0开始的索引值，超出有效范围后自动忽略。</param>
        /// <returns>返回资源数据转换后的文本内容。</returns>
        public string GetString(int index) {
            return GetString(index, System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 获取指定索引的资源数据。
        /// </summary>
        /// <param name="index">从0开始的索引值，超出有效范围后自动忽略。</param>
        /// <param name="encoding">文本编码，为null时自动采用UTF-8。</param>
        /// <returns>返回资源数据转换后的文本内容。</returns>
        public string GetString(int index, System.Text.Encoding encoding) {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            byte[] data = Get(index);
            return encoding.GetString(data);
        }
        /// <summary>
        /// 获取指定索引的资源数据 UTF-8编码。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回资源数据转换后的文本内容。</returns>
        public string GetString(string name) {
            return GetString(name, System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 获取指定索引的资源数据。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="encoding">文本编码，为null时自动采用UTF-8。</param>
        /// <returns>返回资源数据转换后的文本内容。</returns>
        public string GetString(string name, System.Text.Encoding encoding) {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            byte[] data = Get(name);
            return encoding.GetString(data);
        }
        #endregion
        #region Get
        /// <summary>
        /// 获取指定索引的资源数据。
        /// </summary>
        /// <param name="index">从0开始的索引值，超出有效范围后自动忽略。</param>
        /// <returns>返回资源的数据，无效操作时直接为 new byte[0]。</returns>
        public byte[] Get(int index) {
            if (_list == null || index < 0 || index > _list.Count - 1)
                goto lb_Empty;

            Resource item = _list[index];
            if (item.Data == null)
                goto lb_Empty;
            return item.Data;

            lb_Empty:
            return new byte[0];
        }
        /// <summary>
        /// 获取指定名称的资源数据。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回资源的数据，无效操作时直接为 new byte[0]。</returns>
        public byte[] Get(string name) {
            if (_list == null || _list.Count == 0 || string.IsNullOrEmpty(name))
                goto lb_Empty;

            Resource item;
            if (!_list_name.TryGetValue(name, out item) || item.Data == null) {
                goto lb_Empty;
            }
            return item.Data;

            lb_Empty:
            return new byte[0];
        }
        #endregion
        #region Set
        /// <summary>
        /// 设置指定索引的资源数据（不存在时自动追加）UTF-8编码。
        /// </summary>
        /// <param name="index">从0开始的索引值，超出有效范围后自动忽略。</param>
        /// <param name="text">文本内容，null或空文本转换为new byte[0]。</param>
        /// <returns>返回是否成功。</returns>
        public bool Set(int index, string text) {
            return Set(index, text, System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 设置指定索引的资源数据（不存在时自动追加）。
        /// </summary>
        /// <param name="index">从0开始的索引值，超出有效范围后自动忽略。</param>
        /// <param name="text">文本内容，null或空文本转换为new byte[0]。</param>
        /// <param name="encoding">文本编码，为null时自动采用UTF-8。</param>
        /// <returns>返回是否成功。</returns>
        public bool Set(int index, string text, System.Text.Encoding encoding) {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            return Set(index, string.IsNullOrEmpty(text) ? null : encoding.GetBytes(text));
        }
        /// <summary>
        /// 设置指定索引的资源数据（不存在时自动追加）。
        /// </summary>
        /// <param name="index">从0开始的索引值，超出有效范围后自动忽略。</param>
        /// <param name="data">资源数据。</param>
        /// <returns>返回是否成功。</returns>
        public bool Set(int index, byte[] data) {
            if (_list == null || index < 0 || index > _list.Count - 1)
                return false;
            if (data == null)
                data = new byte[0];
            Resource item = _list[index];
            item.Data = data;
            item.Length = data.Length;
            SetOffset(item, false);
            return true;
        }
        /// <summary>
        /// 设置指定名称的资源数据（不存在时自动追加）UTF-8编码。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="text">文本内容，null或空文本转换为new byte[0]。</param>
        /// <returns>返回是否成功。</returns>
        public bool Set(string name, string text) {
            return Set(name, text, System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 设置指定名称的资源数据（不存在时自动追加）。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="text">文本内容，null或空文本转换为new byte[0]。</param>
        /// <param name="encoding">文本编码，为null时自动采用UTF-8。</param>
        /// <returns>返回是否成功。</returns>
        public bool Set(string name, string text, System.Text.Encoding encoding) {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            return Set(name, string.IsNullOrEmpty(text) ? null : encoding.GetBytes(text));
        }
        /// <summary>
        /// 设置指定名称的资源数据（不存在时自动追加）。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="data">资源数据，为null时自动转换为new byte[0]。</param>
        /// <returns>返回是否成功。</returns>
        public bool Set(string name, byte[] data) {
            if (_list == null || string.IsNullOrEmpty(name))
                return false;
            if (data == null)
                data = new byte[0];
            Resource item;
            if (_list_name.TryGetValue(name, out item)) {
                item.Data = data;
                item.Length = data.Length;
                SetOffset(item, false);
            } else {
                item = new Resource() {
                    Name = name,
                    Data = data,
                    Length = data.Length,
                };
                _list.Add(item);
                _list_name.Add(name, item);
                SetOffset(item, true);
            }
            return true;
        }
        #endregion
        #region SetOffset
        void SetOffset(Resource item, bool add) {
            if (add) {
                int offset = 0;
                if (_list.Count > 1) {
                    Resource last = _list[_list.Count - 2];
                    offset = last.Offset + last.Length;
                }
                item.Offset = offset;
            } else {
                int index = _list.IndexOf(item);
                for (int i = index; i < _list.Count; i++) {
                    int offset = 0;
                    if (i > 0) {
                        Resource prev = _list[i - 1];
                        offset = prev.Offset + prev.Length;
                    }
                    _list[i].Offset = offset;
                }
            }
        }
        void SetOffset_Remove(int index) {
            for (int i = index; i < _list.Count; i++) {
                int offset = 0;
                if (i > 0) {
                    Resource prev = _list[i - 1];
                    offset = prev.Offset + prev.Length;
                }
                _list[i].Offset = offset;
            }
        }
        #endregion
        #region Remove
        /// <summary>
        /// 移除指定索引位置的资源。
        /// </summary>
        /// <param name="index">从0开始的索引，自动忽略错误。</param>
        /// <returns>返回是否操作成功。</returns>
        public bool Remove(int index) {
            if (_list == null || index < 0 || index > _list.Count - 1)
                return false;
            Resource item = _list[index];
            _list_name.Remove(item.Name);
            _list.RemoveAt(index);
            SetOffset_Remove(index);
            return true;
        }
        /// <summary>
        /// 移除指定名称的资源。
        /// </summary>
        /// <param name="name">资源名称，自动忽略错误。</param>
        /// <returns>返回操作是否成功。</returns>
        public bool Remove(string name) {
            if (_list == null || string.IsNullOrEmpty(name))
                return false;
            Resource item;
            if (!_list_name.TryGetValue(name, out item))
                return false;
            int index = _list.IndexOf(item);
            _list_name.Remove(item.Name);
            _list.RemoveAt(index);
            SetOffset_Remove(index);
            return true;
        }
        #endregion

        #region CopyTo
        /// <summary>
        /// 复制指定索引的资源到二进制数组中。
        /// </summary>
        /// <param name="index">从0开始的索引值，超出有效范围后自动忽略。</param>
        /// <param name="buffer">二进制数组。</param>
        /// <param name="startIndex">二进制数组起始位置。</param>
        /// <returns>返回是否成功。</returns>
        public bool CopyTo(int index, byte[] buffer, int startIndex) {
            if (buffer == null || startIndex < 0 || startIndex > buffer.Length - 1)
                return false;
            byte[] data = Get(index);
            if (buffer.Length - startIndex < data.Length)
                return false;
            Buffer.BlockCopy(data, 0, buffer, startIndex, data.Length);
            return true;
        }
        /// <summary>
        /// 复制指定名称的资源到二进制数组中。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="buffer">二进制数组。</param>
        /// <param name="startIndex">二进制数组起始位置。</param>
        /// <returns>返回是否成功。</returns>
        public bool CopyTo(string name, byte[] buffer, int startIndex) {
            if (buffer == null || startIndex < 0 || startIndex > buffer.Length - 1)
                return false;
            byte[] data = Get(name);
            if (buffer.Length - startIndex < data.Length)
                return false;
            Buffer.BlockCopy(data, 0, buffer, startIndex, data.Length);
            return true;
        }
        /// <summary>
        /// 复制指定索引的资源到缓冲区中。
        /// </summary>
        /// <param name="index">从0开始的索引值，超出有效范围后自动忽略。</param>
        /// <param name="buffer">缓冲区。</param>
        /// <returns>返回是否成功。</returns>
        public bool CopyTo(int index, Symbol.IO.DynamicBuffer buffer) {
            if (buffer == null)
                return false;
            byte[] data = Get(index);
            buffer.Write(data);
            return true;
        }
        /// <summary>
        /// 复制指定名称的资源到缓冲区中。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="buffer">缓冲区。</param>
        /// <returns>返回是否成功。</returns>
        public bool CopyTo(string name, Symbol.IO.DynamicBuffer buffer) {
            if (buffer == null)
                return false;
            byte[] data = Get(name);
            buffer.Write(data);
            return true;
        }
        /// <summary>
        /// 复制指定索引的资源到文件中。
        /// </summary>
        /// <param name="index">从0开始的索引值，超出有效范围后自动忽略。</param>
        /// <param name="file">文件路径，自动创建文件夹。</param>
        /// <returns>返回是否成功。</returns>
        public bool CopyTo(int index, string file) {
            if (string.IsNullOrEmpty(file))
                return false;
            byte[] data = Get(index);
            {
                string dir = System.IO.Path.GetDirectoryName(file);
                AppHelper.CreateDirectory(dir, false);
            }
            AppHelper.DeleteFile(file);
            using (System.IO.FileStream stream = System.IO.File.OpenWrite(file)) {
                stream.Write(data, 0, data.Length);
            }
            return System.IO.File.Exists(file);
        }
        /// <summary>
        /// 复制指定名称的资源到文件中。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="file">文件路径，自动创建文件夹。</param>
        /// <returns>返回是否成功。</returns>
        public bool CopyTo(string name, string file) {
            if (string.IsNullOrEmpty(file))
                return false;
            byte[] data = Get(name);
            {
                string dir = System.IO.Path.GetDirectoryName(file);
                AppHelper.CreateDirectory(dir, false);
            }
            AppHelper.DeleteFile(file);
            using (System.IO.FileStream stream = System.IO.File.OpenWrite(file)) {
                stream.Write(data, 0, data.Length);
            }
            return System.IO.File.Exists(file);
        }
        /// <summary>
        /// 复制指定索引的资源到流中。
        /// </summary>
        /// <param name="index">从0开始的索引值，超出有效范围后自动忽略。</param>
        /// <param name="stream">流。</param>
        /// <returns>返回是否成功。</returns>
        public bool CopyTo(int index, System.IO.Stream stream) {
            if (stream == null || !stream.CanWrite)
                return false;
            byte[] data = Get(index);
            stream.Write(data, 0, data.Length);
            return true;
        }
        /// <summary>
        /// 复制指定名称的资源到流中。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="stream">流。</param>
        /// <returns>返回是否成功。</returns>
        public bool CopyTo(string name, System.IO.Stream stream) {
            if (stream == null || !stream.CanWrite)
                return false;
            byte[] data = Get(name);
            stream.Write(data, 0, data.Length);
            return true;
        }
        #endregion

        #region Load
        void Load(byte[] data) {
            if (data == null || data.Length == 0)
                return;

            using (System.IO.Stream stream = new System.IO.MemoryStream(data)) {
                Load(stream);
            }
        }
        void Load(string file) {
            if (!System.IO.File.Exists(file))
                return;
            using (System.IO.FileStream stream = System.IO.File.OpenRead(file)) {
                Load(stream);
            }
        }
        bool Load(System.IO.Stream stream) {
            Clear();

            System.Collections.Generic.IDictionary<string, object> items;
            using (System.IO.Stream compresStream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Decompress, true)) {
                {
                    ushort length;
                    byte[] buffer = new byte[2];
                    if (compresStream.Read(buffer, 0, buffer.Length) != buffer.Length) {
                        return false;
                    }
                    length = BitConverter.ToUInt16(buffer, 0);
                    buffer = new byte[length];
                    if (compresStream.Read(buffer, 0, buffer.Length) != buffer.Length) {
                        return false;
                    }
                    string json = System.Text.Encoding.UTF8.GetString(buffer);
                    object root = JSON.Parse(json);
                    _version = TypeExtensions.Convert(FastObject.Path(root, "version"), -1);
                    if (_version < 1)
                        _version = 1;
                    items = FastObject.Path(root, "items") as System.Collections.Generic.IDictionary<string, object>;
                }
                if (items == null)
                    return false;
                foreach (System.Collections.Generic.KeyValuePair<string, object> item in items) {
                    Resource o = new Resource() {
                        Name = item.Key,
                        Offset = TypeExtensions.Convert(FastObject.Path(item.Value, "[0]"), -1),
                        Length = TypeExtensions.Convert(FastObject.Path(item.Value, "[1]"), -1),
                    };
                    if (string.IsNullOrEmpty(o.Name) || o.Offset < 0 || o.Length < 0)
                        continue;
                    o.Data = new byte[o.Length];
                    if (compresStream.Read(o.Data, 0, o.Length) != o.Length)
                        return false;
                    Append(o);
                }
            }
            return true;
        }
        #endregion

        #region Save
        /// <summary>
        /// 保存到缓冲区。
        /// </summary>
        /// <param name="buffer">缓冲区。</param>
        /// <returns>返回是否成功。</returns>
        public bool Save(Symbol.IO.DynamicBuffer buffer) {
            if (_list == null || buffer == null)
                return false;
            byte[] data = Save();
            buffer.Write(data);
            return true;
        }
        /// <summary>
        /// 保存并输出为二进制。
        /// </summary>
        /// <returns>返回二进制数据。</returns>
        public byte[] Save() {
            int size = 8192;
            if (_list != null && _list.Count > 0) {
                Resource item = _list[_list.Count - 1];
                size = item.Offset + item.Length + _list.Count * 120;
            }
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(size)) {
                Save(stream);
                return stream.ToArray();
            }
        }
        /// <summary>
        /// 保存到文件。
        /// </summary>
        /// <param name="file">文件路径，自动创建文件夹。</param>
        /// <returns>返回是否成功。</returns>
        public bool Save(string file) {
            if (string.IsNullOrEmpty(file))
                return false;
            {
                string dir = System.IO.Path.GetDirectoryName(file);
                AppHelper.CreateDirectory(dir, false);
            }
            AppHelper.DeleteFile(file);
            using (System.IO.FileStream stream = System.IO.File.OpenWrite(file)) {
                Save(stream);
            }
            return System.IO.File.Exists(file);
        }
        /// <summary>
        /// 保存到流中。
        /// </summary>
        /// <param name="stream">流。</param>
        /// <returns>返回是否成功。</returns>
        public bool Save(System.IO.Stream stream) {
            if (stream == null || !stream.CanWrite)
                return false;

            using (System.IO.Stream compresStream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Compress, true)) {
                System.Collections.Generic.Dictionary<string, object> root = new System.Collections.Generic.Dictionary<string, object>();
                root.Add("version", _version);
                System.Collections.Generic.Dictionary<string, object> items = new System.Collections.Generic.Dictionary<string, object>();
                root.Add("items", items);
                bool has = _list != null && _list.Count > 0;
                if (has) {
                    for (int i = 0; i < _list.Count; i++) {
                        Resource item = _list[i];
                        items.Add(item.Name, new object[] {
                        item.Offset,
                        item.Length
                    });
                    }
                }
                {
                    string json = Symbol.Serialization.Json.ToString(root);
                    byte[] buffer = BitConverter.GetBytes((ushort)json.Length);
                    compresStream.Write(buffer, 0, buffer.Length);

                    buffer = System.Text.Encoding.UTF8.GetBytes(json);
                    compresStream.Write(buffer, 0, buffer.Length);
                }
                if (has) {
                    for (int i = 0; i < _list.Count; i++) {
                        Resource item = _list[i];
                        compresStream.Write(item.Data, 0, item.Length);
                    }
                }
                compresStream.Flush();
                stream.Flush();
            }
            return true;
        }
        #endregion

        #region Clear
        /// <summary>
        /// 清空所有资源。
        /// </summary>
        public void Clear() {
            if (_list != null) {
                for (int i = 0; i < _list.Count; i++) {
                    _list[i].Data = null;
                }
                _list.Clear();
            }
            if (_list_name != null) {
                _list_name.Clear();
            }
        }
        #endregion

        #region Dispose
        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        public void Dispose() {
            Clear();
            _list = null;
            _list_name = null;
        }
        #endregion

        #endregion

        #region types

        [System.Diagnostics.DebuggerDisplay("{Name} {Length} *{Offset}")]
        class Resource {
            public string Name;
            public int Length;
            public int Offset;
            public byte[] Data;
        }

        #endregion
    }

}