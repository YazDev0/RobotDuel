using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;            // ���� ������ ���
    public Animator animator;           // ������� �����
    public NavMeshAgent agent;          // ����� NavMeshAgent �����

    [Header("Settings")]
    public float detectionRange = 10f;  // ��� ������
    public float attackRange = 2f;      // ��� ������
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    private string currentState = "Idle";

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            // ����
            Attack();
        }
        else if (distance <= detectionRange)
        {
            // ������
            Chase();
        }
        else
        {
            // ����� �� Idle
            Patrol();
        }
    }

    void Patrol()
    {
        if (currentState != "Patrol")
        {
            currentState = "Patrol";
            agent.speed = patrolSpeed;
            animator.CrossFade("Enemy_Idle", 0.1f);
        }
        // ��� ���� ���� ���� Waypoints ����� ���� �����
    }

    void Chase()
    {
        if (currentState != "Chase")
        {
            currentState = "Chase";
            agent.speed = chaseSpeed;
            animator.CrossFade("Enemy_Run", 0.1f);
        }
        agent.SetDestination(player.position);
    }

    void Attack()
    {
        if (currentState != "Attack")
        {
            currentState = "Attack";
            agent.ResetPath();
            animator.CrossFade("Enemy_Attack", 0.1f);
        }
        // ���� ��� ����� ������ (Damage)
    }
}
