using System;
using System.Collections.Generic;
using TFramework.Component.UI;
using TFramework.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuCreator : MonoBehaviour,IPointerClickHandler
{
    public MenuControl menuControl;
    public RectTransform currentRect;

    private void Awake()
    {
        TryGetComponent(out currentRect);
    }

    public void OpenMenu(Vector2 mousePosition,Camera camera)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            currentRect,
            mousePosition,
            camera,
            out Vector2 localPoint);
        menuControl.OpenMenu(GetData(),localPoint);
    }

    List<MenuPathData> GetData()
    {
        List<MenuPathData> dataList = new()
        {
            new MenuPathData("Test/Test2/TestButton1", () => DebugUtility.LogI("Menu".Color(Color.blue), "TestButton1")),
            new MenuPathData("Test/Test3/TestButton2", () => DebugUtility.LogI("Menu".Color(Color.blue), "TestButton2")),
            new MenuPathData("Test/Test4/TestButton3",()=>DebugUtility.LogI("Menu".Color(Color.blue),"TestButton3")),
            new MenuPathData("Test/Test5/Test6/TestButton4",()=>DebugUtility.LogI("Menu".Color(Color.blue),"TestButton4")),
            new MenuPathData("Test/Test7/TestButton5",()=>DebugUtility.LogI("Menu".Color(Color.blue),"TestButton5")),
            new MenuPathData("Test/Test8/Test9/Test10/TestButton6",()=>DebugUtility.LogI("Menu".Color(Color.blue),"TestButton6"))
        };
        return dataList;
    }
    // List<MenuItemData> GetData()
    // {
    //     List<MenuItemData> dataList = new();
    //     dataList.Add(new MenuItemData()
    //     {
    //         ItemName = "Button_1",
    //         ItemState = MenuItemState.Button,
    //         OnClickCallBack = () =>
    //         {
    //             Debug.Log("Click Button 1");
    //         }
    //     });
    //     
    //     dataList.Add(new MenuItemData()
    //     {
    //         ItemName = "Drop_1",
    //         ItemState = MenuItemState.DropMenu,
    //         DropMenuData = new List<MenuItemData>(new []
    //         {
    //             new MenuItemData()
    //             {
    //                 ItemName = "Button_3",
    //                 ItemState = MenuItemState.Button,
    //                 OnClickCallBack = () =>
    //                 {
    //                     Debug.Log("Click Button 3");
    //                 }
    //             },
    //             new MenuItemData()
    //             {
    //                 ItemName = "Drop_2",
    //                 ItemState = MenuItemState.DropMenu,
    //                 DropMenuData = new List<MenuItemData>(new []{
    //                     new MenuItemData()
    //                     {
    //                         ItemName = "Button_4",
    //                         ItemState = MenuItemState.Button,
    //                         OnClickCallBack = () =>
    //                         {
    //                             Debug.Log("Click Button 4");
    //                         }
    //                     },
    //                     new MenuItemData()
    //                     {
    //                         ItemName = "Button_5",
    //                         ItemState = MenuItemState.Button,
    //                         OnClickCallBack = () =>
    //                         {
    //                             Debug.Log("Click Button 5");
    //                         }
    //                     },
    //                     new MenuItemData()
    //                     {
    //                         ItemName = "Button_6",
    //                         ItemState = MenuItemState.Button,
    //                         OnClickCallBack = () =>
    //                         {
    //                             Debug.Log("Click Button 6");
    //                         }
    //                     }
    //                 
    //                     }
    //                 )
    //             }
    //         })
    //         
    //     });
    //     dataList.Add(new MenuItemData()
    //     {
    //         ItemName = "Button_2",
    //         ItemState = MenuItemState.Button,
    //         OnClickCallBack = () =>
    //         {
    //             Debug.Log("Click Button 2");
    //         }
    //     });
    //     return dataList;
    // }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OpenMenu(eventData.position,eventData.pressEventCamera);
        }
    }
}
