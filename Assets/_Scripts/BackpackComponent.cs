using UnityEngine;
using System.Collections;

public class BackpackComponent : MonoBehaviour {

	public int RingsHeld;

	[SerializeField]
	public float timeToPickUp;
	public bool canPickUp;

	// Update is called once per frame
	void Update () 
	{
		if(timeToPickUp > 0)
		{
			timeToPickUp -= Time.deltaTime;
			canPickUp = false;
		}
		else
		{
			canPickUp = true;
		}
	}

	void OnCollisionEnter(Collision other)
	{
		if(canPickUp)
		{
			Destroy(other.collider.gameObject);
			RingsHeld += 1;
		}
	}
}
