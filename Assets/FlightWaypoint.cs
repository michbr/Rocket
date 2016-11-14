using UnityEngine;
using System.Collections;

public class FlightWaypoint : MonoBehaviour {

	private EvolutionController controller;
	private GameObject target;
	private bool active = false;

	public void activate(EvolutionController controller) {
		this.target = controller.gameObject;
		this.controller = controller;
		active = true;
		print("activating " + name);
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject == target && active) {
			controller.waypointReached();
			active = false;
		}
	}

}
