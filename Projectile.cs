using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Projectile : MonoBehaviour
{
    //移动相关
    Rigidbody2D m_Rigidbody2d;

    //音频相关
    public AudioClip m_EnemyHittedClip;


    void Awake()
    {
        m_Rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (transform.position.magnitude > 100f)    //当飞弹飞行100个单位后，删除飞弹
        {
            Destroy(gameObject);
        }
    }


    public void Launch(Vector2 direction, float force)
    {
        m_Rigidbody2d.AddForce(direction * force);      //给物体施加方向和力
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
        if (enemy != null)  //当此函数调用含有EnemyController脚本的组件时
        {
            //enemy.PlaySound(m_EnemyHittedClip);    //播放敌人受击音频（源头来自敌人）
            enemy.Fix();
        }
        Destroy(gameObject);
    }
}
