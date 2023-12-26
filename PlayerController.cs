using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //移动相关
    public InputAction moveAction;

    float moveSpeed = 3f;

    Vector2 m_Position;
    Vector2 m_Move;
    Rigidbody2D m_Rigidbody2d;

    //生命值相关    
    public int health { get { return m_CurrentHealth; } }   //获取当前生命值
    public int maxHealth = 5;

    public int m_CurrentHealth;

    //伤害冷却相关
    float timeInvincible = 2f;
    bool m_IsInvincible;
    float m_DamageCooldown;

    //动画相关
    Animator m_Animator;
    Vector2 m_MoveDirection = new Vector2(1, 0);

    //攻击相关
    public GameObject projectilePrefab;
    public InputAction launchAction;

    //NPC相关
    public InputAction talkAction;

    //音乐相关
    public AudioClip m_AttackClip;
    public AudioClip m_HittedClip;

    AudioSource[] m_Audiosource;    //当一个组件有多个播放器时，需要用数组来引用各个播放器
    AudioSource walkingAudioSource;     //将播放走路音频的播放器与播放其他音频的播放器区分开，防止出现播放错误
    AudioSource actionAudioSource;      //如果用一个音源的话，当停止播放走路音效时，通过playoneshot播放其余音效的效果会减弱



    void Start()
    {
        moveAction.Enable();

        talkAction.Enable();
        talkAction.performed += FindFriend;

        launchAction.Enable();
        launchAction.performed += Launch;      //按下此系统中的按键时将调用Launch函数

        m_Rigidbody2d = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();

        m_Audiosource = GetComponents<AudioSource>();
        walkingAudioSource = m_Audiosource[0];
        actionAudioSource = m_Audiosource[1];

        m_CurrentHealth = maxHealth;
    }

    void Update()
    {
        CheckInvincible();
        SetAnimator();

        
    }

    void FixedUpdate()
    {       
        Run();
        PlayWalkingSound();
    }



    private void Run()      //角色移动
    {
        //计算Input system（新的input系统）的物理值
        m_Move = moveAction.ReadValue<Vector2>();

        //通过角色的Rigidbody 2D控制移动，防止出现碰撞抖动
        m_Position = (Vector2)m_Rigidbody2d.position + m_Move * moveSpeed * Time.deltaTime;
        m_Rigidbody2d.MovePosition(m_Position);
    }

    private void Launch(InputAction.CallbackContext context)    //角色的攻击（投掷物品）
    {
        GameObject projectileObject = Instantiate(projectilePrefab, m_Rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);   //第二个参数表示位置，第三个参数表示旋转（这里表示不旋转）

        Projectile projectile = projectileObject.GetComponent<Projectile>();    //调用Projectile脚本
        projectile.Launch(m_MoveDirection, 300);

        m_Animator.SetTrigger("Launch");
        PlaySound(m_AttackClip);    //播放攻击音频
    }


    private void FindFriend(InputAction.CallbackContext context)   //靠近物体时触发对话
    {
        //第一个参数表示射线方向，第二个是角色看向的方向，第三个是射线的最大距离，第四个是触发检查的layer
        RaycastHit2D hit = Physics2D.Raycast(m_Rigidbody2d.position + Vector2.up * 0.2f, m_MoveDirection, 1.5f, LayerMask.GetMask("NPC"));

        if (hit.collider != null)
        {
            NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();     //引用含有NonPlayerCharacter脚本的组件

            if (character != null)
            {
                UIHandler.instance.DisplayDialogue(character.dialogText);     //根据不同的NPC显示对话
            }
        }
    }


    private void SetAnimator()      //决定角色移动时不同方向的动画
    {
        if (!Mathf.Approximately(m_Move.x, 0f) || !Mathf.Approximately(m_Move.y, 0f))   //检查X或Y的值是否为0。mathf.Approximately用于防止float精度问题
        {
            m_MoveDirection.Set(m_Move.x, m_Move.y);     //角色移动时，将角色的坐标传给m_Direction
            m_MoveDirection.Normalize();      //Normalize用于储存方向的向量，它会将任何不为0的长度设置为1，因为对于方向来说，长度不重要
        }

        m_Animator.SetFloat("Move X", m_MoveDirection.x);
        m_Animator.SetFloat("Move Y", m_MoveDirection.y);
        m_Animator.SetFloat("Speed", m_Move.magnitude);
    }



    public void ChangeHealth(int amount)        //更改角色生命值
    {
        if (amount < 0)     //受伤时设置无敌时间
        {
            if (m_IsInvincible)
            {
                return;
            }
            m_IsInvincible = true;      //当角色受伤且不在无敌时间时赋予无敌时间
            m_DamageCooldown = timeInvincible;

            m_Animator.SetTrigger("Hit");       //改成受击动画状态
            PlaySound(m_HittedClip);    //播放受击音效
        }

        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth + amount, 0, maxHealth);  //限制第一个参数的范围，最小为第二个参数，最大为第三个（角色生命值区间为0-5）

        UIHandler.instance.SetHealthValue(m_CurrentHealth / (float)maxHealth);      //括号里的float保证两个参数相除后的值也是float（即使他们都是int）
    }

    private void CheckInvincible()      //检查角色是否处于无敌状态
    {
        if (m_IsInvincible)     //持续减少无敌时间
        {
            m_DamageCooldown -= Time.deltaTime;
            if (m_DamageCooldown < 0)
            {
                m_IsInvincible = false;
            }
        }
    }


    private void PlayWalkingSound()
    {
        if (m_Move.magnitude > Mathf.Epsilon && !walkingAudioSource.isPlaying)      //后半段很重要，如果在播放过程中重复播放会有电流声！
        {
            walkingAudioSource.Play();        
        }

        else if (m_Move.magnitude <= Mathf.Epsilon && walkingAudioSource.isPlaying)
        {
            walkingAudioSource.Stop();
        }
    }

    public void PlaySound(AudioClip clip)       //通过角色播放音频，使用与走路音源不同的音源
    {
        actionAudioSource.PlayOneShot(clip);
    }
}
