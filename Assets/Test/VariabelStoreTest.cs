using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariabelStoreTest : MonoBehaviour
{
    public int var_int = 0;
    public float var_flt = 0;
    public bool var_bool = false;
    public string var_str = "";

    // Start is called before the first frame update
    void Start()
    {
        VariableStore.CreateDataBase("DB_Links");

        VariableStore.CreateVariable("DB_Links.l_link", var_int, () => var_int, value => var_int = value);
        VariableStore.CreateVariable("DB_Links.l_flt", var_flt, () => var_flt, value => var_flt = value);
        VariableStore.CreateVariable("DB_Links.l_bool", var_bool, () => var_bool, value => var_bool = value);
        VariableStore.CreateVariable("DB_Links.l_str", var_str, () => var_str, value => var_str = value);


        VariableStore.CreateDataBase("DB_Numbers");
        VariableStore.CreateDataBase("DB_Bool");
        VariableStore.CreateDataBase("DB69");

        VariableStore.CreateVariable("DB_Numbers.num1", 1);
        VariableStore.CreateVariable("DB_Numbers.num5", 5);
        VariableStore.CreateVariable("DB_Bool.lightOn", true);
        VariableStore.CreateVariable("DB_Numbers.float", 7.6);
        VariableStore.CreateVariable("str1", "Hello ");
        VariableStore.CreateVariable("str2", "World");

        VariableStore.PrintAllDataBase();

        VariableStore.PrintAllVariables();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            VariableStore.PrintAllVariables();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            string variable = "DB_Num.num1";
            VariableStore.TryGetValue(variable, out object v);
            VariableStore.TrySetValue(variable, (int)v + 5);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            VariableStore.TryGetValue("DB_Num.num1", out object num1);
            VariableStore.TryGetValue("DB_Num.num5", out object num2);

            Debug.Log($"num1 + num2 = {(int)num1 + (int)num2}");
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (VariableStore.TryGetValue("DB_Bool.lightOn", out object lightison) && lightison is bool)
                VariableStore.TrySetValue("DB_Bool.lightOn", !(bool)lightison);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            VariableStore.TryGetValue("str1", out object str_hello);
            VariableStore.TryGetValue("str2", out object str_world);

            VariableStore.TrySetValue("str1", (string)str_hello + str_world);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            VariableStore.TryGetValue("DB_Links.l_link", out object linked_int);
            VariableStore.TrySetValue("DB_Links.l_link", (int)linked_int + 5);
            
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            VariableStore.RemoveVariabel("DB_Links.l_link");
            VariableStore.RemoveVariabel("DB_Bool.lightOn");

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            VariableStore.RemoveAllVariable();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            VariableStore.TryGetValue("DB_Links.l_flt", out object linked_flt);
            VariableStore.TrySetValue("DB_Links.l_flt", (float)linked_flt * 1.75f);

        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            VariableStore.TryGetValue("DB_Links.l_bool", out object linked_bool);
            VariableStore.TrySetValue("DB_Links.l_bool", !(bool)linked_bool);

        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            VariableStore.TryGetValue("DB_Links.l_str", out object linked_str);
            VariableStore.TrySetValue("DB_Links.l_str", (string)linked_str + Random.Range(1000,2000));

        }

    }
}
