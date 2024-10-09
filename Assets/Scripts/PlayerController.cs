using UnityEngine;

// Controls player movement and rotation.
public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float lookSpeed = 2f;
    public float jumpPower = 8.0f;

    public Camera playerCamera;
    
    public int maxAmmo;
    public int defaultAmmo = 10;
    
    public int maxHealth;
    public int defaultHealth = 100;
    
    [HideInInspector]
    public int health;
    
    [HideInInspector]
    public int ammo;
    
    private Rigidbody _rb; 
    private float _rotationX = 0.0f;
    private bool _isGrounded;
    
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        ammo = defaultAmmo;
        health = defaultHealth;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        Move();
        Look();
        currentRotation = transform.eulerAngles
        transform.eulerAngles = new Vector3(0.0f, currentRotation.y, 0.0f);
    }

    private void Move()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(horizontal, 0, vertical);
        transform.Translate(movement * speed * Time.deltaTime);

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (!other.GetComponent<Ball>().live)
            {
                ammo++;
                Debug.Log(ammo.ToString());
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        } 
        else if (collision.gameObject.CompareTag("Ball"))
        {
            if (collision.gameObject.GetComponent<Ball>().live)
            {
                health -= 10;
                Debug.Log(health.ToString());
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

}
