using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform groundCheck;
    public Transform cam;
    public LayerMask groundLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float jumpForce = 5f;
    public float groundCheckRadius = 0.2f;
    public float gravityStrength = 9.81f;

    private Rigidbody rb;
    private Vector3 moveDirection;
    public bool isGrounded;
    public bool isMoving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
    }

    private void Update()
    {
        CheckGrounded();
        ProcessInput();
        RotatePlayer();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplyGravity();
    }

    private void ProcessInput()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        isMoving = verticalInput != 0 || horizontalInput != 0;

        Vector3 forward = Vector3.ProjectOnPlane(cam.forward, GetSurfaceNormal()).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cam.right, GetSurfaceNormal()).normalized;
        moveDirection = (forward * verticalInput + right * horizontalInput).normalized;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    private void MovePlayer()
    {
        Vector3 gravityDirection = -GetSurfaceNormal();
        Vector3 moveVelocity = Vector3.ProjectOnPlane(moveDirection * moveSpeed, gravityDirection);
        rb.velocity = moveVelocity + Vector3.Project(rb.velocity, gravityDirection);
    }

    private void ApplyGravity()
    {
        rb.AddForce(-GetSurfaceNormal() * gravityStrength, ForceMode.Acceleration);
    }

    private void Jump()
    {
        rb.AddForce(GetSurfaceNormal() * jumpForce, ForceMode.Impulse);
    }

    private void RotatePlayer()
    {
        if (isMoving)
        {
            Vector3 velocityDirection = Vector3.ProjectOnPlane(rb.velocity, -GetSurfaceNormal());
            if (velocityDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(velocityDirection, transform.up);
                orientation.rotation = Quaternion.Slerp(orientation.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private Vector3 GetSurfaceNormal()
    {
        return transform.up.normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
