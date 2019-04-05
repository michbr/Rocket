﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEvolutionController : MonoBehaviour {

	public int populationSize = 50;
	public TestEvaluator simulatorPrefab;

	private List<TestEvaluator> simulators = new List<TestEvaluator>();

	private Population population;

	private int currentChromosomeIndex;
	private int chromosomesInEvaluation;
	private double totalFitness;

	private GPUNeuralNet brain;

	void Start() {
		TestEvaluator simulator = Instantiate(simulatorPrefab);
		simulator.evolutionator = this;
		simulators.Add(simulator);


		population = new Population(null, 2, populationSize, .03, .5f, .7);
		runEvaluation();
	}

	private void startNextGeneration() {
		print("total Fitness: " + totalFitness);

		population.setTotalFitness(totalFitness);
		population.spawnGeneration();

		currentChromosomeIndex = 0;
		totalFitness = 0;
		runEvaluation();
	}

	public void reportFitness(TestEvaluator evaluator, double fitness, int index) {
		totalFitness += fitness;
		--chromosomesInEvaluation;
		Chromosome chromosome = population.getChromosomes()[index];
		chromosome.setFitness(fitness);

				print("current index: " + currentChromosomeIndex);
		if (currentChromosomeIndex < populationSize - 1) {
			if (currentChromosomeIndex + chromosomesInEvaluation < populationSize - 1) {
				evaluateChromosome(currentChromosomeIndex+chromosomesInEvaluation+1, evaluator);
			}
			++currentChromosomeIndex;

		} else {
						print("restart generation");
			startNextGeneration();
		}
	}

	private void runEvaluation() {
		print ("Beginning evaluation");
		foreach (TestEvaluator evaluator in simulators) {
			evaluateChromosome(currentChromosomeIndex + chromosomesInEvaluation, evaluator);
		}
	}

	private void evaluateChromosome(int index, TestEvaluator evaluator) {
		Chromosome chromosome = population.getChromosomes()[index];
		evaluator.startEvaluation(brain, index);
		++chromosomesInEvaluation;
	}
}
