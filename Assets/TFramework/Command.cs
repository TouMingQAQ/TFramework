using TFrameworkKit.Console.Command;
using UnityEngine;

namespace TFramework
{
    [Command]
    public static class ABAssets 
    {
        [CommandMethod]
        public static void UnZipToRoot([CommandParameter]string key)
        {
            TFramework.Addressable.Unzip.UnZipToRoot(key);
        }
    }

}
