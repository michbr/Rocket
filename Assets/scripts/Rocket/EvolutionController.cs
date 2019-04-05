using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class EvolutionController : MonoBehaviour {

	public int populationSize = 50;
	public int conncurrentSimulators = 1;
	public RocketEvaluator simulatorPrefab;

	private List<RocketEvaluator> simulators = new List<RocketEvaluator>();

	private Population population;

	private int currentChromosomeIndex;
	private int chromosomesInEvaluation;
	private double totalFitness;
	private double bestFitness = 0;

	private BestEntry currentBest;
	private List<BestEntry> bests = new List<BestEntry>();

	private int inputsRequired = 10;
	private int layers = 40;
	private int layerWidth = 11;

	private GPUNeuralNet brain;

	private int currentGeneration = 0;

	void Start() {
		for (int i = 0; i < conncurrentSimulators; ++i) {
			RocketEvaluator simulator = Instantiate(simulatorPrefab);
			simulator.evolutionator = this;
			simulators.Add(simulator);
		}

		int outputCount = simulators[0].getOutputsRequired();
		brain = GPUNeuralNet.getInstance(inputsRequired, outputCount, layers, layerWidth);
		population = new Population(null, inputsRequired + outputCount + (layerWidth*layers*2), populationSize, .03, .3f, .7);
		runEvaluation();
	}

	//public static GPUNeuralNet CreateNeuralNet(RocketEvaluator evaluator) {
		//return new NeuralNet(NeuronMode.NEURON, true, 3 + 3 + 4, evaluator.getOutputsRequired(), (3 + 3 + 4) * 2, 2);
		//return GPUNeuralNet.getInstance(evaluator.);
	//}

	private void startNextGeneration() {
		print("Generation "+currentGeneration+" total Fitness: " + totalFitness);
		print("Best: " + bestFitness);
		bestFitness = 0;

		++currentGeneration;
		population.setTotalFitness(totalFitness);
		population.spawnGeneration();

		currentChromosomeIndex = 0;
		totalFitness = 0;
		runEvaluation();
	}

	public void reportFitness(RocketEvaluator evaluator, double fitness, int index) {
		totalFitness += fitness;
		if (fitness > bestFitness) {
			bestFitness = fitness;
		}
		--chromosomesInEvaluation;
		Chromosome chromosome = population.getChromosomes()[index];
		chromosome.setFitness(fitness);

//		print("current index: " + currentChromosomeIndex);
		if (currentChromosomeIndex < populationSize - 1) {
			if (currentChromosomeIndex + chromosomesInEvaluation < populationSize - 1) {
				evaluateChromosome(currentChromosomeIndex+chromosomesInEvaluation+1, evaluator);
			}
			++currentChromosomeIndex;

		} else {
//			print("restart generation");
			startNextGeneration();
		}
	}

	private void runEvaluation() {
		foreach (RocketEvaluator evaluator in simulators) {
			evaluateChromosome(currentChromosomeIndex + chromosomesInEvaluation, evaluator);
		}
	}

	private void evaluateChromosome(int index, RocketEvaluator evaluator) {
//		print("evaluate chromosome " + index);
		Chromosome chromosome = population.getChromosomes()[index];
		List<float>  weights = chromosome.getWeights();
		int activationThresholdsCount = evaluator.getOutputsRequired() + (layerWidth * layers);
		int weightsCount = inputsRequired + (layerWidth * layers);
		brain.load(weights.GetRange(weightsCount, activationThresholdsCount).ToArray(), weights.GetRange(0, weightsCount).ToArray());
		evaluator.startEvaluation(brain, index);
		++chromosomesInEvaluation;
	}
}
