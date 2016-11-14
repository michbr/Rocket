
using System;

public class NumericInput : IOutputValue {
	private double value;

	public NumericInput(double value) {
		this.value = value;
	}

	public void setValue(double value) {
		this.value = value;
	}

	public double getOutputValue() {
		return value;
	}
}
