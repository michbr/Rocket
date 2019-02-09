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

	private bool playBest = false;

	private BestEntry currentBest;
	private List<BestEntry> bests = new List<BestEntry>();

	void Start() {
		for (int i = 0; i < conncurrentSimulators; ++i) {
			RocketEvaluator simulator = Instantiate(simulatorPrefab);
			simulator.evolutionator = this;
			simulators.Add(simulator);
		}

		population = new Population(null, CreateNeuralNet(simulators[0]), populationSize, .03, .3, .7);
		runEvaluation();
	}

	public static NeuralNet CreateNeuralNet(RocketEvaluator evaluator) {
		return new NeuralNet(NeuronMode.NEURON, true, 3 + 3 + 4, evaluator.getOutputsRequired(), (3 + 3 + 4) * 2, 2);
	}

	private void startNextGeneration() {
		print("total Fitness: " + totalFitness);

		population.setTotalFitness(totalFitness);
		population.spawnGeneration();

		currentChromosomeIndex = 0;
		totalFitness = 0;
		runEvaluation();
	}

	public void reportFitness(RocketEvaluator evaluator, double fitness, int index) {
		totalFitness += fitness;
		--chromosomesInEvaluation;
		Chromosome chromosome = population.getChromosomes()[index];
		chromosome.setFitness(fitness);
		if (currentBest == null || chromosome.getFitness() > currentBest.getFitnessScore()) {

			print("recording best fitness as: " + chromosome.getFitness());
			BestEntry newBest = new BestEntry(population.getCurrentGenerationNumber(), new List<double>(chromosome.getWeights()), chromosome.getFitness());
			currentBest = newBest;
			bests.Add(newBest);
		}

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
		evaluator.startEvaluation(chromosome.getWeights(), index);
		++chromosomesInEvaluation;
	}
}
