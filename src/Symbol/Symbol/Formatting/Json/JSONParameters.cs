/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Formatting.Json {
    /// <summary>
    /// JSON控制参数。
    /// </summary>
    public sealed class JSONParameters {

        /// <summary>
        /// Use the optimized fast Dataset Schema format (default = True)
        /// </summary>
        public bool UseOptimizedDatasetSchema = true;
        /// <summary>
        /// Use the fast GUID format (default = True)
        /// </summary>
        public bool UseFastGuid = false;
        /// <summary>
        /// Serialize null values to the output (default = True)
        /// </summary>
        public bool SerializeNullValues = true;
        /// <summary>
        /// Use the UTC date format (default = false)
        /// </summary>
        public bool UseUTCDateTime = false;
        /// <summary>
        /// Use the new Date(0000000) (default = false)
        /// </summary>
        public bool UseJavascriptDateTime = false;
        /// <summary>
        /// Use the \/Date(0000000)\/ (default = True)
        /// </summary>
        public bool UseJsonDateTime = true;

        /// <summary>
        /// Show the readonly properties of types in the output (default = False)
        /// </summary>
        public bool ShowReadOnlyProperties = false;
        /// <summary>
        /// Use the $types extension to optimise the output json (default = false)
        /// </summary>
        public bool UsingGlobalTypes = false;
        /// <summary>
        /// Ignore case when processing json and deserializing 
        /// </summary>
        [Obsolete("Not needed anymore and will always match")]
        public bool IgnoreCaseOnDeserialize = false;
        /// <summary>
        /// Anonymous types have read only properties  (default = True)
        /// </summary>
        public bool EnableAnonymousTypes = true;
        /// <summary>
        /// Enable _ extensions $types, $type, $map (default = True)
        /// </summary>
        public bool UseExtensions = false;
        /// <summary>
        /// Use escaped unicode i.e. \uXXXX format for non ASCII characters (default = false)
        /// </summary>
        public bool UseEscapedUnicode = false;
        /// <summary>
        /// Output string key dictionaries as "k"/"v" format (default = False) 
        /// </summary>
        public bool KVStyleStringDictionary = false;
        /// <summary>
        /// Output Enum values instead of names (default = true)
        /// </summary>
        public bool UseValuesOfEnums = true;
        /// <summary>
        /// Ignore attributes to check for (default : XmlIgnoreAttribute, NonSerialized)
        /// </summary>
        public System.Collections.Generic.List<Type> IgnoreAttributes = new System.Collections.Generic.List<Type> {
            typeof(System.Xml.Serialization.XmlIgnoreAttribute),
            typeof(System.NonSerializedAttribute),
            typeof(IgnoreAttribute)
        };
        /// <summary>
        /// If you have parametric and no default constructor for you classes (default = False)
        /// 
        /// IMPORTANT NOTE : If True then all initial values within the class will be ignored and will be not set
        /// </summary>
        public bool ParametricConstructorOverride = false;
        /// <summary>
        /// Serialize DateTime milliseconds i.e. yyyy-MM-dd HH:mm:ss.nnn (default = false)
        /// </summary>
        public bool DateTimeMilliseconds = false;
        /// <summary>
        /// Maximum depth for circular references in inline mode (default = 20)
        /// </summary>
        public byte SerializerMaxDepth = 20;
        /// <summary>
        /// Inline circular or already seen objects instead of replacement with $i (default = False) 
        /// </summary>
        public bool InlineCircularReferences = false;
        /// <summary>
        /// Save property/field names as lowercase (default = false)
        /// </summary>
        public bool SerializeToLowerCaseNames = false;

        /// <summary>
        /// 
        /// </summary>
        public void FixValues() {
            if (UseExtensions == false) // disable conflicting params
            {
                UsingGlobalTypes = false;
                InlineCircularReferences = true;
            }
            if (EnableAnonymousTypes)
                ShowReadOnlyProperties = true;
        }
    }

}