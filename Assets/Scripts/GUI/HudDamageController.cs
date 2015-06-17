using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof (Image))]
public class HudDamageController : MonoBehaviour 
{
	[Range(0f, 1f)] public float flashOpacity = 0.5f;
	[Range(0f, 2f)] public float flashDuration = 0.8f;
	public Sprite[] sprites;

	private Image image;
	private int currentSprite = 0;


	void Start() {
		image = GetComponent<Image>();
		image.color = new Color(1f, 1f, 1f, flashOpacity);
	}


	public void FlashDamage() {
		image.sprite = sprites[currentSprite];
		currentSprite++;
		
		if (currentSprite >= sprites.Length) {
			currentSprite = 0;
		}

		StopCoroutine("FlashDamageCoroutine");
		StartCoroutine("FlashDamageCoroutine");
	}


	IEnumerator FlashDamageCoroutine() {
		image.enabled = true;

		yield return new WaitForSeconds(flashDuration);

		image.enabled = false;
	}
}