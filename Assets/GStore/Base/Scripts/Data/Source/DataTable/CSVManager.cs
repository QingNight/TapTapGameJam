using GStore;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVManager : Singleton<CSVManager>
{
    public static Func<string, string> getRealTableName { set; private get; }
    /// <summary>
    /// 已加载表的单例缓存，比如CSVHero.Instance
    /// </summary>
    Dictionary<string, CSVData> csvDataDic = new Dictionary<string, CSVData>();
    /// <summary>
    /// 已加载表的二进制数据，每个表的单例包含一个CSVTable
    /// </summary>
    Dictionary<string, CSVTable> csvTableDic = new Dictionary<string, CSVTable>();

    public CSVTable GetCSVTable(string tableName)
    {

        CSVTable csvTable;

        if (csvTableDic.ContainsKey(tableName))
        {
            csvTable = csvTableDic[tableName];
        }
        else
        {
            string originName = tableName;
            if (getRealTableName != null)
            {
                tableName = getRealTableName(tableName);
            }
            string path = CSVHelper.Combine(tableName, ".bytes");
            csvTable = GetCSVTableFromFile(tableName, path);
            if (csvTable.GetAllCSVBytesData().Count <= 0 && tableName != originName) //读表失败，尝试读原表
            {
                UnityEngine.Debug.LogError("ab table fail: " + tableName + " " + originName);
                path = CSVHelper.Combine(originName, ".bytes");
                csvTable = GetCSVTableFromFile(originName, path);
            }
            csvTableDic.Add(originName, csvTable);
        }
        return csvTable;
    }

    private CSVTable GetCSVTableFromFile(string tableName, string path)
    {
        CSVTable csvTable = new CSVTable();
        //string path = CSVHelper.Combine(tableName, ".bytes");
#if TableLog
            Debug.LogErrorFormat("path {0}", path);
#endif
        Dictionary<ulong, CSVBytesData> csvBytesDataDic = new Dictionary<ulong, CSVBytesData>();
        byte[] data = CSVHelper.GetBytes(path);
        if (data == null)
        {
            Debug.LogErrorFormat("不存在表 {0}", path);
            return csvTable;
        }
        MemoryStream stream = new MemoryStream(data);
        BinaryReader reader = null;
        List<TableField> tableFieldList = new List<TableField>();
        try
        {
            reader = new BinaryReader(stream);

            int rowsCount = reader.ReadInt32(); //行数，不包括标题行
            int columnsCount = reader.ReadInt32(); //列数（也就是csv表每行字段数）
#if TableLog
            Debug.LogErrorFormat("rowsCount {0} columnsCount {1}", rowsCount, columnsCount);
#endif

            for (int i = 0; i < columnsCount; i++)
            {
                TableField tableField = new TableField();
                tableField.isBase = reader.ReadBoolean();
                tableField.isList = reader.ReadBoolean();
                tableField.define = (TableDefine)reader.ReadByte();
                tableField.fieldType = (TableBaseType)reader.ReadByte();
                tableFieldList.Add(tableField);
#if TableLog
                Debug.LogErrorFormat("isBase {0} isList {1} fieldType byte {2} type {3}", tableField.isBase, tableField.isList, (byte)tableField.fieldType, tableField.fieldType);
#endif
            }

            for (int i = 0; i < rowsCount; i++)
            {
                ulong key = reader.ReadUInt64();

                int count = reader.ReadInt32();
                byte[] allFieldData = reader.ReadBytes(count);
                CSVBytesData csvDate = new CSVBytesData();
                csvDate.Init(allFieldData, tableFieldList, tableName);
                csvBytesDataDic.Add(key, csvDate);
            }
        }
        catch (Exception exception)
        {
            Debug.LogErrorFormat("{0}表 二进制数据解析出错 {1}", tableName, exception.ToString());
            Debug.LogException(exception);
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
        }
        csvTable.Init(csvBytesDataDic, tableFieldList);

        return csvTable;
    }
#if UNITY_EDITOR
    public int GetTableColumnsCount(string path)
    {
        int count = -1;
        BinaryReader reader = null;
        try
        {
            byte[] data = CSVHelper.GetBytes(path);
            MemoryStream stream = new MemoryStream(data);
            List<TableField> tableFieldList = new List<TableField>();
            reader = new BinaryReader(stream);

            int rowsCount = reader.ReadInt32(); //行数，不包括标题行
            int columnsCount = reader.ReadInt32(); //列数（也就是csv表每行字段数）

            count = columnsCount;
        }
        catch (Exception exception)
        {
            Debug.LogErrorFormat("{0}表 二进制数据解析出错 {1}", path, exception.ToString());
            Debug.LogException(exception);
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
        }

        return count;
    }
#endif

    public void AddCSVData(string tableName,CSVData csvData)
    {
        csvDataDic.Add(tableName, csvData);
    }

    public void RemoveCSVData(string tableName)
    {
        if (csvDataDic.ContainsKey(tableName))
        {
            csvDataDic.Remove(tableName);
        }
        if (csvTableDic.ContainsKey(tableName))
        {
            csvTableDic.Remove(tableName);
        }
        
    }

    public void UnLoadData(string tableName)
    {
        if (csvDataDic.ContainsKey(tableName))
        {
            csvDataDic[tableName].UnLoadData();
        }
        if (csvTableDic.ContainsKey(tableName))
        {
            csvTableDic[tableName].UnLoad();
        }
        RemoveCSVData(tableName);
    }

    public void UnLoadALlTable()
    {
        //Debug.LogErrorFormat("UnLoadALlTable");
        foreach (CSVData item in csvDataDic.Values)
        {
            item.UnLoadData(false);
        }
        csvDataDic.Clear();
        csvTableDic.Clear();
    }

#region lua访问接口
    public static CSVTable GetTable(string tableName)
    {
        return Instance.GetCSVTable(tableName);
    }
    public CSVBytesData GetBytesData(string tableName, int key1)
    {
        CSVTable table = GetCSVTable(tableName);
        CSVBytesData csvBytesData = null;
        if (table != null)
        {
            csvBytesData = table.GetByKey(key1);
        }
        return csvBytesData;
    }

    public CSVBytesData GetBytesData(string tableName, int key1, int key2)
    {
        CSVTable table = GetCSVTable(tableName);
        CSVBytesData csvBytesData = null;
        if (table != null)
        {
            csvBytesData = table.GetByKey(key1, key2);
        }
        return csvBytesData;
    }

    public CSVBytesData GetBytesData(string tableName, int key1, int key2, int key3)
    {
        CSVTable table = GetCSVTable(tableName);
        CSVBytesData csvBytesData = null;
        if (table != null)
        {
            csvBytesData = table.GetByKey(key1, key2, key3);
        }
        return csvBytesData;
    }

    public CSVBytesData GetBytesData(string tableName, int key1, int key2, int key3, int key4)
    {
        CSVTable table = GetCSVTable(tableName);
        CSVBytesData csvBytesData = null;
        if (table != null)
        {
            csvBytesData = table.GetByKey(key1, key2, key3, key4);
        }
        return csvBytesData;
    }

    public CSVBytesData GetBytesData(string tableName, int key1, int key2, int key3, int key4, int key5)
    {
        CSVTable table = GetCSVTable(tableName);
        CSVBytesData csvBytesData = null;
        if (table != null)
        {
            csvBytesData = table.GetByKey(key1, key2, key3, key4, key5);
        }
        return csvBytesData;
    }
#endregion
}
