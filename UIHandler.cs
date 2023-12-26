using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    //Ѫ�����
    public static UIHandler instance { get; private set; }  //�κεط�����Get�����������ֻ�е�ǰ�ű�����Set�˱���
    public float currentHealth = 0.5f;

    UIDocument m_UiDocument;
    VisualElement m_Healthbar;

    //NPC�Ի����
    float displayTime = 4f;
    public Text dialogUIElement;

    VisualElement m_NonPlayerDialogue;
    float m_TimerDisplay;


    void Awake()
    {
        instance = this;    //thisָ��ǰclassʵ�������á�this.currentHealth����UIHandler.currentHealth
    }

    void Start()
    {
        m_UiDocument = GetComponent<UIDocument>();
        m_Healthbar = m_UiDocument.rootVisualElement.Q<VisualElement>("HealthBar");     //���ʵ�ǰUI�ļ������UI����Q�������ڶ������Ʒ��ͨ�������������ҵ��ض���һ��

        m_NonPlayerDialogue = m_UiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
        m_NonPlayerDialogue.style.display = DisplayStyle.None;      //��Ϸ��ʼʱ���ضԻ���
        m_TimerDisplay = -1f;

        SetHealthValue(1.0f);
    }

    void Update()
    {
        CheckTimer();
    }



    public void SetHealthValue(float percentage)    //��ȷ����ʾ��ǰ����ֵ
    {
        m_Healthbar.style.width = Length.Percent(percentage * 100f);
    }



    public void DisplayDialogue(string text)   //��ʾ�Ի���
    {
        Label dialogLabel = m_NonPlayerDialogue.Q<Label>("Label");     //ͨ���������ı���NPCDialogue�����������ı�Label
        dialogLabel.text = text;       //����ʾǰ�����ı�������

        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;      //��ʾUI
        m_TimerDisplay = displayTime;
    }

    private void CheckTimer()   //���Ի�����ʾʱ����ʱ��
    {
        if (m_TimerDisplay > 0)
        {
            m_TimerDisplay -= Time.deltaTime;   //��ʼ��ʱ

            if (m_TimerDisplay < 0)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None;    //����UI
            }
        }
    }
}
