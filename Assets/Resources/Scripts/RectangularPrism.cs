using UnityEngine;
using System.Collections;

public class RectangularPrism : AbstractGameObject
{
	public uint XQuads = 1;
	public uint YQuads = 1;
	public uint ZQuads = 1;

	public Texture FrontTexture = null;
	public Vector2 FrontTextureTiling = Vector2.one;
	public Vector2 FrontTextureOffset = Vector2.zero;

	public Texture BackTexture = null;
	public Vector2 BackTextureTiling = Vector2.one;
	public Vector2 BackTextureOffset = Vector2.zero;

	public Texture TopTexture = null;
	public Vector2 TopTextureTiling = Vector2.one;
	public Vector2 TopTextureOffset = Vector2.zero;

	public Texture BottomTexture = null;
	public Vector2 BottomTextureTiling = Vector2.one;
	public Vector2 BottomTextureOffset = Vector2.zero;

	public Texture LeftTexture = null;
	public Vector2 LeftTextureTiling = Vector2.one;
	public Vector2 LeftTextureOffset = Vector2.zero;

	public Texture RightTexture = null;
	public Vector2 RightTextureTiling = Vector2.one;
	public Vector2 RightTextureOffset = Vector2.zero;

	// Use this for initialization
	protected override void Awake()
	{
        base.Awake();
		for (int i = 0; i < XQuads; i++)
		{
			for (int j = 0; j < YQuads; j++)
			{
				GameObject frontChildQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
				GameObject backChildQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
				Material frontMaterial = frontChildQuad.GetComponent<Renderer>().material;
				Material backMaterial = backChildQuad.GetComponent<Renderer>().material;

                Transform frontChildQuadTransform = frontChildQuad.transform;
                Transform backChildQuadTransform = backChildQuad.transform;

                frontChildQuadTransform.parent = TransformCached;
				backChildQuadTransform.parent = TransformCached;

				frontChildQuadTransform.localPosition = new Vector3(-(((float)XQuads) / 2.0f) + (float)i,
					-(((float)YQuads) / 2.0f) + (float)j,
					((float)ZQuads) / 2.0f) + new Vector3(0.5f, 0.5f, 0.0f);
				frontChildQuadTransform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
				frontChildQuadTransform.localScale = Vector3.one;

				backChildQuadTransform.localPosition = new Vector3(-(((float)XQuads) / 2.0f) + (float)i,
					-(((float)YQuads) / 2.0f) + (float)j,
					-((float)ZQuads) / 2.0f) + new Vector3(0.5f, 0.5f, 0.0f);
				backChildQuadTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
				backChildQuadTransform.localScale = Vector3.one;

				if (FrontTexture)
				{
					frontMaterial.mainTexture = FrontTexture;
					frontMaterial.mainTextureOffset = new Vector2((1.0f / XQuads) * ((XQuads - 1) - i) * FrontTextureTiling.x, (1.0f / YQuads) * j * FrontTextureTiling.y) - FrontTextureOffset;
					frontMaterial.mainTextureScale = new Vector2(1.0f / XQuads * FrontTextureTiling.x, 1.0f / YQuads * FrontTextureTiling.y);
				}
				if (BackTexture)
				{
					backMaterial.mainTexture = BackTexture;
					backMaterial.mainTextureOffset = new Vector2((1.0f / XQuads) * i * BackTextureTiling.x, (1.0f / YQuads) * j * BackTextureTiling.y) - BackTextureOffset;
					backMaterial.mainTextureScale = new Vector2(1.0f / XQuads * BackTextureTiling.x, 1.0f / YQuads * BackTextureTiling.y);
				}
			}
		}
		for (int i = 0; i < XQuads; i++)
		{
			for (int j = 0; j < ZQuads; j++)
			{
				GameObject topChildQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
				GameObject bottomChildQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
				Material topMaterial = topChildQuad.GetComponent<Renderer>().material;
				Material bottomMaterial = bottomChildQuad.GetComponent<Renderer>().material;

                Transform topChildQuadTransform = topChildQuad.transform;
                Transform bottomChildQuadTransform = bottomChildQuad.transform;

                topChildQuadTransform.parent = TransformCached;
				bottomChildQuadTransform.parent = TransformCached;

				topChildQuadTransform.localPosition = new Vector3(-(((float)XQuads) / 2.0f) + (float)i,
					((float)YQuads) / 2.0f,
					-(((float)ZQuads) / 2.0f) + (float)j) + new Vector3(0.5f, 0.0f, 0.5f);
				topChildQuadTransform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
				topChildQuadTransform.localScale = Vector3.one;

				bottomChildQuadTransform.localPosition = new Vector3(-(((float)XQuads) / 2.0f) + (float)i,
					-((float)YQuads) / 2.0f,
					-(((float)ZQuads) / 2.0f) + (float)j) + new Vector3(0.5f, 0.0f, 0.5f);
				bottomChildQuadTransform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
				bottomChildQuadTransform.localScale = Vector3.one;

				if (TopTexture)
				{
					topMaterial.mainTexture = TopTexture;
					topMaterial.mainTextureOffset = new Vector2((1.0f / XQuads) * i * TopTextureTiling.x, (1.0f / ZQuads) * j * TopTextureTiling.y) - TopTextureOffset;
					topMaterial.mainTextureScale = new Vector2(1.0f / XQuads * TopTextureTiling.x, 1.0f / ZQuads * TopTextureTiling.y);
				}
				if (BottomTexture)
				{
					bottomMaterial.mainTexture = BottomTexture;
					bottomMaterial.mainTextureOffset = new Vector2((1.0f / XQuads) * i * BottomTextureTiling.x, (1.0f / ZQuads) * ((ZQuads - 1) - j) * BottomTextureTiling.y) - BottomTextureOffset;
					bottomMaterial.mainTextureScale = new Vector2(1.0f / XQuads * BottomTextureTiling.x, 1.0f / ZQuads * BottomTextureTiling.y);
				}
			}
		}
		for (int i = 0; i < YQuads; i++)
		{
			for (int j = 0; j < ZQuads; j++)
			{
				GameObject leftChildQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
				GameObject rightChildQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
				Material leftMaterial = leftChildQuad.GetComponent<Renderer>().material;
				Material rightMaterial = rightChildQuad.GetComponent<Renderer>().material;

                Transform leftChildQuadTransform = leftChildQuad.transform;
                Transform rightChildQuadTransform = rightChildQuad.transform;
                
                leftChildQuadTransform.parent = TransformCached;
                rightChildQuadTransform.parent = TransformCached;

				leftChildQuadTransform.localPosition = new Vector3(-((float)XQuads) / 2.0f,
					-(((float)YQuads) / 2.0f) + (float)i,
					-(((float)ZQuads) / 2.0f) + (float)j) + new Vector3(0.0f, 0.5f, 0.5f);
				leftChildQuadTransform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
				leftChildQuadTransform.localScale = Vector3.one;

				rightChildQuadTransform.localPosition = new Vector3(((float)XQuads) / 2.0f,
					-(((float)YQuads) / 2.0f) + (float)i,
					-(((float)ZQuads) / 2.0f) + (float)j) + new Vector3(0.0f, 0.5f, 0.5f);
				rightChildQuadTransform.localRotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
				rightChildQuadTransform.localScale = Vector3.one;

				if (LeftTexture)
				{
					leftMaterial.mainTexture = LeftTexture;
					leftMaterial.mainTextureOffset = new Vector2((1.0f / ZQuads) * ((ZQuads - 1) - j) * LeftTextureTiling.x, (1.0f / YQuads) * i * LeftTextureTiling.y) - LeftTextureOffset;
					leftMaterial.mainTextureScale = new Vector2(1.0f / ZQuads * LeftTextureTiling.x, 1.0f / YQuads * LeftTextureTiling.y);
				}
				if (RightTexture)
				{
					rightMaterial.mainTexture = RightTexture;
					rightMaterial.mainTextureOffset = new Vector2((1.0f / ZQuads) * j * RightTextureTiling.x, (1.0f / YQuads) * i * RightTextureTiling.y) - RightTextureOffset;
					rightMaterial.mainTextureScale = new Vector2(1.0f / ZQuads * RightTextureTiling.x, 1.0f / YQuads * RightTextureTiling.y);
				}
			}
		}
	}
}
