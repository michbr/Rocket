using System.Collections.Generic;

public class NeuralNet {

	private NeuronMode mode;
	private InputLayer inputLayer;

	private NeuronLayer outputLayer;

	private List<NeuronLayer> hiddenLayers = new List<NeuronLayer>();

	private double defaultActivationThreshold = .5;

	public NeuralNet(NeuronMode mode, bool evolveActivationThreshold, int inputCount, int outputCount, int middleCount, int hiddenLayerCount) {
		this.mode = mode;
		inputLayer = new InputLayer(inputCount);
		outputLayer = new NeuronLayer(mode, outputCount, defaultActivationThreshold, evolveActivationThreshold);
		NeuronLayer prevLayer = null;
		for (int i = 0; i < hiddenLayerCount; ++i) {
			NeuronLayer newLayer = new NeuronLayer(mode, middleCount, defaultActivationThreshold, evolveActivationThreshold);
			hiddenLayers.Add(newLayer);
			if (prevLayer != null) {
				prevLayer.setOutputLayer(newLayer);
			}
			prevLayer = newLayer;
		}
		if (prevLayer != null) { 
			prevLayer.setOutputLayer(outputLayer);
			inputLayer.setOutputLayer(hiddenLayers[0]);
		} else {
			inputLayer.setOutputLayer(outputLayer);
		}
	}

	public List<double> extractWeights() {
		List<double> weights = new List<double>();
		foreach (NeuronLayer layer in hiddenLayers) {
			weights.AddRange(layer.extractWeights());
		}
		weights.AddRange(outputLayer.extractWeights());
		return weights;
	}

	public void setWeights(Queue<double> weights) {
		foreach (NeuronLayer neuronLayer in hiddenLayers) {
			neuronLayer.setWeights(weights);
		}
		outputLayer.setWeights(weights);
	}

	public List<double> calculateOutput() {
		return outputLayer.getOutput();
	}

	public void setInputs(List<double> inputs) {
		for (int i = 0; i < inputs.Count; ++i) {
			inputLayer.setInput(i, inputs[i]);
		}
	}


}
