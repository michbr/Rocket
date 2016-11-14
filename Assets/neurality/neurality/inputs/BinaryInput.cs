

public class BinaryInput : IOutputValue {

	private bool value;

	public BinaryInput(bool value) {
		this.value = value;
	}

	public void setValue(bool value) {
		this.value = value;
	}

	public double getOutputValue() {
		return (value) ? 1.0 : 0.0;
	}
}
