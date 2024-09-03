using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LanguageChangeConfirmation_Restart : UI_Popup
{
    private enum Btns
    {
        Btn_Yes,
        Btn_Close,
        Btn_No
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        // BindObject(typeof(GameObj));
        BindButton(typeof(Btns));

        GetButton((int)Btns.Btn_Yes).gameObject.BindEvent(() =>
        {
          
            Managers.UI.FindPopup<UI_Setting>().SetLanguageWithConfirmation();
            Managers.Data.SaveCurrentSetting();
            
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        },Define.UIEvent.PointerUp);
        GetButton((int)Btns.Btn_Close).gameObject.BindEvent(() =>
        {
            
#if UNITY_EDITOR
            Debug.Log($" CLose Button-------------------");
#endif
            Managers.UI.FindPopup<UI_Setting>().InitialSetting();
            Managers.UI.ClosePopupUI(this);
        },Define.UIEvent.PointerUp);
        GetButton((int)Btns.Btn_No).gameObject.BindEvent(() =>
        {
#if UNITY_EDITOR
            Debug.Log($" NO Button------------------");
#endif
            Managers.UI.FindPopup<UI_Setting>().InitialSetting();
            Managers.UI.ClosePopupUI(this);
        },Define.UIEvent.PointerUp);

        return true;
    }
}