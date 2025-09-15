using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// Mouse sensitivity for camera movement
	public float mouseSensitivity = 100f;
	// Maximum rotation angles
	public float maxRotationX = 80f; // Up/Down
	public float maxRotationY = 80f; // Left/Right

	private float xRotation = 0f;
	private float yRotation = 0f;
	private float smoothXRotation = 0f;
	private float smoothYRotation = 0f;
	public float smoothTime = 0.1f;

	void Start()
	{
		// Lock cursor for camera control
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

	yRotation += mouseX;
	xRotation -= mouseY;

	// Clamp rotations
	xRotation = Mathf.Clamp(xRotation, -maxRotationX, maxRotationX);
	yRotation = Mathf.Clamp(yRotation, -maxRotationY, maxRotationY);

	// Smooth the rotation
	smoothXRotation = Mathf.Lerp(smoothXRotation, xRotation, 1 - Mathf.Exp(-smoothTime * Time.deltaTime * 60f));
	smoothYRotation = Mathf.Lerp(smoothYRotation, yRotation, 1 - Mathf.Exp(-smoothTime * Time.deltaTime * 60f));

	// Apply smoothed rotation to camera
	transform.localRotation = Quaternion.Euler(smoothXRotation, smoothYRotation, 0f);
	}
}
