using UnityEngine;
using System.Collections;

public class ElasticConnection : MonoBehaviour {

	const float resistance = 0.8f;

	public GameObject following;
	public bool followMovement = true;
	public bool followRotation = true;

	public float kMovement = -0.01f;
	public float kRotation = 50f;

	Vector3 initialMovementRelation;
	Vector3 velocity;
	Vector3 initialRotationRelation;


	// Use this for initialization
	void Start () {
		velocity = new Vector3(0, 0, 0);

		ResetMovementRelation();
		ResetRotationRelation();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (followMovement){
			Vector3 x = CurrentOffset - initialMovementRelation;
			velocity += kMovement * x;
			velocity *= resistance;
			transform.position += velocity;
		}

		if (followRotation){
			float step = kRotation * Time.deltaTime;
			transform.rotation = Quaternion.RotateTowards(
				transform.rotation, 
				Quaternion.Euler(following.transform.rotation.eulerAngles + initialRotationRelation), 
			step);
		}
	}

	public void ResetMovementRelation(){
		ResetMovementRelation(transform.position);
	}

	public void ResetMovementRelation(Vector3 startPos){
		initialMovementRelation = startPos - following.transform.position;
	}

	public void ResetRotationRelation(){
		ResetRotationRelation(transform.rotation.eulerAngles);
	}
	
	public void ResetRotationRelation(Vector3 startPos){
		initialRotationRelation = startPos - following.transform.rotation.eulerAngles;
	}

	public Vector3 CurrentOffset{
		get {
			return transform.position - following.transform.position;
		}
	}

	public Vector3 CurrentRotationalOffset{
		get {
			return transform.rotation.eulerAngles - following.transform.rotation.eulerAngles;
		}
	}

	public Vector3 InitialMovementRelation {
		get {
			return initialMovementRelation;
		} set {
			initialMovementRelation = value;
		}
	}
}
