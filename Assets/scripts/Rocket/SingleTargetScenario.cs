using UnityEngine;

public class SingleTargetScenario : RocketEvaluationScenario {

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
		RocketEvaluator rocketEvaluator = (RocketEvaluator)evaluator;
		closestDistance = Vector3.Distance(rocketEvaluator.getTestSubject().transform.position, waypoint.transform.position);
		rocketEvaluator.getTestSubject().setTarget(waypoint.transform.position);
		waypoint.activate(this, rocketEvaluator.getTestSubject().gameObject);
	}

	protected override void onTimeout() {
		RocketEvaluator rocketEvaluator = (RocketEvaluator)evaluator;
		if (!closestDistanceAtEnd) {
			scenarioScore += 1.0 / (1.0 + closestDistance);
		} else {
			scenarioScore += 1.0 / (1.0 + Vector3.Distance(rocketEvaluator.getTestSubject().transform.position, waypoint.transform.position));
		}
	}

	protected override void onUpdate() {
		RocketEvaluator rocketEvaluator = (RocketEvaluator)evaluator;
		if (started) {
			float currentDistance = Vector3.Distance(rocketEvaluator.getTestSubject().transform.position, waypoint.transform.position);
			if (currentDistance < closestDistance) {
				closestDistance = currentDistance;
			}
		}
	}
}
