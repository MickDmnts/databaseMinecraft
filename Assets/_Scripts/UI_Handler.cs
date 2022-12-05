using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class UI_Handler : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] Button deleteButton;

    private void Awake()
    {
        deleteButton.onClick.AddListener(() => DeleteWorld());
        deleteButton.interactable = true;
    }

    void DeleteWorld()
    {
        FindObjectOfType<GridCreator>().ResetDatabaseWorldState();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !Cursor.visible;
        }
    }
}
