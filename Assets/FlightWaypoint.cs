using UnityEngine;
using System.Collections;

public class FlightWaypoint : MonoBehaviour {

	private EvaluationScenario scenario;
	private GameObject target;
	private bool active = false;

	public void activate(EvaluationScenario controller) {
		this.target = controller.gameObject;
		this.scenario = controller;
		active = true;
		
		GetComponent<MeshRenderer>().material.color = Color.green;
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject == target && active) {
			scenario.waypointReached();
			active = false;
			GetComponent<MeshRenderer>().material.color = Color.red;
		}
	}
}
