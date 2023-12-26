using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    //血条相关
    public static UIHandler instance { get; private set; }  //任何地方都能Get这个变量，但只有当前脚本可以Set此变量
    public float currentHealth = 0.5f;

    UIDocument m_UiDocument;
    VisualElement m_Healthbar;

    //NPC对话相关
    float displayTime = 4f;
    public Text dialogUIElement;

    VisualElement m_NonPlayerDialogue;
    float m_TimerDisplay;


    void Awake()
    {
        instance = this;    //this指当前class实例的引用。this.currentHealth就是UIHandler.currentHealth
    }

    void Start()
    {
        m_UiDocument = GetComponent<UIDocument>();
        m_Healthbar = m_UiDocument.rootVisualElement.Q<VisualElement>("HealthBar");     //访问当前UI文件里的子UI，“Q”可以在多个子物品中通过参数和名字找到特定的一个

        m_NonPlayerDialogue = m_UiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
        m_NonPlayerDialogue.style.display = DisplayStyle.None;      //游戏开始时隐藏对话框
        m_TimerDisplay = -1f;

        SetHealthValue(1.0f);
    }

    void Update()
    {
        CheckTimer();
    }



    public void SetHealthValue(float percentage)    //正确地显示当前生命值
    {
        m_Healthbar.style.width = Length.Percent(percentage * 100f);
    }



    public void DisplayDialogue(string text)   //显示对话框
    {
        Label dialogLabel = m_NonPlayerDialogue.Q<Label>("Label");     //通过父物体文本框NPCDialogue引用子物体文本Label
        dialogLabel.text = text;       //在显示前更改文本框内容

        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;      //显示UI
        m_TimerDisplay = displayTime;
    }

    private void CheckTimer()   //检查对话框显示时长计时器
    {
        if (m_TimerDisplay > 0)
        {
            m_TimerDisplay -= Time.deltaTime;   //开始计时

            if (m_TimerDisplay < 0)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None;    //隐藏UI
            }
        }
    }
}
