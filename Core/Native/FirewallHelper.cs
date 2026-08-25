using System;

namespace BoostX.Core.Native
{
    public static class FirewallHelper
    {
        public static void BlockIpOrDomain(string ruleName, string remoteAddresses)
        {
            string cmd = $"netsh advfirewall firewall add rule name=\"{ruleName}\" dir=out action=block remoteip={remoteAddresses}";
            TokenElevation.ExecuteCommand(cmd);
        }

        public static void RemoveRule(string ruleName)
        {
            string cmd = $"netsh advfirewall firewall delete rule name=\"{ruleName}\"";
            TokenElevation.ExecuteCommand(cmd);
        }
    }
}