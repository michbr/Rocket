using System.Collections.Generic;
using UnityEngine;

public class RandomPathScenario : RocketEvaluationScenario {

	public int pathSize;
	public Vector3 fieldSize;

	private List<FlightWaypoint> flightPath = new List<FlightWaypoint>();
	private float closestDistance = 0f;
	private int currentWaypoint = 0;

	public override void generateWaypoints() {
		flightPath.Clear();
		for (int i = 0; i < pathSize; i++) {
			System.Random random = GlobalRandom.getInstance().getRandom();
			float x = (float)random.NextDouble() * fieldSize.x - (float)random.NextDouble() * fieldSize.x;
			float y = (float)random.NextDouble() * fieldSize.y - (float)random.NextDouble() * fieldSize.y;
			float z = (float)random.NextDouble() * fieldSize.z - (float)random.NextDouble() * fieldSize.z;
			FlightWaypoint waypoint = Instantiate(wayPointPrefab, new Vector3(x, y, z), Quaternion.identity);
			flightPath.Add(waypoint);
		}
	}

	public override void clearWaypoints() {
		foreach (FlightWaypoint waypoint in flightPath) {
			Destroy(waypoint.gameObject);
		}
	}

	public override void waypointReached() {
		RocketEvaluator rocketEvaluator = (RocketEvaluator)evaluator;
		print("Waypoint reached!!!");
		timer -= 10;
		closestDistance = 0f;
		scenarioScore += 1.0;
		++currentWaypoint;
		if (currentWaypoint < flightPath.Count) {
			flightPath[currentWaypoint].activate(this, rocketEvaluator.getTestSubject().gameObject);
			rocketEvaluator.getTestSubject().setTarget(flightPath[currentWaypoint].transform.position);
		} else {
			evaluator.reportScenarioScore(scenarioScore);
			//started = false;
			print("setting started to false");
		}
	}

	protected override void onBegin() {
		RocketEvaluator rocketEvaluator = (RocketEvaluator)evaluator;
		currentWaypoint = 0;
		closestDistance = Vector3.Distance(rocketEvaluator.getTestSubject().transform.position, flightPath[0].transform.position);
		rocketEvaluator.getTestSubject().setTarget(flightPath[0].transform.position);
		flightPath[0].activate(this, rocketEvaluator.getTestSubject().gameObject);
	}

	protected override void onTimeout() {
		scenarioScore += 1.0 / (1.0 + closestDistance);
	}

	protected override void onUpdate() {
		RocketEvaluator rocketEvaluator = (RocketEvaluator)evaluator;
		if (started) {
			float currentDistance = Vector3.Distance(rocketEvaluator.getTestSubject().transform.position,
				flightPath[currentWaypoint].transform.position);
			if (currentDistance < closestDistance) {
				closestDistance = currentDistance;
			}
		}
	}
}
