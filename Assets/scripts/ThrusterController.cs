using UnityEngine;

public class ThrusterController : MonoBehaviour {

	public bool isFiring = false;
	public float throttle = 1f;
	public float thrustForce = .1f;

	private Rigidbody parentRigidbody;
	private ParticleSystem emmissionControl;

	private Rigidbody targetRigidBody { get {
			if (parentRigidbody == null) {
				parentRigidbody = GetComponentInParent<Rigidbody>();
			}
			return parentRigidbody;
		}
	}

	private ParticleSystem particleControl {
		get {
			if (emmissionControl == null) {
				emmissionControl = GetComponentInChildren<ParticleSystem>();
			}
			return emmissionControl;
		}
	}

	public bool firing { get {
			return isFiring;
		}
		set {
			isFiring = value;
			if (isFiring) {
				particleControl.Play();
			} else {
				particleControl.Pause();
			}
		}
	}

	void Update() {
		Color color = particleControl.startColor;
		color.a = throttle;
		particleControl.startColor = color;

	}

	void FixedUpdate() {
		if (isFiring) {
			//print("drawing line from " + transform.TransformPoint(transform.localPosition) + " to " + (transform.TransformPoint(transform.localPosition) + ((transform.up * (thrustForce * throttle)))));
			//Debug.DrawLine(location, transform.position, Color.blue);

			//Debug.DrawLine(location, location + ((transform.up * (thrustForce * throttle))), Color.blue);
			Debug.DrawLine(transform.position, transform.position + (-(transform.up * (thrustForce * throttle))), Color.blue);
			targetRigidBody.AddForceAtPosition(transform.up * (thrustForce * throttle), transform.position);
		}
	}
}
