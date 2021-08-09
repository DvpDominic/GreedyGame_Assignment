using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Frame : Layer
{
    private int priority;

    public Frame(Layer obj)
    {
        type = obj.type;
        path = obj.path;
        placement = obj.placement;
        operations = obj.operations;
        priority = 0;
    }

    public void init(GameObject plane, Image prefab,Sprite sprite)
    {
        Image img = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity,plane.transform);
        RectTransform rect = img.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(placement[0].position.x, placement[0].position.y);
        rect.sizeDelta = new Vector2(placement[0].position.width, placement[0].position.height);
        rect.SetAsFirstSibling();
        img.sprite = sprite;


        if (operations != null)
        {
            foreach(Operation operation in operations)
            {
                int temp = (int)operation.name;
                switch (temp)
                {
                    case 0:
                        Color tempColor;
                        if(ColorUtility.TryParseHtmlString(operation.argument,out tempColor))
                        {
                            img.color = tempColor;
                        }
                        else
                        {
                            FindObjectOfType<GameManager>().showError("Frame Color Not Valid");
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }

}
