using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class text : Layer
{
    private int priority;

    public text(Layer obj)
    {
        type = obj.type;
        path = null;
        placement = obj.placement;
        operations = obj.operations;
        priority = 1;
    }

    public void init(GameObject plane, Text prefab, string inputText)
    {
        Text txt = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity, plane.transform);
        RectTransform rect = txt.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(placement[0].position.x, placement[0].position.y);
        rect.sizeDelta = new Vector2(placement[0].position.width, placement[0].position.height);
        rect.SetAsLastSibling();
        txt.text = inputText;

        if (operations != null)
        {
            foreach (Operation operation in operations)
            {
                int temp = (int)operation.name;
                switch (temp)
                {
                    case 0:
                        Color tempColor;
                        if (ColorUtility.TryParseHtmlString(operation.argument, out tempColor))
                        {
                            txt.color = tempColor;
                        }
                        else
                        {
                            FindObjectOfType<GameManager>().showError("Text Color Not Valid");
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
