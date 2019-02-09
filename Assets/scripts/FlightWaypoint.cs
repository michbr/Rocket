using UnityEngine;

public class FlightWaypoint : MonoBehaviour {

	public float waypointSize = 1f;

	private RocketEvaluationScenario scenario;
	private GameObject target;
	private bool active = false;

	public void activate(EvaluationScenario controller, GameObject target) {
		this.target = target;
		scenario = (RocketEvaluationScenario)controller;
		active = true;
		
		GetComponent<MeshRenderer>().material.color = Color.green;
	}

	void Update() {
		if (active && target != null && Vector3.Distance(transform.position, target.transform.position) < waypointSize) {
			scenario.waypointReached();
			active = false;
			GetComponent<MeshRenderer>().material.color = Color.blue;
		}
	}
}
