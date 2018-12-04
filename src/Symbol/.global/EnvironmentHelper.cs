/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

/// <summary>
/// Environment 辅助类。
/// </summary>
public static class EnvironmentHelper {

    #region fields
#if !netcore
    private const short PROCESSOR_ARCHITECTURE_INTEL = 0;
    private const short PROCESSOR_ARCHITECTURE_IA64 = 6;
    private const short PROCESSOR_ARCHITECTURE_AMD64 = 9;
    private const int PROCESSOR_ARCHITECTURE_UNKNOWN = 0x00FFFF;
#endif 
    #endregion

    #region properties
    /// <summary>
    /// 是否为64位操作系统。
    /// </summary>
    public static bool Is64BitOperatingSystem {
        get {
#if !netcore
            SYSTEM_INFO sysinfo = new SYSTEM_INFO();
            GetNativeSystemInfo(out sysinfo);
            if (sysinfo.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_IA64 || sysinfo.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_AMD64)
                return true;
            return false;
#elif netcore13
            return Is64BitProcess;
#else
            return Environment.Is64BitOperatingSystem;
#endif
        }
    }

    /// <summary>
    /// 是否为64位进程。
    /// </summary>
    public static bool Is64BitProcess {
        get {
            return IntPtr.Size == 8;
        }
    }
    #endregion

    #region methods

#if !netcore
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern void GetNativeSystemInfo(out SYSTEM_INFO lpSystemInfo);
#endif
//    /// <summary>
//    /// 获取资源文本
//    /// </summary>
//    /// <param name="key"></param>
//    /// <returns></returns>
//    public static string GetResourceString(string key) {
//#if net20 || net35
//        return FastWrapper.MethodInvoke(typeof(Environment), "GetResourceFromDefault", key) as string;
//#else
//        return key;
//#endif
//    }

#endregion


#region types
#if !netcore
    /// <summary>
    /// 系统信息结构。
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct SYSTEM_INFO {
        internal short wProcessorArchitecture;
        internal short wReserved;
        internal int dwPageSize;
        internal IntPtr lpMinimumApplicationAddress;
        internal IntPtr lpMaximumApplicationAddress;
        internal IntPtr dwActiveProcessorMask;
        internal int dwNumberOfProcessors;
        internal int dwProcessorType;
        internal int dwAllocationGranularity;
        internal short wProcessorLevel;
        internal short wProcessorRevision;
    }
#endif

#endregion
}