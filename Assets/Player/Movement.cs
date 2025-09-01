using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{


    [Header("Components")]
    public CharacterController Character;
    public Animator animator;
    public Transform cameraTransform;

    [Header("Movement")]
    public float speed = 2f;
    public float sprintMultiplier = 2f;
    public float rotationSpeed = 10f;

    [Header("Jump/Gravity")]
    public float gravity = -9.81f;
    public float jumpPower = 2f;
    float Velocity;

    [Header("Shooting")]
    public GameObject bulletPrefab;      // Prefab الرصاصة (مع Rigidbody + Collider)
    public Transform firePoint;          // مكان خروج الرصاصة من السلاح
    public float bulletSpeed = 20f;      // سرعة الرصاصة
    public bool shootTowardCamera = true;   // true = صوب على اتجاه الكاميرا (Crosshair)
    public float aimRayDistance = 200f;     // مدى التصويب من الكاميرا
    public LayerMask aimMask = ~0;          // الطبقات المسموح التصويب عليها (الكل افتراضياً)

    [Header("Audio")]
    public AudioSource audioSource;        // AudioSource مضاف على اللاعب
    public AudioClip shootClip;            // صوت الكليك/الطلقة

    void Update()
    {
        float Horizontal = Input.GetAxis("Horizontal");   // A/D or ←/→
        float Vertical = Input.GetAxis("Vertical");       // W/S or ↑/↓

        // اتجاه الكاميرا (حركة على مستوى XZ فقط)
        Vector3 camForward = cameraTransform.forward; camForward.y = 0f; camForward.Normalize();
        Vector3 camRight = cameraTransform.right; camRight.y = 0f; camRight.Normalize();

        Vector3 move = camRight * Horizontal + camForward * Vertical;
        if (move.magnitude > 1f) move.Normalize();

        // سرعة الركض
        float curSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * sprintMultiplier : speed;
        Vector3 horizontalMove = move * curSpeed; // بدون الجاذبية

        // لفّ الشخصية باتجاه الحركة

        if (move.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        } 

        // قفز + جاذبية
        if (Character.isGrounded)
        {
            Velocity = -1f;
            if (Input.GetButtonDown("Jump"))
                Velocity = Mathf.Sqrt(jumpPower * -2f * gravity);
        }
        Velocity += gravity * Time.deltaTime;

        Vector3 finalMove = horizontalMove;
        finalMove.y = Velocity;
        Character.Move(finalMove * Time.deltaTime);

        // -------- Animation Params --------
        Vector3 localMove = transform.InverseTransformDirection(move);
        float moveX = localMove.x;   // يمين/يسار
        float moveY = localMove.z;   // قدّام/خلف

        animator.SetFloat("MoveX", moveX, 0.1f, Time.deltaTime);
        animator.SetFloat("MoveY", moveY, 0.1f, Time.deltaTime);

        bool isMoving = move.sqrMagnitude > 0.0001f;
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsSprinting", Input.GetKey(KeyCode.LeftShift));

        // هجوم (يشغل صوت مع الكليك + يطلق رصاصة + يشغل أنيميشن)
        if (Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0))
        {
            // 🎵 صوت الكليك/الطلقة يشتغل مباشرة
            if (audioSource && shootClip)
                audioSource.PlayOneShot(shootClip);

            animator.SetTrigger("Attack");
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || cameraTransform == null) return;

        // احسب اتجاه الإطلاق
        Vector3 dir;

        if (shootTowardCamera)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, aimRayDistance, aimMask, QueryTriggerInteraction.Ignore))
            {
                dir = (hit.point - firePoint.position).normalized;
            }
            else
            {
                dir = cameraTransform.forward;
            }
        }
        else
        {
            dir = firePoint.forward;
        }

        // انشئ الرصاصة وادفعها
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(dir));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.velocity = dir * bulletSpeed;
        }

        // خط فحص بصري يساعدك تتأكد من الاتجاه
        Debug.DrawRay(firePoint.position, dir * 5f, Color.green, 1.5f);

        Destroy(bullet, 3f);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb != null && !rb.isKinematic)
        {
            Vector3 force = hit.moveDirection * 5f;
            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}