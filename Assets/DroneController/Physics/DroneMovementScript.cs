using System;
using System.Collections;
using System.Threading;
using DroneController.CameraMovement;
using DroneController.Profiles;
using UnityEngine;

namespace DroneController.Physics
{
	public class DroneMovementScript : MonoBehaviour
	{
		
		public Transform[] proppelers;

		
		public float maxSpeed;

		
		public float maxAngularDrag;

		
		public float minAngularDrag;

		
		public float angularDragZeroingTime;

		
		public float maxThrottleForce;

		
		public float maxRollForce;

		
		public float maxPitchForce;

		
		public float maxRotateForce;

		
		public bool angleLocked;

		
		public float angleLimit;

		
		public float minDrag;

		
		public float maxDrag;

		
		public AnimationCurve dragValueCurve;

		
		public float currentThrottle;

		
		public AnimationCurve inputThrottleCurve;

		
		public float currentYawThrottle;

		
		public AnimationCurve inputYawThrottleCurve;

		
		public float currentPitchThrottle;

		
		public AnimationCurve inputPitchThrottleCurve;

		
		public float currentRollThrottle;

		
		public AnimationCurve inputRollThrottleCurve;

		
		public int _profileIndex;

		
		public Profile[] profiles = new Profile[3];

		
		public int inputEditorSelection;

		[Tooltip("Check this if you want to use your joystick. (DON'T FORGET TO ADJUST THE INPUT SETTINGS!!)")]
		
		public bool joystick_turned_on;

		
		public CameraScript mainCamera;

		[Tooltip("Center Part of actual drone hierarchy.")]
		
		public Transform droneObject;

		[Tooltip("Just a reading of current velocity")]
		
		public float velocity;

		
		public float angularVelocity;

		
		public float dronePitchAmplifier = 1f;

		
		public float soundMultiplier;

		
		public float staticSoundMultiplier;

		
		public float staticSoundStartPos;

		
		public float angularSoundMultiplier;

		
		public float rotationSlowDown = 8f;

		
		public Vector3 centerOfMass;

		
		public float centerOfMassRadiusGizmo = 0.1f;

		
		public Color centerOfMassGizmoColor = Color.green;

		[Header("JOYSTICK AXIS INPUT")]
		
		public bool leftHanded;

		
		public int handedInput;

		
		public string left_analog_x = "Horizontal";

		
		public string left_analog_y = "Vertical";

		
		public string right_analog_x = "Horizontal_Right";

		
		public string right_analog_y = "Horizontal_UpDown";

		
		public KeyCode downButton = KeyCode.JoystickButton13;

		
		public KeyCode upButton = KeyCode.JoystickButton14;

		
		public JoystickDrivingAxis left_analog_y_movement = JoystickDrivingAxis.pitch;

		
		public JoystickDrivingAxis left_analog_x_movement = JoystickDrivingAxis.roll;

		
		public JoystickDrivingAxis right_analog_y_movement = JoystickDrivingAxis.throttle;

		
		public JoystickDrivingAxis right_analog_x_movement = JoystickDrivingAxis.yaw;



		[Header("INPUT TRANSLATED FOR KEYBOARD CONTROLS")]
		
		public bool W;

		
		public bool S;

		
		public bool A;

		
		public bool D;

		
		public bool I;

		
		public bool K;

		
		public bool J;

		
		public bool L;

		[Header("Keyboard Inputs")]
		
		public KeyCode forward = KeyCode.W;

		
		public KeyCode backward = KeyCode.S;

		
		public KeyCode rightward = KeyCode.D;

		
		public KeyCode leftward = KeyCode.A;

		
		public KeyCode upward = KeyCode.I;

		
		public KeyCode downward = KeyCode.K;

		
		public KeyCode rotateRightward = KeyCode.L;

		
		public KeyCode rotateLeftward = KeyCode.J;

		
		public float CustomFeed_pitch;

		
		public float CustomFeed_roll;

		
		public float CustomFeed_yaw;

		
		public float CustomFeed_throttle;

		
		public bool customFeed;

		private Vector3 throttleForce;

		private Vector3 yawThrottleForce;

		private Vector3 pitchThrottleForce;

		private Vector3 rollThrottleForce;

		private Vector3 finalAngularForce;

		private float[] proppelerSpeedPercentage = new float[4];

		private Rigidbody ourDrone;

		private AudioSource droneSound;

		private AudioSource droneStaticSound;

