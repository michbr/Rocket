using System.Collections.Generic;
using UnityEngine;

public class RocketEvaluator : MonoBehaviour {

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
	private int index;

	public EvolutionController evolutionator;

	public void startEvaluation(List<double> weights, int index) {
		brain.setWeights(new Queue<double>(weights));
		createTestSubject();
		scenarios[0].startScenario(this, testSubject);
		cumulativeFitness = 0;
		this.index = index;
	}

	public int getOutputsRequired() {
		return rocketPrefab.thrusters;
	}

	public void reportScenarioScore(double score) {
		cumulativeFitness += score;
		Destroy(testSubject.gameObject);
		if (currentScenario == scenarios.Count - 1) {
			reportFitnessAndReset();
		} else {
			++currentScenario;
			createTestSubject();
			scenarios[currentScenario].startScenario(this, testSubject);
		}
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
	}
}
