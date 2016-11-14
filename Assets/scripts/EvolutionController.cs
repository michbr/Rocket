using UnityEngine;
using System.Collections.Generic;
using System;

public class EvolutionController : MonoBehaviour, FitnessEvaluator {

	public FlightController flightController;
	public Transform startPosition;

	public float evaluationTime = 60;

	public FlightWaypoint wayPointPrefab;
	public int wayPoints;

	private float evaluationStartTime = 0;
	private Population population;
	private NeuralNet net;

	private double cumulativeFitness = 0;

	public List<FlightWaypoint> flightPath = new List<FlightWaypoint>();
	private int currentWaypoint = 0;
	private int currentChromosomeIndex = 0;
	private double totalFitness = 0;

	private Chromosome best;
	private double bestFitness = 0;
	private bool playBest = false;
	
	void Start() { 
		net = new NeuralNet(NeuronMode.NEURON, true, flightController.thrusters + 3 + 3 + 4, flightController.thrusters, (flightController.thrusters + 3 + 3 + 4) * 2, 2);
		population = new Population(this, net, 50, .03, .3, .7);
		evaluationStartTime = Time.time;
		generateWaypoints(wayPoints, new Vector3(128, 128, 128));
		flightController.setTarget(flightPath[0].transform.position);
		flightController.enableAI(net);
	}

	void Update() {
		if (Input.GetButtonUp("RESET")) {
			reset();
		}
		if (Input.GetButtonUp("BEST")) {
			playBest = true;
		}

		
		if (Time.time - evaluationStartTime > evaluationTime) {
			if (playBest) {
				playBest = false;
				net.setWeights(new Queue<double>(best.getWeights()));
				flightController.setTarget(flightPath[currentWaypoint].transform.position);
				reset();
				print("Replaying best solution, fitness: " + best.getFitness());
			}
			else if (currentChromosomeIndex < population.getChromosomes().Count - 1) {
				evaluateFitness(population.getChromosomes()[currentChromosomeIndex]);
				totalFitness += population.getChromosomes()[currentChromosomeIndex].getFitness();
				++currentChromosomeIndex;
				if (currentWaypoint < flightPath.Count) {
					flightController.setTarget(flightPath[currentWaypoint].transform.position);
				}
				net.setWeights(new Queue<double>(population.getChromosomes()[currentChromosomeIndex].getWeights()));
				reset();
			} else {
                evaluateFitness(population.getChromosomes()[currentChromosomeIndex]);
				totalFitness += population.getChromosomes()[currentChromosomeIndex].getFitness();

				print("total Fitness: " + totalFitness);
				population.setTotalFitness(totalFitness);
				population.spawnGeneration();
				resetGeneration();
				reset();
				net.setWeights(new Queue<double>(population.getChromosomes()[currentChromosomeIndex].getWeights()));


			}
		}
	}

	public void waypointReached() {
		print("Waypoint reached!!!");
		cumulativeFitness += 1.0;
		++currentWaypoint;
		if (currentWaypoint < flightPath.Count) {
			flightPath[currentWaypoint].activate(this);
		} else {
			cumulativeFitness += 1 -  ((Time.time - evaluationStartTime) / (evaluationTime));
		}
	}

	private void generateWaypoints(int number, Vector3 maxVals) {
		//if (flightPath == null || flightPath.Count < 1) {
		flightPath.Clear();
			for (int i = 0; i < number; i++) {
				System.Random random = GlobalRandom.getInstance().getRandom();
				float x = (float)random.NextDouble() * maxVals.x - (float)random.NextDouble() * maxVals.x;
				float y = (float)random.NextDouble() * maxVals.y - (float)random.NextDouble() * maxVals.y;
				float z = (float)random.NextDouble() * maxVals.z - (float)random.NextDouble() * maxVals.z;
				FlightWaypoint waypoint = GameObject.Instantiate(wayPointPrefab, new Vector3(x, y, z), Quaternion.identity) as FlightWaypoint;
				flightPath.Add(waypoint);
			}
		//}
	}

	private void clearWaypoints() {
		foreach (FlightWaypoint waypoint in flightPath) {
			Destroy(waypoint.gameObject);
		}
	}

	private void reset() {
		flightController.transform.position = startPosition.position;
		flightController.transform.rotation = Quaternion.identity;
		flightController.resetVelocity();

		currentWaypoint = 0;
		flightPath[0].activate(this);
		evaluationStartTime = Time.time;

	}

	private void resetGeneration() {
		clearWaypoints();
		generateWaypoints(wayPoints, new Vector3(64, 64, 64));
		cumulativeFitness = 0;
		totalFitness = 0;
		currentChromosomeIndex = 0;

	}

	public void evaluateFitness(Chromosome chromosome) {
		if (currentWaypoint < flightPath.Count) {
			chromosome.setFitness(cumulativeFitness + 1.0 / (1.0 + Vector3.Distance(transform.position, flightPath[currentWaypoint].transform.position)));
		} else {
			chromosome.setFitness(cumulativeFitness);
		}
		if (chromosome.getFitness() > bestFitness) {
			print("recording best fitness as: " + chromosome.getFitness());
			best = chromosome;
			bestFitness = chromosome.getFitness();
		}
	}

}
