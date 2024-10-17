/*
 * 自动生成代码
 * 请勿编辑
 */
using System.Collections.Generic;
using System;
using UnityEngine;

public class CSVSetting : CSVData
{
    private static readonly CSVSetting ms_instance = new CSVSetting();

    public static CSVSetting Instance
    {
        get
        {
            return ms_instance;
        }
    }

    public Dictionary<ulong, CSVSetting> csvDataDic = new Dictionary<ulong, CSVSetting>();

    //定义字段
    /// <summary>
    /// settingID
    /// </summary>
    public int setting_id { get; private set; }
    /// <summary>
    /// 英文简写
    /// </summary>
    public string name { get; private set; }
    /// <summary>
    /// 数值
    /// </summary>
    public int int_value { get; private set; }
    /// <summary>
    /// 字符参数
    /// </summary>
    public string string_value { get; private set; }


    static string tableName = "setting";

    //对应的csv文件
    protected override string Name()
    {
        return tableName;
    }

    public static void Load()
    {
        Instance.LoadCSVTable();
    }
    
    /// <summary>
    /// 获取对象(通过多个Key,最多5个key)
    /// </summary>
    public static CSVSetting GetSetting(int setting_id)
    {
        ulong key = CSVHelper.GetKey(setting_id);
        CSVSetting data = Instance.Get(key);
        if(data == null)
        {
            Debug.LogErrorFormat("{0} 表 key {1} 出错了",tableName,setting_id);  
        }
        return data;
    }
   

    public static Dictionary<ulong, CSVSetting> GetAllDic(bool isCache = true)
    {
        return Instance.GetAll(isCache);
    }

    public static void UnLoad(bool isRemove = true)
    {
        Instance.UnLoadData(isRemove);
    }
    
    /// <summary>
    /// 通过ID获取对象
    /// </summary>
    private CSVSetting Get(ulong key, bool isCache = true)
    {
        LoadCSVTable();
        CSVSetting csvData;
        if (!csvDataDic.TryGetValue(key, out csvData))
        {
            CSVBytesData bytesData = GetCSVBytesData(key);
            if (bytesData == null)
            {
                return null;
            }
            csvData = GetCSVData(bytesData);
            if (isCache)
            {
                if (!csvDataDic.ContainsKey(key))
                {
                    csvDataDic.Add(key, csvData);
                }
            }
        }
        return csvData;
    }

    protected CSVSetting GetCSVData(CSVBytesData bytesData)
    {
        CSVSetting csvData = null;
        try
        {
            csvData = new CSVSetting();
            csvData.bytesData = bytesData;
            bytesData.BeginLoad();
            //设置字段值
    	    csvData.setting_id = bytesData.ReadIntValue();
            csvData.name = bytesData.ReadString();
    	    csvData.int_value = bytesData.ReadIntValue();
            csvData.string_value = bytesData.ReadString();

        }
        catch (Exception exception)
        {
            Debug.LogErrorFormat("{0}表 解析出错 {1}",tableName, exception.StackTrace);
            return null;
        }

        return csvData;
    }
    


    private Dictionary<ulong, CSVSetting> GetAll(bool isCache)
    {
        LoadCSVTable();
        Dictionary<ulong, CSVSetting> allDic = new Dictionary<ulong, CSVSetting> ();
        if (csvDataDic.Count == GetAllCSVBytesData().Count)
        {
            allDic = csvDataDic;
        }
        else
        {
            csvDataDic.Clear();
            var _itor = GetAllCSVBytesData().GetEnumerator();
            while (_itor.MoveNext())
            {
                CSVSetting csvData = Get(_itor.Current.Key);
                allDic.Add(_itor.Current.Key, csvData);
            }
            if (isCache)
            {
                csvDataDic = allDic;
            }
        }
        return allDic;
    }

    public override void UnLoadData(bool isRemove = true)
    {
        base.UnLoadData(isRemove);
        csvDataDic.Clear();
        if (isRemove)
        {
            CSVManager.Instance.RemoveCSVData(Name());
        }
    }
}
