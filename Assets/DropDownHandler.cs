using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownHandler : MonoBehaviour
{
    void Start()
    {
        var dropdown = transform.GetComponent<Dropdown>();
        dropdown.options.Clear();
        List<string> items = new List<string>();
        items.Add("- Chọn Slot -");


        //Fill Dropdown with items
        foreach (var item in items)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = item });
        }

        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }

    void DropdownItemSelected(Dropdown dropdown)
    {

    }

}
