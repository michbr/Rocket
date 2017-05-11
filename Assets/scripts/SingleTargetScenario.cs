using UnityEngine;

public class SingleTargetScenario : EvaluationScenario {

	public Vector3 pos;
	public bool closestDistanceAtEnd = true;
	private float closestDistance = 0f;

	private FlightWaypoint waypoint;

	public override void generateWaypoints() {
		waypoint = Instantiate(wayPointPrefab, pos, Quaternion.identity);
	}

	public override void clearWaypoints() {
		Destroy(waypoint.gameObject);
	}

	public override void waypointReached() {
		scenarioScore += .5;
	}

	protected override void onBegin() {
		closestDistance = Vector3.Distance(flightController.transform.position, waypoint.transform.position);
		flightController.setTarget(waypoint.transform.position);
		waypoint.activate(this, flightController.gameObject);
	}

	protected override void onTimeout() {
		if (!closestDistanceAtEnd) {
			scenarioScore += 1.0 / (1.0 + closestDistance);
		} else {
			scenarioScore += 1.0 / (1.0 + Vector3.Distance(flightController.transform.position, waypoint.transform.position));
		}
	}

	protected override void onUpdate() {
		if (started) {
			float currentDistance = Vector3.Distance(flightController.transform.position, waypoint.transform.position);
			if (currentDistance < closestDistance) {
				closestDistance = currentDistance;
			}
		}
	}
}
