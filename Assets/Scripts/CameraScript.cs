using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform viewPoint;
    [Header("Mouse variables and constraints")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float maxYAxisValue = 60f;
    
    private InputHandler inputHandler;
    private float verticalRotScore;

    private Camera camera;
    public Camera Camera
    { 
        get { return camera; } 
    }
    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        camera = Camera.main;
    }
    private void Update()
    {
        HandleRotation();
    }

    private void HandleRotation()
    {
        Vector2 input = mouseSensitivity * inputHandler.MouseInput;

        Vector3 newRotation = new (transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y + input.x, transform.rotation.eulerAngles.z);
        transform.rotation = Quaternion.Euler(newRotation);

        verticalRotScore -= input.y;
        verticalRotScore = Mathf.Clamp(verticalRotScore, -maxYAxisValue, maxYAxisValue);

        viewPoint.rotation = Quaternion.Euler(verticalRotScore,
            viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
    }
    private void LateUpdate()
    {
        camera.transform.SetPositionAndRotation(viewPoint.transform.position, viewPoint.transform.rotation);
    }
}
