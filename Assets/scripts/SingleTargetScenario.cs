using UnityEngine;

public class SingleTargetScenario : EvaluationScenario {

	public Vector3 pos;
	private float closestDistance = 0f;

	private FlightWaypoint waypoint;

	public override void generateWaypoints() {
		waypoint = Instantiate(wayPointPrefab, pos, Quaternion.identity);
	}

	public override void clearWaypoints() {
		Destroy(waypoint.gameObject);
	}

	public override void waypointReached() {
		evaluator.reportScenarioScore(1);
	}

	protected override void onBegin() {
		closestDistance = Vector3.Distance(flightController.transform.position, waypoint.transform.position);
		flightController.setTarget(waypoint.transform.position);
	}

	protected override void onTimeout() {
		scenarioScore += 1.0 / (1.0 + closestDistance);
	}

	protected override void onUpdate() {
		float currentDistance = Vector3.Distance(flightController.transform.position, waypoint.transform.position);
		if ( currentDistance < closestDistance) {
			closestDistance = currentDistance;
		}
	}
}
