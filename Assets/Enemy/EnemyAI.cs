using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform target;            // ÇááÇÚÈ
    public Animator animator;           // Animator ÇáÚÏæ
    public NavMeshAgent agent;          // NavMeshAgent Úáì ÇáÚÏæ
    public CharacterController characterToPush; // ÇÎÊíÇÑí

    [Header("Movement")]
    public float detectionRange = 15f;    // íÈÏÃ íÊİÇÚá ÅĞÇ ŞÑÈ ÇááÇÚÈ
    public float attackRange = 8f;        // íÊæŞİ æíåÇÌã ÚäÏ åĞÇ ÇáãÏì
    public float rotationSpeed = 12f;     // ÓÑÚÉ áİ ÇáæÌå äÍæ ÇáåÏİ
    public float idleStopDistance = 0.1f; // íÚÊÈÑ æÇŞİ ÅĞÇ ÇáÓÑÚÉ ÃŞá ãä åĞÇ

    [Header("Shooting")]
    public GameObject bulletPrefab;     // Prefab ÇáÑÕÇÕÉ
    public Transform firePoint;         // ãßÇä ÎÑæÌ ÇáÑÕÇÕÉ
    public float bulletSpeed = 20f;     // ÓÑÚÉ ÇáÑÕÇÕÉ
    public float fireCooldown = 0.4f;   // Èíä ÇáØáŞÇÊ
    private float fireTimer = 0f;

    [Header("Aiming (like player)")]
    public bool aimLikePlayer = true;     // íæÌå ÇáÑÕÇÕÉ áäŞØÉ İÚáíÉ Úáì ÇááÇÚÈ
    public float aimOffsetY = 1.2f;       // ÇÑÊİÇÚ ÇáÊÕæíÈ (ÕÏÑ/ÑÃÓ ÇááÇÚÈ)
    public float aimRayDistance = 200f;   // ãÏì ÇáÑÄíÉ
    public LayerMask aimMask = ~0;        // ÇáØÈŞÇÊ ÇáãÓãæÍÉ (Çáßá ÇİÊÑÇÖíÇğ)

    [Header("Anim Params")]
    public string moveXParam = "MoveX";
    public string moveYParam = "MoveY";
    public string isMovingParam = "IsMoving";
    public string isSprintingParam = "IsSprinting"; // ÛíÑ ãÓÊÎÏã åäÇ
    public string attackTrigger = "Attack";

    const float DEAD = 0.1f;

    void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        if (agent) agent.updateRotation = false; // Èäáİø íÏæíÇğ
    }

    void Update()
    {
        if (!target || !agent || !animator) return;

        fireTimer -= Time.deltaTime;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > detectionRange)
        {
            agent.ResetPath();
            UpdateAnimFromVelocity(Vector3.zero);
            return;
        }

        if (dist > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            FaceTowards(agent.desiredVelocity.sqrMagnitude > 0.001f ? transform.position + agent.desiredVelocity : target.position);
            UpdateAnimFromVelocity(agent.velocity);
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();
            FaceTowards(target.position);
            UpdateAnimFromVelocity(Vector3.zero);
            TryShoot();
        }
    }

    void UpdateAnimFromVelocity(Vector3 worldVel)
    {
        Vector3 local = transform.InverseTransformDirection(worldVel);
        float moveX = local.x;
        float moveY = local.z;

        animator.SetFloat(moveXParam, moveX, 0.1f, Time.deltaTime);
        animator.SetFloat(moveYParam, moveY, 0.1f, Time.deltaTime);

        bool moving = worldVel.magnitude > idleStopDistance;
        animator.SetBool(isMovingParam, moving);
    }

    void FaceTowards(Vector3 worldPoint)
    {
        Vector3 dir = (worldPoint - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }

    void TryShoot()
    {
        if (fireTimer > 0f) return;

        if (!string.IsNullOrEmpty(attackTrigger))
            animator.SetTrigger(attackTrigger);

        if (bulletPrefab && firePoint)
        {
            Vector3 dir = firePoint.forward;

            if (aimLikePlayer && target)
            {
                // äŞØÉ ÇáÊÕæíÈ (ÕÏÑ/ÑÃÓ ÇááÇÚÈ)
                Vector3 aimPoint = target.position + Vector3.up * aimOffsetY;
                dir = (aimPoint - firePoint.position).normalized;

                // Raycast ãä İæåÉ ÇáÓáÇÍ ÈÇÊÌÇå ÇáåÏİ áÖÈØ ÇáÇÕØÏÇã (ÌÏÇÑ ãËáÇğ)
                if (Physics.Raycast(firePoint.position, dir, out RaycastHit hit, aimRayDistance, aimMask, QueryTriggerInteraction.Ignore))
                {
                    // æÌøå ááøãÓ ÇáäŞØÉ ãÈÇÔÑÉ
                    dir = (hit.point - firePoint.position).normalized;
                }
            }

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(dir));
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = false;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.velocity = dir * bulletSpeed;
            }

            Debug.DrawRay(firePoint.position, dir * 5f, Color.green, 1.5f);
            Destroy(bullet, 3f);
        }

        fireTimer = fireCooldown;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (characterToPush == null) return;
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb != null && !rb.isKinematic)
        {
            rb.AddForce(hit.moveDirection * 5f, ForceMode.Impulse);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
