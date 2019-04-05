using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScenario : EvaluationScenario {

	private NeuralNet brain;

	public List<double> binaryInputs = new List<double>();
	public double expectedOutput;

	protected override void onBegin() {
		brain.setWeights(new Queue<double>(generateRandomWeights(brain.extractWeights().Count)));

	}

	protected override void onTimeout() {
	}

	protected override void onUpdate() {
		setNeuralInputs();
		List<double> output = brain.calculateOutput();
		scenarioScore = 1f - System.Math.Abs(expectedOutput - output [0]);
		evaluator.reportScenarioScore (scenarioScore);

	}

	private List<double> generateRandomWeights(int length) {
		List<double> weights = new List<double>();
		for (int i = 0; i < length; i++) {
			weights.Add(Chromosome.generateRandomGene());
		}
		return weights;
	}

	private void setNeuralInputs() {
		brain.setInputs(binaryInputs);
	}
}
