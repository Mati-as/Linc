using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Mirage;
using UnityEngine;

public class UI_MultiPlayUISpawner :NetworkBehaviour
{

    private GameObject ball;


  

    protected  void Awake()
    {

        if (Server != null)
        {
            // add disconnect event so that OnServerDisconnect will be called when player disconnects
            Server.Disconnected.AddListener(OnServerDisconnect);
        }

        UI_MainController_NetworkInvolved.OnClientConnected -= OnClientConnected;
        UI_MainController_NetworkInvolved.OnClientConnected += OnClientConnected;
        
        UI_MainController_NetworkInvolved.OnConnectedToLocalServer -= OnConnectToLocalServer;
        UI_MainController_NetworkInvolved.OnConnectedToLocalServer += OnConnectToLocalServer;
    }

    // private void Start()
    // {
    //
    //     // var prefabUIwaitForHost =
    //     //     Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{nameof(UI_WaitForHost)}");
    //     // Managers.Network.ClientObjectManager.RegisterPrefab(prefabUIwaitForHost.GetNetworkIdentity());
    //     //
    //     // var prefabUIMultiSelection =
    //     //     Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{nameof(UI_PlayModeSelection)}");
    //     // Managers.Network.ClientObjectManager.RegisterPrefab(prefabUIMultiSelection.GetNetworkIdentity());
    //     //
    //     // var prefabUinstrumentSelection =
    //     //     Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{nameof(UI_InstrumentSelection)}");
    //     // Managers.Network.ClientObjectManager.RegisterPrefab(prefabUinstrumentSelection.GetNetworkIdentity());
    //     //
    //     
    //     
    // }


    protected  void OnDestroy()
    {
        UI_MainController_NetworkInvolved.OnConnectedToLocalServer -= OnClientConnected;
        UI_MainController_NetworkInvolved.OnClientConnected -= OnConnectToLocalServer;
    }


    private void OnClientConnected(INetworkPlayer player)
    {
        
        DOVirtual.Float(0, 0, 0.3f, _ => { })
            .OnComplete(() =>
            {
               
                
                if (Managers.Network.Server.IsHost)
                {
                    var ui_WaitForHost =
                        Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{nameof(UI_WaitForHost)}");
                    var inst_WaitForHost = Instantiate(ui_WaitForHost);

                    var ui_PlayModeSelection =
                        Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{nameof(UI_PlayModeSelection)}");
                    var inst_PlayModeSelection = Instantiate(ui_PlayModeSelection);

                    var ui_UI_InstrumentSelection =
                        Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{nameof(UI_InstrumentSelection)}");
                    var inst_UI_InstrumentSelection = Instantiate(ui_UI_InstrumentSelection);
                    
                    Managers.Network.ServerObjectManager.Spawn(inst_PlayModeSelection);
                    Managers.Network.ServerObjectManager.Spawn(inst_WaitForHost);
                    Managers.Network.ServerObjectManager.Spawn(inst_UI_InstrumentSelection);
                    
                          
                    StoreNetworkIdentity(Define.NetworkObjs.UI_PlayModeSelection, inst_PlayModeSelection);
                    StoreNetworkIdentity(Define.NetworkObjs.UI_InstrumentSelection, inst_UI_InstrumentSelection);
                    StoreNetworkIdentity(Define.NetworkObjs.UI_WaitForHost, inst_WaitForHost);
                        
                    Managers.NetworkObjs.Add((int)Define.NetworkObjs.UI_InstrumentSelection,inst_UI_InstrumentSelection);
                    Managers.NetworkObjs.Add((int)Define.NetworkObjs.UI_WaitForHost,inst_WaitForHost);
                    Managers.NetworkObjs.Add((int)Define.NetworkObjs.UI_PlayModeSelection,inst_PlayModeSelection);

                    Logger.Log($"등록 및 Spawn 완료 : From Server----------------");
                }
                
       
              
                DOVirtual.Float(0, 0, 0.8f, _ => { })
                    .OnComplete(() =>
                    {
                       
                        
                        foreach (var key in Managers.NetworkObjNetworkIds.Keys.ToArray())
                        {
                            Logger.Log($"{(Define.NetworkObjs)key} -> NetID :{Managers.NetworkObjNetworkIds[key]}");
                        }
                  
                    });

                DOVirtual.Float(0, 0, 2f, _ =>
                {

               
                }).OnComplete(() =>
                {
                    if (!Managers.Network.Server.IsHost)
                    {
                        foreach (var networkIdentity in Managers.Network.ClientObjectManager.spawnPrefabs)
                        {
                            Logger.Log($"Client: {networkIdentity.gameObject.name} -> NetID {networkIdentity.NetId}");
                        }
                    }
                    
                    foreach (var key in Managers.Network.ClientObjectManager.spawnableObjects.Values.ToList())
                    {
                        Logger.Log($"Client Registered Spawned 객체: {key.gameObject.name}");
                    }
                });
            
                Logger.Log("Spawnable Ready--------------------------------------------");
                Managers.Network.ClientObjectManager.PrepareToSpawnSceneObjects();
            });
        
    }
    

    private void StoreNetworkIdentity(Define.NetworkObjs obj ,GameObject spawnedObject)
    {
        var networkIdentity = spawnedObject.GetComponent<NetworkIdentity>();
        if (networkIdentity != null)
        {
            Debug.Log($"NetworkIdentity netId: {networkIdentity.NetId}");
            Managers.NetworkObjNetworkIds.Add((int)obj,networkIdentity);
            
        }
        else
        {
            Debug.LogWarning("NetworkIdentity가 없습니다.");
        }
    }
    
    
    
    
    /// <summary>
    /// 동기화를 위해 살짝느리게 실행
    /// </summary>
    /// <param name="player"></param>
       private void OnConnectToLocalServer(INetworkPlayer player)
    {
        DOVirtual.Float(0, 0, 3f, _ => { })
            .OnComplete(() =>
            {
               Logger.Log("Spawnable Ready--------------------------------------------");
                
               // Managers.Network.ClientObjectManager.PrepareToSpawnSceneObjects();
              
           
            });
        
    }

    public void OnServerDisconnect(INetworkPlayer _)
    {
        // after 1 player disconnects then destroy the balll
        if (ball != null)
            ServerObjectManager.Destroy(ball);
    }
}
