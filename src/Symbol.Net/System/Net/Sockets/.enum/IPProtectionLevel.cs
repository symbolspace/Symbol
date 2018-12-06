/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

#if net20 || net35
using System;
namespace System.Net.Sockets {

    /// <summary>
    /// 一个可用于将 IPv6 套接字限制为一个指定范围的值，例如限制为具有相同的链接本地或站点本地前缀的地址。
    /// </summary>
    public enum IPProtectionLevel {
        /// <summary>
        /// IP 保护级别是“未指定的”。在 Windows 7 和 Windows Server 2008 R2 中，针对套接字的 IP 保护级别的默认值是“未指定的”。
        /// </summary>
        Unspecified = -1,
        /// <summary>
        /// IP 保护级别是“不受限的”。此值应由设计为在 Internet 上运行的应用程序使用，包括利用 Windows 中内置的 IPv6 NAT 遍历功能（例如，Teredo）的应用程序。这些应用程序可能会绕过
        /// IPv4 防火墙，因此，必须加强应用程序的安全性以防范针对开放端口的 Internet 攻击。在 Windows Server 2008 R2 和 Windows
        /// Vista 中，针对套接字的 IP 保护级别的默认值是“不受限的”。
        /// </summary>
        Unrestricted = 10,
        /// <summary>
        /// IP 保护级别是“边缘受限的”。此值应由设计为在 Internet 上运行的应用程序使用。此设置不允许使用 Windows Teredo 实现的网络地址转换
        /// (NAT) 遍历。这些应用程序可能会绕过 IPv4 防火墙，因此，必须加强应用程序的安全性以防范针对开放端口的 Internet 攻击。在 Windows
        /// Server 2003 和 Windows XP 中，针对套接字的 IP 保护级别的默认值是“边缘受限的”。
        /// </summary>
        EdgeRestricted = 20,
        /// <summary>
        /// IP 保护级别是“受限的”。此值应由未实现 Internet 方案的 Intranet 应用程序使用。一般情况下，不会针对 Internet 样式的攻击来对这些应用程序进行测试或加强安全性。此设置将限制仅接收链接本地的通信。
        /// </summary>
        Restricted = 30
    }


}
#endif