using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class UI_Handler : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] Button loadButton;
    [SerializeField] Button deleteButton;

    private void Awake()
    {
        loadButton.onClick.AddListener(() => LoadWorld());
        deleteButton.onClick.AddListener(() => DeleteWorld());

        deleteButton.interactable = true;

        if (SQLiteHandler.CheckForCreatedWorld())
        {
            loadButton.interactable = true;
        }
        else
        {
            loadButton.interactable = false;
        }
    }

    void LoadWorld()
    {
        FindObjectOfType<GridCreator>().LoadPastWorld();
    }

    void DeleteWorld()
    {
        SQLiteHandler.SetHasWorldValue(false);
        SQLiteHandler.SetBlockCount(0);
    }
}