		private AudioSource droneAngularSound;

		private Vector3 velocityToSmoothDampToZero;

		
		public float Vertical_W;

		private float Vertical_S;

		private float Horizontal_A;

		
		public float Horizontal_D;

		
		public float Vertical_I;

		private float Vertical_K;

		private float Horizontal_J;

		
		public float Horizontal_L;

		private Vector3[] proppelerForces;

		private float[] proppelerForceBasedOnJoystickInput;

		private int maxPercentageOfForceDrop = 30;

		private Vector3 OurDroneTransformUp;

		private bool flightRecorderOverride;

		private Thread motorsFullSpeedRecovery;

		private Thread dragManagerCalculation;

		private Thread rotationCalculation;

		private float fixedDeltaTime;

		private float deltaTime;

		private float currentDrag;

		private Vector3 ourDroneTransformRight;

		private Vector3 ourDroneTransformForward;

		private Vector3 ourDroneTransformUp;

		private Vector3 ourDroneRotation;

		public Vector3[] ProppelerForces => proppelerForces;

		public float[] ProppelerForceBasedOnJoystickInput => proppelerForceBasedOnJoystickInput;

		public int MaxPercentageOfForceDrop => maxPercentageOfForceDrop;

		public bool FlightRecorderOverride
		{
			get
			{
				return flightRecorderOverride;
			}
			set
			{
				flightRecorderOverride = value;
			}
		}

		public string Left_Analog_X => left_analog_x;

		public string Right_Analog_X => right_analog_x;

		public string Left_Analog_Y => left_analog_y;

		public string Right_Analog_Y => right_analog_y;

		public bool LeftHanded
		{
			get
			{
				return leftHanded;
			}
			set
			{
				leftHanded = value;
			}
		}

		public int HandedInput
		{
			get
			{
				return handedInput;
			}
			set
			{
				handedInput = value;
			}
		}

		public Rigidbody OurDrone => ourDrone;

		private void MotorsForceLogic()
		{
			while (true)
			{
				float num = 0.5f;
				proppelerForceBasedOnJoystickInput[0] = (float)maxPercentageOfForceDrop / 100f * maxThrottleForce * (Horizontal_D - Vertical_W + Horizontal_L * num);
				proppelerForceBasedOnJoystickInput[3] = (float)maxPercentageOfForceDrop / 100f * maxThrottleForce * (Horizontal_D + Vertical_W - Horizontal_L * num);
				proppelerForceBasedOnJoystickInput[1] = (float)maxPercentageOfForceDrop / 100f * maxThrottleForce * (0f - Horizontal_D - Vertical_W - Horizontal_L * num);
				proppelerForceBasedOnJoystickInput[2] = (float)maxPercentageOfForceDrop / 100f * maxThrottleForce * (0f - Horizontal_D + Vertical_W + Horizontal_L * num);
				Vector3[] array = new Vector3[4];
				for (int i = 0; i < proppelerForces.Length; i++)
				{
					proppelerForces[i] = throttleForce * proppelerSpeedPercentage[i];
					array[i] = proppelerForces[i] + OurDroneTransformUp * proppelerForceBasedOnJoystickInput[i];
				}
				for (int j = 0; j < proppelerSpeedPercentage.Length; j++)
				{
					proppelerSpeedPercentage[j] = Mathf.Lerp(proppelerSpeedPercentage[j], 1f, fixedDeltaTime * 10f);
				}
				Thread.Sleep((int)(1f / (1f / fixedDeltaTime) * 1000f));
			}
		}

		private void DragManagerCalculation()
		{
			while (true)
			{
				float num = dragValueCurve.Evaluate(velocity / maxSpeed);
				currentDrag = maxDrag * num;
				currentDrag = Mathf.Clamp(currentDrag, minDrag, maxDrag);
				Thread.Sleep((int)(1f / (1f / deltaTime) * 1000f));
			}
		}

