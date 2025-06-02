using System;
using System.Collections.Generic;
using TFramework.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace TFramework.Component.UI
{
    public class MenuControl : MonoBehaviour
    {
        public RectTransform outRect;
        public Button closeButton;
        public RectTransform menuContent;
        public Menu menuPrefab;

        private void Awake()
        {
            closeButton.gameObject.SetActive(false);
            closeButton.onClick.AddListener(CloseAll);
        }

        public void CloseAll()
        {
            closeButton.gameObject.SetActive(false);
            menuContent.ClearChild();
        }

        public void OpenMenu(List<MenuItemData> dataList,Vector3 localPosition)
        {
            closeButton.gameObject.SetActive(true);
            var newMenu = Instantiate(menuPrefab, menuContent.transform);
            newMenu.transform.localPosition = localPosition;
            newMenu.control = this;
            newMenu.SetData(dataList);
        }
        public void OpenMenu(List<MenuPathData> dataList,Vector3 localPosition)
        {
            closeButton.gameObject.SetActive(true);
            var newMenu = Instantiate(menuPrefab, menuContent.transform);
            newMenu.transform.localPosition = localPosition;
            newMenu.control = this;
            newMenu.SetData(dataList);
        }
    }
}