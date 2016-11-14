
public class NeuronInput {

	private IOutputValue input;
	private double weight;

	public NeuronInput(IOutputValue input, double weight) {
		this.input = input;
		this.weight = weight;
	}

	public IOutputValue getInput() {
		return input;
	}

	public double getWeight() {
		return weight;
	}

	public void setWeight(double weight) {
		this.weight = weight;
	}
}
