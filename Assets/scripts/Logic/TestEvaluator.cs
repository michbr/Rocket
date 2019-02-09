using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEvaluator : AbstractEvaluator {

	public TestEvolutionController evolutionator;
	public List<EvaluationScenario> scenarios;

	private double finalScore;
	private NeuralNet myBrain;
	private int currentScenario;


	private NeuralNet brain {
		get {
			if (myBrain == null)
				myBrain = TestEvolutionController.CreateNeuralNet(this);
			return myBrain;
		}
	}

	public override void reportScenarioScore (double score) {
		finalScore += score;
		if (currentScenario == scenarios.Count - 1) {
			reportFitnessAndReset();
		} else {
			++currentScenario;

			scenarios[currentScenario].startScenario(this);
		}
	}


	public override void startEvaluation(List<double> weights, int index) {
		brain.setWeights(new Queue<double>(weights));
		//createTestSubject();
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
