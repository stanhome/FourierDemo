using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fourier : MonoBehaviour
{
	public ComputeShader cs;

	public Texture2D originalImage;
	[Range(32, 1024)]
	public int renderTextureSize = 1024;
	private int CS_GRID;


	RenderTexture rtFourierSpectrumPhase;

	RenderTexture rtFourierRevert;

	// output
	public MeshRenderer rendererFourierSpectrum = null;
	public MeshRenderer rendererFourierPhase = null;
	public MeshRenderer rendererFourierRevert = null;

	// Start is called before the first frame update
	void Start()
    {
		CS_GRID = renderTextureSize / 16;

		rtFourierSpectrumPhase = createRT(renderTextureSize);
		rtFourierRevert = createRT(renderTextureSize);

		initMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void doFourierTransform() {
		Debug.Log("doFourierTransform begin...");

		int kernel = cs.FindKernel("Fourier");

		cs.SetInt("renderTextureSize", renderTextureSize);

		cs.SetTexture(kernel, "originalImg", originalImage);
		cs.SetTexture(kernel, "RTFourierSpectrumPhase", rtFourierSpectrumPhase);

		cs.Dispatch(0, CS_GRID, CS_GRID, 1);
		Debug.Log("doFourierTransform done.");
	}


	private void initMaterial() {
		rendererFourierSpectrum?.material.SetTexture("_MainTex", rtFourierSpectrumPhase);
		rendererFourierPhase?.material.SetTexture("_MainTex", rtFourierSpectrumPhase);

		if (rendererFourierRevert != null)
		{
			rendererFourierRevert.material.SetTexture("_MainTex", rtFourierRevert);
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
