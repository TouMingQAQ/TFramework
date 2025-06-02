using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TFramework.Component.UI
{
    public struct MenuPathCreator
    {
        public List<MenuItemData> menuData;
        public List<MenuPathData> menuPathData;

        private Dictionary<string, MenuItemData> menuItemMap;
        public void Create()
        {
            if(menuPathData == null)
                return;
            menuItemMap = new();
            menuData = new();
            List<(string[],Action)> pathList = new();
            int maxLength = 0;
            foreach (var menuPath in menuPathData)
            {
                var path = menuPath.Path;
                path = path.Replace('\\', '/');
                var paths = path.Split('/');
                var length = paths.Length;
                if (length > maxLength)
                    maxLength = length;
                pathList.Add((paths,menuPath.CallBack));
            }

            for (int i = 0; i < maxLength; i++)
            {
                foreach (var valueTuple in pathList)
                {
                    Span<string> paths = valueTuple.Item1;
                    var callBack = valueTuple.Item2;
                    var length = paths.Length;
                    if(length <= 0)
                        continue;
                    if(length <= i)
                        continue;
                    var isEnd = paths.Length <= i+1;
                    var itemName = paths[i];
                    var itemState = isEnd ? MenuItemState.Button : MenuItemState.DropMenu;
                    var currentKey = Path.Combine(paths.Slice(0, i+1).ToArray());
                    MenuItemData newData = new()
                    {
                        ItemName = itemName,
                        ItemState =  itemState,
                        OnClickCallBack =  callBack,
                        DropMenuData = new(),
                    };
                    if (i > 0)
                    {
                        var parentKey = Path.Combine(paths.Slice(0, i).ToArray());
                        if (menuItemMap.TryGetValue(parentKey, out var parentData))
                        {
                            parentData.DropMenuData.Add(newData);
                        }
                    }
 
                    menuItemMap[currentKey] = newData;
                    
                }

                if (i == 0)
                {
                    foreach (var item in menuItemMap)
                    {
                        menuData.Add(item.Value);
                    }
                }
            }
        }
        
    }
    [Serializable]
    public struct MenuItemData
    {
        public string ItemName;
        public MenuItemState ItemState;
        public Action OnClickCallBack;
        public List<MenuItemData> DropMenuData;
    }
    [Serializable]
    public struct MenuPathData
    {
        public string Path;
        public Action CallBack;

        public MenuPathData(string path, Action callBack)
        {
            this.Path = path;
            this.CallBack = callBack;
        }
    }

    public enum MenuItemState
    {
        Button,//只是Button
        DropMenu,//弹出另一个Menu
    }
    public class MenuItem : MonoBehaviour,IPointerClickHandler
    {
        
        public Menu menuPrefab;
        public TMP_Text itemNameText;
        public Image dropIcon;
        public RectTransform currentRect;
        private MenuItemData itemData;
        [HideInInspector]
        public MenuControl control;
        [HideInInspector]
        public Menu parentMenu;
        private Menu nextMenu;
        private void Awake()
        {
            TryGetComponent(out currentRect);
        }

        public void SetData(MenuItemData data)
        {
            itemNameText.SetText(data.ItemName);
            this.itemData = data;
            dropIcon.gameObject.SetActive(data.ItemState == MenuItemState.DropMenu);
        }

        public void CloseNextMenu()
        {
            if (nextMenu != null)
            {
                Destroy(nextMenu.gameObject);
                nextMenu = null;
            }
        }

        public void OpenMenu(List<MenuItemData> itemList,Vector3 menuPos)
        {
            nextMenu = Instantiate(menuPrefab, this.transform);
            nextMenu.transform.localPosition = menuPos;
            nextMenu.control = this.control;
            nextMenu.SetData(itemList);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            switch (itemData.ItemState)
            {
                case MenuItemState.Button:
                    itemData.OnClickCallBack?.Invoke();
                    control.CloseAll();
                    break;
                case MenuItemState.DropMenu:
                    if (itemData.DropMenuData != null)
                    {
                        parentMenu?.CloseAllMenu();
                        var pos = GetBestDropMenuPosition(itemData.DropMenuData.Count);
                        OpenMenu(itemData.DropMenuData,pos);
                    }
                    else
                    {
                        Debug.LogError("[MenuItem]: DropMenu itemData list is null");
                    }
                    break;
            }
        }
        

        protected Vector3 GetBestDropMenuPosition(int nextMenuItemCount)
        {
            if (nextMenuItemCount <= 0)
                return Vector3.zero;
            var currentSize = currentRect.sizeDelta;
    
            // 默认偏移量（右侧，垂直居中）
            var offsetX = currentSize.x*0.5f;
            var offsetY = currentSize.y * 0.5f ;
            return new Vector3(offsetX, offsetY, 0);
            
        }
        
        
    }
}