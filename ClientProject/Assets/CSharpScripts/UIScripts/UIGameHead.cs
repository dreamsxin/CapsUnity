﻿using UnityEngine;
using System.Collections;

public class UIGameHead : UIWindow
{
    UILabel [] m_itemUILabel = new UILabel [2];

    public override void OnCreate()
    {
        base.OnCreate();

        AddChildComponentMouseClick("EditorBtn", OnEditStageClicked);
		
		AddChildComponentMouseClick("UseItem1Btn", delegate()
		{
			UserOrBuyItem(PurchasedItem.Item_Hammer);
		});
		AddChildComponentMouseClick("UseItem2Btn", delegate()
		{
			UserOrBuyItem(PurchasedItem.Item_PlusStep);
		});

        for (int i = 0; i < 2; ++i )
        {
            m_itemUILabel[i] = GetChildComponent<UILabel>("ItemCount" + (i + 1));
        }
    }
    public override void OnShow()
    {
        base.OnShow();
        RefreshItemCount();
    }
	
	public void Reset()
	{

	}
	
    public override void OnUpdate()
    {
        base.OnUpdate();
        UIDrawer.Singleton.DefaultAnchor = UIWindowManager.Anchor.Top;

        if (GlobalVars.CurStageData.Target == GameTarget.BringFruitDown)
        {
            if (GlobalVars.CurStageData.Nut1Count > 0)
            {
                UIDrawer.Singleton.DrawSprite("Fruit1Icon", -40, 46, "Kiwifruit_Icon");
                UIDrawer.Singleton.DrawNumber("Fruit1Count", 0, 46, GlobalVars.CurGameLogic.PlayingStageData.Nut1Count, "", 24, 1);
                UIDrawer.Singleton.DrawSprite("Fruit1CountSplash", 31, 46, "backslash");
                UIDrawer.Singleton.DrawNumber("Fruit1Total", 60, 46, GlobalVars.CurStageData.Nut1Count, "", 24, 1);
            }
            
            if (GlobalVars.CurStageData.Nut2Count > 0)
            {
                UIDrawer.Singleton.DrawSprite("Fruit2Icon", 140, 46, "Cherry_Icon");
                UIDrawer.Singleton.DrawNumber("Fruit2Count", 180, 46, GlobalVars.CurGameLogic.PlayingStageData.Nut2Count, "", 24, 1);
                UIDrawer.Singleton.DrawSprite("Fruit2CountSplash", 214, 46, "backslash");
                UIDrawer.Singleton.DrawNumber("Fruit2Total", 240, 46, GlobalVars.CurStageData.Nut2Count, "", 24, 1);
            }
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.ClearJelly)
        {
            UIDrawer.Singleton.DrawSprite("JellyCountIcon", -40, 46, "IceBlock_Icon");
            UIDrawer.Singleton.DrawNumber("Jelly1", -2, 46, GlobalVars.CurGameLogic.PlayingStageData.GetSingleJellyCount(), "", 22, 2);
            UIDrawer.Singleton.DrawSprite("JellyCountSplash", 40, 46, "backslash");
            UIDrawer.Singleton.DrawNumber("Jelly2", 65, 46, GlobalVars.CurStageData.GetSingleJellyCount(), "", 22, 2);
            
            if (GlobalVars.CurStageData.GetDoubleJellyCount() > 0)
            {
                UIDrawer.Singleton.DrawSprite("DoubleJellyCountIcon", 140, 46, "DoubleIceBlock_Icon");
                UIDrawer.Singleton.DrawNumber("DoubleJelly1", 170, 46, GlobalVars.CurGameLogic.PlayingStageData.GetDoubleJellyCount(), "", 23, 2);
                UIDrawer.Singleton.DrawSprite("DoubleJellyCountSplash", 220, 46, "backslash");
                UIDrawer.Singleton.DrawNumber("DoubleJelly2", 240, 46, GlobalVars.CurStageData.GetDoubleJellyCount(), "", 23, 2);
            }
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.GetScore)
        {
            UIDrawer.Singleton.DrawSprite("TargetText", 10, 50, "TargetTextImg");
            UIDrawer.Singleton.DrawNumber("TargetScore", 104, 50, GlobalVars.CurStageData.StarScore[0], "", 24, 7);
        }
    }
	
	void UserOrBuyItem(PurchasedItem item)
	{
        if (GlobalVars.CurGameLogic.GetGameFlow() != TGameFlow.EGameState_Playing)
        {
            return;
        }

        if (GlobalVars.PurchasedItemArray[(int)item] == 0)
        {
            UIPurchase purchaseWindow = UIWindowManager.Singleton.GetUIWindow<UIPurchase>();
            purchaseWindow.ShowWindow();
            purchaseWindow.OnPurchase = delegate()
            {
                if (item == PurchasedItem.Item_PlusStep)
                {
                    --GlobalVars.Coins;
                    GA.API.Business.NewEvent("BuyStep", "RMB", 1);
                    PlayerPrefs.SetInt("Coins", GlobalVars.Coins);
                    GlobalVars.CurGameLogic.PlayingStageData.StepLimit += 5;        //步数加5
                }
                else if (item == PurchasedItem.Item_Hammer)
                {
                    GlobalVars.CurGameLogic.UsingItem = item;                       //进入使用道具状态，等着选目标
                }
            };
            return;
        }

        UIUseItem useItemWindow = UIWindowManager.Singleton.GetUIWindow<UIUseItem>();
        useItemWindow.ShowWindow();
        useItemWindow.OnUse = delegate()
        {
            if (item == PurchasedItem.Item_Hammer)
            {
                GlobalVars.CurGameLogic.UsingItem = item;
            }
            else if (item == PurchasedItem.Item_PlusStep)
            {
                GlobalVars.CurGameLogic.PlayingStageData.StepLimit += 5;        //步数加5
                --GlobalVars.PurchasedItemArray[(int)item];                     //减少道具数量
                RefreshItemCount();
                PlayerPrefsExtend.SetIntArray("PurchasedItemArray", GlobalVars.PurchasedItemArray);
            }
        };
	}

    public void RefreshItemCount()
    {
        for (int i = 0; i < 2; ++i)
        {
            if (GlobalVars.PurchasedItemArray[i] == 0)
            {
                m_itemUILabel[i].gameObject.SetActive(false);
            }
            else
            {
                m_itemUILabel[i].gameObject.SetActive(true);
                m_itemUILabel[i].text = GlobalVars.PurchasedItemArray[i].ToString();
            }
        }
    }

    private void OnEditStageClicked()
    {
        if (UIWindowManager.Singleton.GetUIWindow<UIStageEditor>() == null)
        {
            UIWindowManager.Singleton.CreateWindow<UIStageEditor>(UIWindowManager.Anchor.Right);
        }
        UIWindowManager.Singleton.GetUIWindow<UIStageEditor>().ShowWindow();        //显示编辑窗口
        GlobalVars.EditStageMode = true;        //编辑器里自动进入关卡编辑模式
    }
}
