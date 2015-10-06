using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeController : MonoBehaviour {

	public Camera camera;
	public float cameraDistance; //0 is random
	public GameObject cubePrefabObject;
	public List<GameObject> spheres;

	// Use this for initialization
	void Start () {
		this.generateNewCube ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//generate a random position within certain bound for a cube to be in
	Vector3 generateRandomPosition(){
		//Z is depth here
		float z = this.camera.transform.position.z + cameraDistance;
		if (this.cameraDistance == 0) {
			z += this.randomInBetween(5f, 10f);
		}

		float x = this.randomInBetween (-10f, 10f); //between left and right walls
		float y = this.randomInBetween (0f, 5f); //between ground and viewable height

		return new Vector3 (x, y, z);
	}

	//generate a new cube in the space
	public void generateNewCube() {
		GameObject newCube = Instantiate (this.cubePrefabObject);
		newCube.GetComponent<Renderer> ().enabled = true;
		newCube.transform.position = this.generateRandomPosition();
		spheres.Add (newCube);

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
