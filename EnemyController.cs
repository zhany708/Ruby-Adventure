using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //移动相关
    public bool vertical;

    float moveSpeed = 2.0f;
    Vector2 m_Position;
    Rigidbody2D m_Rigidbody2d;


    //计时器相关
    float timeMovement = 3.0f;
    float m_Timer;
    int m_Direction = 1;

    //动画相关
    Animator m_Animator;

    //攻击相关
    public ParticleSystem smokeEffect;

    bool m_Aggressive = true;

    //音频相关
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




    private void Run()      //敌人的前进（只能上下左右四个方向，不能斜着走）
    {
        m_Position = m_Rigidbody2d.position;

        if (vertical)
        {
            m_Position.y += moveSpeed * m_Direction * Time.deltaTime;   //m_Direction决定了移动方向，要么正要么负

            m_Animator.SetFloat("Move X", 0);
            m_Animator.SetFloat("Move Y", m_Direction);     //m_Direction决定动画方向
        }

        else
        {
            m_Position.x += moveSpeed * m_Direction * Time.deltaTime;

            m_Animator.SetFloat("Move X", m_Direction);
            m_Animator.SetFloat("Move Y", 0);
        }

        m_Rigidbody2d.MovePosition(m_Position);
    }


    public void Fix()   //对机器人的修理
    {
        m_Aggressive = false;
        m_Rigidbody2d.simulated = false;    //将组件从物理系统中移除，也就是说不会再有碰撞发生
        smokeEffect.Stop();    //停止烟雾效果（如果删除的话会看起来不真实）

        m_Animator.SetTrigger("Fixed");
            
        m_AudioSource.Stop();      //暂停敌人走路音频
        PlaySound(m_FixedClip);    //播放修理音频
    }


    private void CheckTimer()   //检查敌人行走方向的计时器
    {
        m_Timer -= Time.deltaTime;

        if (m_Timer <= 0)
        {
            m_Direction = -m_Direction;   //逆转方向
            m_Timer = timeMovement;       //重置当前计时器的时间
        }
    }


    private void OnCollisionEnter2D(Collision2D other)      //检查敌人是否与玩家碰撞
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();     //调用PlayerController脚本

        if (player != null)     //只对含有PlayerController脚本的物体生效
        {
            player.ChangeHealth(-1);
        }      
    }

    private void PlaySound(AudioClip clip)
    {
        m_AudioSource.PlayOneShot(clip);
    }
}
