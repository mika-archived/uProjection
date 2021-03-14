/*-------------------------------------------------------------------------------------------
 * Copyright (c) Natsuneko. All rights reserved.
 * Licensed under the MIT License. See LICENSE in the project root for license information.
 *------------------------------------------------------------------------------------------*/

using System.Diagnostics;

using UnityEditor;

namespace Mochizuki.UProjection
{
    public static class Documentation
    {
        [MenuItem("Mochizuki/uProjection/Document")]
        public static void ShowDocument()
        {
            Process.Start("https://docs.mochizuki.moe/unity/u-projection/");
        }
    }
}