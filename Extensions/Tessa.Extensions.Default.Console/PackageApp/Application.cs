﻿using System.Collections.Generic;

namespace Tessa.Extensions.Default.Console.PackageApp
{
    public sealed class Application
    {
        #region Properties

        public string Alias { get; set; }

        public string Name { get; set; }

        public string Group { get; set; }

        public string Version { get; set; }

        public string ExeFileName { get; set; }

        public byte[] Icon { get; set; }

        public bool Admin { get; set; }

        public List<ApplicationFile> Files { get; } = new List<ApplicationFile>();

        #endregion
    }
}