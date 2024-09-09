using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using ArduinoBluetoothAPI;
using System;
using UnityEngine.UI;


public class Trigger_carrot : MonoBehaviour
{
    //BT ��ſ�
    private Stick_DataController bluetoothHelper;
    public Text RD;
    public Text x1;
    public Text y1;
    public Text z1;
    public Text x2;
    public Text y2;
    public Text z2;
    void Start()
    {
        bluetoothHelper = Stick_DataController.Instance;
    }

    void Update()
    {
        RD.text = bluetoothHelper.received_message;
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

        bluetoothHelper.SendData(bytestosend);

    }

    public void T_ON()
    {
        byte[] bytestosend = { 0x55, 0x51, 0x00, 0xC8, 0x64, 0x01, 0xF5, 0x64, 0x54 };

        bluetoothHelper.SendData(bytestosend);
    }

    public void HapticStick_Off()
    {
        byte[] bytestosend = { 0x55, 0x52, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x54 };

        bluetoothHelper.SendData(bytestosend);
    }
    
 


}


