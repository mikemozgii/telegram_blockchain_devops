using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TONBRAINS.TONOPS.Core.SSH
{
    public class SSHBash
    {
        public Stream GenerateBashStream(IEnumerable<string> commands)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write($"#!/bin/bash\n");
            foreach (var c in commands)
            {
                writer.Write($"{c }\n");
            }
            writer.Flush();
            stream.Position = 0;

            return stream;
        }
    }
}