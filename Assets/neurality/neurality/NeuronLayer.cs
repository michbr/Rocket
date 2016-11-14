using System.Collections.Generic;

public class NeuronLayer {
	private List<AbstractNeuron> neuronList = new List<AbstractNeuron>();

	public NeuronLayer(NeuronMode mode, int neuronCount, double activationThreshold, bool evolveActivationThreshold) {
		for (int i = 0; i < neuronCount; ++i) {
			if (mode == NeuronMode.NEURON) {
				neuronList.Add(new Neuron(activationThreshold, evolveActivationThreshold));
			} else if (mode == NeuronMode.PERCEPTRON) {
				neuronList.Add(new Perceptron(activationThreshold, evolveActivationThreshold));
			}
		}
	}

	public List<double> extractWeights() {
		List<double> weights = new List<double>();
		foreach (AbstractNeuron neuron in neuronList) {
			weights.AddRange(neuron.extractWeights());
		}
		return weights;
	}

	public void setWeights(Queue<double> weights) {
		foreach (AbstractNeuron neuron in neuronList) {
			neuron.setWeights(weights);
		}
	}

	public void setOutputLayer(NeuronLayer outputLayer) {
		System.Random random = GlobalRandom.getInstance().getRandom();
		foreach (AbstractNeuron neuron in neuronList) {
			foreach(AbstractNeuron output in outputLayer.neuronList) {
				output.addInput(neuron, random.NextDouble() - random.NextDouble());
			}
		}
	}

	public List<double> getOutput() {
		List<double> result = new List<double>();
		foreach(AbstractNeuron neuron in neuronList) {
			result.Add(neuron.getOutputValue());
		}
		return result;
	}

	public List<AbstractNeuron> getNeuronList() {
		return neuronList;
	}
}
