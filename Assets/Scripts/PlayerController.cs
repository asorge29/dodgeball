using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float speed = 5.0f;
    public float sprintSpeed = 10.0f;
    public float maxSprintTime = 6.0f;
    public float lookSpeed = 2f;
    public float jumpPower = 8.0f;

    public Camera playerCamera;
    
    public GameObject ballPrefab;
    public float throwPower = 10.0f;
    
    public int maxAmmo = 10;
    public int defaultAmmo = 10;
    
    public int maxHealth = 100;
    public int defaultHealth = 100;
    
    private Rigidbody _rb; 
    private float _rotationX;
    private NetworkVariable<bool> _isGrounded = new NetworkVariable<bool>();
    private bool _sprinting;
    private float _sprintRemaining;
    
    void Start()
    {
        if (!IsOwner)
        { 
            playerCamera.gameObject.SetActive(false);
        }
        else
        {
            _rb = GetComponent<Rigidbody>();
            _sprintRemaining = maxSprintTime;
            playerCamera.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    void Update()
    {
        if (!IsOwner) return;
        Move();
        Look();
    }

    public void Move()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(horizontal, 0, vertical).normalized;
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (_sprintRemaining > 0)// && _isGrounded.Value)
            {
                _sprinting = true;
                _sprintRemaining -= Time.deltaTime;
            }
            else
            {
                _sprinting = false;
            }
        }
        else
        {
            _sprinting = false;
            _sprintRemaining += Time.deltaTime / 2.0f;
            _sprintRemaining = Mathf.Clamp(_sprintRemaining, 0, maxSprintTime);
        }
        
        float currentSpeed = _sprinting ? sprintSpeed : speed;
        
        if (Input.GetKeyDown(KeyCode.Space))// && _isGrounded.Value)
        {
            JumpServerRpc();
        }
        
        MoveServerRpc(movement, currentSpeed);
    }
    
    [ServerRpc]
    void MoveServerRpc(Vector3 movement, float currentSpeed)
    {
        transform.Translate(movement * currentSpeed * Time.deltaTime);
    }

    [ServerRpc]
    void JumpServerRpc()
    {
        if (!_isGrounded.Value) return;
        _rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        JumpClientRpc();
    }

    [ClientRpc]
    void JumpClientRpc()
    {
        if (!IsOwner) return;
        _rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }
    
    public void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);
        
        LookServerRpc(_rotationX, mouseX);
    }

    [ServerRpc]
    void LookServerRpc(float rotationX, float mouseX)
    {
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
        
        UpdateCameraRotationClientRpc(rotationX);
    }

    [ClientRpc]
    void UpdateCameraRotationClientRpc(float rotationX)
    {
        if (!IsOwner) return;
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    [ServerRpc]
    private void UpdateGroundedStateServerRpc(bool isGrounded)
    {
        _isGrounded.Value = isGrounded;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            UpdateGroundedStateServerRpc(true);
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            UpdateGroundedStateServerRpc(false);
        }
    }
}