		private void RotationCalculation()
		{
			while (true)
			{
				if (joystick_turned_on || (!joystick_turned_on && customFeed))
				{
					currentYawThrottle = inputYawThrottleCurve.Evaluate((Horizontal_L >= 0f) ? Horizontal_L : (0f - Horizontal_L));
					currentYawThrottle *= ((Horizontal_L >= 0f) ? 1 : (-1));
					yawThrottleForce = maxRotateForce * ourDroneTransformUp * currentYawThrottle;
					currentPitchThrottle = inputPitchThrottleCurve.Evaluate((Vertical_W >= 0f) ? Vertical_W : (0f - Vertical_W));
					currentPitchThrottle *= ((Vertical_W >= 0f) ? 1 : (-1));
					pitchThrottleForce = maxPitchForce * ourDroneTransformRight * currentPitchThrottle;
					currentRollThrottle = inputRollThrottleCurve.Evaluate((Horizontal_D > 0f) ? Horizontal_D : (0f - Horizontal_D));
					currentRollThrottle *= ((Horizontal_D >= 0f) ? 1 : (-1));
					rollThrottleForce = maxRollForce * -ourDroneTransformForward * currentRollThrottle;
					if (angleLocked)
					{
						if (ourDroneRotation.x > angleLimit && ourDroneRotation.x < 180f)
						{
							if (currentPitchThrottle > 0f)
							{
								pitchThrottleForce = Vector3.zero;
							}
						}
						else if (ourDroneRotation.x < 360f - angleLimit && ourDroneRotation.x > 180f && currentPitchThrottle < 0f)
						{
							pitchThrottleForce = Vector3.zero;
						}
						if (ourDroneRotation.z > angleLimit && ourDroneRotation.z < 180f)
						{
							if (currentRollThrottle < 0f)
							{
								rollThrottleForce = Vector3.zero;
							}
						}
						else if (ourDroneRotation.z < 360f - angleLimit && ourDroneRotation.z > 180f && currentRollThrottle > 0f)
						{
							rollThrottleForce = Vector3.zero;
						}
					}
					finalAngularForce = rollThrottleForce + pitchThrottleForce + yawThrottleForce;
				}
				else if (!joystick_turned_on)
				{
					Vector3 vector = Vertical_W * maxRollForce * ourDroneTransformRight;
					Vector3 vector2 = Vertical_S * maxRollForce * ourDroneTransformRight;
					Vector3 vector3 = Horizontal_D * maxRollForce * -ourDroneTransformForward;
					Vector3 vector4 = Horizontal_A * maxRollForce * -ourDroneTransformForward;
					Vector3 vector5 = Horizontal_L * maxRotateForce * -ourDroneTransformUp;
					Vector3 vector6 = Horizontal_J * maxRotateForce * -ourDroneTransformUp;
					finalAngularForce = vector3 + vector4 + vector + vector2 + vector6 + vector5;
				}
				Thread.Sleep((int)(1f / (1f / fixedDeltaTime) * 1000f));
			}
		}

		public virtual void Awake()
		{
			ourDrone = GetComponent<Rigidbody>();
			ourDrone.maxAngularVelocity = 20f;
			StartCoroutine(FindMainCamera());
			FindDroneSoundComponent();
			for (int i = 0; i < proppelerSpeedPercentage.Length; i++)
			{
				proppelerSpeedPercentage[i] = 1f;
			}
			proppelerForces = new Vector3[4];
			proppelerForceBasedOnJoystickInput = new float[4];
		}

		public virtual void Start()
		{
		}

		private void OnEnable()
		{
			motorsFullSpeedRecovery = new Thread(MotorsForceLogic);
			dragManagerCalculation = new Thread(DragManagerCalculation);
			rotationCalculation = new Thread(RotationCalculation);
			motorsFullSpeedRecovery.Start();
			dragManagerCalculation.Start();
			rotationCalculation.Start();
		}

		private void OnDisable()
		{
			motorsFullSpeedRecovery.Abort();
			dragManagerCalculation.Abort();
			rotationCalculation.Abort();
		}

		public virtual void FixedUpdate()
		{
			fixedDeltaTime = Time.fixedDeltaTime;
			GetVelocity();
			MovementUpDown();
			PitchingRollingYawing();
			SettingControllerToInputSettings();
		}

