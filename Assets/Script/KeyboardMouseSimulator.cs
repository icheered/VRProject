using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMouseSimulator : MonoBehaviour
{
    public bool hideHandModels = true;
    public GameObject RightHand;
    public GameObject LeftHand;
    public GameObject rightHandModel;
    public GameObject leftHandModel;
    public GameObject UICanvas;

    public float speed = 5.0f;    // Movement speed
    public float sensitivity = 2.0f; // Mouse sensitivity

    private Vector3 movement;
    private CharacterController characterController;
    private float yaw = 0.0f;     // Horizontal rotation for the character
    private float pitch = 0.0f;    // Vertical rotation for the character
    private float handYaw = 0.0f;  // Horizontal rotation for the hand
    private float handPitch = 0.0f; // Vertical rotation for the hand

    private bool leftHandSelected = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        if (hideHandModels)
        {
            rightHandModel.SetActive(false);
            leftHandModel.SetActive(false);
        }
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Update the rotation of the character based on the mouse movement
        yaw += sensitivity * Input.GetAxis("Mouse X");
        pitch -= sensitivity * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        // Apply the rotation to the character
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);


        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Toggle active state of UICanvas
            UICanvas.SetActive(!UICanvas.activeSelf);
        }

        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        movement = transform.TransformDirection(movement);
        movement *= speed * Time.deltaTime;

        characterController.Move(movement);
    }
}
