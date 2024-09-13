using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Mirage;
using Mirage.SocketLayer;
using Mirage.Sockets.Udp;
using Network;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Managers : MonoBehaviour
{
    public static Managers s_instance;

    public static ushort Port { get; private set; }
    public static Managers Instance => s_instance;

    private static UdpSocketFactory s_udpSocketFactory =new UdpSocketFactory();
    private static SocketFactory s_socketFactory;
    private static DeviceManager s_deviceManager = new();
    private static SceneManagerEx s_sceneManager = new();
    private static SoundManager s_soundManager = new();
    private static DataManager _sDataManager = new();
    private static UIManager s_uiManager = new();
    private static ContentPlayManager s_contentPlayManager = new(); 
    private static ResourceManager s_resourceManager = new ResourceManager();
    private static A_SettingManager s_settingManager = new A_SettingManager();
    private static NetworkManager s_networkManager;
    public static Dictionary<int, NetworkIdentity> NetworkObjNetworkIds;
    public static Dictionary<int, GameObject> NetworkObjs ;
    public static ServerBroadcaster BroadCaster = new();
    public static ClientListener Listener = new();
    private static CursorImageManager s_cursorImageManager= new CursorImageManager();

    public static bool IsSoundSetLoaded;// 중복로드 방지
    public static DataManager Data
    { get { Init(); return _sDataManager; }}
    public static UdpSocketFactory UdpSocketFactory{get{Init();
        return s_udpSocketFactory;
    }}
    
    public static CursorImageManager cursorImageManager
    {  
        get 
        { 
            Init(); 
            return s_cursorImageManager; 
        } 
    }
    
    public static SocketFactory SocketFactory {
        get { Init();
            return s_socketFactory;
        }
    }
    public static NetworkManager Network
    {
        get { return s_networkManager; }
        set { s_networkManager = value; }
    }

    public static ContentPlayManager ContentInfo  { get { Init(); return s_contentPlayManager;}}

    public static DeviceManager DeviceManager
    {
        get { Init();
            return s_deviceManager;
        }
    }

    public static A_SettingManager Setting  { get { Init(); return s_settingManager;}}
  
    public static UIManager UI
    { get { Init(); return s_uiManager;}}
     public static ResourceManager Resource { get { Init(); return s_resourceManager; } }
    
    public static SceneManagerEx Scene
    { get { Init(); return s_sceneManager;}}

    public static SoundManager Sound
    { get { Init(); return s_soundManager; }}

    // public static string GetText(int id)
    // {
    //     if (Managers.Data.Texts.TryGetValue(id, out TextData value) == false)
    //         return null;
    //
    //     return Data.Preference[(int)Define.Preferences.EngMode] == 0 ? value.kor:value.eng;
    // }


    private void Start()
    {
        Init();
    }

    private static void Init()
    {
        if (s_instance == null)
        {
            var go = GameObject.Find("@Managers");
            if (go == null)
                go = new GameObject { name = "@Managers" };

            s_instance = Utils.GetOrAddComponent<Managers>(go);
            //DontDestroyOnLoad(go);
            
            //DataMAnager는 반드시 제일 먼저 초기화 되어야합니다.
            _sDataManager.Init();
            s_resourceManager.Init();
            s_sceneManager.Init();
            s_soundManager.Init();
            s_contentPlayManager.Init();
            s_deviceManager.Init();

            s_cursorImageManager.Init();
            s_soundManager.Init();
            BroadCaster =  Utils.GetOrAddComponent<ServerBroadcaster>(go);
            Listener =  Utils.GetOrAddComponent<ClientListener>(go);

            Port = 7777;
           
            InitialSet();
            NetworkObjNetworkIds = new Dictionary<int,NetworkIdentity>();
            NetworkObjs = new Dictionary<int, GameObject>();
          
        }
    }

    private static void InitialSet()
    {
        
        // UI.SetScreenMode((int)(Managers.Data.Preference[(int)Define.Preferences.Fullscreen]) == 0 ? false : true);
        //
        // var resolution = (int)(Managers.Data.Preference[(int)Define.Preferences.Resolution]);
        // switch (resolution)
        // {
        //     case 1280:  UI.SetResolution(1280,720,UI.isFullScreen);
        //         break;
        //     case 1920: UI.SetResolution(1920,1080,UI.isFullScreen);
        //         break;
        //     case 2560: UI.SetResolution(2560,1440,UI.isFullScreen);
        //         break;
        // }
       
        Application.targetFrameRate = 60;
    }

    public static NetworkServer NetworkServer { get; set; }

    
    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork) // IPv4 주소만 사용
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
    
    public static void RestartSceneWithRemoveDontDestroy()
    {
        
        // 1. 모든 루트 객체 가져오기
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.name == null)  // 
            {
                Logger.Log($"destroy{obj.name}");
                Destroy(obj);
            }
            Destroy(obj);
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}