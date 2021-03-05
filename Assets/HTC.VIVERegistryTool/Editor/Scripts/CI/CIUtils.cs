﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HTC.VIVERegistryTool.Editor.CI
{
    public static class BuildUtils
    {
        public static void Build()
        {
            string[] args = GetExecuteMethodArguments(typeof(BuildUtils).FullName + "." + nameof(Build));
            string scene = args.ElementAtOrDefault(0);
            string outputPathName = args.ElementAtOrDefault(1);
            string buildTargetName = args.ElementAtOrDefault(2);

            Debug.LogFormat("Building scene \"{0}\" on build target \"{1}\" to \"{2}\".", scene, buildTargetName, outputPathName);
            BuildReport report = BuildPipeline.BuildPlayer(new string[] { scene }, outputPathName, StringToBuildTarget(buildTargetName), BuildOptions.None);
        }

        public static BuildTarget StringToBuildTarget(string name)
        {
            BuildTarget target;
            if (Enum.TryParse(name, true, out target))
            {
                return target;
            }

            Debug.LogError("Error parsing the string to BuildTarget: " + name);

            return BuildTarget.NoTarget;
        }

        public static string[] GetExecuteMethodArguments(string methodFullName)
        {
            List<string> optionArgs = new List<string>();
            string[] allArgs = Environment.GetCommandLineArgs();

            bool isOptionFound = false;
            foreach (string arg in allArgs)
            {
                if (!isOptionFound)
                {
                    if (arg.Length < 2)
                    {
                        continue;
                    }

                    if (arg.ToLower() == methodFullName.ToLower())
                    {
                        isOptionFound = true;
                    }
                }
                else
                {
                    if (arg[0] == '-')
                    {
                        break;
                    }

                    optionArgs.Add(arg);
                }
            }

            return optionArgs.ToArray();
        }
    }
}