		public virtual void Update()
		{
			deltaTime = Time.deltaTime;
			SettingCenterOffMass();
			DragManager();
			DroneSound();
			CameraCorrectPickAndTranslatingInputToWSAD();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = centerOfMassGizmoColor;
			Gizmos.DrawSphere(base.transform.position + base.transform.forward * centerOfMass.z + base.transform.right * centerOfMass.x + base.transform.up * centerOfMass.y, centerOfMassRadiusGizmo);
			if ((bool)ourDrone)
			{
				float num = 0.25f;
				Gizmos.color = ((currentPitchThrottle > 0f) ? Color.red : Color.green);
				Gizmos.DrawRay(ourDrone.transform.position, pitchThrottleForce * num);
				Gizmos.color = ((currentRollThrottle > 0f) ? Color.red : Color.green);
				Gizmos.DrawRay(ourDrone.transform.position, rollThrottleForce * num);
				Gizmos.color = ((currentYawThrottle > 0f) ? Color.red : Color.green);
				Gizmos.DrawRay(ourDrone.transform.position, yawThrottleForce * num);
				Gizmos.color = ((currentThrottle > 0f) ? Color.red : Color.green);
				Gizmos.DrawRay(ourDrone.transform.position, throttleForce * num);
				for (int i = 0; i < proppelerForces.Length; i++)
				{
					Gizmos.color = Color.magenta;
					Gizmos.DrawRay(proppelers[i].position, proppelerForces[i] * num);
					Gizmos.color = Color.blue;
					Gizmos.DrawRay(proppelers[i].position + ourDrone.transform.forward * 0.01f + proppelerForces[i] * num, ourDrone.transform.up * proppelerForceBasedOnJoystickInput[i] * num);
				}
			}
		}

		private void FindDroneSoundComponent()
		{
			try
			{
				if ((bool)base.gameObject.transform.Find("drone_sound").GetComponent<AudioSource>())
				{
					droneSound = base.gameObject.transform.Find("drone_sound").GetComponent<AudioSource>();
				}
				else
				{
					MonoBehaviour.print("Found drone_sound but it has no AudioSource component.");
				}
			}
			catch (Exception ex)
			{
				MonoBehaviour.print("No Sound Child GameObject ->" + ex.StackTrace.ToString());
			}
			try
			{
				if ((bool)base.gameObject.transform.Find("drone_angularSound").GetComponent<AudioSource>())
				{
					droneAngularSound = base.gameObject.transform.Find("drone_angularSound").GetComponent<AudioSource>();
				}
				else
				{
					MonoBehaviour.print("Found drone_sound but it has no AudioSource component.");
				}
			}
			catch (Exception ex2)
			{
				MonoBehaviour.print("No Sound Child GameObject ->" + ex2.StackTrace.ToString());
			}
			try
			{
				if ((bool)base.gameObject.transform.Find("drone_staticSound").GetComponent<AudioSource>())
				{
					droneStaticSound = base.gameObject.transform.Find("drone_staticSound").GetComponent<AudioSource>();
				}
				else
				{
					MonoBehaviour.print("Found drone_sound but it has no AudioSource component.");
				}
			}
			catch (Exception ex3)
			{
				MonoBehaviour.print("No Sound Child GameObject ->" + ex3.StackTrace.ToString());
			}
		}

		private void Left_Analog_Y_Translation()
		{
			if (left_analog_y_movement == JoystickDrivingAxis.pitch)
			{
				W = Input.GetAxisRaw(left_analog_y) > 0f;
				S = Input.GetAxisRaw(left_analog_y) < 0f;
			}
			else if (left_analog_y_movement == JoystickDrivingAxis.roll)
			{
				D = Input.GetAxisRaw(left_analog_y) > 0f;
				A = Input.GetAxisRaw(left_analog_y) < 0f;
			}
			else if (left_analog_y_movement == JoystickDrivingAxis.throttle)
			{
				I = Input.GetAxisRaw(left_analog_y) > 0f;
				K = Input.GetAxisRaw(left_analog_y) < 0f;
			}
			else if (left_analog_y_movement == JoystickDrivingAxis.yaw)
			{
				J = 0f - Input.GetAxisRaw(left_analog_y) > 0f;
				L = 0f - Input.GetAxisRaw(left_analog_y) < 0f;
			}
		}

		private void Left_Analog_X_Translation()
		{
			if (left_analog_x_movement == JoystickDrivingAxis.pitch)
			{
				W = Input.GetAxisRaw(left_analog_x) > 0f;
				S = Input.GetAxisRaw(left_analog_x) < 0f;
			}
			else if (left_analog_x_movement == JoystickDrivingAxis.roll)
			{
				D = Input.GetAxisRaw(left_analog_x) > 0f;
				A = Input.GetAxisRaw(left_analog_x) < 0f;
			}
			else if (left_analog_x_movement == JoystickDrivingAxis.throttle)
			{
				I = Input.GetAxisRaw(left_analog_x) > 0f;
				K = Input.GetAxisRaw(left_analog_x) < 0f;
			}
			else if (left_analog_x_movement == JoystickDrivingAxis.yaw)
			{
				J = 0f - Input.GetAxisRaw(left_analog_x) > 0f;
				L = 0f - Input.GetAxisRaw(left_analog_x) < 0f;
			}
		}

