/*  
 *  author£ºsymbolspace
 *  e-mail£ºsymbolspace@outlook.com
 */
namespace Symbol.IO.Packing {
    partial class TreePackage {
        interface IEntry {
            void SetKey(string value);
            void SetIsAdd(bool value);
            KeyEntry KeyEntry { get; set; }
        }
    }
}