using UnityEngine;

public class Program : MonoBehaviour
{
    private Game Game;

    private void Awake()
    {
        ResourceManager.ClearCache();
        DefDatabaseRegistry.AddAllDefs();
        DefDatabaseRegistry.ResolveAllReferences();
        DefDatabaseRegistry.OnLoadingDone();
    }

    private void Start()
    {
        Game = new Game();
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.GameState == GameState.Uninitialized) Game.Initialize();
        else Game.HandleInputs();
    }
}
