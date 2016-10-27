using UnityEngine;
using System.Collections;

public class SpaceTile : MonoBehaviour {

	GameObject SpotLight = null;

	public char MapKey { get; private set; }
	public bool IsLightActive {
		get {
			return SpotLight.activeSelf;
		}
	}

	void Awake() {
		Transform light = transform.Find ("SpotLight");
		if (light != null) {
			SpotLight = light.gameObject;
		}
	}

	public void SetMapKey(char key) {
		MapKey = key;
	}

	public void SetLightActive(bool state) {
		SpotLight.SetActive (state);
	}

	public void ToggleLight() {
		if (SpotLight != null) {
			SpotLight.SetActive (!SpotLight.activeSelf);
		}
	}
}
