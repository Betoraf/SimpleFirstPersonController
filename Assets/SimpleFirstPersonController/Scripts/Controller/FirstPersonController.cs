using UnityEngine;
using Betoraf.UI;

namespace Betoraf.Controller
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player Controller Settings")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float cameraSensitivity = 1f;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float gravity = -9.81f;

        [Header("Mobile Input Settings")]
        [SerializeField] private FixedJoystick moveJoystick;
        [SerializeField] private bool isMobile = false;
        [SerializeField] private UniversalButton jumpButton;
        [SerializeField] private GameObject mobileUI;

        private int rightFingerId = -1;
        private float halfScreenWidth;
        private Vector2 lookInput;
        private float cameraPitch;

        private float verticalVelocity;
        private bool isGrounded;

        private void Start()
        {
            halfScreenWidth = Screen.width / 2;
            jumpButton.onPressDown.AddListener(Jump);

            if (!isMobile)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                mobileUI.SetActive(false);
            }
        }

        private void Update()
        {
            isGrounded = characterController.isGrounded;

            if (isMobile)
            {
                HandleMobileInput();
            }
            else
            {
                HandlePCInput();
            }

            ApplyGravity();
        }

        private void HandleMobileInput()
        {
            GetTouchInput();
            if (rightFingerId != -1) LookAround();
            MoveWithJoystick();
        }

        private void HandlePCInput()
        {
            LookWithMouse();
            MoveWithKeyboard();
            HandlePCJump();
        }

        private void GetTouchInput()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (touch.position.x > halfScreenWidth && rightFingerId == -1)
                        {
                            rightFingerId = touch.fingerId;
                        }
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (touch.fingerId == rightFingerId)
                        {
                            rightFingerId = -1;
                        }
                        break;

                    case TouchPhase.Moved:
                        if (touch.fingerId == rightFingerId)
                        {
                            lookInput = touch.deltaPosition * cameraSensitivity * Time.deltaTime;
                        }
                        break;

                    case TouchPhase.Stationary:
                        if (touch.fingerId == rightFingerId)
                        {
                            lookInput = Vector2.zero;
                        }
                        break;
                }
            }
        }

        private void LookAround()
        {
            cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
            transform.Rotate(Vector3.up, lookInput.x);
        }

        private void MoveWithJoystick()
        {
            if (moveJoystick != null)
            {
                Vector2 joystickInput = new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical);
                Vector3 moveDirection = new Vector3(joystickInput.x, 0, joystickInput.y).normalized * moveSpeed * Time.deltaTime;
                characterController.Move(transform.right * moveDirection.x + transform.forward * moveDirection.z);
            }
        }

        private void LookWithMouse()
        {
            float mouseSensitivityMultiplier = isMobile ? cameraSensitivity : cameraSensitivity * 30f;
            lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivityMultiplier * Time.deltaTime;

            cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
            transform.Rotate(Vector3.up, lookInput.x);
        }

        private void MoveWithKeyboard()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized * moveSpeed * Time.deltaTime;
            characterController.Move(transform.right * moveDirection.x + transform.forward * moveDirection.z);
        }

        private void HandlePCJump()
        {
            if (isGrounded && Input.GetKeyDown(KeyCode.Space) && !isMobile)
            {
                Jump();
            }
        }

        private void Jump()
        {
            if (isGrounded)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        private void ApplyGravity()
        {
            if (!isGrounded)
            {
                verticalVelocity += gravity * Time.deltaTime;
            }
            else if (verticalVelocity < 0)
            {
                verticalVelocity = -2f;
            }

            Vector3 move = new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime;
            characterController.Move(move);
        }
    }
}