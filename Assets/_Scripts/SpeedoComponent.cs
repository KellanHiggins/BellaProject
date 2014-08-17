using UnityEngine;
using System.Collections;

public class SpeedoComponent : MonoBehaviour 
{
	[SerializeField]
	public GameObject breathInput;
	private PEPTestDataInput testData;

	[SerializeField]
	private GameObject needle;

	[SerializeField]
	private GameObject speedoResting;

	[SerializeField]
	private GameObject speedoExhaling;

	private float needleLowerRotate;
	private float needleUpperRotate;

	private float newNeedlePosition;
	private float oldNeedlePosition;

	private float lastFrameBreath;

	private Quaternion oldNeedleRotation;
	private Quaternion newNeedleRotation;

	private float timeToRotate;


	void Awake()
	{
		testData = breathInput.GetComponent<PEPTestDataInput>();

		needleLowerRotate = -80f;
		needleUpperRotate = -150f;
		lastFrameBreath = testData.BreathStrength;
	}

	void Update () 
	{
		CheckStatusOfBreath();
		SetNeedle();
//		LerpNeedleToPosition();
		lastFrameBreath = testData.BreathStrength;
	}

	private void CheckStatusOfBreath()
	{
		if(testData.Status == BreathStatus.Exhale)
		{
			speedoResting.SetActive(false);
			speedoExhaling.SetActive(true);
		}
		if(testData.Status == BreathStatus.Inhale)
		{
			speedoResting.SetActive(true);
			speedoExhaling.SetActive(false);
		}
	}

	private void SetNeedle ()
	{
		if(testData.Status == BreathStatus.Exhale)
		{
			needle.gameObject.transform.localRotation = Quaternion.Euler(
				0f, 0f, (testData.BreathStrength / testData.PressureMaximum) * needleUpperRotate);
//			if(testData.BreathStrength != lastFrameBreath) // This is not waiting in between fires.
//			{
//				timeToRotate = .1f;
//				oldNeedleRotation = needle.gameObject.transform.localRotation;
//				newNeedleRotation = Quaternion.Euler(
//					0f, 0f, (testData.BreathStrength / testData.PressureMaximum) * needleUpperRotate);
//			}
		}
		else
			needle.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	private void LerpNeedleToPosition ()
	{
		if(timeToRotate > 0)
		{
			Debug.Log("Is this firing? " + timeToRotate);
			needle.gameObject.transform.localRotation = Quaternion.Slerp(oldNeedleRotation, newNeedleRotation, timeToRotate / .1f );
			timeToRotate -= Time.deltaTime;
		}
//		else
//		{
//			oldNeedleRotation = newNeedleRotation;
//		}
//		
	}
}













