using UnityEditor.Search;
using UnityEngine;

// Controls player movement and rotation.
public class PlayerController : MonoBehaviour
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
    
    [HideInInspector]
    public int health;
    
    [HideInInspector]
    public int ammo;
    
    private Rigidbody _rb; 
    private float _rotationX;
    private bool _isGrounded;
    private bool _sprinting;
    private float _sprintRemaining;
    
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        ammo = defaultAmmo;
        health = defaultHealth;
        Cursor.lockState = CursorLockMode.Locked;
        _sprintRemaining = maxSprintTime;
    }
    
    void Update()
    {
        Move();
        Look();
        Vector3 currentRotation = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0.0f, currentRotation.y, 0.0f);
        if (Input.GetMouseButtonDown(0))
        {
            Throw();
        }
    }

    private void Move()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(horizontal, 0, vertical).normalized;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (_sprintRemaining > 0)
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
            _sprintRemaining += Time.deltaTime;
        }
        
        float currentSpeed = _sprinting ? sprintSpeed : speed;
        transform.Translate(movement * currentSpeed * Time.deltaTime);
        _sprintRemaining = Mathf.Clamp(_sprintRemaining, 0, maxSprintTime);
        

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);
        
        playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        transform.Rotate(0, mouseX, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        } 
        else if (collision.gameObject.CompareTag("Ball"))
        {
            Ball ball = collision.gameObject.GetComponent<Ball>();
            if (ball.live && ball.parent != transform.GetInstanceID()) 
            {
                health -= 10;
                Debug.Log(health.ToString());
            }
            else if (!ball.live && ammo < maxAmmo)
            {
                ammo++;
                Debug.Log(ammo.ToString());
            }
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = false;
        }
    }

    private void Throw()
    {
        if (ballPrefab != null && ammo > 0)
        {
            GameObject ball = Instantiate(ballPrefab, playerCamera.transform.position, playerCamera.transform.rotation);
            ball.GetComponent<Ball>().parent = transform.GetInstanceID();
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                Vector3 throwDirection = playerCamera.transform.forward;
                ballRb.AddForce(throwDirection * throwPower, ForceMode.Impulse);
                ammo--;
            }
        }
    }
}
