using UnityEngine.UIElements;

namespace TFramework.ToolBox.UIToolkitKit
{
    [UxmlElement]
    public partial class TwoPaneSplitView : VisualElement
    {
        public TwoPaneSplitView()
        {
            var view = new UnityEngine.UIElements.TwoPaneSplitView();
            view.Add(new VisualElement()
            {
                name = "LeftPane",
            });
            view.Add(new VisualElement()
            {
                name = "RightPane",
            });
            Add(view);
            style.flexGrow = 1;
        }
    }
}