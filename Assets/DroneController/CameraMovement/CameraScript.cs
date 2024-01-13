using System;
using System.Collections;
using DroneController.Physics;
using UnityEngine;

namespace DroneController.CameraMovement
{
	public class CameraScript : MonoBehaviour
	{
		public int inputEditorFPS;

		public bool FPS;

		public Vector3 positionInsideDrone;

		public Vector3 rotationInsideDrone;

		public float fpsFieldOfView = 90f;

		public GameObject ourDrone;

		[Header("Position of the camera behind the drone.")]
		public Vector3 positionBehindDrone = new Vector3(0f, 2f, -4f);

		[Tooltip("How fast the camera will follow drone position. (The lower the value the faster it will follow)")]
		[Range(0f, 0.1f)]
		public float cameraFollowPositionTime = 0.1f;

		[Tooltip("Value where if the camera/drone is moving upwards will raise the camera view upward to get a better look at what is above, same goes when going downwards.")]
		public float extraTilt = 10f;

		[Tooltip("Parts of drone we wish to see in the third person.")]
		public float tpsFieldOfView = 60f;

		[Header("Mouse movement variables")]
		[Tooltip("Allows to freely view around the drone with your mouse and not depending on drone look rotation.")]
		public bool freeMouseMovement;

		[Tooltip("Value that will determine how fast your free look mouse will behave.")]
		public float mouseSensitvity = 100f;

		[Tooltip("Value that will follow the camera view behind the mouse movement.(The lower the value, the faster it will follow mouse movement)")]
		public float mouseFollowTime = 0.2f;

		private Vector3 velocitiCameraFollow;

		private float cameraYVelocity;

		private float previousFramePos;

		private float currentXPos;

		private float currentYPos;

		private float xVelocity;

		private float yVelocity;

		private float mouseXwanted;

		private float mouseYwanted;

		private float zScrollAmountSensitivity = 1f;

		private float yScrollAmountSensitivity = -0.5f;

		private float zScrollValue;

		private float yScrollValue;

		public virtual void Start()
		{
			StartCoroutine(KeepTryingToFindOurDrone());
		}

		public virtual void Awake()
		{
		}

		public virtual void Update()
		{
			GetComponent<Camera>().fieldOfView = fpsFieldOfView;
		}

		public virtual void FixedUpdate()
		{
			FPVTPSCamera();
			ScrollMath();
		}

		private void FPSCameraPositioning()
		{
			if (base.transform.parent == null)
			{
				base.transform.SetParent(ourDrone.GetComponent<DroneMovementScript>().droneObject.transform);
			}
			base.transform.localPosition = positionInsideDrone;
			base.transform.localEulerAngles = rotationInsideDrone;
		}

		private void TPSCameraPositioning()
		{
			if (base.transform.parent != null)
			{
				base.transform.SetParent(null);
			}
			FollowDroneMethod();
		}

		private void FollowDroneMethod()
		{
			if ((bool)ourDrone)
			{
				base.transform.position = Vector3.SmoothDamp(base.transform.position, ourDrone.transform.position + (positionBehindDrone + new Vector3(0f, yScrollValue, zScrollValue)), ref velocitiCameraFollow, cameraFollowPositionTime);
				base.transform.rotation = Quaternion.Euler(base.transform.eulerAngles.x, 0f, base.transform.eulerAngles.z);
			}
		}

		private void TiltCameraUpDown()
		{
			cameraYVelocity = Mathf.Lerp(cameraYVelocity, (base.transform.position.y - previousFramePos) * (0f - extraTilt), Time.deltaTime * 10f);
			previousFramePos = base.transform.position.y;
		}

		private void FreeMouseMovementView()
		{
			if (freeMouseMovement)
			{
				mouseXwanted -= Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitvity;
				mouseYwanted += Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitvity;
				currentXPos = Mathf.SmoothDamp(currentXPos, mouseXwanted, ref xVelocity, mouseFollowTime);
				currentYPos = Mathf.SmoothDamp(currentYPos, mouseYwanted, ref yVelocity, mouseFollowTime);
				base.transform.rotation = Quaternion.Euler(new Vector3(14f, 0f, 0f)) * Quaternion.Euler(currentXPos, currentYPos, 0f);
			}
			else if ((bool)ourDrone)
			{
				base.transform.rotation = Quaternion.Euler(new Vector3(14f + cameraYVelocity, 0f, 0f));
			}
		}

		public void FPVTPSCamera()
		{
			if (FPS)
			{
				FPSCameraPositioning();
			}
			else
			{
				TPSCameraPositioning();
			}
		}

		public void ScrollMath()
		{
			if (Input.GetAxis("Mouse ScrollWheel") != 0f)
			{
				zScrollValue += Input.GetAxis("Mouse ScrollWheel") * zScrollAmountSensitivity;
				yScrollValue += Input.GetAxis("Mouse ScrollWheel") * yScrollAmountSensitivity;
			}
		}

		private IEnumerator KeepTryingToFindOurDrone()
		{
			while (ourDrone == null)
			{
				try
				{
					DroneMovement[] array = UnityEngine.Object.FindObjectsOfType<DroneMovement>();
					DroneMovement[] array2 = array;
					foreach (DroneMovement droneMovement in array2)
					{
						if (!droneMovement.FlightRecorderOverride)
						{
							ourDrone = droneMovement.gameObject;
						}
					}
					if (!ourDrone)
					{
						ourDrone = array[UnityEngine.Random.Range(0, array.Length - 1)].gameObject;
					}
				}
				catch (Exception ex)
				{
					MonoBehaviour.print("Are you supposed to have only one drone on the scene? <color=red>I can't find it!</color> -> " + ex);
				}
				yield return null;
			}
		}
	}
}
