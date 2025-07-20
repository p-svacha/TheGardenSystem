using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [Header("Elements")]
    public UI_DatePanel DatePanel;
    public UI_ResourcePanel ResourcePanel;
    public UI_OrderPanel OrderPanel;

    private void Awake()
    {
        Instance = this;
    }
}
