using UnityEngine;

namespace DroneController.Propelers
{
	public class ElisaScript : MonoBehaviour
	{
		[Tooltip("Propellers from your drone. Assing cross propellers. X ")]
		public GameObject[] elisa;

		[Tooltip("How fast propellers will rotate when they are idle.")]
		public float idleRotationSpeed = 1000f;

		[Tooltip("How fast propellers will rotate when we are moving.")]
		public float movingRotationSpeed = 2000f;

		[Tooltip("Fixing propellers rotation if somethign went wrong during import from Blender or 3DsMax.")]
		public float elisaAngle;

		public bool spinDifference = true;

		[HideInInspector]
		public float atWhatSpeedsShowWingtipVortices = 20f;

		private float currentYRotation;

		private float rotationSpeed = 1000f;

		private int amountOfWingtipVorticesOnElisas;

		private ParticleSystem[] wingtipVortices;

		private float wantedAlpha;

		private float currentAlpha;

		private DroneMovement droneMovementScript;

		public virtual void Awake()
		{
			droneMovementScript = GetComponent<DroneMovement>();
		}

		public void RotationInputs()
		{
			if (Input.GetKey(droneMovementScript.forward) || Input.GetKey(droneMovementScript.downward) || Input.GetKey(droneMovementScript.downButton) || Input.GetKey(droneMovementScript.leftward) || Input.GetKey(droneMovementScript.rightward) || Input.GetKey(droneMovementScript.upward) || Input.GetKey(droneMovementScript.downButton) || Input.GetKey(droneMovementScript.downward) || droneMovementScript.Vertical_W != 0f || droneMovementScript.Horizontal_D != 0f || droneMovementScript.Vertical_I != 0f || droneMovementScript.Horizontal_L != 0f)
			{
				rotationSpeed = movingRotationSpeed;
			}
			else
			{
				rotationSpeed = idleRotationSpeed;
			}
		}

		public void RotationDifferentials()
		{
			currentYRotation += Time.deltaTime * rotationSpeed;
			for (int i = 0; i < elisa.Length; i++)
			{
				if (spinDifference)
				{
					if (i % 2 == 0)
					{
						elisa[i].transform.localRotation = Quaternion.Euler(new Vector3(elisaAngle, currentYRotation, base.transform.rotation.z));
					}
					else
					{
						elisa[i].transform.localRotation = Quaternion.Euler(new Vector3(elisaAngle, 0f - currentYRotation, base.transform.rotation.z));
					}
				}
				else
				{
					elisa[i].transform.localRotation = Quaternion.Euler(new Vector3(elisaAngle, currentYRotation, base.transform.rotation.z));
				}
			}
		}

		private void LocateWintipParticles()
		{
			amountOfWingtipVorticesOnElisas = 0;
			for (int i = 0; i < elisa.Length; i++)
			{
				if ((bool)elisa[i].GetComponent<ParticleSystem>())
				{
					amountOfWingtipVorticesOnElisas++;
				}
			}
			wingtipVortices = new ParticleSystem[amountOfWingtipVorticesOnElisas];
			for (int j = 0; j < amountOfWingtipVorticesOnElisas; j++)
			{
				wingtipVortices[j] = elisa[j].GetComponent<ParticleSystem>();
			}
		}
	}
}
