using UnityEngine;

public abstract class EvaluationScenario : MonoBehaviour {

	public float scenarioTime = 10;

	protected double scenarioScore;
	protected AbstractEvaluator evaluator;

	protected float timer;
	protected bool started;

	public virtual void startScenario(AbstractEvaluator evaluator) {
		this.evaluator = evaluator;
		timer = 0f;
		scenarioScore = 0;
		started = true;
		onBegin();
	}

	public virtual void Update() {
		if (started) {
			onUpdate ();
		}
	}

	protected abstract void onBegin();
	protected abstract void onTimeout();
	protected abstract void onUpdate();
}
