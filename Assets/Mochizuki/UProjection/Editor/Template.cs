/*-------------------------------------------------------------------------------------------
 * Copyright (c) Natsuneko. All rights reserved.
 * Licensed under the MIT License. See LICENSE in the project root for license information.
 *------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;

namespace Mochizuki.UProjection
{
    [Serializable]
    public class ProjectTemplate
    {
        public string name;

        public string uuid;

        public List<File> files;
    }

    [Serializable]
    public class File
    {
        public bool isDirectory;

        public string name;

        public SourceFile source;
    }

    [Serializable]
    public class SourceFile
    {
        public string uuid;

        public string name;
    }
}