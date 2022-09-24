﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Reflection;
using System.ComponentModel;

namespace Optimate
{

    public static class Helpers
    {
        public static class Logger
        {

            private static DateTime SessionTimeStart;

            private static string directory = "";

            private static string logpath = "";

            public static string user = "";

            private static void Initialize()
            {
                SessionTimeStart = DateTime.Now; 
                var AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                directory = Path.Combine(AssemblyPath, @"Logs");
                logpath = Path.Combine(directory, string.Format(@"log_{0}_{1}_{2}.txt", SessionTimeStart.ToString("dd-MMM-yyyy"), SessionTimeStart.ToString("hh-mm-ss"), user.Replace(@"\", @"_")));
            }
            public static void AddLog(string log_entry)
            {
                Initialize();
                using (var data = new StreamWriter(logpath, true))
                {
                    data.WriteLine(log_entry);
                }
            }
            
        }
        public static class LevenshteinDistance
        {
            /// <summary>
            /// Compute the distance between two strings.
            /// </summary>
            public static int Compute(string s, string t)
            {
                int n = s.Length;
                int m = t.Length;
                int[,] d = new int[n + 1, m + 1];

                // Step 1
                if (n == 0)
                {
                    return m;
                }

                if (m == 0)
                {
                    return n;
                }

                // Step 2
                for (int i = 0; i <= n; d[i, 0] = i++)
                {
                }

                for (int j = 0; j <= m; d[0, j] = j++)
                {
                }

                // Step 3
                for (int i = 1; i <= n; i++)
                {
                    //Step 4
                    for (int j = 1; j <= m; j++)
                    {
                        // Step 5
                        int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                        // Step 6
                        d[i, j] = Math.Min(
                            Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                            d[i - 1, j - 1] + cost);
                    }
                }
                // Step 7
                return d[n, m];
            }

        }
    }


}
