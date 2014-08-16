using UnityEngine;
using System.Collections;

public class CameraFollowComponent : MonoBehaviour {

	public GameObject Player;

	[SerializeField]
	private Vector3 distanceFromPlayer;

	// Update is called once per frame
	void Update () 
	{
		gameObject.transform.position = Player.gameObject.transform.position + distanceFromPlayer;
	}
}
