#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/*
 * xuanziliang to do: 
 * * 特效评测类
 * EffectEvla2：整套运行的代码
 * SingleEffectEvla：单个特效对象，里面有模型和特效的数据
 * CSVEffectEvlaHelper：保存数据帮助类
 */

public class EffectEvla2 : EffectEvla
{
    public static string[] Qualitys = { "High", "Middle", "Low" };
    private string[] effectSearchPath = { "Assets/Biwu2/DynamicArt/Effect" };
    private SingleEffectEvla[] singleEffectEvlas = null;
    private int _arrayIndex = 0;
    private float stopTime = 2;// 3秒停止一个特效
    private float saveTimeMax = 0.0333f; // 每0.3秒记录一次数据
    private float saveTime = 0; // 每0.3秒记录一次数据
    private bool isRealese = false;

    private int _qualityIndex =0;
    string[] pathsSelect = null;

    public EffectEvla2(string[] paths)
    {
        this.pathsSelect = paths;
        
        _qualityIndex = Qualitys.Length;
    }

    public void Init()
    {
        this.Init(Camera.main);
    }

    public override void Init(Camera camera)
    {
        CSVEffectEvlaHelper.GetInstance();

        base.Init(camera);

        ChangeQuality(Qualitys[_qualityIndex - 1]);

        InitData(camera);
    }

    void InitData(Camera camera)
    {
        Transform effectHolder = camera.gameObject.transform.Find("effect");
        Vector3 position_effect = effectHolder.position; 

        // 搜索路径下所有的特效
        string[] paths = this.pathsSelect; 
        if (paths == null)
        {
            var fabs = AssetDatabase.FindAssets("t:prefab", effectSearchPath);
            paths = new string[fabs.Length];
            for (int i = 0; i < fabs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(fabs[i]);
                paths[i] = path;
            }
        }

        singleEffectEvlas = new SingleEffectEvla[paths.Length];

        for (int i = 0; i < paths.Length; i++)
        {
            string path = paths[i];
            float progress = (i + 1.0f) / paths.Length;
            EditorUtility.DisplayProgressBar("加载特效中，请耐心等待", "加载特效：" + path, progress);

            SingleEffectEvla singleEffectEvla = new SingleEffectEvla(_qualityIndex);
            singleEffectEvla.LoadEffect(path, position_effect);

            singleEffectEvlas[i] = singleEffectEvla;
        }
        EditorUtility.ClearProgressBar();
    }

    public void Release()
    {
        if (isRealese) return;
        isRealese = true;
        for (int i = 0; i < singleEffectEvlas.Length; i++)
        {
            SingleEffectEvla singleEffectEvla = singleEffectEvlas[i];
            singleEffectEvla.UnLoadEffect();
        }
        singleEffectEvlas = null;

        CSVEffectEvlaHelper.GetInstance();
        CSVEffectEvlaHelper.DestroyInstance();
    }
    
    public bool Update(bool isAutoSavePic, int maxOverdrawSaveNumber)
    {
     
        // 当前effect
        SingleEffectEvla singleEffectEvla = GetSingleEffectEvlaNow();
        if (singleEffectEvla == null)
        {
            time = 0;
            _arrayIndex = 0;
            _qualityIndex = _qualityIndex - 1;

            if (_qualityIndex >= 1)
            {
                Debug.Log("品质改变"+ GetQualityChecking());
                ChangeQuality(Qualitys[_qualityIndex-1]);

                foreach(SingleEffectEvla sEffectEvla in singleEffectEvlas)
                {
                    sEffectEvla.ChangeQuality(_qualityIndex);
                }
                return true;
            }

            // 保存csv
            try
            {
                SaveData();
            }
            catch(Exception e)
            {
                Debug.LogError(e.ToString());
            }
            Debug.Log("运行所有特效完毕，已保存数据");
            return false;
        }

        time += Time.deltaTime;
        saveTime += Time.deltaTime;

        // 
        singleEffectEvla.Simsimulate(time);

        // 记录本次数据
        if (saveTime >= saveTimeMax)
        {
            RecordOverDrawData(singleEffectEvla);
            saveTime = 0;
        }
        
        // 超过总时间，自动跳到下一个effect
        if (time > stopTime)
        {
            time = 0;
            _arrayIndex++;
            // 因为取的是平均值，所以可以最后一帧才判断截图
            CheckAndSaveTexture(singleEffectEvla, isAutoSavePic, maxOverdrawSaveNumber);
            singleEffectEvla.Stop();
        }

        return true;
    }

    public SingleEffectEvla GetSingleEffectEvlaNow()
    {
        if (singleEffectEvlas == null)
        {
            return null;
        }
        if (_arrayIndex >= singleEffectEvlas.Length)
        {
            return null;
        }

        return singleEffectEvlas[_arrayIndex];
    }

