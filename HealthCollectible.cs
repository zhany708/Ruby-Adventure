using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    //��Ƶ���
    public AudioClip collectedClip;

    private void OnTriggerEnter2D(Collider2D other)     //�������Ƿ���ʳƷ
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && controller.health < controller.maxHealth)
        {
            controller.ChangeHealth(1);   //����һ������
            Destroy(gameObject);    //������ɾ����ǰ������������壨ʳ�

            controller.PlaySound(collectedClip);    //�˴�ͨ����ɫ������Ƶ����Ϊ���ͨ��ʳ�ﲥ�ŵĻ���ʳ��ݻٺ���ƵҲ����ʧ
        }
    }
}
