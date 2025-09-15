using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// Ball prefab to spawn
	public GameObject ballPrefab;
	// Transform child representing spawn point
	public Transform spawnPoint;
	// Force applied to the ball
	public float launchForce = 10f;
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

	private GameObject heldBall = null;
	private bool canThrow = true;
	private float cooldownTimer = 0f;
	public float throwCooldown = 1f;

	void Start()
	{
		// Lock cursor for camera control
		Cursor.lockState = CursorLockMode.Locked;
		SpawnHeldBall();
	}

	void Update()
	{
		// Handle cooldown timer
		if (!canThrow)
		{
			cooldownTimer -= Time.deltaTime;
			if (cooldownTimer <= 0f)
			{
				canThrow = true;
				SpawnHeldBall();
			}
		}

		// On left mouse click, shoot ball
		if (Input.GetMouseButtonDown(0) && canThrow && heldBall != null)
		{
			ShootBall();
		}

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

	// Shoots a ball from spawnPoint towards the raycast hit point
	void ShootBall()
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
		RaycastHit hit;
		Vector3 targetPoint;
		if (Physics.Raycast(ray, out hit))
		{
			targetPoint = hit.point;
		}
		else
		{
			// If nothing hit, shoot far forward
			targetPoint = ray.GetPoint(100f);
		}

		// Throw the held ball
		Rigidbody rb = heldBall.GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = false;
			Vector3 direction = (targetPoint - spawnPoint.position).normalized;
			rb.AddForce(direction * launchForce, ForceMode.Impulse);
		}
		heldBall.transform.SetParent(null);
		heldBall = null;
		canThrow = false;
		cooldownTimer = throwCooldown;
	}

	// Spawns a ball at the spawn point and holds it
	void SpawnHeldBall()
	{
		heldBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
		Rigidbody rb = heldBall.GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = true;
		}
		// Parent the ball to the spawn point for visual holding
		heldBall.transform.SetParent(spawnPoint);
		heldBall.transform.localPosition = Vector3.zero;
	}
}
