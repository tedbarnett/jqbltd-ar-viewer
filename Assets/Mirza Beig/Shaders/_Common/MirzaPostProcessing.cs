
// =================================	
// Namespaces.
// =================================

using UnityEngine;

// =================================	
// Define namespace.
// =================================

namespace MirzaBeig
{
    namespace Shaders
    {
        namespace ImageEffects
        {
            [ExecuteInEditMode]
            [System.Serializable]

            public class MirzaPostProcessing : MonoBehaviour
            {
                public Material material;

                void OnRenderImage(RenderTexture source, RenderTexture destination)
                {
                    Graphics.Blit(source, destination, material);
                }
            }

        }

    }

}