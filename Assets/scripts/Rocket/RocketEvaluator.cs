using System.Collections.Generic;
using UnityEngine;

public class RocketEvaluator : AbstractEvaluator {

	public FlightController rocketPrefab;

	public List<EvaluationScenario> scenarios;

	private FlightController testSubject;

	private NeuralNet myBrain;

	private NeuralNet brain {
		get {
			if (myBrain == null)
				myBrain = EvolutionController.CreateNeuralNet(this);
			return myBrain;
		}
	}

	private double cumulativeFitness = 0;
	private int currentScenario;

	public EvolutionController evolutionator;

	public override void startEvaluation(List<double> weights, int index) {
		brain.setWeights(new Queue<double>(weights));
		createTestSubject();
		scenarios[0].startScenario(this);
		cumulativeFitness = 0;
		this.index = index;
	}

	public int getOutputsRequired() {
		return rocketPrefab.thrusters;
	}

	public override void reportScenarioScore(double score) {
		cumulativeFitness += score;
		Destroy(testSubject.gameObject);
		if (currentScenario == scenarios.Count - 1) {
			reportFitnessAndReset();
		} else {
			++currentScenario;
			createTestSubject();
			scenarios[currentScenario].startScenario(this);
		}
	}

	public FlightController getTestSubject() {
		return testSubject;
	}

	private void reportFitnessAndReset() {
		double lastFitness = cumulativeFitness;
		cumulativeFitness = 0;
		currentScenario = 0;
		Destroy(testSubject.gameObject);
		evolutionator.reportFitness(this, lastFitness, index);
	}

	private void createTestSubject() {
		testSubject = Instantiate(rocketPrefab, transform.position, transform.rotation);
		testSubject.enableAI(brain);
		testSubject.evaluator = this;
	}
}
