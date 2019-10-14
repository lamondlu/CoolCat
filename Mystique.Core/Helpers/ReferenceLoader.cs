using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mystique.Core.Helpers
{
    public class ReferenceLoader
    {
        private string _referenceContent = string.Empty;

        public ReferenceLoader(string jsonFile)
        {

        }

        private void LoadReferenceContent(string jsonFile)
        {
            using (var sr = new StreamReader(jsonFile))
            {
                _referenceContent = sr.ReadToEnd();
            }
        }

        public List<string> GetReferences()
        {
            return new List<string>();
        }
    }
}
