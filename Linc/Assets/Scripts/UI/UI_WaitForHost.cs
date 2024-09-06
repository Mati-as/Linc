using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI_WaitForHost : UI_Popup
{
    public override bool Init()
    {
        if (base.Init() == false) return false;


        if (Managers.Network.Server.IsHost)
        {
            gameObject.SetActive(false);
        }
        
        return true;
    }
}
