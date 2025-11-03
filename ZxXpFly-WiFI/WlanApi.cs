using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

public static class WlanApi
{
    private const uint WLAN_CLIENT_VERSION_VISTA = 2;
    private const uint ERROR_SUCCESS = 0;

    #region dll import
    [DllImport("Wlanapi.dll")]
    private static extern uint WlanOpenHandle(uint dwClientVersion, IntPtr pReserved,
                                              out uint pdwNegotiatedVersion, out IntPtr phClientHandle);

    [DllImport("Wlanapi.dll")]
    private static extern uint WlanCloseHandle(IntPtr hClientHandle, IntPtr pReserved);

    [DllImport("Wlanapi.dll")]
    private static extern uint WlanEnumInterfaces(IntPtr hClientHandle, IntPtr pReserved,
                                                  out IntPtr ppInterfaceList);

    [DllImport("Wlanapi.dll")]
    private static extern uint WlanGetAvailableNetworkList(
        IntPtr hClientHandle, [MarshalAs(UnmanagedType.LPStruct)] Guid pInterfaceGuid,
        uint dwFlags, IntPtr pReserved, out IntPtr ppAvailableNetworkList);

    [DllImport("Wlanapi.dll")]
    private static extern void WlanFreeMemory(IntPtr pMemory);
    #endregion

    #region structures
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WLAN_INTERFACE_INFO
    {
        public Guid InterfaceGuid;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string strInterfaceDescription;
        public int isState;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WLAN_INTERFACE_INFO_LIST
    {
        public uint dwNumberOfItems;
        public uint dwIndex;
        // 这里**不能**直接写 WLAN_INTERFACE_INFO InterfaceInfo[1];
        // 因为 C# 会把整个数组一次性算大小，导致尺寸不对。
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WLAN_AVAILABLE_NETWORK
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] dot11Ssid;
        public uint dot11SsidLength;
        public uint dot11BssType;
        public uint dwNumberOfBssids;
        public byte bNetworkConnectable;
        public uint wlanNotConnectableReason;
        public uint dwNumberOfPhyTypes;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public uint[] dot11PhyTypes;
        public byte bMorePhyTypes;
        public uint wlanSignalQuality;   // 0-100
        public byte bSecurityEnabled;
        public uint dot11DefaultAuthAlgorithm;
        public uint dot11DefaultCipherAlgorithm;
        public uint dwFlags;
        public uint dwReserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WLAN_AVAILABLE_NETWORK_LIST
    {
        public uint dwNumberOfItems;
        public uint dwIndex;
    }
    #endregion

    public static List<(string ssid, uint signal)> GetAvailableNetworks()
    {
        var ans = new List<(string, uint)>();

        uint ver;
        IntPtr client = IntPtr.Zero;
        if (WlanOpenHandle(WLAN_CLIENT_VERSION_VISTA, IntPtr.Zero, out ver, out client) != ERROR_SUCCESS)
            return ans;

        try
        {
            IntPtr pIfList;
            if (WlanEnumInterfaces(client, IntPtr.Zero, out pIfList) != ERROR_SUCCESS)
                return ans;

            uint ifCount = (uint)Marshal.ReadInt32(pIfList);               // dwNumberOfItems
            int ifSize = Marshal.SizeOf(typeof(WLAN_INTERFACE_INFO));
            IntPtr pIf = (IntPtr)(pIfList.ToInt64() + Marshal.SizeOf(typeof(WLAN_INTERFACE_INFO_LIST)));

            for (int i = 0; i < ifCount; i++)
            {
                var iface = (WLAN_INTERFACE_INFO)Marshal.PtrToStructure(
                                (IntPtr)(pIf.ToInt64() + i * ifSize),
                                typeof(WLAN_INTERFACE_INFO));

                IntPtr pNetList;
                if (WlanGetAvailableNetworkList(client, iface.InterfaceGuid,
                                                2 /*include all*/, IntPtr.Zero, out pNetList) != ERROR_SUCCESS)
                    continue;

                uint netCount = (uint)Marshal.ReadInt32(pNetList);
                int netSize = Marshal.SizeOf(typeof(WLAN_AVAILABLE_NETWORK));
                IntPtr pNet = (IntPtr)(pNetList.ToInt64() + Marshal.SizeOf(typeof(WLAN_AVAILABLE_NETWORK_LIST)));

                for (int j = 0; j < netCount; j++)
                {
                    var net = (WLAN_AVAILABLE_NETWORK)Marshal.PtrToStructure(
                                    (IntPtr)(pNet.ToInt64() + j * netSize),
                                    typeof(WLAN_AVAILABLE_NETWORK));
                    if (net.dot11SsidLength == 0) continue;

                    string ssid = Encoding.UTF8.GetString(net.dot11Ssid, 0, (int)net.dot11SsidLength);
                    ans.Add((ssid, net.wlanSignalQuality));
                }

                WlanFreeMemory(pNetList);
            }
            WlanFreeMemory(pIfList);
        }
        finally
        {
            WlanCloseHandle(client, IntPtr.Zero);
        }
        return ans;
    }
}