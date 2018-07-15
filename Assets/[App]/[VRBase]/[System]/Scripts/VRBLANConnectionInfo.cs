


public class VRBLANConnectionInfo
{
    public string ipAddress;
    public string name;


    public VRBLANConnectionInfo(string fromAddress, string data)
    {
        ipAddress = fromAddress.Substring(fromAddress.LastIndexOf(":") + 1, fromAddress.Length - fromAddress.LastIndexOf(":")-1);
        name = "local";
    }
}