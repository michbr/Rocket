using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class EvolutionController : MonoBehaviour {

	public int populationSize = 50;
	public int wayPoints;
	public bool autoGeneratePath = true;

	public RocketEvaluator evaluator {
		get {
			if (myEvaluator == null) {
				myEvaluator = GetComponent<RocketEvaluator>();
			}
			return myEvaluator;
		}
	}

	private RocketEvaluator myEvaluator;

	private Population population;
	private NeuralNet net;

	private int currentChromosomeIndex = 0;
	private double totalFitness = 0;

	private bool playBest = false;

	private BestEntry currentBest;
	private List<BestEntry> bests = new List<BestEntry>();

	void Start() { 
		net = new NeuralNet(NeuronMode.NEURON, true, 3 + 3 + 4, evaluator.getOutputsRequired(), (3 + 3 + 4) * 2, 2);
		population = new Population(null, net, populationSize, .03, .3, .7);
		net.setWeights(new Queue<double>(population.getChromosomes()[currentChromosomeIndex].getWeights()));
		evaluator.startEvaluation(net);
		//if (autoGeneratePath) {
//			generateWaypoints(wayPoints, new Vector3(128, 128, 128));
		//}
//		flightController.enableAI(net);
//		reset();
	}

//	void Update() {

//		float currentDistance = Vector3.Distance(transform.position, flightPath[currentWaypoint].transform.position);
//		if (currentDistance < closestDistance) {
//			closestDistance = currentDistance;
//		}
//		if (Input.GetButtonUp("RESET")) {
//			reset();
//		}
//		if (Input.GetButtonUp("BEST")) {
//			playBest = true;
//		}
//
//			if (playBest) {
//				playBest = false;
//				net.setWeights(new Queue<double>(currentBest.getWeights()));
//				reset();
//				print("Replaying best solution. Found in generation " + currentBest.getGeneration() + " fitness: " + currentBest.getFitnessScore());
//			}
//			else if (currentChromosomeIndex < population.getChromosomes().Count - 1) {
//				evaluateFitness(population.getChromosomes()[currentChromosomeIndex]);
//				totalFitness += population.getChromosomes()[currentChromosomeIndex].getFitness();
//				++currentChromosomeIndex;
//				reset();
//			} else {
//                evaluateFitness(population.getChromosomes()[currentChromosomeIndex]);
//				totalFitness += population.getChromosomes()[currentChromosomeIndex].getFitness();
//
//				print("total Fitness: " + totalFitness);
//				population.setTotalFitness(totalFitness);
//				population.spawnGeneration();
//				resetGeneration();
//				reset();
//			}
//	}

	void OnApplicationQuit() {
		Stream stream = File.Create("RocketData.dat");
		StreamWriter writer = new StreamWriter(stream);

		// write entry data
		foreach (BestEntry entry in bests) {
			writer.Write(entry.toString() + '\n');
		}

		print("writing out rocket entries");
		// write rocket data
		writer.Flush();
		stream.Close();
	}



//	private void reset() {
//
//		currentWaypoint = 0;
//		flightPath[0].activate(this);
////		flightController.setTarget(flightPath[0].transform.position);
//
//		evaluationStartTime = Time.time;
//		net.setWeights(new Queue<double>(population.getChromosomes()[currentChromosomeIndex].getWeights()));
//		cumulativeFitness = 0;
//		closestDistance = Vector3.Distance(startPosition.position, flightPath[0].transform.position);
//	}

	private void startNextGeneration() {
		totalFitness = 0;
		currentChromosomeIndex = 0;
		population.setTotalFitness(totalFitness);
		population.spawnGeneration();

		net.setWeights(new Queue<double>(population.getChromosomes()[currentChromosomeIndex].getWeights()));
		evaluator.startEvaluation(net);
	}

	public void reportFitness(double fitness) {
//		if (currentWaypoint < flightPath.Count) {
//			chromosome.setFitness(cumulativeFitness + 1.0 / (1.0 + closestDistance));
//		} else {
//			chromosome.setFitness(cumulativeFitness);
//		}
		totalFitness += fitness;
		Chromosome chromosome = population.getChromosomes()[currentChromosomeIndex];
		chromosome.setFitness(fitness);
		if (currentBest == null || chromosome.getFitness() > currentBest.getFitnessScore()) {

			print("recording best fitness as: " + chromosome.getFitness());
			BestEntry newBest = new BestEntry(population.getCurrentGenerationNumber(), new List<double>(chromosome.getWeights()), chromosome.getFitness());
			currentBest = newBest;
			bests.Add(newBest);
		}

		if (currentChromosomeIndex < populationSize - 1) {
			++currentChromosomeIndex;
			net.setWeights(new Queue<double>(population.getChromosomes()[currentChromosomeIndex].getWeights()));
			evaluator.startEvaluation(net);
		} else {
			startNextGeneration();
		}
	}
}
