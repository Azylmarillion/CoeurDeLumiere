using UnityEngine;

[ExecuteInEditMode]
public class WILL_CartoonWaterShaEdit : MonoBehaviour
{
    #region F/P
    [SerializeField] Renderer[] waterRenderers = new Renderer[] { };

        [Space]
        [SerializeField] 
            Color waterColor = new Color();
        [SerializeField] 
            Texture waterTex = null;
        [SerializeField]
            Vector2 waterTile = new Vector2();
        [SerializeField, Range(0, 1)]
            float textureVisibility = 0;
        [Space]
        [SerializeField] 
            Texture distortionTex = null;
        [SerializeField]
            Vector2 distortionTile = new Vector2();

        [Space]
        [SerializeField] 
            float waterHeight = 0;
        [SerializeField]
            float waterDeep = 0;
        [SerializeField,Range(0, 0.1f)]
            float waterDepthParam = 0;
        [SerializeField, Range(0, 1)]
            float waterMinAlpha = 0;
        [Space]
        [SerializeField]
            Color borderColor = new Color();
        [SerializeField, Range(0,1)]
            float borderWidth = 0;

        [Space]
        [SerializeField]
            Vector2 moveDirection = new Vector2();

        MaterialPropertyBlock materialPropertyBlock;

        public MaterialPropertyBlock MaterialPropertyBlock
        {
            get { return materialPropertyBlock; }
        }
    #endregion

    #region Meths
    #region PersonalMeths
    private void SetUpPropertyBlock(MaterialPropertyBlock propertyBlock)
    {
        propertyBlock.SetColor("_WaterColor", waterColor);
        propertyBlock.SetColor("_BorderColor", borderColor);

        propertyBlock.SetVector("_Tiling", waterTile);
        propertyBlock.SetVector("_DistTiling", distortionTile);
        propertyBlock.SetVector("_MoveDirection", new Vector4(moveDirection.x, 0f, moveDirection.y, 0f));

        if (waterTex != null)
        {
            propertyBlock.SetTexture("_WaterTex", waterTex);
        }

        if (distortionTex != null)
        {
            propertyBlock.SetTexture("_DistTex", distortionTex);
        }

        propertyBlock.SetFloat("_TextureVisibility", textureVisibility);
        propertyBlock.SetFloat("_WaterHeight", waterHeight);
        propertyBlock.SetFloat("_WaterDeep", waterDeep);
        propertyBlock.SetFloat("_WaterDepth", waterDepthParam);
        propertyBlock.SetFloat("_WaterMinAlpha", waterMinAlpha);
        propertyBlock.SetFloat("_BorderWidth", borderWidth);
    }
    #endregion
    #region UniMeths
    public void Awake()
    {
        materialPropertyBlock = new MaterialPropertyBlock();
        SetUpPropertyBlock(materialPropertyBlock);

        if (waterRenderers != null)
        {
            for (int i = 0; i < waterRenderers.Length; i++)
            {
                waterRenderers[i].SetPropertyBlock(materialPropertyBlock);
            }
        }
    }

#if UNITY_EDITOR
    public void OnEnable()
    {
        materialPropertyBlock = new MaterialPropertyBlock();
        SetUpPropertyBlock(materialPropertyBlock);
    }

    public void Update()
    {
        SetUpPropertyBlock(materialPropertyBlock);

        if (waterRenderers != null)
        {
            for (int i = 0; i < waterRenderers.Length; i++)
            {
                waterRenderers[i].SetPropertyBlock(materialPropertyBlock);
            }
        }
    }
#endif
    #endregion
    #endregion    
}
