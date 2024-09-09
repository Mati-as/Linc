using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using ArduinoBluetoothAPI;
using System;


public class trigger_off : MonoBehaviour
{
    private BluetoothHelper bluetoothHelper;
    private string deviceName;

    void Start()
    {
        //Bluetooth ���� permssion ȹ�� ��û (�ʼ�)
#if UNITY_2020_2_OR_NEWER
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation)
          || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)
          || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN")
          || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADVERTISE")
          || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
            Permission.RequestUserPermissions(new string[] {
                Permission.CoarseLocation,
                Permission.FineLocation,
                "android.permission.BLUETOOTH_SCAN",
                "android.permission.BLUETOOTH_ADVERTISE",
                "android.permission.BLUETOOTH_CONNECT"
              });
#endif
#endif

        //deviceName = "HapticPad";   //������ ��ġ�� �̸�
        deviceName = "PAD_V2";   //������ ��ġ�� �̸�
        try
        {
            bluetoothHelper = BluetoothHelper.GetInstance(deviceName);

            LinkedList<BluetoothDevice> ds = bluetoothHelper.getPairedDevicesList();

            foreach (BluetoothDevice d in ds)
            {
                Debug.Log($"{d.DeviceName} {d.DeviceAddress}");
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

    }


    //public int hit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Stick")) //������
        {
            //Trigger_Kiwro obj = GameObject.FindGameObjectWithTag("Kiwro").GetComponent<Trigger_Kiwro>();
            Off_magnetic();
        }

        /*if (other.gameObject.CompareTag("Stick2")) //����
        {
            //Trigger_Kiwro obj = GameObject.FindGameObjectWithTag("Kiwro").GetComponent<Trigger_Kiwro>();
            Tactile2();
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Stick"))
        {
            Off_magnetic();
        }
    }

    public void Tactile()//������
    {
        char temp_mode_stx = '<';
        char temp_mode_etx = '>';
        byte[] af = BitConverter.GetBytes(temp_mode_stx);
        byte[] a = BitConverter.GetBytes('5');
        byte[] al = BitConverter.GetBytes(temp_mode_etx);
        byte[] b = BitConverter.GetBytes(53);
        byte[] c = BitConverter.GetBytes('0');
    }

    public void On_magnetic()
    {
        char temp_mode_stx = '<';
        char temp_mode_etx = '>';
        byte[] af = BitConverter.GetBytes(temp_mode_stx);
        byte[] a = BitConverter.GetBytes('5');
        byte[] al = BitConverter.GetBytes(temp_mode_etx);
        //byte[] b = BitConverter.GetBytes(53);
        byte[] c = BitConverter.GetBytes('0');


        byte[] bytestosend = { af[0], a[0], al[0], 0xFF, 0x53, c[0], 0xFE };
        bluetoothHelper.SendData(bytestosend);
    }

    public void Off_magnetic()
    {
        char temp_mode_stx = '<';
        char temp_mode_etx = '>';
        byte[] af = BitConverter.GetBytes(temp_mode_stx);
        byte[] a = BitConverter.GetBytes('B');
        byte[] al = BitConverter.GetBytes(temp_mode_etx);
        //byte[] b = BitConverter.GetBytes(54);
        byte[] c = BitConverter.GetBytes('0');


        byte[] bytestosend = { af[0], a[0], al[0], 0xFF, 0x54, c[0], 0xFE };
        bluetoothHelper.SendData(bytestosend);
    }
}


