/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */


namespace Symbol.IO.Packing {
    partial class TreePackage {

        static byte[] Compressor_Lzma(byte[] buffer, bool compress) {
            return compress
                    ? Compression.LzmaHelper.Compress(buffer)
                    : Compression.LzmaHelper.Decompress(buffer);
        }

    }
}