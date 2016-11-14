using System.Collections.Generic;

public class InputLayer {

	private List<IOutputValue> inputs = new List<IOutputValue>();

	public InputLayer(int inputCount) {
		for (int i = 0; i < inputCount; ++i) {
			inputs.Add(new NumericInput(0.0));
		}
	}

	public void setOutputLayer(NeuronLayer layer) {
		System.Random random = GlobalRandom.getInstance().getRandom();
		foreach (IOutputValue input in inputs) {
			foreach(AbstractNeuron neuron in layer.getNeuronList()) {
				neuron.addInput(input, random.NextDouble() - random.NextDouble());
			}
		}
	}

	public void setInput(int index, double value) {
		if (index >= 0 && index < inputs.Count) {
			((NumericInput)inputs[index]).setValue(value);
		}
	}

}
