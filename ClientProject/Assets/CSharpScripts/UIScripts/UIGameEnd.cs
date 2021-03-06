using UnityEngine;
using System.Collections;

public class UIGameEnd : UIWindow 
{
    UIButton m_playOnBtn;
    UIButton m_playOnWithPayBtn;
    UIButton m_EndGameBtn;
	NumberDrawer m_curScore;
	NumberDrawer m_targetScore;

    NumberDrawer m_curJelly;
    NumberDrawer m_targetJelly;

    NumberDrawer m_curNut1;
    NumberDrawer m_targetNut1;
    NumberDrawer m_curNut2;
    NumberDrawer m_targetNut2;

    UISprite m_continueTipBoard;
    UISprite m_pauseTipBoard;
    UILabel m_itemIntro;            //使用道具继续游戏的提示文字


    UIToggle m_scoreCheck;
    UIToggle m_jellyCheck;
    UIToggle m_nutsCheck;
    UIToggle m_collectCheck;

    NumberDrawer m_levelLabel;

    UISprite[] m_collectSprite = new UISprite[3];
    UILabel[] m_collectLabel = new UILabel[3];

    UISprite m_planOnItemIcon;
	
    public override void OnCreate()
    {
        base.OnCreate();

        m_playOnBtn = GetChildComponent<UIButton>("PlayOnBtn");
        m_playOnWithPayBtn = GetChildComponent<UIButton>("PlayOnWithPayBtn");
        m_EndGameBtn = GetChildComponent<UIButton>("EndGameBtn");

        m_curScore = GetChildComponent<NumberDrawer>("CurScore");
        m_targetScore = GetChildComponent<NumberDrawer>("TargetScore");

        m_curJelly = GetChildComponent<NumberDrawer>("CurJelly");
        m_targetJelly = GetChildComponent<NumberDrawer>("TargetJelly");

        m_curNut1 = GetChildComponent<NumberDrawer>("CurNut1");
        m_targetNut1 = GetChildComponent<NumberDrawer>("TargetNut1");
        m_curNut2 = GetChildComponent<NumberDrawer>("CurNut2");
        m_targetNut2 = GetChildComponent<NumberDrawer>("TargetNut2");

        m_scoreCheck = GetChildComponent<UIToggle>("CheckBoxScore");
        m_jellyCheck = GetChildComponent<UIToggle>("CheckBoxJelly");
        m_nutsCheck = GetChildComponent<UIToggle>("CheckBoxNuts");
        m_collectCheck = GetChildComponent<UIToggle>("CheckCollect");

        m_continueTipBoard = GetChildComponent<UISprite>("ContinueTipBoard");
        m_pauseTipBoard = GetChildComponent<UISprite>("PauseTipBoard");

        m_planOnItemIcon = GetChildComponent<UISprite>("ItemIcon");
        m_itemIntro = GetChildComponent<UILabel>("ItemIntro");
		
        AddChildComponentMouseClick("EndGameBtn", OnEndGameClicked);
        AddChildComponentMouseClick("PlayOnBtn", OnPlayOnClicked);
        AddChildComponentMouseClick("PlayOnWithPayBtn", OnPlayOnClicked);

        m_levelLabel = GetChildComponent<NumberDrawer>("LevelNumber");

        for (int i = 0; i < 3; ++i)
        {
            m_collectSprite[i] = GetChildComponent<UISprite>("Collect" + i);
            m_collectLabel[i] = GetChildComponent<UILabel>("CollectLabel" + i);
        }
    }
    public override void OnShow()
    {
        base.OnShow();
        UISprite sprite = GetChildComponent<UISprite>("FailedReason");

        if ((GameLogic.Singleton.CheckLimit() || GameLogic.Singleton.ResortFailed)           //若关卡步数（时间）限制到了，或重排失败
            && GameLogic.Singleton.GetGameFlow() == TGameFlow.EGameState_End)
        {
            if (GlobalVars.CurStageData.StepLimit > 0)
            {
                sprite.spriteName = "NoMoves";
            }
            else if (GlobalVars.CurStageData.TimeLimit > 0)
            {
                sprite.spriteName = "NoTime";
            }

            m_continueTipBoard.gameObject.SetActive(true);
            m_pauseTipBoard.gameObject.SetActive(false);

            m_curScore.SetNumber(GameLogic.Singleton.GetProgress());
            m_targetScore.SetNumber(GlobalVars.CurStageData.StarScore[0]);

            if (GameLogic.Singleton.ResortFailed)
            {
                m_playOnBtn.gameObject.SetActive(false);
                m_playOnWithPayBtn.gameObject.SetActive(false);

                m_continueTipBoard.gameObject.SetActive(false);
                m_planOnItemIcon.gameObject.SetActive(false);
                m_EndGameBtn.LocalPositionX(0);
            }
            else
            {
                if (GlobalVars.CurStageData.StepLimit > 0)
                {
                    m_planOnItemIcon.spriteName = PurchasedItem.ItemAfterGame_PlusStep.ToString();
                    m_itemIntro.text = Localization.instance.Get("Intro_" + PurchasedItem.ItemAfterGame_PlusStep.ToString());
                }
                else if (GlobalVars.CurStageData.TimeLimit > 0)
                {
                    m_planOnItemIcon.spriteName = PurchasedItem.ItemAfterGame_PlusTime.ToString();
                    m_itemIntro.text = Localization.instance.Get("Intro_" + PurchasedItem.ItemAfterGame_PlusTime.ToString());
                }

                m_EndGameBtn.LocalPositionX(-108);
                m_continueTipBoard.gameObject.SetActive(true);
                m_planOnItemIcon.gameObject.SetActive(true);

                m_playOnBtn.gameObject.SetActive(false);
                m_playOnWithPayBtn.gameObject.SetActive(true);
            }
        }
        else
        {
            m_planOnItemIcon.gameObject.SetActive(false);
            sprite.spriteName = "GamePauseText";

            m_continueTipBoard.gameObject.SetActive(false);
            m_pauseTipBoard.gameObject.SetActive(true);

            m_playOnBtn.gameObject.SetActive(true);
            m_playOnWithPayBtn.gameObject.SetActive(false);

            m_curScore.SetNumberRapid(GameLogic.Singleton.GetProgress());
            m_targetScore.SetNumberRapid(GlobalVars.CurStageData.StarScore[0]);
        }

        m_scoreCheck.SetWithoutTrigger((GameLogic.Singleton.GetProgress() >= GlobalVars.CurStageData.StarScore[0]));
        

        if (GlobalVars.CurStageData.Target == GameTarget.ClearJelly)
        {
            m_jellyCheck.transform.parent.gameObject.SetActive(true);
            m_nutsCheck.transform.parent.gameObject.SetActive(false);
            m_collectCheck.transform.parent.gameObject.SetActive(false);

            int totalJellyCount = GlobalVars.CurStageData.GetSingleJellyCount() + GlobalVars.CurStageData.GetDoubleJellyCount() * 2;
            int curJellyCount = GameLogic.Singleton.PlayingStageData.GetSingleJellyCount() + GameLogic.Singleton.PlayingStageData.GetDoubleJellyCount() * 2;

            m_curJelly.SetNumber(totalJellyCount - curJellyCount);
            m_targetJelly.SetNumber(totalJellyCount);

            m_jellyCheck.value = (curJellyCount == 0);
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.BringFruitDown)
        {
            m_curNut1.SetNumber(GameLogic.Singleton.PlayingStageData.Nut1Count);
            m_targetNut1.SetNumber(GlobalVars.CurStageData.Nut1Count);
            m_curNut2.SetNumber(GameLogic.Singleton.PlayingStageData.Nut2Count);
            m_targetNut2.SetNumber(GlobalVars.CurStageData.Nut2Count);

            m_nutsCheck.transform.parent.gameObject.SetActive(true);
            m_jellyCheck.transform.parent.gameObject.SetActive(false);
            m_collectCheck.transform.parent.gameObject.SetActive(false);

            m_nutsCheck.value = (GameLogic.Singleton.PlayingStageData.Nut1Count == GlobalVars.CurStageData.Nut1Count && GameLogic.Singleton.PlayingStageData.Nut2Count == GlobalVars.CurStageData.Nut2Count);
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.Collect)      //处理搜集关的显示
        {
            m_nutsCheck.transform.parent.gameObject.SetActive(false);
            m_jellyCheck.transform.parent.gameObject.SetActive(false);
            m_collectCheck.transform.parent.gameObject.SetActive(true);

            for (int i = 0; i < 3; ++i)
            {
                if (GlobalVars.CurStageData.CollectCount[i] > 0)
                {
                    m_collectLabel[i].gameObject.SetActive(true);


                    switch (GlobalVars.CurStageData.CollectSpecial[i])
                    {
                        case TSpecialBlock.ESpecial_Normal:
                            {
                                m_collectSprite[i].spriteName = "Item" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None);
                            }
                            break;
                        case TSpecialBlock.ESpecial_NormalPlus6:
                            {
                                m_collectSprite[i].spriteName = "TimeAdded" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None);
                            }
                            break;
                        case TSpecialBlock.ESpecial_EatLineDir0:
                            m_collectSprite[i].spriteName = "Line" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None) + "_3";
                            break;
                        case TSpecialBlock.ESpecial_EatLineDir1:
                            m_collectSprite[i].spriteName = "Line" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None) + "_1";
                            break;
                        case TSpecialBlock.ESpecial_EatLineDir2:
                            m_collectSprite[i].spriteName = "Line" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None) + "_2";
                            break;
                        case TSpecialBlock.ESpecial_Bomb:
                            m_collectSprite[i].spriteName = "Bomb" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None);
                            break;
                        case TSpecialBlock.ESpecial_EatAColor:
                            m_collectSprite[i].spriteName = "Rainbow";
                            break;
                        default:
                            break;
                    }
                    m_collectLabel[i].text = GameLogic.Singleton.PlayingStageData.CollectCount[i].ToString() + "/" + GlobalVars.CurStageData.CollectCount[i].ToString();
                    if (GameLogic.Singleton.PlayingStageData.CollectCount[i] >= GlobalVars.CurStageData.CollectCount[i])
                    {
                        m_collectLabel[i].color = new Color(0, 1, 0);
                    }
                    else
                    {
                        m_collectLabel[i].color = new Color(0, 0, 0);
                    }
                }
                else
                {
                    m_collectLabel[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            m_nutsCheck.transform.parent.gameObject.SetActive(false);
            m_jellyCheck.transform.parent.gameObject.SetActive(false);
            m_collectCheck.transform.parent.gameObject.SetActive(false);
        }

        m_levelLabel.SetNumberRapid(GlobalVars.CurStageNum);        //显示关卡编号
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnEndGameClicked()
    {
        HideWindow();
		
		GameLogic.Singleton.ResumeGame();
		GameLogic.Singleton.PlayEndGameAnim();		//play the end anim(move the game area out of screen)

        GameLogic.Singleton.HideUI();

        UIWindowManager.Singleton.GetUIWindow<UIStageTarget>().Mode = UIStageTarget.TargetMode.GameFailed;
        if (GlobalVars.UseSFX)
        {
            NGUITools.PlaySound(CapsConfig.CurAudioList.GameFailedClip);
        }
        UIWindowManager.Singleton.GetUIWindow<UIStageTarget>().ShowWindow(delegate()
        {
            Timer.AddDelayFunc(0.3f, delegate()
            {
                UIWindowManager.Singleton.GetUIWindow<UIStageTarget>().HideWindow();
            });
        });

        Timer.AddDelayFunc(1.3f, delegate()
        {
            UIWindowManager.Singleton.GetUIWindow<UIRetry>().RefreshData();
            UIWindowManager.Singleton.GetUIWindow<UIRetry>().ShowWindow();
        });
    }

    private void OnPlayOnClicked()
    {
        if ((GameLogic.Singleton.GetGameFlow() == TGameFlow.EGameState_Playing && GlobalVars.CurStageData.StepLimit > 0 && GameLogic.Singleton.PlayingStageData.StepLimit > 0) ||          //若是限制步数的关卡，还有步数
            (GameLogic.Singleton.GetGameFlow() == TGameFlow.EGameState_Playing && GlobalVars.CurStageData.TimeLimit > 0 && GameLogic.Singleton.GetTimeRemain() > 0)            ||            //若是限制时间的关卡，还有时间
            (GameLogic.Singleton.GetGameFlow() != TGameFlow.EGameState_End && GameLogic.Singleton.GetGameFlow() != TGameFlow.EGameState_Playing)	//若不是在Playing状态
			)
        {
            //直接关闭窗口恢复游戏
            HideWindow(delegate()
            {
                GameLogic.Singleton.ResumeGame();
            });
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
        }
        else                                            //若没步数或时间了，就要购买和使用道具
        {
            
                HideWindow(delegate()
                {
                    if (GlobalVars.CurStageData.StepLimit > 0)
                    {
                        GlobalVars.UsingItem = PurchasedItem.ItemAfterGame_PlusStep;
                    }
                    if (GlobalVars.CurStageData.TimeLimit > 0)
                    {
                        GlobalVars.UsingItem = PurchasedItem.ItemAfterGame_PlusTime;
                    }

                    if ((int)Unibiller.GetCurrencyBalance("gold") >= 70)        //是否有足够的钱购买道具
                    {
                        UIWindowManager.Singleton.GetUIWindow<UIPurchaseNoTarget>().ShowWindow();
                    }
                    else
                    {
                        UIPurchaseNotEnoughMoney uiWindow = UIWindowManager.Singleton.GetUIWindow<UIPurchaseNotEnoughMoney>();
                        uiWindow.ShowWindow();
                        GlobalVars.OnCancelFunc = delegate()              //若取消，还显示GameEnd窗口
                        {
                            ShowWindow();
                            GlobalVars.UsingItem = PurchasedItem.None;
                        };
                        GlobalVars.OnPurchaseFunc = delegate()            //若成功，显示使用道具窗口
                        {
                            UIWindowManager.Singleton.GetUIWindow<UIPurchaseNoTarget>().ShowWindow();
                        };
                    }
                });
        }
    }
}
