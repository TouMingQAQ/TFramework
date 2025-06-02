using System;
using System.Collections.Generic;
using TFramework.Runtime;
using UnityEngine;

namespace TFramework.Component.UI
{
  
    public class Menu : MonoBehaviour
    {
        public MenuItem itemPrefab;
        public Transform itemContent;
        public RectTransform currentRect;
        public List<MenuItem> currentItemList = new();

        [HideInInspector]
        public MenuControl control;
        private void Awake()
        {
            TryGetComponent(out currentRect);
        }

        public void CloseAllMenu()
        {
            foreach (var menuItem in currentItemList)
            {
                menuItem.CloseNextMenu();
            }
        }

        public void SetData(List<MenuPathData> menuList)
        {
            MenuPathCreator creator = new()
            {
                menuPathData = menuList
            };
            creator.Create();
            SetData(creator.menuData);
        }
        public void SetData(List<MenuItemData> itemList)
        {
            foreach (var item in currentItemList)
            {
                Destroy(item.gameObject);
            }
            currentItemList.Clear();
            foreach (var menuItemData in itemList)
            {
                var item = Instantiate(itemPrefab, itemContent);
                item.parentMenu = this;
                item.SetData(menuItemData);
                item.control = this.control;
                currentItemList.Add(item);
            }
        }
        
    }
}
