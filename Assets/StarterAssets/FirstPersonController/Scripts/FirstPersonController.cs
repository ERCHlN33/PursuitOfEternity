using Unity.VisualScripting;
using UnityEngine;

public class FirstPersonController: MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Transform cameraTransform;

    Vector3 velocity;
    bool isGrounded;
    bool isCrouching = false;
    public float mouseSensitivity = 100.0f;
    public Transform playerBody;
    float xRotation = 0f;
    public float climbSpeed = 2.0f;
    public float checkRadius = 0.5f;
    [SerializeField] bool isClimbing = false;
    float verticalInput;
    float x, z;


    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()

    {
        if (isClimbing)
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.up * climbSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.S))
            {

                transform.Translate(Vector3.down * climbSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.D))
            {

                transform.Translate(Vector3.right * climbSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.A))
            {

                transform.Translate(Vector3.left * climbSpeed * Time.deltaTime);
            }
        }
        MouseMove();
        if (!isClimbing)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Crouch();

            }
            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
        }




        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");


    }
    public void Crouch()
    {
        if (isClimbing) return;
        // Karakterin boyunu küçültmek için bir scale faktörü belirleyin
        float crouchScale = 0.1f;

        // Eğer karakter zaten eğiliyorsa, kalktığında tekrar normal boyuta döndürün
        if (isCrouching)
        {
            transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            isCrouching = false;
            cameraTransform.localPosition = new Vector3(0, 1, 0); // Kameranın yüksekliğini arttırın
        }
        else
        {
            // Karakterin boyutunu küçültün
            transform.localScale = new Vector3(0.2f, crouchScale, 0.2f);
            isCrouching = true;
            cameraTransform.localPosition = new Vector3(0, 0.2f, 0); // Kameranın yüksekliğini azaltın
        }
    }
    void MouseMove()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Wall"))
        {
            if (!GetComponent<Rigidbody>())
            {
                Rigidbody rb = this.gameObject.AddComponent<Rigidbody>();
                rb.freezeRotation = true;
                rb.isKinematic = true;
            }

            isClimbing = true;
        }
    }

    private void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.CompareTag("Wall"))
        {
            if (isClimbing && GetComponent<Rigidbody>())
            {
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("Wall"))
        {
            isClimbing = false;

            if (GetComponent<Rigidbody>())
            {
                Destroy(GetComponent<Rigidbody>());
            }
        }
    }
}

