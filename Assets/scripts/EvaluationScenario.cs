using UnityEngine;

public abstract class EvaluationScenario : MonoBehaviour {

	public FlightWaypoint wayPointPrefab;
	public float scenarioTime = 10;

	protected double scenarioScore;
	protected RocketEvaluator evaluator;
	protected FlightController flightController;

	private float timer;
	private bool started;

	public void startScenario(RocketEvaluator evaluator, FlightController flightController) {
		this.evaluator = evaluator;
		this.flightController = flightController;
		timer = 0f;
		scenarioScore = 0;
		started = true;
		generateWaypoints();
		onBegin();
	}

	void Update() {
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

	protected abstract void onBegin();
	protected abstract void onTimeout();
	protected abstract void onUpdate();
}
