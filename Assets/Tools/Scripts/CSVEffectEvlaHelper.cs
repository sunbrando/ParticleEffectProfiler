#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 * xuanziliang to do: 
 * * 保存数据帮助类
 */
public class CSVEffectEvlaHelper {
    private static CSVEffectEvlaHelper _csvEffectEvlaHelper = null;
    private static string _directoryPath = "";
    private string _csvPath = "";
    private FileStream _fileStream;
    private StreamWriter _streamWriter;

    public static CSVEffectEvlaHelper GetInstance()
    {
        if (_csvEffectEvlaHelper == null)
        {
            _csvEffectEvlaHelper = new CSVEffectEvlaHelper();
            _csvEffectEvlaHelper.Init();
        }

        return _csvEffectEvlaHelper;
    }
    public static void DestroyInstance()
    {
        _csvEffectEvlaHelper.Release();
        _csvEffectEvlaHelper = null;
    }

    public void Init()
    {
        string strTime = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);
        _directoryPath = string.Format("{0}/../{1}", Application.dataPath, strTime);
        _csvPath = string.Format("{0}/data{1}.csv", _directoryPath, strTime);
        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);
        
        
        _fileStream = new FileStream(_csvPath, FileMode.OpenOrCreate, FileAccess.Write);
        _streamWriter = new StreamWriter(_fileStream, System.Text.Encoding.UTF8);

        // 插入头
        _streamWriter.WriteLine("特效路径,"
            + "（低）原填充像素点," + "（低）实际填充像素点,（低）平均每像素overdraw,（低）带宽,"
            + "（中）原填充像素点,（中）实际填充像素点,（中）平均每像素overdraw,（中）带宽,"
            + "（高）原填充像素点,（高）实际填充像素点,（高）平均每像素overdraw,（高）带宽"
            );
            
        Debug.Log("数据文件存放路径为："+ _csvPath); 
    }

    public string GetDirectoryPath()
    {
        return _directoryPath;
    }

    public void Release()
    {
        _streamWriter.Close();
        _fileStream.Close();

        _streamWriter = null;
        _fileStream = null;
    }

    public void Flush()
    {
        if (_streamWriter == null) return;

        _streamWriter.Flush();
    }

    public void WriteData(EffectEvlaData2[] effectEvlaData)
    {
        if (_streamWriter == null) return;

        string data = effectEvlaData[effectEvlaData.Length - 1].effectPath;
        for(int i = effectEvlaData.Length-1; i >= 0; i--)
        {
            EffectEvlaData2 effectEvlaData2 = effectEvlaData[i];
            data = string.Format("{0},{1},{2},{3},{4}", data,
            effectEvlaData2.GetPixDrawAverage(),
            effectEvlaData2.GetPixActualDrawAverage(),
            effectEvlaData2.GetPixRate(),
            effectEvlaData2.tape);

            /*data = data + ","
                + effectEvlaData2.GetPixDrawAverage() + ","
                + effectEvlaData2.GetPixActualDrawAverage() + ","
                + effectEvlaData2.GetPixRate() + ","
                + effectEvlaData2.tape;*/
        }

        //Debug.Log(data);
        /*string data = string.Format("{0},{1},{2},{3},{4}", 
            effectEvlaData.effectPath, effectEvlaData.GetPixDrawAverage(), 
            effectEvlaData.GetPixActualDrawAverage(), 
            effectEvlaData.GetPixRate(), 
            effectEvlaData.tape);*/
        //Debug.Log("effectEvlaData.GetPixDrawAverage()"+ effectEvlaData.GetPixDrawAverage()+"d "+ effectEvlaData.GetPixActualDrawAverage()+"times "+ effectEvlaData.pixDrawTimes + "a "+effectEvlaData.pixActualDrawTotal);
        try
        {
            _streamWriter.WriteLine(data);
        }
        catch(Exception e)
        {
            
            Debug.LogError(e.ToString());
        }
        
    }
}
#endif
