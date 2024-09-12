using System.Runtime.InteropServices;
using Mirage;
using UnityEngine;
using UnityEngine.Serialization;

namespace Network
{
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public class ServerBroadcaster : MonoBehaviour
    {


        public void SendBroadcast()
        {
            
            Logger.Log("Send BroadCast.....");
            var address = new ServerSendIPAdress();
            address.ServerIP = Managers.UdpSocketFactory.Address;
            address.ServerPort = Managers.UdpSocketFactory.Port;
            Managers.Network.Client.Send(address);

        }
 
        
    }
    
    public struct ServerSendIPAdress  
    {
        public string ServerIP;
        public int ServerPort;
    }
}