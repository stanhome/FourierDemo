using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fourier : MonoBehaviour
{
	public ComputeShader cs;

	[Range(32, 1024)]
	public int renderTextureSize = 1024;
	private int CS_GRID;

	private Texture _originalImage;

	RenderTexture _rtFourierSpectrumPhase;
	RenderTexture _rtFourierRevert;

	//input
	public MeshRenderer rendererOriginalRenderer = null;

	// output
	public MeshRenderer rendererFourierSpectrum = null;
	public MeshRenderer rendererFourierPhase = null;
	public MeshRenderer rendererFourierRevert = null;

	// Start is called before the first frame update
	void Start()
    {
		CS_GRID = renderTextureSize / 16;

		_rtFourierSpectrumPhase = createRT(renderTextureSize);
		_rtFourierRevert = createRT(renderTextureSize);

		initMaterial();
    }


	public void doFourierTransform() {
		Debug.Log("doFourierTransform begin...");

		int kernel = cs.FindKernel("Fourier");

		cs.SetInt("renderTextureSize", renderTextureSize);

		cs.SetTexture(kernel, "originalImg", _originalImage);
		cs.SetTexture(kernel, "RTFourierSpectrumPhase", _rtFourierSpectrumPhase);

		cs.Dispatch(kernel, CS_GRID, CS_GRID, 1);
		Debug.Log("doFourierTransform done.");
	}

	public void doFourierTransformRevert() {
		Debug.Log("doFourierTransformRevert begin ...");

		int kernel = cs.FindKernel("FourierRevert");

		cs.SetInt("renderTextureSize", renderTextureSize);

		cs.SetTexture(kernel, "RTFourierSpectrumPhase", _rtFourierSpectrumPhase);
		cs.SetTexture(kernel, "RTFourierRevert", _rtFourierRevert);

		cs.Dispatch(kernel, CS_GRID, CS_GRID, 1);
		Debug.Log("doFourierTransformRevert done.");
	}


	private void initMaterial() {
		_originalImage = rendererOriginalRenderer.material.GetTexture("_MainTex");

		rendererFourierSpectrum?.material.SetTexture("_MainTex", _rtFourierSpectrumPhase);
		rendererFourierPhase?.material.SetTexture("_MainTex", _rtFourierSpectrumPhase);

		if (rendererFourierRevert != null)
		{
			rendererFourierRevert.material.SetTexture("_MainTex", _rtFourierRevert);
		}
	}

	private RenderTexture createRT(int rtSize) {
		RenderTexture ret = new RenderTexture(rtSize, rtSize, 24);
		ret.format = RenderTextureFormat.ARGBFloat;
		ret.enableRandomWrite = true;
		ret.filterMode = FilterMode.Point;

		ret.Create();
		ret.DiscardContents();

		return ret;
	}
}
