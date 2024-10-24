using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using ArduinoBluetoothAPI;
using System;
using System.Net.WebSockets;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class Stick_DataController : MonoBehaviour
{
    public static event Action OnConnectedEvent;
    public static event Action OnConnectionFailedEvent;
    private static Stick_DataController instance;
    public static Stick_DataController Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("BT_Comm");
                instance = obj.AddComponent<Stick_DataController>();
            }
            return instance;
        }
    }
    public BluetoothHelper bluetoothHelper;
    private string deviceName;
    public string received_message;



    private static Quaternion s_stickA_Quaternion;
    public Quaternion StickA_Quaternion
    {
        get
        {
            return s_stickA_Quaternion;
        }
        private set => s_stickA_Quaternion = value;
    }

    public Vector3 acc_Stick1;


    private float cx1 = 0f;
    private float cy1 = 0f;
    private float cz1 = 0f;
    private float cw1 = 0f;

    public Quaternion quat_Stick2;
    public Vector3 acc_Stick2;

    private float cx2 = 0f;
    private float cy2 = 0f;
    private float cz2 = 0f;
    private float cw2 = 0f;
  

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeOnStart()
    {
       
        var instance = Stick_DataController.Instance;
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        
        received_message = "";
        deviceName = "HapticStick_V3";  
        
        try
        {
            bluetoothHelper = BluetoothHelper.GetInstance(deviceName);
            bluetoothHelper.OnConnected += OnConnected;
            bluetoothHelper.setTerminatorBasedStream("\n");
            bluetoothHelper.OnDataReceived += OnMessageReceived; //read the data
            bluetoothHelper.OnConnectionFailed += OnConnFailed;

            LinkedList<BluetoothDevice> ds = bluetoothHelper.getPairedDevicesList();

            foreach (BluetoothDevice d in ds)
            {
                Debug.Log("BT_DEBUG: " + $"{d.DeviceName} {d.DeviceAddress}");
            }

        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

   
    public void ParseAndConvertToQuaternion()
    {
        string[] values;
        string[] parts = received_message.Split('*');
        if (parts.Length > 1)
        {
             values = parts[1].Split(',');
        }
        else
        {
            Logger.Log("recivedMeesage is non-Validate");
            return;
        }
        Logger.Log($"recieved Data: {parts[1]}");

        if (values.Length < 7)
        {
            Logger.Log("recivedMeesage is non-Validate (missed some data)");
            return;
        }
        
        if (!float.TryParse(values[0], out float z1) ||
            !float.TryParse(values[1], out float x1) ||
            !float.TryParse(values[2], out float y1) ||
            !float.TryParse(values[3], out float w1) ||
            !float.TryParse(values[4], out float gx1) ||
            !float.TryParse(values[5], out float gy1) ||
            !float.TryParse(values[6], out float gz1))
        {
            Debug.Log("Failed to parse one or more float values");
            return;
        }


        if (((z1 < 1f) && (z1 > -1f) && (z1 != 0f)) &&
         ((x1 < 1f) && (x1 > -1f) && (x1 != 0f)) &&
         ((y1 < 1f) && (y1 > -1f) && (y1 != 0f)) &&
         ((w1 < 1f) && (w1 > -1f) && (w1 != 0f)))
        {

            cz1 = z1;
            cy1 = y1;
            cx1 = -x1;
            cw1 = w1;
        }

        
        acc_Stick1 = new Vector3(gx1, gy1, gz1);
//        Logger.Log($"sending data from deviceManager: {new Quaternion(-cx1, cz1, cy1, cw1)}");
        
         StickA_Quaternion = new Quaternion(-cx1, cz1, cy1, cw1);
    }


    void OnConnFailed(BluetoothHelper helper)
    {
        OnConnectionFailedEvent?.Invoke();
    }
    public void SendData(byte[] data)
    {
        bluetoothHelper.SendData(data);
    }
    public bool isConnected()
    {
        return bluetoothHelper.isConnected();
    }
    public bool isDevicePaired()
    {
        return bluetoothHelper.isDevicePaired();
    }
    void OnConnected(BluetoothHelper helper)
    {
        try
        {
            helper.StartListening();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        OnConnectedEvent?.Invoke();

    }
    public void TryConnect()
    {
        if (!bluetoothHelper.isConnected())
        {
            if (bluetoothHelper.isDevicePaired())
                bluetoothHelper.Connect(); // tries to connect                                      
        }
        else
        {
            bluetoothHelper.Disconnect();
        }
    }
    public void Disconnect()
    {
        bluetoothHelper.Disconnect();
    }
    //Asynchronous method to receive messages
    void OnMessageReceived(BluetoothHelper helper)
    {
        received_message = helper.Read();
        Managers.DeviceManager.IsQuatInfoReady = true;
        ParseAndConvertToQuaternion();
//        Debug.Log(received_message);
    }
    void OnDestroy()
    {
        if (bluetoothHelper != null)
            bluetoothHelper.Disconnect();
    }

}