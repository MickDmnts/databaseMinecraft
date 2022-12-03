using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BlockSelector : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] List<GameObject> hotbarItems;

    int selectedItem;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hotbarItems[0].GetComponent<Image>().color = Color.green;

            selectedItem = 0;

            DeselectOtherItems(selectedItem);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hotbarItems[1].GetComponent<Image>().color = Color.green;

            selectedItem = 1;

            DeselectOtherItems(selectedItem);
        }
    }

    void DeselectOtherItems(int selectedItems)
    {
        for (int i = 0; i < hotbarItems.Count; i++)
        {
            if (i == selectedItems) continue;

            hotbarItems[i].GetComponent<Image>().color = Color.white;
        }
    }

    public int GetSelectedItem() => selectedItem;
}
