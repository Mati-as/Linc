using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DG.Tweening;
using Network;
using UnityEngine;
using INetworkPlayer = Mirage.INetworkPlayer;

public class ClientListener : MonoBehaviour
{
    public int broadcastPort = 7777;
    private UdpClient udpClient;

    public void ListenBroadcastMessage()
    {
        // register a handler to listen for the broadcasted message
        //Managers.Network.Client.Connect(Managers.UdpSocketFactory.Address);
       // Managers.Network.Client.MessageHandler.RegisterHandler<ServerBroadcastMessage>(OnReceiveBroadcast());

       Logger.Log($" Connecting To : {Managers.HostIPAdress}");
       Logger.Log("Listening for broadcast messages...");
    }

    // this function will be triggered when the client receives a broadcast message
    private void OnReceiveBroadcast(INetworkPlayer player, ServerBroadcastMessage message)
    {
        Logger.Log($"Received broadcast: ServerIP: {message.ServerIP}, Port: {message.ServerPort}");

        // connect to the server using the received IP and port
        Managers.UdpSocketFactory.Address = message.ServerIP;
        Managers.UdpSocketFactory.Port = (ushort)message.ServerPort;
        Managers.Network.Client.Connect(Managers.UdpSocketFactory.Address);
    }
    
    

}