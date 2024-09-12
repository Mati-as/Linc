using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Network;
using UnityEngine;
using INetworkPlayer = Mirage.INetworkPlayer;

public class ClientListener : MonoBehaviour
{
    public int broadcastPort = 7777;
    private UdpClient udpClient;

    public void ListenBroadcastMessage()
    {
        //udpClient = new UdpClient(broadcastPort);

        Managers.Network.Client.MessageHandler.RegisterHandler<ServerSendIPAdress>(OnClientReciveIPAdress, false);
        
        
    }
    public void OnClientReciveIPAdress(INetworkPlayer player, ServerSendIPAdress address)  
    {
        Logger.Log("Listening BroadCast.....");
        Managers.Network.Client.Connect(address.ServerIP);
    }
    //
    // string ExtractIP(string message)
    // {
    //    
    //     int startIndex = message.IndexOf("ServerIP:") + "ServerIP:".Length;
    //     int endIndex = message.IndexOf(";", startIndex);
    //     return message.Substring(startIndex, endIndex - startIndex);
    // }
    //
    // int ExtractPort(string message)
    // {
    //   
    //     int startIndex = message.IndexOf("Port:") + "Port:".Length;
    //     return int.Parse(message.Substring(startIndex));
    // }
    //
    // void OnBroadcastReceived(IAsyncResult result)
    // {
    //     IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, broadcastPort);
    //     byte[] data = udpClient.EndReceive(result, ref endPoint);
    //     byte[] nullDeletedData  = data.Where(b => b != 0).ToArray();
    //     byte[] filteredData = Encoding.UTF8.GetPreamble().Concat(nullDeletedData).ToArray();
    //     string message = Encoding.UTF8.GetString(filteredData).Trim();
    //
    //    
    //     if (message != string.Empty)
    //     {
    //         Logger.Log($"Received message : {message},length : {filteredData.Length}");
    //     }
    //     
    //
    //     // 서버 IP 및 포트를 추출하여 연결
    //     string serverIp = ExtractIP(message);
    //     int serverPort = ExtractPort(message);
    //
    //     // 서버 IP와 포트를 이용해 클라이언트 연결 시도
    //     Managers.UdpSocketFactory.Address = message;
    //     Managers.UdpSocketFactory.Port = (ushort)serverPort;
    //     Managers.Network.Client.Connect(Managers.UdpSocketFactory.Address);
    //
    //     // 다시 브로드캐스트 수신 대기
    //     udpClient.BeginReceive(OnBroadcastReceived, null);
    // }
    //
    // void OnDestroy()
    // {
    //     udpClient?.Close();
    // }
    

}