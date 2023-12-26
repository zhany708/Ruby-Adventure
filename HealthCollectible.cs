using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    //音频相关
    public AudioClip collectedClip;

    private void OnTriggerEnter2D(Collider2D other)     //检查玩家是否触碰食品
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && controller.health < controller.maxHealth)
        {
            controller.ChangeHealth(1);   //增加一点生命
            Destroy(gameObject);    //触发后删除当前代码关联的物体（食物）

            controller.PlaySound(collectedClip);    //此处通过角色播放音频，因为如果通过食物播放的话，食物摧毁后音频也会消失
        }
    }
}
