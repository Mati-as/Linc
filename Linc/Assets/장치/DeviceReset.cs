using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using ArduinoBluetoothAPI;
using System;

public class DeviceReset : MonoBehaviour
{

    //BT 통신용
    private Stick_DataController bluetoothHelper;

    public Image btnImage;

    void Start()
    {
        bluetoothHelper = Stick_DataController.Instance;
    }

    public void Onbtnclick()
    {
        Color color;
        ColorUtility.TryParseHtmlString("#2AD994", out color);
        btnImage.color = color;

        char temp_mode_stx = '<';
        char temp_mode_etx = '>';
        byte[] af = BitConverter.GetBytes(temp_mode_stx);
        byte[] a = BitConverter.GetBytes('D');
        byte[] al = BitConverter.GetBytes(temp_mode_etx);
        byte[] b = BitConverter.GetBytes(0x00);
        byte[] c = BitConverter.GetBytes(0x00);

        byte[] bytestosend = { af[0], a[0], al[0], 0xFF, b[0], c[0], 0xFE };

        bluetoothHelper.SendData(bytestosend);
    }
}
