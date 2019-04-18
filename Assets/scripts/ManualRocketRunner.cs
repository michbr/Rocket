using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRocketRunner : MonoBehaviour {

	public GameObject target;
	public FlightController rocketPrefab;
	private FlightController testSubject;

	private int inputsRequired = 10;
	private int layers = 4;
	private int layerWidth = 11;

	private GPUNeuralNet brain;
	private float[] weights;
	private float[] activationThresholds;

	// Start is called before the first frame update
	void Start(){
		int outputCount = rocketPrefab.thrusters;

		brain = GPUNeuralNet.getInstance(inputsRequired, 5, layers, layerWidth);

		testSubject = Instantiate(rocketPrefab, transform.position, transform.rotation);
		testSubject.setTarget(target.transform.position);
		testSubject.GetComponent<Rigidbody>().isKinematic = true;
		testSubject.enableAI(brain);
		activationThresholds = createPositiveArray(outputCount + (layers * layerWidth));
		weights = createArray(inputsRequired + (layerWidth * layers));
		brain.load(activationThresholds, weights);
	}

	public float[] createArray(int size) {
		float[] result = new float[size];
		for (int i = 0; i < size; ++i) {
			result[i] = generateRandomValue();
		}
		return result;
	}

	public float[] createPositiveArray(int size) {
		float[] result = new float[size];
		for (int i = 0; i < size; ++i) {
			result[i] = generateRandomPositiveValue();
		}
		return result;
	}

	public static float generateRandomValue() {
		System.Random random = GlobalRandom.getInstance().getRandom();
		return (float)random.NextDouble() - (float)random.NextDouble();
	}

	public static float generateRandomPositiveValue() {
		System.Random random = GlobalRandom.getInstance().getRandom();
		return (float)random.NextDouble();
	}
}
