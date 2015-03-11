using UnityEngine;
using System.Collections;

public class Text3DCameraController : MonoBehaviour {
	
	void Update () {
		
		Vector3 position=transform.position;
		
		if(position.x>346f){
			return;
		}
		
		position.x+=Time.deltaTime*4f;
		
		transform.position=position;
		
		transform.Rotate(0f,-2f*Time.deltaTime,-0.2f*Time.deltaTime);
		
	}
}
