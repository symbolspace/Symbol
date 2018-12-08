/*  
 *  author£ºsymbolspace
 *  e-mail£ºsymbolspace@outlook.com
 */


namespace Symbol.IO.Packing {
    partial class TreePackage {

        static byte[] Compressor_QuickLZ(byte[] buffer, bool compress) {
            return compress
                    ? Compression.CSharpQuickLZ_150_1.Compress(buffer)
                    : Compression.CSharpQuickLZ_150_1.Decompress(buffer);
        }

    }
}