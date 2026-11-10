using System.Collections.Generic;
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    [Tooltip("Whether to reject duplicate IDs if they have different types. Duplicate IDs " + 
    " with the same type are always overwritten. For example, if this is set to true, " + 
    "you cannot add a float with id 'foo' and a int with int 'foo'.")]
    [SerializeField] private bool strictIDs = true;

    private readonly Dictionary<int, float> floats = new();
    private readonly Dictionary<int, int> ints = new();
    private readonly Dictionary<int, bool> bools = new();
    private readonly Dictionary<int, string> strings = new();
    private readonly Dictionary<int, Vector3> vector3s = new();
    private readonly Dictionary<int, GameObject> gameObjects = new();

    private readonly HashSet<int> ids = new();

    /// <summary> Convert a string name to a int ID. </summary>
    /// <param name="name"> The string name to convert. </param>
    /// <returns> The ID </returns>
    public static int GetID(string name) => Animator.StringToHash(name);

    public bool HasID(string name) => ids.Contains(GetID(name));

    public bool HasID(int id) => ids.Contains(id);

    /// <summary> Get a float variable with a certain name. If the variable with
    /// the name is not found, returns 0. Capital-sensitive. </summary>
    public float GetFloat(string name) => GetFloat(GetID(name));

    /// <summary> Get the float value of a variable given a certain ID.
    /// If no variable with that ID is found, returns 0. </summary>
    public float GetFloat(int id) => floats.GetValueOrDefault(id, 0f);

    /// <summary> Set the value of a float variable with a certain name. 
    /// Returns whether the setting was succesful. </summary>
    public bool SetFloat(string name, float newValue) => SetFloat(GetID(name), newValue);

    /// <summary> Set a float variable with a certain value. Returns whether 
    /// setting the value was succesful. Can fail if strict ID checks are on. </summary> 
    public bool SetFloat(int id, float newValue)
    {
        if(strictIDs && ids.Contains(id) && !floats.ContainsKey(id)) return false;

        floats[id] = newValue;
        ids.Add(id);
        return true;
    }

    /// <summary> Get the value of an int variable, given the variable name. </summary>
    public int GetInt(string name) => GetInt(GetID(name));

    /// <summary> Get the value of an int variable, given the variable ID. Returns 0 if the 
    /// variable with the given ID does not exist. </summary>
    public int GetInt(int id) => ints.GetValueOrDefault(id, 0);

    /// <summary> Set or update the value of an variable, given the variable name and the new value. 
    /// Returns whether setting the value was succesful. Can fail if strict ID check flag is set. </summary>
    public bool SetInt(string name, int newValue) => SetInt(GetID(name), newValue);

    /// <summary>  Set or update the value of a variable, given the ID and new value.
    ///  Returns whether setting the variable was succesful. Can fail is strict ID check flag is set. </summary>
    public bool SetInt(int id, int newValue)
    {
        if(strictIDs && ids.Contains(id) && !ints.ContainsKey(id)) return false;

        ints[id] = newValue;
        ids.Add(id);
        return true;
    }

    /// <summary> Get the boolean variable with a certain string name. Returns false
    /// if the variable cannot be found. </summary>
    public bool GetBool(string name) => GetBool(GetID(name));

    /// <summary> Get the boolean variable with a certain ID. Returns false if
    ///  the variable cannot be found. </summary>
    public bool GetBool(int id) => bools.GetValueOrDefault(id, false);

    /// <summary> Set a boolean variable with a certain name. Returns whether the settings was 
    /// succesful. Can fail if strict ID check flag is set. </summary>
    public bool SetBool(string name, bool newValue) => SetBool(GetID(name), newValue);

    /// <summary> Set / update the boolean variable with a certain ID. 
    /// Returns whether setting the value was succesful. </summary>
    public bool SetBool(int id, bool newValue)
    {
        if(strictIDs && ids.Contains(id) && !bools.ContainsKey(id)) return false;

        bools[id] = newValue;
        ids.Add(id);
        return true;
    }

    /// <summary> Get a variable of type string given a certain variable name. Returns an
    /// empty string if a variable does not exist. </summary>
    public string GetString(string name) => GetString(GetID(name));

    /// <summary> Get a string variable with a certain ID. Returns an empty string 
    /// if the variable cannot be found. </summary>
    public string GetString(int id) => strings.GetValueOrDefault(id, "");

    /// <summary> Set / update a string variable with a certain name, given a new string value. 
    /// Returns whether the setting / updating was succesful. Can fail if strict ID check flag is set. </summary>
    public bool SetString(string name, string newValue) => SetString(GetID(name), newValue);

    /// <summary>
    /// Set / update a string variable with a certain ID, given a new string value.
    /// Returns whether the setting / updating wa succesful. Can fail if strict ID check flag is set. </summary>
    public bool SetString(int id, string newValue)
    {
        if(strictIDs && ids.Contains(id) && !strings.ContainsKey(id)) return false;

        strings[id] = newValue;
        ids.Add(id);
        return true;
    }

    /// <summary> Get a variable of type Vector3 given a certain variable name. Returns Vector3.zero
    ///  if the variable does not exist. </summary>
    public Vector3 GetVector3(string name) => GetVector3(GetID(name));

    /// <summary> Get a variable of type Vector3 given a certain variable ID. Returns
    /// Vector3.zero if the variable does not exist. </summary>
    public Vector3 GetVector3(int id) => vector3s.GetValueOrDefault(id, Vector3.zero);

    /// <summary>
    /// Set / update a Vector3 variable with a certain ID, given a new Vector3 value.
    /// Returns whether the setting / updating was succesful. Can fail if strict ID check flag is set. </summary>
    public bool SetVector3(string name, Vector3 newValue) => SetVector3(GetID(name), newValue);

    /// <summary>
    /// Set / update a Vector3 variable with a certain ID, given a new Vector3 value.
    /// Returns whether the setting / updating was succesful. Can fail if strict ID check flag is set. </summary>
    public bool SetVector3(int id, Vector3 newValue)
    {
        if(strictIDs && ids.Contains(id) && !vector3s.ContainsKey(id)) return false;

        vector3s[id] = newValue;
        ids.Add(id);
        return true;
    }

    /// <summary> Get a variable of type GameObject given a certain variable name. Returns
    /// null if the variable does not exist. </summary>
    public GameObject GetGameObject(string name) => GetGameObject(GetID(name));

    /// <summary> Get a variable of type GameObject given a certain variable ID. Returns
    /// null if the variable does not exist. </summary>
    public GameObject GetGameObject(int id) => gameObjects.GetValueOrDefault(id, null);

    /// <summary>
    /// Set / update a GameObject variable with a certain ID, given a new GameObject value.
    /// Returns whether the setting / updating was succesful. Can fail if strict ID check flag is set. </summary>
    public bool SetGameObject(string name, GameObject newValue) => SetGameObject(GetID(name), newValue);
    
    /// <summary>
    /// Set / update a GameObject variable with a certain ID, given a new GameObject value.
    /// Returns whether the setting / updating was succesful. Can fail if strict ID check flag is set. </summary>
    public bool SetGameObject(int id, GameObject newValue)
    {
        if(strictIDs && ids.Contains(id) && !gameObjects.ContainsKey(id)) return false;

        gameObjects[id] = newValue;
        ids.Add(id);
        return true;
    }
}