		private void Right_Analog_Y_Translation()
		{
			if (right_analog_y_movement == JoystickDrivingAxis.pitch)
			{
				W = 0f - Input.GetAxisRaw(right_analog_y) > 0f;
				S = 0f - Input.GetAxisRaw(right_analog_y) < 0f;
			}
			else if (right_analog_y_movement == JoystickDrivingAxis.roll)
			{
				D = Input.GetAxisRaw(right_analog_y) > 0f;
				A = Input.GetAxisRaw(right_analog_y) < 0f;
			}
			else if (right_analog_y_movement == JoystickDrivingAxis.throttle)
			{
				I = 0f - Input.GetAxisRaw(right_analog_y) > 0f;
				K = 0f - Input.GetAxisRaw(right_analog_y) < 0f;
			}
			else if (right_analog_y_movement == JoystickDrivingAxis.yaw)
			{
				J = 0f - Input.GetAxisRaw(right_analog_y) > 0f;
				L = 0f - Input.GetAxisRaw(right_analog_y) < 0f;
			}
		}

		private void Right_Analog_X_Translation()
		{
			if (right_analog_x_movement == JoystickDrivingAxis.pitch)
			{
				W = 0f - Input.GetAxisRaw(right_analog_x) > 0f;
				S = 0f - Input.GetAxisRaw(right_analog_x) < 0f;
			}
			else if (right_analog_x_movement == JoystickDrivingAxis.roll)
			{
				D = Input.GetAxisRaw(right_analog_x) > 0f;
				A = Input.GetAxisRaw(right_analog_x) < 0f;
			}
			else if (right_analog_x_movement == JoystickDrivingAxis.throttle)
			{
				I = Input.GetAxisRaw(right_analog_x) > 0f;
				K = Input.GetAxisRaw(right_analog_x) < 0f;
			}
			else if (right_analog_x_movement == JoystickDrivingAxis.yaw)
			{
				J = 0f - Input.GetAxisRaw(right_analog_x) > 0f;
				L = 0f - Input.GetAxisRaw(right_analog_x) < 0f;
			}
		}

		private void Input_Mobile_Sensitvity_Calculation()
		{
			if (W)
			{
				Vertical_W = Mathf.LerpUnclamped(Vertical_W, 1f, Time.deltaTime * 10f);
			}
			else
			{
				Vertical_W = Mathf.LerpUnclamped(Vertical_W, 0f, Time.deltaTime * 10f);
			}
			if (S)
			{
				Vertical_S = Mathf.LerpUnclamped(Vertical_S, -1f, Time.deltaTime * 10f);
			}
			else
			{
				Vertical_S = Mathf.LerpUnclamped(Vertical_S, 0f, Time.deltaTime * 10f);
			}
			if (A)
			{
				Horizontal_A = Mathf.LerpUnclamped(Horizontal_A, -1f, Time.deltaTime * 10f);
			}
			else
			{
				Horizontal_A = Mathf.LerpUnclamped(Horizontal_A, 0f, Time.deltaTime * 10f);
			}
			if (D)
			{
				Horizontal_D = Mathf.LerpUnclamped(Horizontal_D, 1f, Time.deltaTime * 10f);
			}
			else
			{
				Horizontal_D = Mathf.LerpUnclamped(Horizontal_D, 0f, Time.deltaTime * 10f);
			}
			if (I)
			{
				Vertical_I = Mathf.LerpUnclamped(Vertical_I, 1f, Time.deltaTime * 10f);
			}
			else
			{
				Vertical_I = Mathf.LerpUnclamped(Vertical_I, 0f, Time.deltaTime * 10f);
			}
			if (K)
			{
				Vertical_K = Mathf.LerpUnclamped(Vertical_K, -1f, Time.deltaTime * 10f);
			}
			else
			{
				Vertical_K = Mathf.LerpUnclamped(Vertical_K, 0f, Time.deltaTime * 10f);
			}
			if (J)
			{
				Horizontal_J = Mathf.LerpUnclamped(Horizontal_J, 1f, Time.deltaTime * 10f);
			}
			else
			{
				Horizontal_J = Mathf.LerpUnclamped(Horizontal_J, 0f, Time.deltaTime * 10f);
			}
			if (L)
			{
				Horizontal_L = Mathf.LerpUnclamped(Horizontal_L, -1f, Time.deltaTime * 10f);
			}
			else
			{
				Horizontal_L = Mathf.LerpUnclamped(Horizontal_L, 0f, Time.deltaTime * 10f);
			}
		}

