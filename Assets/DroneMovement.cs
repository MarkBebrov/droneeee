using DroneController.Physics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DroneMovement : DroneMovementScript
{
	public override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
	}

	public override void Update()
	{
		base.Update();
		SceneChangeOnClick();
		FlipDroneOnClick();
	}

	private void SceneChangeOnClick()
	{
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	private void FlipDroneOnClick()
	{
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton1))
		{
			FlipDrone();
		}
	}
}
