using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public Transform orientation; 
    public Transform groundCheck;
    public Transform cam;
    public LayerMask groundLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;

    public float jumpForce = 5f;
    public float groundCheckRadius = 0.2f;

    private Rigidbody rb;
    [HideInInspector] public bool isMoving;
    private Vector3 moveDirection;
    [HideInInspector] public bool isGrounded;


    private void OnDisable()
    {
        rb.velocity = Vector3.zero; 
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        CheckGrounded();
        HandleInput();
       RotatePlayerOrientation();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplyGravity();
    }

    private void HandleInput()
    {
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        Vector3 forwardOnPlane = Vector3.ProjectOnPlane(camForward, GetProjectionPlane()).normalized;
        Vector3 rightOnPlane = Vector3.ProjectOnPlane(camRight, GetProjectionPlane()).normalized;

        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        isMoving = verticalInput != 0 || horizontalInput != 0;

        moveDirection = (forwardOnPlane * verticalInput + rightOnPlane * horizontalInput).normalized;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Vector3 jumpDirection = GetProjectionPlane();

            rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);

        }
    }

    public Vector3 GetProjectionPlane()
    {
        Vector3 localUp = transform.up;

        if (Vector3.Dot(localUp, Vector3.up) > 0.9f)
            return Vector3.up;
        if (Vector3.Dot(localUp, Vector3.down) > 0.9f)
            return Vector3.down;
        if (Vector3.Dot(localUp, Vector3.right) > 0.9f)
            return Vector3.right;
        if (Vector3.Dot(localUp, Vector3.left) > 0.9f)
            return Vector3.left;
        if (Vector3.Dot(localUp, Vector3.forward) > 0.9f)
            return Vector3.forward;
        if (Vector3.Dot(localUp, Vector3.back) > 0.9f)
            return Vector3.back;

        return Vector3.up; 
    }


    private void MovePlayer()
    {
        Vector3 gravityDirection = -GetProjectionPlane(); 

        Vector3 moveVelocity = Vector3.ProjectOnPlane(moveDirection * moveSpeed, gravityDirection);

        rb.velocity = moveVelocity + Vector3.Project(rb.velocity, gravityDirection);
    }

    private void ApplyGravity()
    {
        Vector3 gravityDirection = -GetProjectionPlane(); 
        float gravityStrength = 9.81f; 

        rb.AddForce(gravityDirection * gravityStrength, ForceMode.Acceleration);
    }

    private void RotatePlayerOrientation()
    {
        Vector3 gravityDirection = -GetProjectionPlane(); 
        Vector3 velocityDirection = Vector3.ProjectOnPlane(rb.velocity, gravityDirection); 

        if (velocityDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocityDirection, transform.up);

            orientation.rotation = Quaternion.Slerp(orientation.rotation, targetRotation, Time.deltaTime * 30f);
        }
    }
    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
