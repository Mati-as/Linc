public class UI_Help : UI_Popup
{
    private enum Btns
    {
        Prev,
        Next,
        Btn_Close
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        // BindObject(typeof(GameObj));
        BindButton(typeof(Btns));


        GetButton((int)Btns.Btn_Close).gameObject.BindEvent(() => { Managers.UI.ClosePopupUI(this); });

        return true;
    }
}