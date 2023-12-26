using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //�ƶ����
    public bool vertical;

    float moveSpeed = 2.0f;
    Vector2 m_Position;
    Rigidbody2D m_Rigidbody2d;


    //��ʱ�����
    float timeMovement = 3.0f;
    float m_Timer;
    int m_Direction = 1;

    //�������
    Animator m_Animator;

    //�������
    public ParticleSystem smokeEffect;

    bool m_Aggressive = true;

    //��Ƶ���
    public AudioClip m_FixedClip;

    AudioSource m_AudioSource;
    


    void Start()
    {
        m_Rigidbody2d = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        m_AudioSource = GetComponent<AudioSource>();

        m_Timer = timeMovement;
    }

    void Update()
    {
        CheckTimer();
    }

    void FixedUpdate()
    {
        if (!m_Aggressive)
        {
            return;
        }

        Run();
    }




    private void Run()      //���˵�ǰ����ֻ�����������ĸ����򣬲���б���ߣ�
    {
        m_Position = m_Rigidbody2d.position;

        if (vertical)
        {
            m_Position.y += moveSpeed * m_Direction * Time.deltaTime;   //m_Direction�������ƶ�����Ҫô��Ҫô��

            m_Animator.SetFloat("Move X", 0);
            m_Animator.SetFloat("Move Y", m_Direction);     //m_Direction������������
        }

        else
        {
            m_Position.x += moveSpeed * m_Direction * Time.deltaTime;

            m_Animator.SetFloat("Move X", m_Direction);
            m_Animator.SetFloat("Move Y", 0);
        }

        m_Rigidbody2d.MovePosition(m_Position);
    }


    public void Fix()   //�Ի����˵�����
    {
        m_Aggressive = false;
        m_Rigidbody2d.simulated = false;    //�����������ϵͳ���Ƴ���Ҳ����˵����������ײ����
        smokeEffect.Stop();    //ֹͣ����Ч�������ɾ���Ļ��ῴ��������ʵ��

        m_Animator.SetTrigger("Fixed");
            
        m_AudioSource.Stop();      //��ͣ������·��Ƶ
        PlaySound(m_FixedClip);    //����������Ƶ
    }


    private void CheckTimer()   //���������߷���ļ�ʱ��
    {
        m_Timer -= Time.deltaTime;

        if (m_Timer <= 0)
        {
            m_Direction = -m_Direction;   //��ת����
            m_Timer = timeMovement;       //���õ�ǰ��ʱ����ʱ��
        }
    }


    private void OnCollisionEnter2D(Collision2D other)      //�������Ƿ��������ײ
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();     //����PlayerController�ű�

        if (player != null)     //ֻ�Ժ���PlayerController�ű���������Ч
        {
            player.ChangeHealth(-1);
        }      
    }

    private void PlaySound(AudioClip clip)
    {
        m_AudioSource.PlayOneShot(clip);
    }
}
