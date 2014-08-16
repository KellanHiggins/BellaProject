using UnityEngine;
using System.Collections;

public class CameraFollowComponent : MonoBehaviour {

	public GameObject Player;

	private NavMeshAgent navMeshAgent;

	[SerializeField]
	private Vector3 distanceFromPlayer;

	[SerializeField]
	private float direction;

	// Bobbing things
	private float currentBobbing = 0.0f; // keep track of y axis modification
	private float phase = Mathf.PI;
	public float bobbingFreq = 1f; // in Hz, TODO make it dependent on speed
	public float bobbingRatio = 0.05f; // as a factor of character height
	private float bobbingAmount;
	private float heightDependency;
	private float bobbingDelta; // how much has bobbing changed in the last frame

	void Awake()
	{
		navMeshAgent = Player.GetComponent<NavMeshAgent>();
	}

	// Update is called once per frame
	void Update () 
	{
		FigureOutCamBob();

		// This sets the camera to be located directly above the player at all times.

		// Add in ability to have the camera bob.

		gameObject.transform.position = Player.gameObject.transform.position + distanceFromPlayer + Vector3.up * direction;
	}

	
	void FigureOutCamBob ()
	{
		if(navMeshAgent.velocity != Vector3.zero)
		{
			direction = UpdatePhaseAndBobbingDelta(1.0f) * bobbingRatio;
		}
		else
		{
			direction = 0;
		}
	}


	private float UpdatePhaseAndBobbingDelta ( float direction )
	{
		
		float twoPi = Mathf.PI * 2;
		// bobbing happens here, add one to Sin to avoid going under the terrain
		float previousBobbing = currentBobbing;
		currentBobbing = (Mathf.Cos(phase) + 1.0f);
		bobbingDelta = currentBobbing - previousBobbing;
		// update phase in a frame-independent fashion
		phase = phase + (direction * (twoPi * Time.deltaTime * bobbingFreq));
		// wrap phase
		if (Mathf.Abs(phase) > Mathf.PI) 
		{
			phase = phase - (direction * twoPi);
//			stateMask = stateMask | isStepping;
		}
//		else
//			stateMask = stateMask & ~isStepping;

		return currentBobbing;
	}

}
