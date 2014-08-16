using UnityEngine;
using System.Collections;

public class PepMeasurementsComponent : MonoBehaviour {

	private PEPTestDataInput testData;

	public bool BreathsAreConsistent = false;
	public int BreathsInARow = 0;
	public bool ConstantBreath = true;
	public BreathStatus lastFrameStatus = BreathStatus.Ready;

	void Awake() 
	{
		testData = gameObject.GetComponent<PEPTestDataInput>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		TestForConsistency();

		if(lastFrameStatus == BreathStatus.Exhale && testData.Status == BreathStatus.Inhale)
		{
			Debug.Log("Breath has ended");
			if(BreathsAreConsistent == true && ConstantBreath == true)
			{
				BreathsInARow += 1;
			}
		}

		// Updates the status of the breath to see if there was a change
		lastFrameStatus = testData.Status;
	}

	private void TestForConsistency()
	{
		// For when the status changes
		if(testData.Status == BreathStatus.Exhale && lastFrameStatus == BreathStatus.Inhale)
		{
			ConstantBreath = true;
		}

		if(testData.Status == BreathStatus.Inhale && BreathsAreConsistent == false)
		{
			BreathsAreConsistent = true;
		}



		if(testData.Status == BreathStatus.Exhale)
		{
			if(testData.BreathStrength < testData.PressureMaximum && testData.BreathStrength > testData.PressureMinimum)
			{
				Debug.Log("This is good to go");
			}
			else
			{
				Debug.Log("The breath strengh is wrong");
				BreathsAreConsistent = false;
				ConstantBreath = false;
				BreathsInARow = 0;
			}
		}
	}
}
