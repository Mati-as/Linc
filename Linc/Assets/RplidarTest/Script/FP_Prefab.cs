using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class FP_Prefab : RaySynchronizer
{
    
    private readonly string GAME_MANAGER = "GameManager";
    private Image _image;

    public FP_controller FPC;
    private float Timer = 0f;
    //public static float Limit_Time { get; set; }

    private RectTransform FP;
    private GameObject Image;
    private static bool _isImageOn;
    
    public static event Action onPrefabInput; 
    

    public override void Init()
    {
         
        GameObject.FindWithTag("UICamera").TryGetComponent(out _uiCamera);
        
         _rectTransform = GetComponent<RectTransform>();
         _image = GetComponent<Image>();
    }




    public override void OnEnable()
    {
        base.OnEnable();
        
        if(_image==null) _image = GetComponent<Image>();
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        
        //모드설정에따라 이미지 활성화 비활성화
        Debug.Assert(_image != null);

        _image.enabled = true;
        
        FP = this.GetComponent<RectTransform>();
        FPC = Manager_Sensor.instance.Get_RPC();
        //Image = this.transform.GetChild(0).gameObject;
        Image = gameObject;
        //Debug.Log(FP.anchoredPosition.x + "," + FP.anchoredPosition.y);
        if (FPC.Check_FPposition(FP))
        {
            Image.SetActive(true);
            base.Start();
            base.InvokeRayEvent();
        }

    }

    private void OnDestroy()
    {
        
        Destroy(this.gameObject);
    }

    private Button _btn;
    private RectTransform _rectTransform;
    public override void ShootRay()
    {
       
        base.ShootRay();


        
    }
    

}