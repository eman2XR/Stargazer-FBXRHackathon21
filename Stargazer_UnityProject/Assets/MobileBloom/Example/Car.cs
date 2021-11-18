using UnityEngine;

public class Car : MonoBehaviour {
	public float turnspeed = 0.17f;
	float destination;
	float direction;

	void Update(){
		if (Input.GetMouseButtonDown (0)) {
			FindDestination (Input.mousePosition);
		}

		transform.position = Vector3.Lerp(transform.position, new Vector3(destination, transform.position.y, transform.position.z), turnspeed);

		if (mod(mod(transform.position.x) - mod(destination)) < 0.15f)
		{
			direction = 0f;
		}

		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, direction * 15, direction * 30), turnspeed);
		transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, Mathf.Sin(Time.realtimeSinceStartup * 5) * 0.5f + 1f, transform.position.z), Time.deltaTime);
	}

	void FindDestination(Vector3 tPos){
		if (tPos.x > Screen.width*0.5f) {
			if (destination != 2f) {
				destination += 2f;
				direction = 1f;
			} 
		}
		else 
		{
			if (destination != -2f) {
				destination -= 2f;
				direction = -1f;
			} 
		}
	}

	float mod(float a){
		return a < 0 ? a * -1f : a;
	}

}
