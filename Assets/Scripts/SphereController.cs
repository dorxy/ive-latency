using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SphereController : MonoBehaviour {

	public Camera camera;
	public float cameraDistance; //0 is random
	public GameObject spherePrefabObject;
	public List<GameObject> spheres;

	// Use this for initialization
	void Start () {
		this.generateNewSphere ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//generate a random position within certain bound for a sphere to be in
	Vector3 generateRandomPosition(){
		//Z is depth here
		Vector3 point = new Vector3 (0, 0, 0);
		do {
			float z = this.camera.transform.position.z + cameraDistance;
			if (this.cameraDistance == 0) {
				z += this.randomInBetween (5f, 10f);
			}

			float x = this.randomInBetween (-10f, 10f); //between left and right walls
			float y = this.randomInBetween (0.5f, 5f); //between ground and viewable height

			point = new Vector3 (x, y, z);
		} while(!pointInViewport(point));

		return point;
	}

	bool pointInViewport(Vector3 point){
		Vector3 viewPortPoint = this.camera.WorldToViewportPoint(point);
		return viewPortPoint.x <= 1 && viewPortPoint.x >= 0 && viewPortPoint.y <= 1 && viewPortPoint.y >= 0;
	}

	//generate a new sphere in the space
	public void generateNewSphere() {
		GameObject newSphere = Instantiate (this.spherePrefabObject);
		newSphere.GetComponent<Renderer> ().enabled = true;
		newSphere.transform.position = this.generateRandomPosition();
		spheres.Add (newSphere);
	}

	float randomInBetween(float from, float to){
		return (Random.value * (to - from)) + from;
	}

	void OnGUI() {
		Event e = Event.current;
		
		if (e.isKey && e.type == EventType.keyDown) {
			switch (e.keyCode) {
			case KeyCode.S:
				if (this.cameraDistance == 0) {
					this.cameraDistance = 7.5f;
				} else {
					this.cameraDistance = 0;
				}
				break;
			}
		}
	}
}
