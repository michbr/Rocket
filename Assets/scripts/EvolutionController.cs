using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class EvolutionController : MonoBehaviour, FitnessEvaluator {

	public FlightController flightController;
	public Transform startPosition;

	public float evaluationTime = 60;
	public int populationSize = 50;
	public int wayPoints;
	public bool autoGeneratePath = true;

	public FlightWaypoint wayPointPrefab;

	private float evaluationStartTime = 0;
	private Population population;
	private NeuralNet net;

	private double cumulativeFitness = 0;

	public List<FlightWaypoint> flightPath = new List<FlightWaypoint>();
	private int currentWaypoint = 0;
	private int currentChromosomeIndex = 0;
	private double totalFitness = 0;

	private bool playBest = false;

	private BestEntry currentBest;
	private List<BestEntry> bests = new List<BestEntry>();

	private float closestDistance = 0f;
	
	void Start() { 
		net = new NeuralNet(NeuronMode.NEURON, true, 3 + 3 + 4, flightController.thrusters, (3 + 3 + 4) * 2, 2);
		population = new Population(this, net, populationSize, .03, .3, .7);
		if (autoGeneratePath) {
			generateWaypoints(wayPoints, new Vector3(128, 128, 128));
		}
		flightController.enableAI(net);
		reset();
	}

	void Update() {
		float currentDistance = Vector3.Distance(transform.position, flightPath[currentWaypoint].transform.position);
		if (currentDistance < closestDistance) {
			closestDistance = currentDistance;
		}
		if (Input.GetButtonUp("RESET")) {
			reset();
		}
		if (Input.GetButtonUp("BEST")) {
			playBest = true;
		}
		
		if (Time.time - evaluationStartTime > evaluationTime) {
			if (playBest) {
				playBest = false;
				net.setWeights(new Queue<double>(currentBest.getWeights()));
				reset();
				print("Replaying best solution. Found in generation " + currentBest.getGeneration() + " fitness: " + currentBest.getFitnessScore());
			}
			else if (currentChromosomeIndex < population.getChromosomes().Count - 1) {
				evaluateFitness(population.getChromosomes()[currentChromosomeIndex]);
				totalFitness += population.getChromosomes()[currentChromosomeIndex].getFitness();
				++currentChromosomeIndex;
				reset();
			} else {
                evaluateFitness(population.getChromosomes()[currentChromosomeIndex]);
				totalFitness += population.getChromosomes()[currentChromosomeIndex].getFitness();

				print("total Fitness: " + totalFitness);
				population.setTotalFitness(totalFitness);
				population.spawnGeneration();
				resetGeneration();
				reset();
			}
		}
	}

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


	public void waypointReached() {
		print("Waypoint reached!!!");
		closestDistance = 0f;
		cumulativeFitness += 1.0;
		++currentWaypoint;
		if (currentWaypoint < flightPath.Count) {
			flightPath[currentWaypoint].activate(this);
			flightController.setTarget(flightPath[currentWaypoint].transform.position);
		} else {
			cumulativeFitness += 1 -  ((Time.time - evaluationStartTime) / (evaluationTime));
		}
	}

	private void generateWaypoints(int number, Vector3 maxVals) {
		flightPath.Clear();
		for (int i = 0; i < number; i++) {
				System.Random random = GlobalRandom.getInstance().getRandom();
				float x = (float)random.NextDouble() * maxVals.x - (float)random.NextDouble() * maxVals.x;
				float y = (float)random.NextDouble() * maxVals.y - (float)random.NextDouble() * maxVals.y;
				float z = (float)random.NextDouble() * maxVals.z - (float)random.NextDouble() * maxVals.z;
				FlightWaypoint waypoint = GameObject.Instantiate(wayPointPrefab, new Vector3(x, y, z), Quaternion.identity) as FlightWaypoint;
				flightPath.Add(waypoint);
			}
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
		flightController.setTarget(flightPath[0].transform.position);

		evaluationStartTime = Time.time;
		net.setWeights(new Queue<double>(population.getChromosomes()[currentChromosomeIndex].getWeights()));
		cumulativeFitness = 0;
		closestDistance = Vector3.Distance(startPosition.position, flightPath[0].transform.position);
	}

	private void resetGeneration() {
		if (autoGeneratePath) {
			clearWaypoints();
			generateWaypoints(wayPoints, new Vector3(64, 64, 64));
		}
		totalFitness = 0;
		currentChromosomeIndex = 0;

	}

	public void evaluateFitness(Chromosome chromosome) {
		if (currentWaypoint < flightPath.Count) {
			chromosome.setFitness(cumulativeFitness + 1.0 / (1.0 + closestDistance));
		} else {
			chromosome.setFitness(cumulativeFitness);
		}
		if (currentBest == null || chromosome.getFitness() > currentBest.getFitnessScore()) {

			print("recording best fitness as: " + chromosome.getFitness());
			BestEntry newBest = new BestEntry(population.getCurrentGenerationNumber(), new List<double>(chromosome.getWeights()), chromosome.getFitness());
			currentBest = newBest;
			bests.Add(newBest);
		}
	}
}
