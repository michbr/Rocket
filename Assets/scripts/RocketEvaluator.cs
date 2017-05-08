using System.Collections.Generic;
using UnityEngine;

public class RocketEvaluator : MonoBehaviour {

	public FlightController rocketPrefab;

	public Transform startPos;

	public List<EvaluationScenario> scenarios;

	private FlightController testSubject;
	private NeuralNet brain;

	private double cumulativeFitness = 0;
	private int currentScenario;

	public EvolutionController evolutionator {
		get {
			if (myEvolutionator == null) {
				myEvolutionator = GetComponent<EvolutionController>();
			}
			return myEvolutionator;
		}
	}

	private EvolutionController myEvolutionator;

	public void startEvaluation(NeuralNet net) {
		brain = net;
		createTestSubject();
		scenarios[0].startScenario(this, testSubject);
		cumulativeFitness = 0;
		print("reset fitness");
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
		evolutionator.reportFitness(lastFitness);
	}

	private void createTestSubject() {
		testSubject = Instantiate(rocketPrefab, startPos.position, startPos.rotation);
		testSubject.enableAI(brain);
	}
}
