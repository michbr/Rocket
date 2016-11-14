public class Perceptron : AbstractNeuron {
	public Perceptron(double activationThreshold, bool evolveActivationThreshold) : base(activationThreshold, evolveActivationThreshold) {

	}

	public override double getOutputValue() {
		double sum = 0;
		foreach (NeuronInput input in inputs) {
			sum += input.getInput().getOutputValue() * input.getWeight();
		}
		return ((sum - activationThreshold) > 0) ? 1.0 : 0;
	}

}