		private void Joystick_Input_Sensitivity_Calculation()
		{
			Left_Analog_Y_Movement();
			Left_Analog_X_Movement();
			Right_Analog_Y_Movement();
			Right_Analog_X_Movement();
		}

		private void Left_Analog_Y_Movement()
		{
			if (left_analog_y_movement == JoystickDrivingAxis.pitch)
			{
				Vertical_W = Input.GetAxis(left_analog_y);
				Vertical_S = Input.GetAxis(left_analog_y);
				CustomFeed_pitch = Vertical_W;
			}
			else if (left_analog_y_movement == JoystickDrivingAxis.roll)
			{
				Horizontal_D = Input.GetAxis(left_analog_y);
				Horizontal_A = Input.GetAxis(left_analog_y);
				CustomFeed_roll = Horizontal_D;
			}
			else if (left_analog_y_movement == JoystickDrivingAxis.throttle)
			{
				Vertical_I = Input.GetAxis(left_analog_y);
				Vertical_K = Input.GetAxis(left_analog_y);
				CustomFeed_throttle = Vertical_I;
			}
			else if (left_analog_y_movement == JoystickDrivingAxis.yaw)
			{
				Horizontal_J = Input.GetAxis(left_analog_y);
				Horizontal_L = Input.GetAxis(left_analog_y);
				CustomFeed_yaw = Horizontal_J;
			}
		}

		private void Left_Analog_X_Movement()
		{
			if (left_analog_x_movement == JoystickDrivingAxis.pitch)
			{
				Vertical_W = Input.GetAxis(left_analog_x);
				Vertical_S = Input.GetAxis(left_analog_x);
				CustomFeed_pitch = Vertical_W;
			}
			else if (left_analog_x_movement == JoystickDrivingAxis.roll)
			{
				Horizontal_D = Input.GetAxis(left_analog_x);
				Horizontal_A = Input.GetAxis(left_analog_x);
				CustomFeed_roll = Horizontal_D;
			}
			else if (left_analog_x_movement == JoystickDrivingAxis.throttle)
			{
				Vertical_I = Input.GetAxis(left_analog_x);
				Vertical_K = Input.GetAxis(left_analog_x);
				CustomFeed_throttle = Vertical_I;
			}
			else if (left_analog_x_movement == JoystickDrivingAxis.yaw)
			{
				Horizontal_J = Input.GetAxis(left_analog_x);
				Horizontal_L = Input.GetAxis(left_analog_x);
				CustomFeed_yaw = Horizontal_J;
			}
		}

		private void Right_Analog_Y_Movement()
		{
			if (right_analog_y_movement == JoystickDrivingAxis.pitch)
			{
				Vertical_W = 0f - Input.GetAxis(right_analog_y);
				Vertical_S = 0f - Input.GetAxis(right_analog_y);
				CustomFeed_pitch = Vertical_W;
			}
			else if (right_analog_y_movement == JoystickDrivingAxis.roll)
			{
				Horizontal_D = Input.GetAxis(right_analog_y);
				Horizontal_A = Input.GetAxis(right_analog_y);
				CustomFeed_roll = Horizontal_D;
			}
			else if (right_analog_y_movement == JoystickDrivingAxis.throttle)
			{
				Vertical_I = 0f - Input.GetAxis(right_analog_y);
				Vertical_K = 0f - Input.GetAxis(right_analog_y);
				CustomFeed_throttle = Vertical_I;
			}
			else if (right_analog_y_movement == JoystickDrivingAxis.yaw)
			{
				Horizontal_J = Input.GetAxis(right_analog_y);
				Horizontal_L = Input.GetAxis(right_analog_y);
				CustomFeed_yaw = Horizontal_J;
			}
		}

