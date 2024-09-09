using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using DG.Tweening;
using Mirage;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using NetworkPlayer = Mirage.NetworkPlayer;

public class UI_MainController_NetworkInvolved : UI_Scene
{

 
    public enum Btns
    {
        Btn_StartGame,
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


    
    //network
    public static event Action<INetworkPlayer> OnConnectedToLocalServer;
    public static event Action<INetworkPlayer> OnClientConnected;
    public static event Action OnStartBtnClickedAction;
    public static event Action OnConnectFailed;

    private bool _isSynced;

    private void Awake()
    {
        UI_Lobby = Utils.FindChild<UI_Lobby>(gameObject, recursive: true).gameObject;
        UI_Lobby.SetActive(false);
    }

    public override bool Init()
    {
        
        Managers.Network = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>();
        
  
       
        Managers.Network.Server.Connected.AddListener(player =>
        {
            
            // Check if the connected client is NOT the local player (host)
            if (!player.IsHost && !_isSynced)
            {
                _isSynced = true;
                Debug.Log("A non-host client connected to the server.");
                StartCoroutine(DelayedClientConnectedCo(player));
            }
            else
            {
                
                Debug.Log("Host client connected, skipping OnClientConnected logic.");
            }
        });

        // Client-side connection listener (no RPC calls here)
       Managers.Network.Client.Connected.AddListener(player =>
        {
            
            Debug.Log("Connected to server.");
            if (!player.IsHost)
            {
              //  GetButton((int)Btns.Btn_StartGame).gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "시작 대기 중";
                Debug.Log("A non-host client connected to the server.");
                StartCoroutine(DelayedToSeverConnectedCo(player)); // Trigger server-side logic for non-host clients
            }
            else
            {
                Debug.Log("Host client connected, skipping OnClientConnected logic.");
            }
           
        });

       Managers.Network.Client.Disconnected.AddListener(_ =>
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
        GetButton((int)Btns.Btn_StartGame).gameObject.BindEvent(OnStartBtnClicked);
        GetButton((int)Btns.Btn_StartGame).gameObject.SetActive(false);
        GetButton((int)Btns.Btn_StartGame).gameObject.GetComponent<Image>().DOFade(0, 0.01f);
        GetButton((int)Btns.Btn_StartGame).gameObject.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.01f);
        _canvas = GetComponent<Canvas>();
        _canvas.sortingOrder = -321; // 항상플레이 화면에만 있을수 있도록

        SetInGameUIs(false);

 
        
        GetButton((int)Btns.Btn_Quit).gameObject.BindEvent(OnQuitBtnClicked);
        
   
        return base.Init();
    }
    private IEnumerator DelayedClientConnectedCo(INetworkPlayer player)
    {
        yield return new WaitForSeconds(2.5f); // 1 second delay, adjust as needed
        OnClientConnected?.Invoke(player);
        // Trigger server-side logic for non-host clients
    }
    
    private IEnumerator DelayedToSeverConnectedCo(INetworkPlayer player)
    {
        yield return new WaitForSeconds(2.5f); // 1 second delay, adjust as needed
        OnConnectedToLocalServer?.Invoke(player); // Trigger server-side logic for non-host clients
    }
    private void OnDestroy()
    {
        
    }

    private void SetInGameUIs(bool value)
    {
        GetButton((int)Btns.Btn_Quit).gameObject.SetActive(value);
        GetButton((int)Btns.Btn_Setting).gameObject.SetActive(value);
        GetButton((int)Btns.Btn_ApplicationQuit).gameObject.SetActive(value);
    }

    public void ShowStartBtn()
    {
        GetButton((int)Btns.Btn_StartGame).gameObject.SetActive(true);
        Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Common/UI_Message_Button", 0.3f);
        
        GetButton((int)Btns.Btn_StartGame).gameObject.GetComponent<Image>().DOFade(1, 0.5f).SetDelay(1.5f);
        GetButton((int)Btns.Btn_StartGame).gameObject.GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.5f).SetDelay(1.5f);

    }

    private void OnQuitBtnClicked()
    {
        Managers.UI.CloseAllPopupUI();
        Managers.Scene.ChangeScene(Define.Scene.linc_main_solo);
    }
    private void OnApplicationQuitClicked()
    {
        
    }

    private void ToggleAnimation()
    {
        _isUiOn = !_isUiOn;   
        _menuAnimator.SetBool(UI_ON,_isUiOn);
    }


    public static bool IsStartBtnClicked { get; private set; } 
    
    private void OnStartBtnClicked()
    {
     
        Debug.Log("Clicked");
        _bg.DOFade(0, 1f);
       
        GetButton((int)Btns.Btn_StartGame).gameObject.GetComponent<Image>().DOFade(0, 0.5f);
        GetButton((int)Btns.Btn_StartGame).gameObject.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.5f).OnComplete(
            () =>
            {
                GetButton((int)Btns.Btn_StartGame).gameObject.SetActive(false);
            });
        SetInGameUIs(true);
        IsStartBtnClicked = true;
        OnStartBtnClickedAction?.Invoke();
    }



}
