using System.Runtime.InteropServices;using DG.Tweening;
using Mirage;
using UnityEngine;
using UnityEngine.Serialization;
using DG.Tweening;

namespace Network
{
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public class ServerBroadcaster : MonoBehaviour
    {
        public void SendBroadcast()
        {
            
            //
            // var address = new ServerBroadcastMessage();
            // address.ServerIP = Managers.UdpSocketFactory.Address;
            // address.ServerPort = Managers.UdpSocketFactory.Port;
            //
            // var seq = DOTween.Sequence();
            // seq.AppendCallback(() =>
            // {
            //     Managers.Network.BroadcastMessage($"{address.ServerIP}",SendMessageOptions.DontRequireReceiver);
            //     Logger.Log($"Send BroadCast.....{address.ServerIP}");
            // });
            // seq.AppendInterval(1f);
            // seq.SetLoops(10);
        }
 
        
    }
    

}   
[NetworkMessage]
public struct ServerBroadcastMessage 
{
    public string ServerIP;
    public ushort ServerPort;
}