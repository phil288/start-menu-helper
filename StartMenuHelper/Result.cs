using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace StartMenuHelper
{
    public class Result
    {
        public string Filename { get; set; }
        public string fullLocation { get; set; }

        public Result(string fullLocation)
        {
            this.fullLocation = fullLocation;
            this.Filename = Path.GetFileNameWithoutExtension(this.fullLocation);
        }
    }
}
