using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class VariableStore
{
    private const string Default_DataBase_Name = "Default";
    public const char DataBase_Variable_Relation_Id = '.';

    public class DataBase 
    { 
        public DataBase(string name) 
        {
            this.name = name;
            Variables = new Dictionary<string, Variable>();
        }

        public string name;
        public Dictionary<string, Variable> Variables = new Dictionary<string, Variable>();
    }

    public abstract class Variable 
    {
        public abstract object Get();
        public abstract void Set(object value);
    }

    public class Variable<T> : Variable
    {
        private T value;

        private Func<T> getter;
        private Action<T> setter;

        //linked to extrenal DataBase
        public Variable(T defaultValue = default, Func<T> getter = null, Action<T> setter = null)
        {
            value = defaultValue;

            if (getter == null)
                this.getter = () => value;
            else
                this.getter = getter;

            if (setter == null)
                this.setter = newValue => value = newValue;
            else
                this.setter = setter;   
        }

        public override object Get() => getter();

        public override void Set(object newValue) => setter((T)newValue);
    }

    private static Dictionary<string, DataBase> dataBases = new Dictionary<string, DataBase>() { { Default_DataBase_Name, new DataBase(Default_DataBase_Name)} };
    private static DataBase DefaultDataBase  => dataBases[Default_DataBase_Name];
   
    public static bool CreateDataBase(string name)
    {
        if (!dataBases.ContainsKey(name))
        {
            dataBases[name] = new DataBase(name);
            return true;
        }
        return false;
    }

    public static DataBase GetDataBase(string name)
    { 
        if(name == string.Empty)
        {
            return DefaultDataBase;
        }

        if (!dataBases.ContainsKey(name))
        {
            CreateDataBase(name);
        }
        return dataBases[name];
    }

    public static bool CreateVariable<T>(string name, T defaultValue, Func<T> getter = null, Action<T> setter = null)
    {
        (string[] parts, DataBase db, string variableName) = ExtractInfo(name);

        if (db.Variables.ContainsKey(variableName))
        {
            return false;
        }

        db.Variables[variableName] = new Variable<T>(defaultValue, getter, setter);

        return true;
    }

    public static bool TryGetValue(string name, out object variable)
    {
        (string[] parts, DataBase db, string variableName) = ExtractInfo(name);

        if (!db.Variables.ContainsKey(variableName))
        {
            variable = null;
            return false;
        }
        variable = db.Variables[variableName].Get();
        return true;
    }

    public static bool TrySetValue<T>(string name, T value)
    {
        (string[] parts, DataBase db, string variableName) = ExtractInfo(name);

        if (!db.Variables.ContainsKey(variableName))
            return false;

        db.Variables[variableName].Set(value);
        return true;
    }

    private static(string[], DataBase, string) ExtractInfo(string name)
    {
        string[] parts = name.Split(DataBase_Variable_Relation_Id);
        DataBase db = parts.Length > 1 ? GetDataBase(parts[0]) : DefaultDataBase;
        string variableName = parts.Length > 1 ? parts[1] : parts[0];

        return (parts, db, variableName);
    }

    public static void PrintAllDataBase()
    {
        foreach(KeyValuePair<string, DataBase> dbEntry in dataBases)
        {
            Debug.Log($"DataBase : '<color=#FFB145>{dbEntry.Key}</color>'");
        }
    }

    public static void RemoveVariabel(string name)
    {
        (string[] parts, DataBase db, string variableName) = ExtractInfo(name);
        if(db.Variables.ContainsKey(variableName))
           db.Variables.Remove(variableName);
    }

    public static void RemoveAllVariable()
    {
        dataBases.Clear();
        dataBases[Default_DataBase_Name] = new DataBase(Default_DataBase_Name);
    }

    public static void PrintAllVariables(DataBase database = null)
    {
        if(database != null)
        {
            PrintAllDatabaseVariables(database);
            return;
        }

        foreach (var dbEntry in dataBases)
        {
            PrintAllDatabaseVariables(dbEntry.Value);
        }
    }

    private static void PrintAllDatabaseVariables(DataBase database)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Database: <color=#F38544>{database.name}</color>");
        foreach (KeyValuePair<string, Variable> variablePair in database.Variables)
        {
            string variableName = variablePair.Key;
            object variableValue = variablePair.Value.Get();
            sb.AppendLine($"\t<color=#FFB145>Variable [{variableName}]</color> = <color=#FFD22D>{variableValue}</color>");
        }
        Debug.Log(sb.ToString());
    }
}