		private void Right_Analog_X_Movement()
		{
			if (right_analog_x_movement == JoystickDrivingAxis.pitch)
			{
				Vertical_W = 0f - Input.GetAxis(right_analog_x);
				Vertical_S = 0f - Input.GetAxis(right_analog_x);
				CustomFeed_pitch = Vertical_W;
			}
			else if (right_analog_x_movement == JoystickDrivingAxis.roll)
			{
				Horizontal_D = Input.GetAxis(right_analog_x);
				Horizontal_A = Input.GetAxis(right_analog_x);
				CustomFeed_roll = Horizontal_D;
			}
			else if (right_analog_x_movement == JoystickDrivingAxis.throttle)
			{
				Vertical_I = Input.GetAxis(right_analog_x);
				Vertical_K = Input.GetAxis(right_analog_x);
				CustomFeed_throttle = Vertical_I;
			}
			else if (right_analog_x_movement == JoystickDrivingAxis.yaw)
			{
				Horizontal_J = Input.GetAxis(right_analog_x);
				Horizontal_L = Input.GetAxis(right_analog_x);
				CustomFeed_yaw = Horizontal_J;
			}
		}

		public void DragManager()
		{
			ourDrone.drag = currentDrag;
		}

		public void GetVelocity()
		{
			velocity = ourDrone.velocity.magnitude * 3.6f;
			angularVelocity = ourDrone.angularVelocity.magnitude;
		}

		public void SettingCenterOffMass()
		{
			ourDrone.centerOfMass = centerOfMass;
		}

		public void SettingControllerToInputSettings()
		{
			if (!customFeed)
			{
				if (!joystick_turned_on)
				{
					Input_Mobile_Sensitvity_Calculation();
				}
				else
				{
					Joystick_Input_Sensitivity_Calculation();
				}
			}
			else
			{
				CustomInputFeed();
			}
		}

		private void CustomInputFeed()
		{
			Vertical_W = CustomFeed_pitch;
			Vertical_S = CustomFeed_pitch;
			if (Vertical_W > 0f)
			{
				W = true;
			}
			else
			{
				W = false;
			}
			if (Mathf.Abs(Vertical_S) > 0f)
			{
				S = true;
			}
			else
			{
				S = false;
			}
			Horizontal_A = CustomFeed_roll;
			Horizontal_D = CustomFeed_roll;
			if (Mathf.Abs(Horizontal_A) > 0f)
			{
				A = true;
			}
			else
			{
				A = false;
			}
			if (Horizontal_D > 0f)
			{
				D = true;
			}
			else
			{
				D = false;
			}
			Vertical_I = CustomFeed_throttle;
			Vertical_K = CustomFeed_throttle;
			if (Vertical_I > 0f)
			{
				I = true;
			}
			else
			{
				I = false;
			}
			if (Mathf.Abs(Vertical_K) > 0f)
			{
				K = true;
			}
			else
			{
				K = false;
			}
			Horizontal_J = 0f - CustomFeed_yaw;
			Horizontal_L = 0f - CustomFeed_yaw;
			if (Mathf.Abs(Horizontal_J) > 0f)
			{
				J = true;
			}
			else
			{
				J = false;
			}
			if (Horizontal_L > 0f)
			{
				L = true;
			}
			else
			{
				L = false;
			}
		}

		public void CameraCorrectPickAndTranslatingInputToWSAD()
		{
			if (!customFeed && mainCamera.ourDrone.transform == base.transform)
			{
				if (!joystick_turned_on)
				{
					W = (Input.GetKey(forward) ? true : false);
					S = (Input.GetKey(backward) ? true : false);
					A = (Input.GetKey(leftward) ? true : false);
					D = (Input.GetKey(rightward) ? true : false);
					I = (Input.GetKey(upward) ? true : false);
					J = (Input.GetKey(rotateLeftward) ? true : false);
					K = (Input.GetKey(downward) ? true : false);
					L = (Input.GetKey(rotateRightward) ? true : false);
				}
				if (joystick_turned_on)
				{
					Left_Analog_Y_Translation();
					Left_Analog_X_Translation();
					Right_Analog_Y_Translation();
					Right_Analog_X_Translation();
				}
			}
		}

		public void DroneSound()
		{
			if ((bool)droneSound)
			{
				droneSound.pitch = 1f + (Mathf.Abs(Vertical_I) + Mathf.Abs(Vertical_K) + Mathf.Abs(Horizontal_J) + Mathf.Abs(Horizontal_L)) * dronePitchAmplifier;
				droneSound.volume = (Mathf.Abs(Vertical_I) + Mathf.Abs(Vertical_K) + Mathf.Abs(Horizontal_J) + Mathf.Abs(Horizontal_L)) * soundMultiplier;
			}
			if ((bool)droneAngularSound)
			{
				droneAngularSound.volume = (Mathf.Abs(Vertical_W) + Mathf.Abs(Vertical_S) + Mathf.Abs(Horizontal_A) + Mathf.Abs(Horizontal_D)) * angularSoundMultiplier;
			}
			if ((bool)droneStaticSound)
			{
				droneStaticSound.pitch = staticSoundStartPos + angularVelocity * staticSoundMultiplier;
			}
		}

