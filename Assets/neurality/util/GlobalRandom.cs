

public class GlobalRandom {

	private static GlobalRandom instance;
	private System.Random random = new System.Random();
	
	private GlobalRandom() {

	}

	public System.Random getRandom() {
		return random;
	}

	public static GlobalRandom getInstance() {
		if (instance == null) {
			instance = new GlobalRandom();
		}
		return instance;
	}
}
