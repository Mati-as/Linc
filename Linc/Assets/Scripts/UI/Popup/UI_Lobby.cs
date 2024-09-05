using System.Collections;
using DG.Tweening;
using Mirage;
using TMPro;
using UnityEngine;

public class UI_Lobby : UI_Popup
{
    public enum Btns
    {
        Btn_StartGame,
        Btn_Back,
        Btn_StartHost,
        Btn_StartClient,
        Btn_QuitConnection,
    }

    public enum UIs
    {
        MyPlayer,
        Opponent,
        TryingConnection
    }

    private bool _isGamePlayable;
    private TextMeshProUGUI _tmp;
    private WaitForSeconds _wait;
    private readonly float _waitAmount = 0.8f;
    private Sequence _onConnectTMPSeq;
    private NetworkManager _networkManager;

    public override bool Init()
    {
        if (base.Init() == false) return false;

        _networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>();

       
        _networkManager.Server.Connected.AddListener(player =>
        {
            // Check if the connected client is NOT the local player (host)
            if (!player.IsHost)
            {
                Debug.Log("A non-host client connected to the server.");
                OnClientConnected(); // Trigger server-side logic for non-host clients
            }
            else
            {
                Debug.Log("Host client connected, skipping OnClientConnected logic.");
            }
        });

        // Client-side connection listener (no RPC calls here)
        _networkManager.Client.Connected.AddListener(player =>
        {
            Debug.Log("Connected to server.");
            if (!player.IsHost)
            {
                Debug.Log("A non-host client connected to the server.");
                OnConnectedToServer(); // Trigger server-side logic for non-host clients
            }
            else
            {
                Debug.Log("Host client connected, skipping OnClientConnected logic.");
            }
           
        });

        _networkManager.Client.Disconnected.AddListener(_ =>
        {
            Debug.Log("Disconnected from server.");
            OnConnectFailed();
        });

        BindObject(typeof(UIs));
        BindButton(typeof(Btns));

        GetObject((int)UIs.Opponent).SetActive(false);
        _tmp = GetObject((int)UIs.TryingConnection).GetComponent<TextMeshProUGUI>();
        GetObject((int)UIs.TryingConnection).SetActive(false);

        GetButton((int)Btns.Btn_StartHost).gameObject.BindEvent(OnHostBtnClicked);
        GetButton((int)Btns.Btn_StartClient).gameObject.BindEvent(OnClientBtnClicked);
        GetButton((int)Btns.Btn_Back).gameObject.BindEvent(() =>
        {
            Managers.UI.CloseAllPopupUI();
            Managers.Scene.ChangeScene(Define.Scene.linc_main_solo);
        });
        GetButton((int)Btns.Btn_StartGame).gameObject.BindEvent(() =>
        {
            Managers.UI.CloseAllPopupUI();
            Managers.Scene.ChangeScene(Define.Scene.linc_multimode);
        });
        GetButton((int)Btns.Btn_StartGame).gameObject.SetActive(false);
      
        GetButton((int)Btns.Btn_QuitConnection).gameObject.BindEvent(OnQuitBtnClicked);
        GetButton((int)Btns.Btn_QuitConnection).gameObject.SetActive(false);
        return true;
    }

    private void OnClientBtnClicked()
    {  
        _onConnectTMPSeq = DOTween.Sequence();
        _onConnectTMPSeq.AppendCallback(() => { _tmp.text = "방 참여 시도 중..."; });
        _onConnectTMPSeq.AppendInterval(0.7f);
        _onConnectTMPSeq.AppendCallback(() => { _tmp.text += "."; });
        _onConnectTMPSeq.AppendInterval(0.7f);
        _onConnectTMPSeq.AppendCallback(() => { _tmp.text += "."; });
        _onConnectTMPSeq.SetLoops(5, LoopType.Restart);
        
        GetButton((int)Btns.Btn_StartHost).gameObject.SetActive(false);
        GetButton((int)Btns.Btn_StartClient).gameObject.SetActive(false);
        GetButton((int)Btns.Btn_QuitConnection).gameObject.SetActive(true);
        GetObject((int)UIs.TryingConnection).SetActive(true);
    }

   
    private void OnConnectedToServer()
    {
        Debug.Log("OnConnectedToServer called on the client.");
        _onConnectTMPSeq?.Kill();
        StartCoroutine(OnConnectedToServerFromClientCo());
    }

    private IEnumerator OnConnectedToServerFromClientCo()
    {
        if (_wait == null) _wait = new WaitForSeconds(_waitAmount);
        _tmp.text = "방 연결완료!";

        yield return _wait;
        GetButton((int)Btns.Btn_StartGame).gameObject.SetActive(true);
        GetObject((int)UIs.TryingConnection).SetActive(false);
        GetObject((int)UIs.Opponent).SetActive(true);
        _isGamePlayable = true;
    }

   
    private void OnConnectFailed()
    {
        Debug.Log("Connection failed.");
        _tmp.text = "방 찾기를 실패했어요. 다시 방을 만들거나 방 참여를 시도 해주세요.";
        GetObject((int)UIs.Opponent).SetActive(false);
    }

    private void OnHostBtnClicked()
    {
        _onConnectTMPSeq = DOTween.Sequence();
        _onConnectTMPSeq.AppendCallback(() => { _tmp.text = "방 만들기 완료\n다른 플레이어 입장 대기 중.."; });
        _onConnectTMPSeq.AppendInterval(0.7f);
        _onConnectTMPSeq.AppendCallback(() => { _tmp.text += "."; });
        _onConnectTMPSeq.AppendInterval(0.7f);
        _onConnectTMPSeq.AppendCallback(() => { _tmp.text += "."; });
        _onConnectTMPSeq.SetLoops(5, LoopType.Restart);
        
         
        GetButton((int)Btns.Btn_StartHost).gameObject.SetActive(false);
        GetButton((int)Btns.Btn_StartClient).gameObject.SetActive(false);
        GetButton((int)Btns.Btn_QuitConnection).gameObject.SetActive(true);
        GetObject((int)UIs.TryingConnection).SetActive(true);
    }

   
    
    private void OnClientConnected()
    {
        Debug.Log("OnClientConnected called on the server.");
        OnConnectedToServer(); // Call ClientRpc from server
        _onConnectTMPSeq?.Kill();
        StartCoroutine(OnClientConnectedCo());
    }

    private IEnumerator OnClientConnectedCo()
    {
        if (_wait == null) _wait = new WaitForSeconds(_waitAmount);
        _tmp.text = "방 연결완료!";
        GetButton((int)Btns.Btn_QuitConnection).gameObject.SetActive(false);
        yield return _wait;
        GetButton((int)Btns.Btn_StartGame).gameObject.SetActive(true);
        GetObject((int)UIs.TryingConnection).SetActive(false);
        GetObject((int)UIs.Opponent).SetActive(true);
        _isGamePlayable = true;
    }
    
    
    private void OnQuitBtnClicked()
    {
        _onConnectTMPSeq?.Kill();
        StartCoroutine(OnQuitBtnClickedCo());
    }

    private IEnumerator OnQuitBtnClickedCo()
    {
        GetButton((int)Btns.Btn_StartHost).gameObject.SetActive(true);
        GetButton((int)Btns.Btn_StartClient).gameObject.SetActive(true);
        GetButton((int)Btns.Btn_QuitConnection).gameObject.SetActive(false);
      
        GetButton((int)Btns.Btn_StartGame).gameObject.SetActive(false);
        GetObject((int)UIs.TryingConnection).SetActive(false);
        GetObject((int)UIs.Opponent).SetActive(false);
        _isGamePlayable = false;
        yield break;
    }
}
