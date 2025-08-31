using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 25f;
    public float lifeTime = 10f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Deal(GameObject go)
    {
        // ����� EnemyStats �� PlayerStats ��� �������
        var enemy = go.GetComponentInParent<EnemyStats>();
        if (enemy != null) { enemy.TakeDamage(damage); return; }

        var player = go.GetComponentInParent<PlayerStats>();
        if (player != null) { player.TakeDamage(damage); return; }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return; // ����� ������ ����� (�������)
        Deal(other.gameObject);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision col)
    {
        Deal(col.collider.gameObject);
        Destroy(gameObject);
    }
}
