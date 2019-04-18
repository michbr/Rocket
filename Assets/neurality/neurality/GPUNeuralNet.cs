using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GPUNeuralNet {

	[DllImport("CudaTest", EntryPoint = "Run")]
	private static extern int Run(float[] inputs, int inputCount, float[] outputs, int outputCount, int layers, int layerWidth);

	[DllImport("CudaTest", EntryPoint = "Initialize")]
	private static extern int Inititalize(int activationThresholdsCount, int weightsCount);

	[DllImport("CudaTest", EntryPoint = "Load")]
	private static extern int Load(float[] activationThresholds, float[] weights);

	private static GPUNeuralNet instance;
	public static GPUNeuralNet getInstance(int inputCount, int outputCount, int layers, int layerWidth) {
		if (instance == null) {
			instance = new GPUNeuralNet(inputCount, outputCount, layers, layerWidth);
		}
		return instance;
	}

	private int outputCount;
	private int inputCount;
	private int layers;
	private int layerWidth;

	private int weightsCount;
	private int activationThresholdsCount;

	private GPUNeuralNet(int inputCount, int outputCount, int layers, int layerWidth) {
		this.outputCount = outputCount;
		this.inputCount = inputCount;
		this.layers = layers;
		this.layerWidth = layerWidth;
		weightsCount = (inputCount + (layers * layerWidth))*layerWidth;
		
		activationThresholdsCount = outputCount + (layers * layerWidth);

		if (Inititalize(activationThresholdsCount, weightsCount) != 0) {
			throw new System.Exception("Initialize failed!");
			Application.Quit();
		}
	}

	public void load(float [] activationThresholds, float[] weights) {

		if (weights.Length != weightsCount) {
			throw new System.Exception("Invalid number of weights supplied. Expected: " + weightsCount + " got: " + weights.Length);
		}

		if (activationThresholds.Length != activationThresholdsCount) {
			throw new System.Exception("Invalid number of activation thresholds supplied. Expected: " + activationThresholdsCount + " got: " + activationThresholds.Length);
		}
		if (Load(activationThresholds, weights) != 0) {
			throw new System.Exception("Load failed!");
			Application.Quit();

		}
	}

	public float[] run(float [] inputs) {
		float[] outputs = new float[outputCount];
		if (Run(inputs, inputCount, outputs, outputCount, layers, layerWidth) != 0) {
			throw new System.Exception("Run failed!");
			Application.Quit();
		}

		return outputs;
	}
}
