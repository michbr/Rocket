using UnityEngine;
using System.Collections.Generic;

public class BestEntry {
	int generation;
	List<double> weights;
	double fitnessScore;

	public BestEntry(int generation, List<double> weights, double fitnessScore) {
		this.generation = generation;
		this.weights = weights;
		this.fitnessScore = fitnessScore;
	} 

	public int getGeneration() {
		return generation;
	}

	public List<double> getWeights() {
		return weights;
	}

	public double getFitnessScore() {
		return fitnessScore; 
	}

	public string toString() {
		string data = generation + ": " + fitnessScore + "-> ";
		foreach (double weight in weights) {
			data += weight + " ";
		}
        return data;
	}
}
