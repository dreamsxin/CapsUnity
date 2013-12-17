﻿using UnityEngine;
using System.Collections;
using System;

public class UIBuyHeart : UIWindow 
{
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("ConfirmBtn", OnConfirmClicked);
        AddChildComponentMouseClick("CancelBtn", OnCancelClicked);
    }
    public override void OnShow()
    {
        base.OnShow();
    }

    private void OnConfirmClicked()
    {
		HideWindow();
		--GlobalVars.Coins;
		PlayerPrefs.SetInt("Coins", GlobalVars.Coins);
		GlobalVars.HeartCount = 5;
        if (CapsApplication.Singleton.CurStateEnum == StateEnum.Game)
        {
            UIWindowManager.Singleton.GetUIWindow<UIRetry>().ShowWindow();
        }
    }

    private void OnCancelClicked()
    {
        HideWindow();
        if (CapsApplication.Singleton.CurStateEnum == StateEnum.Game)
        {
            UIWindowManager.Singleton.GetUIWindow<UIRetry>().ShowWindow();
        }
    }
}
