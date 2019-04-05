using System;
using UnityEngine;
using System.Collections.Generic;

public class FlightController : MonoBehaviour {

	public List<ThrusterController> thrusterControllers = new List<ThrusterController>();
	public int thrusters;
	public Transform centerPoint;
	public GameObject ThrusterPrefab;
	public RocketEvaluator evaluator;

	public bool binaryControl = false;
	
	private GPUNeuralNet brain;
	private bool AIEnablied = false;
	private Rigidbody rb;
	public Vector3 targetPos;

	void Start() {
		rb = GetComponent<Rigidbody>();
		thrusterControllers.AddRange(GetComponentsInChildren<ThrusterController>());
		if (thrusterControllers.Count == 0) {
			addThrusters();
		}
	}

	void Update() {


		if (AIEnablied) {
			float[] output = calculateThrusterSettings();
			for(int i = 0; i < output.Length; ++i) {
				float newThrottle = output[i] < 0 ? 0 : output[i];
				if (newThrottle > 1.0f) {
					newThrottle = 1.0f;
				}
				//print(i + ": " + newThrottle);
				thrusterControllers[i].throttle = newThrottle;
			}
		} else {
//			if (Input.GetButton("Jump")) {
//				foreach (ThrusterController thrusterController in thrusterControllers) {
//					thrusterController.firing = true;
//				}
//			} else {
//				foreach (ThrusterController thrusterController in thrusterControllers) {
//					thrusterController.firing = false;
//				}
//			}
		}
	} 

	public void setTarget(Vector3 targetPos) {
		this.targetPos = targetPos;
	}

	public void enableAI(GPUNeuralNet brain) {
		this.brain = brain;
		AIEnablied = true;
		foreach (ThrusterController thruster in thrusterControllers) {
			thruster.firing = true;
		}
	}

	public void resetVelocity() {
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		foreach (ThrusterController thrusterController in thrusterControllers) {
			thrusterController.throttle = 0;
		}
		rb.Sleep();
		//print("reset velocity: " + rb.velocity);
	}

	private float[] calculateThrusterSettings() {
		List<float> currentThrusterSettings = new List<float>();
		//print("setting velocity to: " + rb.velocity);
		currentThrusterSettings.Add(rb.velocity.x);
		currentThrusterSettings.Add(rb.velocity.y);
		currentThrusterSettings.Add(rb.velocity.z);

		Vector3 direction = getDirectionVector();
		Debug.DrawLine(transform.position, transform.position + direction, Color.red);

		//print("setting direction to: " + direction);
		currentThrusterSettings.Add(direction.x);
		currentThrusterSettings.Add(direction.y);
		currentThrusterSettings.Add(direction.z);

		//print("setting rotation to: " + transform.rotation);
		currentThrusterSettings.Add(transform.rotation.x);
		currentThrusterSettings.Add(transform.rotation.y);
		currentThrusterSettings.Add(transform.rotation.z);
		currentThrusterSettings.Add(transform.rotation.w);

		//foreach (ThrusterController thruster in thrusterControllers) {
		//print("add thruster value: " + thruster.throttle);
		//currentThrusterSettings.Add(thruster.throttle);
		//}
		return brain.run(currentThrusterSettings.ToArray());
	}

	private Vector3 getDirectionVector() {
		return (targetPos - transform.position).normalized;
	}

	private void addThrusters() {
		float radius = .2f;
		float y_offset = -1.2f;

		for (int i = 0; i < thrusters; i++) {
			float angle =  (Mathf.PI*2 / (float)(thrusters)) * (float)i;
			//print("Creating thruster at angle: " + angle);
			if (angle > 2 * Mathf.PI) {
				break;
			}
			//print("x, z coords: (" + Mathf.Cos(angle) + ", " + Mathf.Sin(angle) + ")");
			Vector3 radiusVector = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
			radiusVector.Normalize();
			Vector3 thrusterPos = transform.position + radiusVector * radius;
			thrusterPos.y += y_offset;

			Vector3 thrustDirection = new Vector3(0, 1, 0);
			Vector3 thrustPerpendicular = transform.forward;
			if (centerPoint != null) {
				thrustDirection = centerPoint.position - thrusterPos;

				//thrustPerpendicular = new Vector3(thrustDirection.x, thrustDirection.y, thrustDirection.z);
				//thrustDirection.Normalize();
				//thrustPerpendicular.Normalize();

				thrustPerpendicular = findPerpendicular(thrustDirection);

			}

			if (ThrusterPrefab != null) {
				GameObject thruster = GameObject.Instantiate(ThrusterPrefab, thrusterPos, Quaternion.identity) as GameObject;
				thruster.transform.rotation = Quaternion.LookRotation(thrustPerpendicular, thrustDirection);
				//thruster.transform.LookAt(thrustPerpendicular);
				thruster.transform.SetParent(transform);
				thrusterControllers.Add(thruster.GetComponent<ThrusterController>());
			}
		}
	}

	private Vector3 findPerpendicular(Vector3 a) {
		Vector3 result;
        if (a.x < a.y && a.x < a.z) {
			result = new Vector3(1, a.y, a.z);
		} else if (a.y < a.z) {
			result = new Vector3(a.x, 1, a.z);
		} else {
			result = new Vector3(a.x, a.y, 1);
		}
		result.Normalize();
		a.Normalize();
		return Vector3.Cross(a, result);
	}

	private List<double> generateRandomWeights(int length) {
		List<double> weights = new List<double>();
		for (int i = 0; i < length; i++) {
			weights.Add(Chromosome.generateRandomGene());
		}
		return weights;
	}
}
