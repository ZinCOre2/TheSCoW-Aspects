using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public SceneController SceneController;
    public Grid2d Grid;   
    public CameraController CameraController;
    public AudioManager AudioManager;    
    public UIController UIController;
    public WorldUIManager WorldUIManager;    
    public SelectionManager SelectionManager;
    public EntityManager EntityManager;
    public AbilityHolder AbilityHolder;    
    public TurnManager TurnManager;
    public UnitHolder UnitHolder;
    public ObjectPooler ObjectPooler;
    
    public void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
