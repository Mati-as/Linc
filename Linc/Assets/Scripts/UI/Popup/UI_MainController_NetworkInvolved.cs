using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using DG.Tweening;
using Mirage;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_MainController_NetworkInvolved : UI_Scene
{

    public NetworkManager networkManager;
    public enum Btns
    {
        Btn_Start,
        Btn_HideMenu,
        Btn_Play,
        Btn_Replay,
        Btn_Quit,
        Btn_ApplicationQuit,
        Btn_Setting
        
    }

    public enum UIObjs
    {
        Btn_Menus,
        Image_Background
    }
    
    
    private Canvas _canvas;
    private Animator _menuAnimator;
    private int UI_ON = Animator.StringToHash("On");
    private bool _isUiOn = false;

    private Image _bg;
    private Color _defaultColor;

    public GameObject UI_Lobby;


    public static event Action OnStartBtnClickedAction;
    
    //network
    public static event Action OnConnectedToLocalServer;
    public static event Action OnClientConnected;
    public static event Action OnConnectFailed;

    public override bool Init()
    {
        if (networkManager == null)
        {
            networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>();
        }
  
       
        networkManager.Server.Connected.AddListener(player =>
        {
            
            // Check if the connected client is NOT the local player (host)
            if (!player.IsHost)
            {
                
                Debug.Log("A non-host client connected to the server.");
                OnClientConnected?.Invoke(); // Trigger server-side logic for non-host clients
            }
            else
            {
                
                Debug.Log("Host client connected, skipping OnClientConnected logic.");
            }
        });

        // Client-side connection listener (no RPC calls here)
       networkManager.Client.Connected.AddListener(player =>
        {
            
            Debug.Log("Connected to server.");
            if (!player.IsHost)
            {
                Debug.Log("A non-host client connected to the server.");
                OnConnectedToLocalServer?.Invoke();; // Trigger server-side logic for non-host clients
            }
            else
            {
                Debug.Log("Host client connected, skipping OnClientConnected logic.");
            }
           
        });

       networkManager.Client.Disconnected.AddListener(_ =>
        {
            Debug.Log("Disconnected from server.");
            OnConnectFailed?.Invoke();;
        });

       
        
        BindObject(typeof(UIObjs));
        BindButton(typeof(Btns));
        
        _menuAnimator = GetObject((int)UIObjs.Btn_Menus).gameObject.GetComponent<Animator>();
        GetButton((int)Btns.Btn_HideMenu).gameObject.BindEvent(ToggleAnimation);
        
        _bg = GetObject((int)UIObjs.Image_Background).gameObject.GetComponent<Image>();
        _defaultColor = _bg.color;
        GetButton((int)Btns.Btn_Start).gameObject.BindEvent(OnStartBtnClicked);
        GetButton((int)Btns.Btn_Start).gameObject.SetActive(false);
        GetButton((int)Btns.Btn_Start).gameObject.GetComponent<Image>().DOFade(0, 0.01f);
        GetButton((int)Btns.Btn_Start).gameObject.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.01f);
        _canvas = GetComponent<Canvas>();
        _canvas.sortingOrder = -321; // 항상플레이 화면에만 있을수 있도록

        SetInGameUIs(false);
            
        
        UI_Lobby = Utils.FindChild<UI_Lobby>(gameObject, recursive: true).gameObject;
        UI_Lobby.SetActive(false);
        
        GetButton((int)Btns.Btn_Quit).gameObject.BindEvent(OnQuitBtnClicked);
        
        //event setting
        global::UI_Lobby.OnHostStartSetting -=ClientRPCEvent_OnHostStartSetting;
        global::UI_Lobby.OnHostStartSetting +=ClientRPCEvent_OnHostStartSetting;

        UI_InstrumentSelection.OnInstrumentSelected -= RPCEvent_OnHostGameSettingFinished;
        UI_InstrumentSelection.OnInstrumentSelected += RPCEvent_OnHostGameSettingFinished;
        return base.Init();
    }

    private void OnDestroy()
    {
        //event setting
        global::UI_Lobby.OnHostStartSetting -=ClientRPCEvent_OnHostStartSetting;
        UI_InstrumentSelection.OnInstrumentSelected -= RPCEvent_OnHostGameSettingFinished;
    }

    private void SetInGameUIs(bool value)
    {
        GetButton((int)Btns.Btn_Quit).gameObject.SetActive(value);
        GetButton((int)Btns.Btn_Setting).gameObject.SetActive(value);
        GetButton((int)Btns.Btn_ApplicationQuit).gameObject.SetActive(value);
    }

    public void ShowStartBtn()
    {
        GetButton((int)Btns.Btn_Start).gameObject.SetActive(true);
        Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Common/UI_Message_Button", 0.3f);
        
        GetButton((int)Btns.Btn_Start).gameObject.GetComponent<Image>().DOFade(1, 0.5f).SetDelay(1.5f);
        GetButton((int)Btns.Btn_Start).gameObject.GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.5f).SetDelay(1.5f);

    }

    private void OnQuitBtnClicked()
    {
        Managers.UI.CloseAllPopupUI();
        Managers.Scene.ChangeScene(Define.Scene.linc_main_solo);
    }
    private void OnStartBtnClicked()
    {
        Debug.Log("Clicked");
        _bg.DOFade(0, 1f);
       
        GetButton((int)Btns.Btn_Start).gameObject.GetComponent<Image>().DOFade(0, 0.5f);
        GetButton((int)Btns.Btn_Start).gameObject.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.5f).OnComplete(
            () =>
            {
                GetButton((int)Btns.Btn_Start).gameObject.SetActive(false);
            });
        SetInGameUIs(true);
        
        OnStartBtnClickedAction?.Invoke();
    }

    private void OnApplicationQuitClicked()
    {
        
    }

    private void ToggleAnimation()
    {
        _isUiOn = !_isUiOn;   
        _menuAnimator.SetBool(UI_ON,_isUiOn);
    }
    
  
    private void RPCEvent_OnHostGameSettingFinished()
    {
        if (!Identity.isActiveAndEnabled)
        {
            Logger.LogError("xxxxxxxxxxx netId Is inactive.");
        }
        else
        {
            Logger.Log("ooooooooo netId is Active.");
        }
        RpcInstrumentSelection((int)Define.Instrument.Drum, (int)Define.Instrument.HandBell);
    }
    
    [ClientRpc]
    private void RpcInstrumentSelection(int hostInstrument, int clientInstrument)
    {
        Logger.Log($"Instrument RPC received: HostInstrument={hostInstrument}, ClientInstrument={clientInstrument}");
        PerformButtonClick();
    }

    private void PerformButtonClick()
    {
        Logger.Log("Performing Button Click (both)");
        Managers.UI.ClosePopupUI(Managers.UI.FindPopup<UI_WaitForHost>());
        Managers.UI.SceneUI.GetComponent<UI_MainController_NetworkInvolved>().UI_Lobby.SetActive(false);
        Managers.UI.SceneUI.GetComponent<UI_MainController_NetworkInvolved>().ShowStartBtn();
    }
    
    
   
    public void ClientRPCEvent_OnHostStartSetting()
    {
        if (!Identity.isActiveAndEnabled)
        {
            Logger.LogError("xxxxxxxxxxx netId Is inactive.");
        }
        else
        {
            Logger.Log("ooooooooooo netId is Active.");
        }
        ClientRPC_ShowWaitForHostUI();
    }
    
    [ClientRpc]
    public void ClientRPC_ShowWaitForHostUI()
    {
        Managers.UI.ShowPopupUI<UI_WaitForHost>();
    }
    
    

}
