using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public SceneController SceneController;
    public UIController UIController;
    public SelectionManager SelectionManager;
    public Grid2d Grid;
    public UnitManager UnitManager;
    public TurnManager TurnManager;
    public AbilityHolder AbilityHolder;

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
