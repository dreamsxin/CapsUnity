﻿using UnityEngine;
using System.Collections;

public enum TLoginFlow
{
    LoginFlow_LoginScreen,
    LoginFlow_Map,
}

public class LoginState : State
{
    public static LoginState Instance;
    public TLoginFlow CurFlow { get; set; }

    public override void DoDeInitState()
    {
        base.DoDeInitState();
        UIWindowManager.Singleton.GetUIWindow<UIMap>().HideWindow();
        UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().HideWindow();
    }

    public override void DoInitState()
    {
		base.DoInitState();
		
		Instance = this;

        if (UIWindowManager.Singleton.GetUIWindow<UIMap>() == null)
        {
            UIWindowManager.Singleton.CreateWindow<UIMainMenu>(UIWindowManager.Anchor.BottomLeft);
            UIWindowManager.Singleton.CreateWindow<UIQuitConfirm>();
            UIWindowManager.Singleton.CreateWindow<UIStageInfo>();
            UIWindowManager.Singleton.CreateWindow<UIMap>();
            UIWindowManager.Singleton.CreateWindow<UINoMoreHearts>();
            UIWindowManager.Singleton.CreateWindow<UIHowToPlay>();
            UIWindowManager.Singleton.CreateWindow<UIWindow>("UILoading", UIWindowManager.Anchor.Center);
            UIWindowManager.Singleton.CreateWindow<UIDialog>(UIWindowManager.Anchor.Bottom);
            UIWindowManager.Singleton.CreateWindow<UIStore>();
            //UIWindowManager.Singleton.CreateWindow<UIWait>();
            UIWindowManager.Singleton.CreateWindow<UIPurchaseNotEnoughMoney>();
            UIWindowManager.Singleton.CreateWindow<UIMessageBox>();
            UIWindowManager.Singleton.GetUIWindow<UIMap>().RefreshButtons();
        }

        if (GlobalVars.UseMusic)
        {
            UIToolkits.PlayMusic(CapsConfig.CurAudioList.MapMusic);
        }

        if (UIWindowManager.Singleton.GetUIWindow<UIMap>().Visible)
        {
            CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
        }
    }

    public override void OnBackKey()
    {
        base.OnBackKey();
        if (UIWindowManager.Singleton.GetUIWindow<UIHowToPlay>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIHowToPlay>().OnClose();
            return;
        }

        if (UIWindowManager.Singleton.GetUIWindow("UIMainMenuExtend").Visible)      //若主菜单开启状态
        {
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().HideExtendMainMenu();       //显示主菜单按钮
            return;
        }

        if (CurFlow == TLoginFlow.LoginFlow_Map)
        {
            if (GlobalVars.InMapFTUE)
            {
                return;
            }

            //if (UIWindowManager.Singleton.GetUIWindow<UIWait>().Visible)
            //{
            //    return;
            //}

            if (UIWindowManager.Singleton.GetUIWindow<UIStore>().Visible)
            {
                UIWindowManager.Singleton.GetUIWindow<UIStore>().OnCloseBtn();
                return;
            }

            if (UIWindowManager.Singleton.GetUIWindow<UIPurchaseNotEnoughMoney>().Visible)
            {
                UIWindowManager.Singleton.GetUIWindow<UIPurchaseNotEnoughMoney>().OnCloseBtn();
                return;
            }

			if (UIWindowManager.Singleton.GetUIWindow<UINoMoreHearts>().Visible)
	        {
	            UIWindowManager.Singleton.GetUIWindow<UINoMoreHearts>().Close();
	            return;
	        }
			
			if(UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().Visible)
			{
				UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().HideWindow();
				UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
				return;
			}

            if (UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().Visible)
            {
                return;
            }

            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMap>().HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UILogin>().ShowWindow();
			CurFlow = TLoginFlow.LoginFlow_LoginScreen;
        }
        else
        {
            if (UIWindowManager.Singleton.GetUIWindow<UIQuitConfirm>().Visible)
            {
                UIWindowManager.Singleton.GetUIWindow<UIQuitConfirm>().HideWindow();
                return;
            }
            UIWindowManager.Singleton.GetUIWindow<UIQuitConfirm>().ShowWindow();
        }
    }
}
