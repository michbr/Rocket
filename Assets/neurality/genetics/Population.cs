using System.Collections.Generic;

public class Population  {
	private FitnessEvaluator fitnessEvaluator;

	private List<Chromosome> chromosomes = new List<Chromosome>();
	private double mutationRate;
	private double crossoverRate;
	private NeuralNet net;

	private int chromosomeLength;
	private int populationSize;
	private double totalFitness;
	private int generationCount;
	private double mutationPower;

	public Population(FitnessEvaluator fitnessEvaluator, NeuralNet net, int populationSize, double mutationRate, double mutationPower, double crossoverRate) {
		this.fitnessEvaluator = fitnessEvaluator;
		this.mutationRate = mutationRate;
		this.crossoverRate = crossoverRate;
		this.populationSize = populationSize;
		this.mutationPower = mutationPower;
		chromosomeLength = net.extractWeights().Count;
		this.net = net;
		populate();
	}

	public void run(int generations) {
		for (int i = 0; i < generations; ++i) {
			runGeneration();
		}
	}

	public void run(double targetFitness) {
		while (getBest().getFitness() < targetFitness) {
			runGeneration();
		}
	}

	public List<Chromosome> getChromosomes() {
		return chromosomes;
	}

	public void runGeneration() {
		evaluatePopulationFitness();
		spawnGeneration();
	}

	public void setTotalFitness(double fitness) {
		totalFitness = fitness;
	}

	public void spawnGeneration() {
		chromosomes.Sort();
		++generationCount;
		List<Chromosome> newPopulation = new List<Chromosome>();
		while (newPopulation.Count < populationSize) {
			Chromosome parent1 = selectRandomChromosome();
			Chromosome parent2 = selectRandomChromosome();
			crossover(parent1, parent2, newPopulation);
		}
		chromosomes = newPopulation;
	}

	public int getCurrentGenerationNumber() {
		return generationCount;
	}

	private void evaluatePopulationFitness() {
		totalFitness = 0;
		foreach (Chromosome chromosome in chromosomes) {
			fitnessEvaluator.evaluateFitness(chromosome);
			totalFitness += chromosome.getFitness();
		}
	}

	private void crossover(Chromosome a, Chromosome b, List<Chromosome> population) {
		System.Random random = GlobalRandom.getInstance().getRandom();
		if (random.NextDouble() > crossoverRate) {
			a.mutate(mutationRate, mutationPower);
			b.mutate(mutationRate, mutationPower);
			population.Add(a);
			population.Add(b);
		} else {
			int crossoverIndex = random.Next(chromosomeLength - 1);

			Chromosome child1 = a.crossover(b, crossoverIndex);
			Chromosome child2 = b.crossover(a, crossoverIndex);

			child1.mutate(mutationRate, mutationPower);
			child2.mutate(mutationRate, mutationPower);
			population.Add(child1);
			population.Add(child2);
		}
	}

	private void populate() {
		for (int i = 0; i < populationSize; ++i) {
			chromosomes.Add(generateRandomChromosome());
		}
	}

	private Chromosome selectRandomChromosome() {
		System.Random random = GlobalRandom.getInstance().getRandom();
		double slice = totalFitness * random.NextDouble();

		double accumulatedFitness = 0;
		foreach (Chromosome chromosome in chromosomes) {
			accumulatedFitness += chromosome.getFitness();

			if (accumulatedFitness >= slice) {
				return chromosome;
			}
		}

		return chromosomes[chromosomes.Count - 1];
	}

	private Chromosome generateRandomChromosome() {
		List<double> weights = new List<double>();
		for (int i = 0; i < chromosomeLength; i++) {
			weights.Add(Chromosome.generateRandomGene());
		}
		Chromosome result = new Chromosome(weights);
		return result;
	}

	private Chromosome getBest() {
		evaluatePopulationFitness();
		return chromosomes[chromosomes.Count - 1];
	}
}
