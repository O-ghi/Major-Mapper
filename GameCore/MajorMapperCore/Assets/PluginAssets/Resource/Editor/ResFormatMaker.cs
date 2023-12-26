/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2019.03.26
*/
using UnityEditor;
using UnityEngine;

public class ResFormatMaker //: AssetPostprocessor
{
  /*  private string dealAsset;
    //模型导入之前调用
    public void OnPreprocessModel()
    {
        //SetAssetFormat(this.assetPath, false);
    }

    //纹理导入之前调用，针对入到的纹理进行设置
    public void OnPreprocessTexture()
    {
        //SetAssetFormat(this.assetPath, false);
    }

    //音频导入
    public void OnPreprocessAudio()
    {
       // SetAssetFormat(this.assetPath, false);
    }

    public static void CopySetting(string formAssetPath, string toAssetPath, BuildTarget target)
    {
        var importera = AssetImporter.GetAtPath(formAssetPath);
        var importerb = AssetImporter.GetAtPath(toAssetPath);

        if (importera == null || importerb == null)
            return;

        //贴图
        var ta = importera as TextureImporter;
        var tb = importerb as TextureImporter;
        if (ta != null && tb != null)
        {
            //tb.SetPlatformTextureSettings(ta.GetPlatformTextureSettings(StriOS));
            if(target == BuildTarget.Android)
            {
                tb.SetPlatformTextureSettings(ta.GetPlatformTextureSettings(StrAndroid));
            }
            else
            {
                tb.SetPlatformTextureSettings(ta.GetPlatformTextureSettings(StriOS));
            }
            tb.mipmapEnabled = false;
            //tb.textureFormat = TextureImporterFormat.DXT5;
            //Debuger.Log("tb mipmap " + tb.mipmapEnabled);
            //Debuger.Log("tb format " + tb.textureFormat);
            //Debuger.Log("tb quality " + tb.compressionQuality);

            tb.SaveAndReimport();
            //Debuger.Log("2 tb mipmap " + tb.mipmapEnabled);
            //Debuger.Log("2 tb format " + tb.textureFormat);
            //Debuger.Log("2 tb quality " + tb.compressionQuality);

            return;
        }

        //声音
        var aa = importera as AudioImporter;
        var ab = importerb as AudioImporter;
        if (aa != null && ab != null)
        {
            ab.SetOverrideSampleSettings(StriOS, aa.GetOverrideSampleSettings(StriOS));
            ab.SetOverrideSampleSettings(StrAndroid, aa.GetOverrideSampleSettings(StrAndroid));
            ab.SaveAndReimport();
            return;
        }

        //模型
        var ma = importera as ModelImporter;
        var mb = importera as ModelImporter;
        if (ma != null && mb != null)
        {
            mb.isReadable = ma.isReadable;
            mb.importAnimatedCustomProperties = ma.importAnimatedCustomProperties;
            mb.animationCompression = ma.animationCompression;
            mb.swapUVChannels = ma.swapUVChannels;
            mb.importBlendShapes = ma.importBlendShapes;
            mb.importNormals = ma.importNormals;
            mb.normalCalculationMode = ma.normalCalculationMode;
            mb.normalSmoothingAngle = ma.normalSmoothingAngle;
            mb.importTangents = ma.importTangents;
            mb.importLights = ma.importLights;
            mb.weldVertices = ma.weldVertices;
            mb.animationType = ma.animationType;
            mb.optimizeGameObjects = ma.optimizeGameObjects;
            mb.optimizeMeshVertices = ma.optimizeMeshVertices;
            mb.indexFormat = ma.indexFormat;
            mb.keepQuads = ma.keepQuads;
            mb.materialImportMode = ma.materialImportMode;
            mb.importVisibility = ma.importVisibility;
            mb.importCameras = ma.importCameras;
            mb.preserveHierarchy = ma.preserveHierarchy;
            mb.addCollider = ma.addCollider;
            mb.importAnimation = ma.importAnimation;

            mb.generateSecondaryUV = ma.generateSecondaryUV;
            mb.secondaryUVAngleDistortion = ma.secondaryUVAngleDistortion;
            mb.secondaryUVAreaDistortion = ma.secondaryUVAreaDistortion;
            mb.secondaryUVHardAngle = ma.secondaryUVHardAngle;
            mb.secondaryUVPackMargin = ma.secondaryUVPackMargin;
            mb.SaveAndReimport();
            return;
        }
    }

    [MenuItem("Tools/Resource/自动设置资源格式 #&F")]
    public static void AutoAssetFormat()
    {
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        for (int i = 0; i < objs.Length; ++i)
        {
            EditorUtility.DisplayProgressBar("格式设置", "正在设置格式:" + objs[i].name, (float)i / objs.Length);
            //SetAssetFormat(AssetDatabase.GetAssetPath(objs[i]));
        }
        EditorUtility.ClearProgressBar();
        Debuger.Log("批量格式设置完成");
    }

    private const string StriOS = "iPhone";
    private const string StrAndroid = "Android";
    public static void SetAssetFormat(string assetPath, bool save = true)
    {
        var importer = AssetImporter.GetAtPath(assetPath);
        if (importer == null)
            return;

        //贴图
        var ti = importer as TextureImporter;
        if (ti != null)
        {
            for (int i = 0; i < 2; ++i)
            {
                var platfrom = i == 0 ? StriOS : StrAndroid;
                var settings = ti.GetPlatformTextureSettings(platfrom);
                settings.overridden = true;
                settings.textureCompression = TextureImporterCompression.Compressed;
                if (platfrom == StrAndroid)
                {
                    settings.crunchedCompression = true;
                    if (ti.DoesSourceTextureHaveAlpha())
                        settings.format = TextureImporterFormat.ETC2_RGBA8Crunched;
                    else
                        settings.format = TextureImporterFormat.ETC_RGB4Crunched;
                }
                else
                {
                    if (ti.DoesSourceTextureHaveAlpha())
                        settings.format = TextureImporterFormat.ASTC_8x8;
                    else
                        settings.format = TextureImporterFormat.ASTC_8x8;
                }
                ti.SetPlatformTextureSettings(settings);
            }
            ti.isReadable = false;
            ti.mipmapEnabled = false;
        }

        //声音
        var ai = importer as AudioImporter;
        if (ai != null)
        {
            ai.ambisonic = false;
            ai.forceToMono = false;
            ai.loadInBackground = false;
            AudioImporterSampleSettings setting = new AudioImporterSampleSettings();
            setting.quality = 0.5f;
            setting.loadType = AudioClipLoadType.Streaming;
            setting.compressionFormat = AudioCompressionFormat.Vorbis;
            setting.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
            ai.SetOverrideSampleSettings(StriOS, setting);
            ai.SetOverrideSampleSettings(StrAndroid, setting);
        }

        //模型
        var mi = importer as ModelImporter;
        if (mi != null)
        {
            mi.materialImportMode = ModelImporterMaterialImportMode.None;
            mi.isReadable = false;
            mi.optimizeMeshVertices = true;
            mi.importBlendShapes = false;
            mi.importCameras = false;
            mi.importLights = false;
            mi.importVisibility = false;
            mi.optimizeGameObjects = true;
        }

        if (save)
            importer.SaveAndReimport();
    }*/
}