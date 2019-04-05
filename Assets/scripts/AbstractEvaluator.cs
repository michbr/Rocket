﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractEvaluator : MonoBehaviour {

	protected int index;

	public abstract void startEvaluation (GPUNeuralNet brain, int index);
	public abstract void reportScenarioScore (double score);
}
