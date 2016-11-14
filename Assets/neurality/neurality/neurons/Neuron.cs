
public class Neuron : AbstractNeuron {
	public Neuron(double activationThreshold, bool evolveActivationThreshold) : base(activationThreshold, evolveActivationThreshold) {
		
	}

	public override double getOutputValue() {
		double sum = 0;
		foreach (NeuronInput input in inputs) {
			sum += input.getInput().getOutputValue() * input.getWeight();
		}
		double finalInput = (1.0) / (1 + System.Math.Pow(System.Math.E, -sum));
		return finalInput - activationThreshold;
	}
}
