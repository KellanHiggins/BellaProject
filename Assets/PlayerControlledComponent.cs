using UnityEngine;
using System.Collections;

public class PlayerControlledComponent : MonoBehaviour 
{

	public Ray ray;
	public RaycastHit rayHit;

	public NavMeshAgent navMeshAgent;

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
		GetSpeedNumber();


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

	private void GetSpeedNumber()
	{

	}
}
















