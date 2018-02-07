using UnityEngine;
using UnityEngine.UI;

public class GasLevelIndicator : MonoBehaviour {

	public RectTransform redBar;
	public RectTransform blueBar;
	public RectTransform yellowBar;

	public void SetLevels(float r, float b, float y) {
		redBar.localScale = new Vector3(r, redBar.localScale.y, redBar.localScale.z);
		blueBar.localScale = new Vector3(b, blueBar.localScale.y, blueBar.localScale.z);
		yellowBar.localScale = new Vector3(y, yellowBar.localScale.y, yellowBar.localScale.z);
	}
}
