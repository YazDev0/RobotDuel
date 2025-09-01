using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Damage")]
    public float damage = 25f;
    public float lifeTime = 10f;

    [Header("Effects")]
    public GameObject hitEffectPrefab;     // 🎇 تأثير الاصطدام
    public GameObject muzzleEffectPrefab;  // 🔥 تأثير الفوهة

    void Start()
    {
        // 🟢 تأثير الفوهة عند ظهور الرصاصة
        if (muzzleEffectPrefab != null)
        {
            GameObject muzzle = Instantiate(muzzleEffectPrefab, transform.position, transform.rotation);
            Destroy(muzzle, 1f); // يختفي بعد ثانية
        }

        // عمر الرصاصة
        Destroy(gameObject, lifeTime);
    }

    void Deal(GameObject go)
    {
        var enemy = go.GetComponentInParent<EnemyStats>();
        if (enemy != null) { enemy.TakeDamage(damage); return; }

        var player = go.GetComponentInParent<PlayerStats>();
        if (player != null) { player.TakeDamage(damage); return; }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        // 🟢 انشئ التأثير عند مكان الاصطدام
        SpawnHitEffect(other.ClosestPoint(transform.position));

        Deal(other.gameObject);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision col)
    {
        // 🟢 انشئ التأثير عند نقطة الاصطدام
        SpawnHitEffect(col.contacts[0].point);

        Deal(col.collider.gameObject);
        Destroy(gameObject);
    }

    void SpawnHitEffect(Vector3 pos)
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, pos, Quaternion.identity);
            Destroy(effect, 2f); // دمر التأثير بعد ثانيتين
        }
    }
}