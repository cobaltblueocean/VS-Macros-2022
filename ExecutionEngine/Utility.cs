using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ExecutionEngine
{
    public class Utility
    {
        public static string GetVisualStudioPath()
        {
            String result = "";
            var process = Process.GetCurrentProcess(); // Or whatever method you are using
            string fullPath = process.MainModule.FileName;
            result = FileVersionInfo.GetVersionInfo(fullPath).FileVersion;

            return result;
        }
    }
}
