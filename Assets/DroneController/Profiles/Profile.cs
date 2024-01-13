using System;
using UnityEngine;

namespace DroneController.Profiles
{
	[Serializable]
	public class Profile
	{
		public float maxSpeed;

		public float maxThrottleForce;

		public float maxPitchForce;

		public float maxRollForce;

		public float maxRotateForce;

		public bool angleLocked;

		public float angleLimit;

		public float dronePitchAmplifier;

		public float soundMultiplier;

		public float angularSoundMultiplier;

		public float staticSoundMultiplier;

		public float staticSoundStartPos;

		public float minDrag;

		public float maxDrag;

		public AnimationCurve speedValueCurve;

		public AnimationCurve inputThrottleCurve;

		public AnimationCurve inputPitchThrottleCurve;

		public AnimationCurve inputYawThrottleCurve;

		public AnimationCurve inputRollThrottleCurve;

		public float maxAngularDrag;

		public float minAngularDrag;

		public float angularDragZeroingTime;

		public float rotationSlowDown;
	}
}