    public string GetEffectPathShowNow()
    {
        SingleEffectEvla singleEffectEvla = GetSingleEffectEvlaNow();
        if (singleEffectEvla != null)
            return singleEffectEvla.GetEffectEvlaData().effectPath;

        return "没有显示的特效";
    }

    public int GetRemainNumber()
    {
        return singleEffectEvlas.Length - _arrayIndex - 1;
    }


    #region 记录数据
    public void SaveData()
    {
        // 排序
        Array.Sort(singleEffectEvlas, new SingleEffectEvlaComparer2());

        for (int i = 0; i < singleEffectEvlas.Length; i++)
        {
            SingleEffectEvla singleEffectEvla = singleEffectEvlas[i];
            EffectEvlaData2 effectEvlaData2 = singleEffectEvla.GetEffectEvlaData();
            // pixrate大于1.5的才记录
            if (effectEvlaData2.GetPixRate() >= 1.5f)
            {
                CSVEffectEvlaHelper cSVEffectEvlaHelper = CSVEffectEvlaHelper.GetInstance();
                cSVEffectEvlaHelper.WriteData(singleEffectEvla.GetEffectEvlaDatas());
                cSVEffectEvlaHelper.Flush();
            }   
        }
    }
    #endregion

    void CheckAndSaveTexture(SingleEffectEvla singleEffectEvla, bool isAutoSavePic, int maxOverdrawSaveNumber)
    {
        if (!isAutoSavePic)
        {
            return;
        }
        // 获取data
        EffectEvlaData2 effectEvlaData2 = singleEffectEvla.GetEffectEvlaData();
        double overdrawRate = effectEvlaData2.GetPixRate();
        if (overdrawRate < maxOverdrawSaveNumber)
        {
            return;
        }

        CSVEffectEvlaHelper cSVEffectEvlaHelper = CSVEffectEvlaHelper.GetInstance();
        string directoryPath = cSVEffectEvlaHelper.GetDirectoryPath();
        FileInfo fi = new FileInfo(directoryPath);
        if (!fi.Directory.Exists)
        {
            fi.Directory.Create();
        }
        string fileName = string.Format("{0}/{1}{2}.png", directoryPath, effectEvlaData2.GetEffectName(), effectEvlaData2.quality);

        RenderTexture activeTextrue = RenderTexture.active;
        RenderTexture rt = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
        _camera.targetTexture = rt;
        _camera.Render();
        RenderTexture.active = rt;
        Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        byte[] bytes = png.EncodeToPNG();

        FileStream file = File.Open(fileName, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();

        RenderTexture.active = activeTextrue;
        Texture2D.DestroyImmediate(png);
        rt.Release();

        _camera.targetTexture = null;
    }


    public void PrintAll()
    {
        for (int i = 0; i < singleEffectEvlas.Length; i++)
        {
            SingleEffectEvla singleEffectEvla = singleEffectEvlas[i];
            singleEffectEvla.Print();
        }
    }

    public void ChangeQuality(string quality)
    {
        if (quality == "High")
        {
            _camera.cullingMask = LayerMask.GetMask(new string[] { "EffLevel3", "EffLevel2", "EffLevel1", "Default" });
        }
        else if(quality == "Middle")
        {
            _camera.cullingMask = LayerMask.GetMask(new string[] { "EffLevel2", "EffLevel1", "Default" });
        }
        else
        {
            _camera.cullingMask = LayerMask.GetMask(new string[] { "EffLevel1", "Default" });
        }
    }

    public string GetQualityChecking()
    {
        if (_qualityIndex >= 1 && Qualitys.Length > 0)
            return Qualitys[_qualityIndex-1];
        return "无";
    }
}

// 排序
public class SingleEffectEvlaComparer2 : IComparer
{
    public int Compare(object e1, object e2)
    {
        SingleEffectEvla singleEffectEvla1 = e1 as SingleEffectEvla;
        SingleEffectEvla singleEffectEvla2 = e2 as SingleEffectEvla;
        if (singleEffectEvla1 == null || singleEffectEvla2 == null)
        {
            throw new ArgumentException("null argument");
        }

        EffectEvlaData2 effectEvlaData1 = singleEffectEvla1.GetEffectEvlaData();
        EffectEvlaData2 effectEvlaData2 = singleEffectEvla2.GetEffectEvlaData();

        double pixRate1 = effectEvlaData1.GetPixRate();
        double pixRate2 = effectEvlaData2.GetPixRate();

        if (pixRate1 < pixRate2)
        {
            return 1;
        }
        if (pixRate1 > pixRate2)
        {
            return -1;
        }
        return 0;
    }
}
#endif
