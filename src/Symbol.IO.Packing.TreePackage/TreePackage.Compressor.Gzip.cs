/*  
 *  author£ºsymbolspace
 *  e-mail£ºsymbolspace@outlook.com
 */


namespace Symbol.IO.Packing {
    partial class TreePackage {

        static byte[] Compressor_Gzip(byte[] buffer, bool compress) {
            return compress
                    ? Compression.GzipHelper.Compress(buffer)
                    : Compression.GzipHelper.Decompress(buffer);
        }

    }
}