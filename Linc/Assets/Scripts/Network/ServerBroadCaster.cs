using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace Network
{
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public class ServerBroadcaster : MonoBehaviour
    {
        public string broadcastMessage_IpAddressPort;// 서버의 IP와 포트를 포함한 메시지
        public int broadcastPort = 7777;

        private UdpClient udpClient;

        public void SendBroadcast()
        {
            udpClient = new UdpClient();
            udpClient.EnableBroadcast = true;
            broadcastMessage_IpAddressPort = "1";// $"ServerIP:{Managers.UdpSocketFactory.Address};Port:{Managers.Port}";
            InvokeRepeating("BroadcastServerInfo", 0f, 1f); // 5초마다 브로드캐스트 전송
        }

        void BroadcastServerInfo()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
             
            byte[] encoded = Encoding.UTF8.GetBytes(broadcastMessage_IpAddressPort);
            udpClient.Send(encoded, encoded.Length, endPoint);

           
            Logger.Log($"Broadcasting message : {broadcastMessage_IpAddressPort}" +
                      $",expected received message: {Encoding.UTF8.GetString(encoded)}\n" +
                      $"length:{encoded.Length}");
            
            if (encoded.Length > 512)
            {
                Logger.LogError("Received data is too large for UDP packet.");
            }
        }

        void OnDestroy()
        {
            udpClient?.Close();
        }
    }
}