using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Grayout")]
public class GrayoutEffect : ImageEffectBase {
	public Texture  textureRamp;
	public float    rampOffset;
	public float effectAmount = 0;

	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		material.SetTexture("_RampTex", textureRamp);
		material.SetFloat("_RampOffset", rampOffset);
		material.SetFloat("_EffectAmount", effectAmount);
		Graphics.Blit (source, destination, material);
	}
}