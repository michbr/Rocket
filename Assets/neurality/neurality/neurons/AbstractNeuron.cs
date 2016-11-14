using System.Collections.Generic;

public abstract class AbstractNeuron : IOutputValue {
	protected double activationThreshold;
	protected List<NeuronInput> inputs = new List<NeuronInput>();
	private bool evolveActivationThreshold;

	public AbstractNeuron(double activationThreshold, bool evolveActivationThreshold) {
		this.activationThreshold = activationThreshold;
		this.evolveActivationThreshold = evolveActivationThreshold;
	}

	public void addInput(IOutputValue n, double weight) {
		inputs.Add(new NeuronInput(n, weight));
	}

	public List<double> extractWeights() {
		List<double> weights = new List<double>();
		foreach (NeuronInput input in inputs) {
			weights.Add(input.getWeight());
		}
		if (evolveActivationThreshold) {
			weights.Add(activationThreshold);
		}
		return weights;
	}

	public abstract double getOutputValue();

	public void setWeights(Queue<double> weights) {
		foreach(NeuronInput input in inputs) {
			input.setWeight(weights.Dequeue());
		}
		if (evolveActivationThreshold) {
			activationThreshold = weights.Dequeue();
		}
	}

}
