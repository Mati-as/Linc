using System;
using System.Linq;
using DG.Tweening;
using Mirage;
using UnityEngine;

public class UI_MultiPlayUISpawner : CharacterSpawner
{

    private GameObject ball;

    protected override void Awake()
    {
        base.Awake();

        if (Server != null)
        {
            // add disconnect event so that OnServerDisconnect will be called when player disconnects
            Server.Disconnected.AddListener(OnServerDisconnect);
        }

        UI_MainController_NetworkInvolved.OnClientConnected -= OnClientConnected;
        UI_MainController_NetworkInvolved.OnClientConnected += OnClientConnected;


    }

    private void Start()
    {
        var prefabUIMultiSelection =
            Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{nameof(UI_PlayModeSelection)}");
        Managers.Network.ClientObjectManager.RegisterPrefab(prefabUIMultiSelection.GetNetworkIdentity());
        var prefabUIwaitForHost =
            Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{nameof(UI_WaitForHost)}");
        Managers.Network.ClientObjectManager.RegisterPrefab(prefabUIwaitForHost.GetNetworkIdentity());
    }
    

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UI_MainController_NetworkInvolved.OnClientConnected -= OnClientConnected;
    }


    private void OnClientConnected(INetworkPlayer player)
    {    
       
        
        DOVirtual.Float(0, 0, 1f, _ => { })
            .OnComplete(() =>
            {
                
                if (Managers.Network.Server.IsHost)
                {
                    Debug.Log("UI_WaitForHost UI Generate-----------------------");
                    
                    var ui_waitForHost =
                        Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{nameof(UI_PlayModeSelection)}");
                    var inst_waitforHost = Instantiate(ui_waitForHost);
                    
                    Managers.Network.ServerObjectManager.Spawn(inst_waitforHost);
                    
                    
                    var ui_MultimodeSelection =
                        Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{nameof(UI_WaitForHost)}");
                    var inst_playmode = Instantiate(ui_MultimodeSelection);
                    
                  
                    Managers.Network.ServerObjectManager.Spawn(inst_playmode);
                }
            

            });
    }

    // override OnServerAddPlayer so to do custom spawn location for character
    // this method will be called by base class when player sends `AddCharacterMessage`
    public override void OnServerAddPlayer(INetworkPlayer player)
    {
        // if (Managers.Network.Server.IsHost)
        // {
        //     Debug.Log("UI_WaitForHost UI Generate-----------------------");
        //     var ui_waitForHost = Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{nameof(UI_PlayModeSelection)}");
        //     var inst_waitforHost = Instantiate(ui_waitForHost);
        //     ServerObjectManager.Spawn(inst_waitforHost);
        // }
        //
        // else
        // {
        //     Debug.Log("UI_PlayModeSelection UI Generate-----------------------");
        //     var ui_MultimodeSelection =
        //         Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{nameof(UI_WaitForHost)}");
        //     var inst_playmode = Instantiate(ui_MultimodeSelection);
        //     ServerObjectManager.Spawn(inst_playmode);
        // }
    }

    public void OnServerDisconnect(INetworkPlayer _)
    {
        // after 1 player disconnects then destroy the balll
        if (ball != null)
            ServerObjectManager.Destroy(ball);
    }
}
