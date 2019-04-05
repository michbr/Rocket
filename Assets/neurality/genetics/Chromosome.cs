using System;
using System.Collections.Generic;


public class Chromosome : System.IComparable<Chromosome> {
	private List<float> weights;
	private double fitness; 

	public Chromosome(List<float> weights) {
		this.weights = weights;
	}

	public List<float> getWeights() {
		return weights;
	}

	public void setFitness(double fitness) {
		this.fitness = fitness;
	}

	public double getFitness() {
		return fitness;
	}

	public Chromosome crossover(Chromosome other, int index) {
		if (other == this) {
			return this;
		} else {
			List<float> childGenes = new List<float>();
			for (int i = 0; i < index; ++i) {
				childGenes.Add(weights[i]);
			}

			for (int i = index; i < weights.Count; ++i) {
				childGenes.Add(other.weights[i]);
			}

			Chromosome child = new Chromosome(childGenes);
			return child;
		}
	}

	public void mutate(double mutationRate, float mutationViolence) {
		List<float> newWeights = new List<float>();
		System.Random random = GlobalRandom.getInstance().getRandom();
		foreach (float weight in weights) {
			if (random.NextDouble() < mutationRate) {
				newWeights.Add(weight + (generateRandomGene() * mutationViolence));
			} else {
				newWeights.Add(weight);
			}
		}
		weights = newWeights;
	}

	public void mutateViolently(double mutationRate) {
		List<float> newWeights = new List<float>();
		System.Random random = GlobalRandom.getInstance().getRandom();
		foreach (float weight in weights) {
			if (random.NextDouble() < mutationRate) {
				newWeights.Add(generateRandomGene());
			} else {
				newWeights.Add(weight);
			}
		}
		weights = newWeights;
	}

	public static float generateRandomGene() {
		System.Random random = GlobalRandom.getInstance().getRandom();
		return (float)random.NextDouble() - (float)random.NextDouble();
	}

	public int CompareTo(Chromosome other) {
		if (fitness < other.fitness) {
			return -1;
		} else if (fitness > other.fitness) {
			return 1;
		}
		return 0;
	}
}
