using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using ArduinoBluetoothAPI;
using System;
using UnityEngine.Serialization;

public class DeviceManager : MonoBehaviour
{
    public Text btnText;
    public Text btnText2;
    public Image btnImage;
    public Image btnImage2;

   public Stick_DataController StickData;


    public bool IsConnected { get;  private set; }
    private bool _isVibrateMode;

    public bool IsVibrateMode
    {
        get
        {
            return _isVibrateMode;
        }
        set
        {
            _isVibrateMode = value;
        }
    }



    public void Init()
    {
        StickData = Stick_DataController.Instance;
        Stick_DataController.OnConnectedEvent += OnConnected;
        Stick_DataController.OnConnectionFailedEvent += OnConnFailed;
    }
    void Start()
    {
        StickData = Stick_DataController.Instance;
        Stick_DataController.OnConnectedEvent += OnConnected;
        Stick_DataController.OnConnectionFailedEvent += OnConnFailed;
        //
        // btnText.text = "���� �� ��";
        // btnText2.text = "���� �� ��";
    }


    void OnConnected()
    {
        IsConnected = true;
        return ;
        // Color color;
        // ColorUtility.TryParseHtmlString("#2AD994", out color);
        // //btnImage.GetComponent<Renderer>().material.color = color;
        // btnImage.color = color;
        // btnText.text = "�����";
        // btnImage2.color = color;
        // btnText2.text = "�����";
    }
    void OnConnFailed()
    {
        IsConnected = false;
        return ;
        // Color color;
        // ColorUtility.TryParseHtmlString("#C2C7DE", out color);
        // btnImage.color = color;
        // btnImage2.color = color;
        // //btnImage.GetComponent<Renderer>().material.color = color;
        // btnText.text = "���� �� ��";
        // btnText2.text = "���� �� ��";
        // Debug.Log("DEBUG: Connection Failed");
    }

    public bool OnBtnClick()
    {
        Debug.Log("DEBUG:BTN Click");

        if (!StickData.isConnected())
        {
            //      Color color;
            //   ColorUtility.TryParseHtmlString("#2AD994", out color);
            //      Debug.Log("DEBUG:Try To Connection");
            if (StickData.isDevicePaired())
            {
                StickData.TryConnect();
                Debug.Log("DEBUG: Paired");
                return true;
            }

            Debug.Log("DEBUG: NOT Paired");
            return false;
            // btnImage.color = color;
            // btnImage2.color = color;
        }

        StickData.Disconnect();
        return false;

        //  Color color;
        //    ColorUtility.TryParseHtmlString("#C2C7DE", out color);
        // btnText.text = "���� �� ��";
        // btnText2.text = "���� �� ��";

        // btnImage.color = color;
        // btnImage2.color = color;
    }
    
    public Text RD;

    private readonly int ON = 0x51;
    private readonly int OFF = 0x52;

    void Update()
    {
        RD.text = StickData.received_message;
    }

    public void On_Data_Only()
    {
        Logger.Log("Haptic Stick On: On_Data Only ");

        byte[] bytestosend = { 0x55, 0x51, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x54 };
    }
    public void BT_Session()
    {
        Debug.Log("BT Session");

        byte[] bytestosend = { 0x55, 0x51, 0x00, 0xC8, 0x64, 0x01, 0xF5, 0x64, 0x54 };

        StickData.SendData(bytestosend);

    }

    public void SendDataAndVibrate()
    {
        byte[] bytestosend = { 0x55, 0x51, 0x00, 0xC8, 0x64, 0x01, 0xF5, 0x64, 0x54 };

        StickData.SendData(bytestosend);
    }

    public void HapticStick_Off()
    {
        byte[] bytestosend = { 0x55, 0x52, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x54 };

        StickData.SendData(bytestosend);
    }


    public void SetHapticStick(bool isOn =true, bool isVibrate=false)
    {
        if (isOn)
        {
            if (isVibrate)
            {
                byte[] bytestosend = { 0x55, 0x51, 0x00, 0xC8, 0x64, 0x01, 0xF5, 0x64, 0x54 };
               
                StickData.SendData(bytestosend);return;
            }
            else
            {
                byte[] bytestosend  = { 0x55, 0x52, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x54 };
                StickData.SendData(bytestosend);return;
            }
        }
        
        if(!isOn)
        {
            byte[] bytestosend  = { 0x55, 0x52, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x54 };
            StickData.SendData(bytestosend);return;
        }
        
    }


}
