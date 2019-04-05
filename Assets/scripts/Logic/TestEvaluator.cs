using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEvaluator : AbstractEvaluator {

	public TestEvolutionController evolutionator;
	public List<EvaluationScenario> scenarios;

	private double finalScore;
	private GPUNeuralNet brain;
	private int currentScenario;

	public override void reportScenarioScore (double score) {
		finalScore += score;
		if (currentScenario == scenarios.Count - 1) {
			reportFitnessAndReset();
		} else {
			++currentScenario;

			scenarios[currentScenario].startScenario(this);
		}
	}


	public override void startEvaluation(GPUNeuralNet brain, int index) {
		//createTestSubject();
		this.brain = brain;
		scenarios[0].startScenario(this);
		this.index = index;
	}

	private void reportFitnessAndReset() {
		evolutionator.reportFitness(this, finalScore, index);
		finalScore = 0;
	}

	public int getOutputsRequired() {
		return 1;
	}
}
