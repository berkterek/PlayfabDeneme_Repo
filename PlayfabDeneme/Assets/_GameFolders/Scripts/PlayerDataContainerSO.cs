using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data",menuName = "Terek Gaming/Data Containers/Player Data")]
public class PlayerDataContainerSO : ScriptableObject
{
    [field:SerializeField]
    public int Score { get; set; }

    void OnEnable()
    {
        Score = 0;
    }

    void OnDisable()
    {
        Score = 0;
    }
}
