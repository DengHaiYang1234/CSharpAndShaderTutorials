using UnityEditor;
using UnityEngine;

//自动添加Spawn Zone脚本至GameLevel中的Level Object
static class RegisterLevelObjectMenuItem
{
	const string menuItem = "GameObject/Register Level Object";

	[MenuItem(menuItem,true)]
	static bool ValidateRegisterLevelObject()
	{
		if(Selection.objects.Length == 0)
		{
			return false;
		}

		foreach (Object o in Selection.objects)
		{
			if(!(o is GameObject))
				return false;
		}
		return true;
	}
	
    [MenuItem(menuItem)]
    static void RegisterLevelObject()
    {
       foreach (Object o in Selection.objects)
	   {
		   	Register(o as GameObject);
	   }
    }

	static void Register(GameObject o)
	{
        if (PrefabUtility.GetPrefabType(o) == PrefabType.Prefab)
        {
            Debug.LogError(o.name + " is a prefab asset.", o);
            return;
        }

        var levleObject = o.GetComponent<GameLevelObject>();
        if (levleObject == null)
        {
            Debug.LogError(o.name + " isn't a game levelObject.", o);
            return;
        }

        foreach (var rootObject in o.scene.GetRootGameObjects())
        {
            var gameLevel = rootObject.GetComponent<GameLevel>();
            if (gameLevel != null)
			{
				if(gameLevel.HasLevelObject(levleObject))
				{
					Debug.LogError(o.name + " is already registered",o);
					return;
				}

				Undo.RecordObject(gameLevel,"Register Level Object.");
				gameLevel.RegisterLevelObject(levleObject);
				Debug.Log(o.name + " registerd to game level" + gameLevel.name + " in scene " + o.scene.name + ".",o);
				return;
			}
        }

		Debug.LogError(o.name + " isn't part of a game level",o);
	}
}
