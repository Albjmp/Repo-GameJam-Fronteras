using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace GameJamFronteras.EditorTools
{
    /// <summary>
    /// Postprocesador y herramienta de editor que:
    /// 1. Corrige automáticamente cualquier material incompatible (Standard / Built-in / Pink) a Universal Render Pipeline/Lit.
    /// 2. Asigna y vincula automáticamente texturas (Albedo, Normal, Metálico, Oclusión, Emisión) a los materiales al importar modelos 3D o materiales.
    /// 3. Ofrece opciones en la barra de menú "Tools/Materiales (URP)" para procesar por lotes todo el proyecto en cualquier momento.
    /// </summary>
    public class URPMaterialAutoFixer : AssetPostprocessor
    {
        private const string URP_LIT_SHADER_NAME = "Universal Render Pipeline/Lit";

        // Sufijos para auto-detección y emparejamiento de texturas
        private static readonly string[] DiffuseSuffixes = new string[] {
            "_diffuse", "_albedo", "_basecolor", "_base_color", "_col", "_color", "_d", "_alb", "_diff",
            " diffuse", " albedo", " basecolor", " base color", " col", " color",
            "_albedotransparency", " albedotransparency"
        };

        private static readonly string[] NormalSuffixes = new string[] {
            "_normal", "_bump", "_norm", "_nrm", "_n",
            " normal", " bump", " norm", " nrm"
        };

        private static readonly string[] MetallicSuffixes = new string[] {
            "_metallic", "_metal", "_maskmap", "_mask", "_metallicsmoothness", "_met",
            " metallic", " metal", " maskmap", " mask", " metallicsmoothness"
        };

        private static readonly string[] OcclusionSuffixes = new string[] {
            "_occlusion", "_occ", "_ao", "_ambientocclusion",
            " occlusion", " occ", " ao", " ambient occlusion"
        };

        private static readonly string[] EmissionSuffixes = new string[] {
            "_emission", "_emissive", "_emit", "_em",
            " emission", " emissive", " emit"
        };

        /// <summary>
        /// Se ejecuta automáticamente cada vez que se agregan, importan o mueven assets en el proyecto.
        /// </summary>
        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            bool anyChanged = false;

            foreach (string path in importedAssets)
            {
                if (path.EndsWith(".mat", System.StringComparison.OrdinalIgnoreCase))
                {
                    Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                    if (mat != null)
                    {
                        if (ProcessMaterial(mat, path))
                        {
                            anyChanged = true;
                        }
                    }
                }
            }

            if (anyChanged)
            {
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Se ejecuta automáticamente cuando un modelo 3D (FBX, OBJ, etc.) termina de importarse.
        /// </summary>
        void OnPostprocessModel(GameObject root)
        {
            if (root == null) return;

            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            string modelDir = Path.GetDirectoryName(assetPath);

            foreach (Renderer renderer in renderers)
            {
                Material[] sharedMats = renderer.sharedMaterials;
                bool changed = false;

                for (int i = 0; i < sharedMats.Length; i++)
                {
                    Material mat = sharedMats[i];
                    if (mat == null) continue;

                    string matPath = AssetDatabase.GetAssetPath(mat);
                    if (string.IsNullOrEmpty(matPath)) continue;

                    if (IsPinkOrIncompatibleShader(mat.shader))
                    {
                        if (FixMaterialShader(mat))
                        {
                            changed = true;
                        }
                    }

                    // Auto-asignar textura si le falta la textura principal
                    if (mat.GetTexture("_BaseMap") == null && mat.GetTexture("_MainTex") == null)
                    {
                        if (TryAutoAssignTextures(mat, Path.GetDirectoryName(matPath), modelDir))
                        {
                            changed = true;
                        }
                    }
                }

                if (changed)
                {
                    renderer.sharedMaterials = sharedMats;
                }
            }
        }

        /// <summary>
        /// Procesa un material verificando si necesita cambio de shader a URP Lit o vinculación de texturas.
        /// </summary>
        public static bool ProcessMaterial(Material mat, string matPath)
        {
            bool changed = false;

            if (IsPinkOrIncompatibleShader(mat.shader))
            {
                if (FixMaterialShader(mat))
                {
                    changed = true;
                }
            }

            if (SynchronizeTextures(mat))
            {
                changed = true;
            }

            if (mat.GetTexture("_BaseMap") == null && mat.GetTexture("_MainTex") == null)
            {
                string folder = Path.GetDirectoryName(matPath);
                if (TryAutoAssignTextures(mat, folder, null))
                {
                    changed = true;
                }
            }

            if (changed)
            {
                EditorUtility.SetDirty(mat);
            }

            return changed;
        }

        /// <summary>
        /// Determina si un shader es incompatible con URP o produce el típico color rosado.
        /// </summary>
        public static bool IsPinkOrIncompatibleShader(Shader shader)
        {
            if (shader == null) return true;
            string sName = shader.name;
            if (string.IsNullOrEmpty(sName)) return true;

            if (sName == "Hidden/InternalErrorShader") return true;
            if (sName == "Standard" || sName.StartsWith("Standard ") || sName.StartsWith("Standard (")) return true;
            if (sName.StartsWith("Legacy Shaders/")) return true;
            if (sName.StartsWith("Mobile/") && !sName.Contains("Universal")) return true;
            if (sName == "Autodesk Interactive") return true;

            return false;
        }

        /// <summary>
        /// Convierte el shader de un material a Universal Render Pipeline/Lit conservando sus texturas y colores.
        /// </summary>
        public static bool FixMaterialShader(Material mat)
        {
            Shader targetShader = Shader.Find(URP_LIT_SHADER_NAME);
            if (targetShader == null)
            {
                Debug.LogWarning("[URPMaterialAutoFixer] Shader '" + URP_LIT_SHADER_NAME + "' no encontrado en el proyecto.");
                return false;
            }

            // Guardar propiedades previas antes de cambiar de shader
            Texture mainTex = mat.HasProperty("_MainTex") ? mat.GetTexture("_MainTex") : null;
            Vector2 mainScale = mat.HasProperty("_MainTex") ? mat.GetTextureScale("_MainTex") : Vector2.one;
            Vector2 mainOffset = mat.HasProperty("_MainTex") ? mat.GetTextureOffset("_MainTex") : Vector2.zero;

            Color baseColor = mat.HasProperty("_BaseColor") ? mat.GetColor("_BaseColor") :
                             (mat.HasProperty("_Color") ? mat.GetColor("_Color") : Color.white);

            Texture bumpMap = mat.HasProperty("_BumpMap") ? mat.GetTexture("_BumpMap") : null;
            Texture metalMap = mat.HasProperty("_MetallicGlossMap") ? mat.GetTexture("_MetallicGlossMap") : null;
            Texture emissMap = mat.HasProperty("_EmissionMap") ? mat.GetTexture("_EmissionMap") : null;
            Color emissColor = mat.HasProperty("_EmissionColor") ? mat.GetColor("_EmissionColor") : Color.black;
            Texture occlMap = mat.HasProperty("_OcclusionMap") ? mat.GetTexture("_OcclusionMap") : null;

            bool isTransparent = false;
            if (mat.HasProperty("_Surface"))
            {
                isTransparent = mat.GetFloat("_Surface") > 0.5f;
            }
            else if (mat.GetTag("RenderType", false) == "Transparent" || mat.renderQueue >= 2500)
            {
                isTransparent = true;
            }

            // Asignar el shader Lit de URP
            mat.shader = targetShader;

            // Restaurar y vincular propiedades
            if (mainTex != null)
            {
                mat.SetTexture("_BaseMap", mainTex);
                mat.SetTextureScale("_BaseMap", mainScale);
                mat.SetTextureOffset("_BaseMap", mainOffset);

                mat.SetTexture("_MainTex", mainTex);
                mat.SetTextureScale("_MainTex", mainScale);
                mat.SetTextureOffset("_MainTex", mainOffset);
            }

            mat.SetColor("_BaseColor", baseColor);
            mat.SetColor("_Color", baseColor);

            if (bumpMap != null)
            {
                mat.SetTexture("_BumpMap", bumpMap);
                mat.EnableKeyword("_NORMALMAP");
            }

            if (metalMap != null)
            {
                mat.SetTexture("_MetallicGlossMap", metalMap);
                mat.EnableKeyword("_METALLICSPECGLOSSMAP");
            }

            if (emissMap != null)
            {
                mat.SetTexture("_EmissionMap", emissMap);
                mat.SetColor("_EmissionColor", emissColor);
                mat.EnableKeyword("_EMISSION");
            }

            if (occlMap != null)
            {
                mat.SetTexture("_OcclusionMap", occlMap);
                mat.EnableKeyword("_OCCLUSIONMAP");
            }

            if (isTransparent)
            {
                mat.SetFloat("_Surface", 1.0f);
                mat.SetFloat("_Blend", 0.0f);
                mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetFloat("_ZWrite", 0.0f);
                mat.renderQueue = 3000;
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }
            else
            {
                mat.SetFloat("_Surface", 0.0f);
                mat.SetFloat("_Blend", 0.0f);
                mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetFloat("_ZWrite", 1.0f);
                mat.renderQueue = -1;
                mat.SetOverrideTag("RenderType", "Opaque");
                mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }

            Debug.Log($"[URPMaterialAutoFixer] Material corregido a URP Lit: {mat.name}");
            return true;
        }

        /// <summary>
        /// Asegura que las propiedades _BaseMap y _MainTex estén sincronizadas.
        /// </summary>
        public static bool SynchronizeTextures(Material mat)
        {
            bool changed = false;

            if (mat.HasProperty("_MainTex") && mat.HasProperty("_BaseMap"))
            {
                Texture main = mat.GetTexture("_MainTex");
                Texture baseMap = mat.GetTexture("_BaseMap");

                if (baseMap == null && main != null)
                {
                    mat.SetTexture("_BaseMap", main);
                    mat.SetTextureScale("_BaseMap", mat.GetTextureScale("_MainTex"));
                    mat.SetTextureOffset("_BaseMap", mat.GetTextureOffset("_MainTex"));
                    changed = true;
                }
                else if (main == null && baseMap != null)
                {
                    mat.SetTexture("_MainTex", baseMap);
                    mat.SetTextureScale("_MainTex", mat.GetTextureScale("_BaseMap"));
                    mat.SetTextureOffset("_MainTex", mat.GetTextureOffset("_BaseMap"));
                    changed = true;
                }
            }

            return changed;
        }

        /// <summary>
        /// Busca archivos de textura en carpetas candidatas y las asigna según coincidencia de nombre.
        /// </summary>
        public static bool TryAutoAssignTextures(Material mat, string folder1, string folder2)
        {
            bool assigned = false;
            string matName = mat.name.ToLower().Replace(" ", "").Replace("_", "").Replace("-", "");

            List<string> candidateFolders = new List<string>();
            if (!string.IsNullOrEmpty(folder1) && Directory.Exists(folder1))
            {
                candidateFolders.Add(folder1);
                string sub1 = Path.Combine(folder1, "Textures");
                if (Directory.Exists(sub1)) candidateFolders.Add(sub1);
                string sub2 = Path.Combine(folder1, "Texture");
                if (Directory.Exists(sub2)) candidateFolders.Add(sub2);
                string parent = Path.GetDirectoryName(folder1);
                if (!string.IsNullOrEmpty(parent))
                {
                    string pSub = Path.Combine(parent, "Textures");
                    if (Directory.Exists(pSub)) candidateFolders.Add(pSub);
                }
            }

            if (!string.IsNullOrEmpty(folder2) && Directory.Exists(folder2) && !candidateFolders.Contains(folder2))
            {
                candidateFolders.Add(folder2);
                string sub = Path.Combine(folder2, "Textures");
                if (Directory.Exists(sub)) candidateFolders.Add(sub);
            }

            foreach (string folder in candidateFolders)
            {
                string[] files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".tga" && ext != ".tif" && ext != ".tiff" && ext != ".psd")
                        continue;

                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string cleanFileName = fileName.ToLower().Replace(" ", "").Replace("_", "").Replace("-", "");

                    // Comprobar coincidencia con el nombre del material
                    if (cleanFileName.Contains(matName) || matName.Contains(cleanFileName))
                    {
                        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(file);
                        if (tex == null) continue;

                        string lowerName = fileName.ToLower();

                        // Difuso / Albedo
                        if (mat.GetTexture("_BaseMap") == null && MatchesSuffix(lowerName, DiffuseSuffixes))
                        {
                            mat.SetTexture("_BaseMap", tex);
                            mat.SetTexture("_MainTex", tex);
                            assigned = true;
                            Debug.Log($"[URPMaterialAutoFixer] Textura Albedo asignada a {mat.name}: {fileName}");
                        }
                        // Normal map
                        else if (mat.GetTexture("_BumpMap") == null && MatchesSuffix(lowerName, NormalSuffixes))
                        {
                            mat.SetTexture("_BumpMap", tex);
                            mat.EnableKeyword("_NORMALMAP");
                            assigned = true;
                            Debug.Log($"[URPMaterialAutoFixer] Normal Map asignado a {mat.name}: {fileName}");
                        }
                        // Metálico / Mask map
                        else if (mat.GetTexture("_MetallicGlossMap") == null && MatchesSuffix(lowerName, MetallicSuffixes))
                        {
                            mat.SetTexture("_MetallicGlossMap", tex);
                            mat.EnableKeyword("_METALLICSPECGLOSSMAP");
                            assigned = true;
                            Debug.Log($"[URPMaterialAutoFixer] Metallic Map asignado a {mat.name}: {fileName}");
                        }
                        // Oclusión
                        else if (mat.GetTexture("_OcclusionMap") == null && MatchesSuffix(lowerName, OcclusionSuffixes))
                        {
                            mat.SetTexture("_OcclusionMap", tex);
                            mat.EnableKeyword("_OCCLUSIONMAP");
                            assigned = true;
                            Debug.Log($"[URPMaterialAutoFixer] Occlusion Map asignado a {mat.name}: {fileName}");
                        }
                        // Emisión
                        else if (mat.GetTexture("_EmissionMap") == null && MatchesSuffix(lowerName, EmissionSuffixes))
                        {
                            mat.SetTexture("_EmissionMap", tex);
                            mat.SetColor("_EmissionColor", Color.white);
                            mat.EnableKeyword("_EMISSION");
                            assigned = true;
                            Debug.Log($"[URPMaterialAutoFixer] Emission Map asignado a {mat.name}: {fileName}");
                        }
                        // Si no coincide con un sufijo especial y el material no tiene textura base
                        else if (mat.GetTexture("_BaseMap") == null &&
                                 !MatchesSuffix(lowerName, NormalSuffixes) &&
                                 !MatchesSuffix(lowerName, MetallicSuffixes) &&
                                 !MatchesSuffix(lowerName, EmissionSuffixes))
                        {
                            mat.SetTexture("_BaseMap", tex);
                            mat.SetTexture("_MainTex", tex);
                            assigned = true;
                            Debug.Log($"[URPMaterialAutoFixer] Textura Base asignada a {mat.name}: {fileName}");
                        }
                    }
                }

                if (assigned) break;
            }

            return assigned;
        }

        private static bool MatchesSuffix(string name, string[] suffixes)
        {
            foreach (string s in suffixes)
            {
                if (name.EndsWith(s) || name.Contains(s)) return true;
            }
            return false;
        }

        // --- Menús en la barra de herramientas ---

        [MenuItem("Tools/Materiales (URP)/Reparar Todos los Materiales Rosados")]
        public static void FixAllPinkMaterialsMenu()
        {
            string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
            int fixedCount = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat != null)
                {
                    if (ProcessMaterial(mat, path))
                    {
                        fixedCount++;
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "Reparar Materiales (URP)",
                fixedCount > 0 
                    ? $"¡Se repararon {fixedCount} materiales rosados con éxito a URP Lit!"
                    : "Todos los materiales ya están correctamente configurados para URP. ¡No se encontraron materiales rosados!",
                "OK"
            );
        }

        [MenuItem("Tools/Materiales (URP)/Auto-Vincular Texturas Faltantes")]
        public static void AutoAssignMissingTexturesMenu()
        {
            string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
            int fixedCount = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat != null)
                {
                    if (mat.GetTexture("_BaseMap") == null && mat.GetTexture("_MainTex") == null)
                    {
                        string folder = Path.GetDirectoryName(path);
                        if (TryAutoAssignTextures(mat, folder, null))
                        {
                            EditorUtility.SetDirty(mat);
                            fixedCount++;
                        }
                    }
                }
            }

            if (fixedCount > 0)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            EditorUtility.DisplayDialog(
                "Auto-Vincular Texturas",
                fixedCount > 0 
                    ? $"¡Se asignaron texturas faltantes a {fixedCount} materiales!"
                    : "No se encontraron materiales con texturas faltantes que requieran asignación.",
                "OK"
            );
        }
    }
}
