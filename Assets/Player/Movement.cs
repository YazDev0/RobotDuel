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

    void Update()
    {
        float Horizontal = Input.GetAxis("Horizontal");   // A/D or ←/→
        float Vertical = Input.GetAxis("Vertical");     // W/S or ↑/↓

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
        // حول متجه الحركة إلى مساحة اللاعب، حتى الـBlend Tree يعرف يمين/يسار/قدّام/خلف
        Vector3 localMove = transform.InverseTransformDirection(move);
        float moveX = localMove.x;   // يمين/يسار
        float moveY = localMove.z;   // قدّام/خلف

        // قيم سلسة للـBlend Tree
        animator.SetFloat("MoveX", moveX, 0.1f, Time.deltaTime);
        animator.SetFloat("MoveY", moveY, 0.1f, Time.deltaTime);

        // مفيد لو عندك انتقالات تعتمد على الحركة/الركض
        bool isMoving = move.sqrMagnitude > 0.0001f;
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsSprinting", Input.GetKey(KeyCode.LeftShift));

        // هجوم (زر الفأرة الأيسر أو "Fire1" من Input Manager)
        if (Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0))
            animator.SetTrigger("Attack");
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
