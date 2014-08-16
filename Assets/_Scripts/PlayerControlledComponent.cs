using UnityEngine;
using System.Collections;

public class PlayerControlledComponent : MonoBehaviour 
{

	public Ray ray;
	public RaycastHit rayHit;

	public NavMeshAgent navMeshAgent;

	public float MaxSpeed = 5;

	[SerializeField]
	private int speedPercent = 100;

	public int SpeedPercent
	{
		get { return speedPercent; }
		set 
		{
			if(value < 0)
			{
				speedPercent = 0;
			}
			else if (value > 100)
			{
				speedPercent = 0;
			}
			else
			{
				speedPercent = value;
			}
		}
	}

	[SerializeField]
	private GameObject BreathInput;

	private PEPTestDataInput testData;

	void Start()
	{
		navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
		if(BreathInput != null)
		{
			testData = BreathInput.GetComponent<PEPTestDataInput>();
		}
		else
		{
			Debug.LogError("There is no breath input in the scene. This is bad");
		}
	}


	void Update()
	{
		GetTouchInput();
		SpeedPercent = SetSpeedNumber();
		Debug.Log("This is the speed percentage" + SpeedPercent);
		navMeshAgent.speed = MaxSpeed * ((float)speedPercent / 100);
	}

	private void GetTouchInput()
	{
		if(Input.GetButtonDown("MouseTouch1"))
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast(ray, out rayHit, 200f);
			if(rayHit.collider != null)
			{
				Debug.Log(rayHit.collider.gameObject);
				Debug.Log(rayHit.point);
				navMeshAgent.destination = rayHit.point;
			}
		}
	}

	private int tempSpeed = 100; // Not kept in the function for Garbage Collection
	
	private int SetSpeedNumber()
	{
		switch (testData.Status)
		{
		case (PEPTestDataInput.BreathStatus.Inhale):
			tempSpeed = Inhale();
			break;

		case (PEPTestDataInput.BreathStatus.Exhale):
			tempSpeed = Exhale();
			break;

		case (PEPTestDataInput.BreathStatus.Rest):
			tempSpeed = Rest();
			break;
			
		case (PEPTestDataInput.BreathStatus.Finished):
			tempSpeed = Finished();
			break;
		}

		return tempSpeed;
	}

	private int Inhale ()
	{
		// This assumes the person is breathing outside of the mask
		return 100;
	}

	// All the information for the Exhale speed portion

	private int tempExhaleSpeed = 100;

	private int Exhale ()
	{
		if(testData.BreathStrength < testData.PressureMaximum && testData.BreathStrength > testData.PressureMinimum)
		{
			tempExhaleSpeed = 100; // returns percentage of speed
		}
		else if(testData.BreathStrength > testData.PressureMaximum)
		{
			tempExhaleSpeed = 20;
		}
		else if(testData.BreathStrength < testData.PressureMinimum)
		{
			tempExhaleSpeed =  Mathf.RoundToInt((testData.BreathStrength / (float)testData.PressureMinimum) * 100f);

		}
		return tempExhaleSpeed;
	}

	private int Rest ()
	{
		// This assumes the person is breathing outside of the mask
		return 100;
	}

	private int Finished ()
	{
		return 100;
	}
}
