		public void FlipDrone()
		{
			ourDrone.transform.eulerAngles = new Vector3(ourDrone.transform.eulerAngles.x, ourDrone.transform.eulerAngles.y + 180f, 0f);
		}

		public void MovementUpDown()
		{
			if (Vertical_I >= 0f)
			{
				currentThrottle = inputThrottleCurve.Evaluate(Vertical_I);
				throttleForce = currentThrottle * maxThrottleForce * ourDrone.transform.up;
			}
			OurDroneTransformUp = ourDrone.transform.up;
			if (!flightRecorderOverride)
			{
				for (int i = 0; i < proppelers.Length; i++)
				{
					ourDrone.AddForceAtPosition(proppelerForces[i], proppelers[i].transform.position, ForceMode.Force);
				}
			}
		}

		public void PitchingRollingYawing()
		{
			ourDroneTransformForward = ourDrone.transform.forward;
			ourDroneTransformRight = ourDrone.transform.right;
			ourDroneTransformUp = ourDrone.transform.up;
			ourDroneRotation = ourDrone.transform.rotation.eulerAngles;
			if (finalAngularForce.magnitude > 0f)
			{
				ourDrone.angularDrag = maxAngularDrag;
			}
			else
			{
				ourDrone.angularDrag = Mathf.Lerp(ourDrone.angularDrag, minAngularDrag, Time.deltaTime * angularDragZeroingTime);
			}
			if (!flightRecorderOverride)
			{
				ourDrone.angularVelocity = Vector3.Lerp(ourDrone.angularVelocity, Vector3.zero, Time.deltaTime * rotationSlowDown);
				ourDrone.angularVelocity += finalAngularForce;
			}
		}

		public void UpdateValuesFromEditor()
		{
			maxSpeed = profiles[_profileIndex].maxSpeed;
			maxThrottleForce = profiles[_profileIndex].maxThrottleForce;
			maxRollForce = profiles[_profileIndex].maxRollForce;
			maxPitchForce = profiles[_profileIndex].maxPitchForce;
			maxRotateForce = profiles[_profileIndex].maxRotateForce;
			angleLocked = profiles[_profileIndex].angleLocked;
			angleLimit = profiles[_profileIndex].angleLimit;
			dronePitchAmplifier = profiles[_profileIndex].dronePitchAmplifier;
			soundMultiplier = profiles[_profileIndex].soundMultiplier;
			angularSoundMultiplier = profiles[_profileIndex].angularSoundMultiplier;
			staticSoundMultiplier = profiles[_profileIndex].staticSoundMultiplier;
			staticSoundStartPos = profiles[_profileIndex].staticSoundStartPos;
			minDrag = profiles[_profileIndex].minDrag;
			maxDrag = profiles[_profileIndex].maxDrag;
			dragValueCurve = profiles[_profileIndex].speedValueCurve;
			inputThrottleCurve = profiles[_profileIndex].inputThrottleCurve;
			inputYawThrottleCurve = profiles[_profileIndex].inputYawThrottleCurve;
			inputPitchThrottleCurve = profiles[_profileIndex].inputPitchThrottleCurve;
			inputRollThrottleCurve = profiles[_profileIndex].inputRollThrottleCurve;
			maxAngularDrag = profiles[_profileIndex].maxAngularDrag;
			minAngularDrag = profiles[_profileIndex].minAngularDrag;
			angularDragZeroingTime = profiles[_profileIndex].angularDragZeroingTime;
			rotationSlowDown = profiles[_profileIndex].rotationSlowDown;
		}

		public void SlowdownThisProppelerSpeed(Transform _proppeler, int _thatproppelerIndex)
		{
			proppelerSpeedPercentage[_thatproppelerIndex] = 0.4f;
		}

		private IEnumerator FindMainCamera()
		{
			while (!mainCamera)
			{
				try
				{
					mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
				}
				catch (Exception ex)
				{
					MonoBehaviour.print("<color=red>Missing main camera! check the tags!</color> -> " + ex);
				}
				yield return new WaitForEndOfFrame();
			}
		}
	}
}
