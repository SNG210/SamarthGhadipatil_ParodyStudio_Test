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
    public float rotationSpeed = 10;

    private Rigidbody rb;
    public bool isGrounded;
    public bool isMoving;

    float verticalInput;
    float horizontalInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
    }

    private void Update()
    {
        CheckGrounded();
        HandleRotation();
        InputAxis();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplyGravity();
    }

    private Vector3 MoveDirection()
    {      
        isMoving = verticalInput != 0 || horizontalInput != 0;

        return Vector3.ProjectOnPlane((cam.forward * verticalInput + cam.right * horizontalInput).normalized, GetSurfaceNormal());

    }

    private void InputAxis()
    {
         verticalInput = Input.GetAxisRaw("Vertical");
         horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    private void MovePlayer()
    {
        rb.velocity = MoveDirection() * moveSpeed + Vector3.Project(rb.velocity, GetSurfaceNormal()) ;
    }

    private 

    void HandleRotation()
    {
        Vector3 targetDirection = MoveDirection();

        if (targetDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, GetSurfaceNormal());
            orientation.rotation = Quaternion.Slerp(orientation.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void ApplyGravity()
    {
        rb.AddForce(-GetSurfaceNormal() * gravityStrength, ForceMode.Acceleration);
    }

    private void Jump()
    {
        rb.AddForce(GetSurfaceNormal() * jumpForce, ForceMode.Impulse);
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
