using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RocketEvaluationScenario : EvaluationScenario {

	public FlightWaypoint wayPointPrefab;

	public override void startScenario(AbstractEvaluator evaluator) {
		generateWaypoints();
		base.startScenario (evaluator);
	}

	public override void Update() {
		if (started) {
			timer += Time.deltaTime;
			if (timer > scenarioTime) {
				onTimeout();
				evaluator.reportScenarioScore(scenarioScore);
				clearWaypoints();
				started = false;
			} else {
				onUpdate();
			}
		}
	}

	public abstract void generateWaypoints();
	public abstract void clearWaypoints();
	public abstract void waypointReached();
}
