﻿using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="GameSettingsGenerator.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2018 Omiya Games
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// </copyright>
    /// <author>Taro Omiya</author>
    /// <date>5/17/2017</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A list of helper methods that writes all the
    /// <see cref="ISettingsVersion"/> and the latest
    /// <see cref="IStoredSetting"/> into a single,
    /// readable C# file.
    /// </summary>
    /// <seealso cref="GameSettings"/>
    /// <seealso cref="ISettingsVersion"/>
    public static partial class GameSettingsGenerator
    {
#if UNITY_EDITOR
        public static string GameSettingsFullPath
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(Application.dataPath);
                builder.Append(Utility.PathDivider);
                builder.Append("Omiya Games");
                builder.Append(Utility.PathDivider);
                builder.Append("Scripts");
                builder.Append(Utility.PathDivider);
                builder.Append("Singleton");
                builder.Append(Utility.PathDivider);
                builder.Append("Settings");
                builder.Append(Utility.PathDivider);
                builder.Append("GameSettings.AutoGenerated.cs");
                return builder.ToString();
            }
        }

        public static string GameSettingsUnityPath
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("Assets");
                builder.Append(Utility.PathDivider);
                builder.Append("Omiya Games");
                builder.Append(Utility.PathDivider);
                builder.Append("Scripts");
                builder.Append(Utility.PathDivider);
                builder.Append("Singleton");
                builder.Append(Utility.PathDivider);
                builder.Append("Settings");
                builder.Append(Utility.PathDivider);
                builder.Append("GameSettings.AutoGenerated.cs");
                return builder.ToString();
            }
        }

        public static VersionGeneratorArgs GetAllVersions()
        {
            VersionGeneratorArgs returnArgs = new VersionGeneratorArgs();
            Type versionInterface = typeof(ISettingsVersionGenerator);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && !x.IsAbstract && versionInterface.IsAssignableFrom(x));
            string errorMessage;
            foreach (Type versionType in types)
            {
                if (returnArgs.AddVersion(Activator.CreateInstance(versionType) as ISettingsVersionGenerator, out errorMessage) == false)
                {
                    Debug.LogWarning(errorMessage);
                }
            }
            return returnArgs;
        }

        public static void WriteCode(VersionGeneratorArgs versionsArgs, NamespaceGeneratorArgs usingsArgs, SettingsGeneratorArgs settingsArgs)
        {
            int numTabs = 0;
            using (TextWriter writer = new StreamWriter(GameSettingsFullPath, false, Encoding.UTF8))
            {
                // List out the namespace we're using
                WriteAllUsings(writer, numTabs, usingsArgs);

                // Declare the current namespace
                numTabs = WriteStartEncapsulation(writer, numTabs, "namespace OmiyaGames.Settings");

                // Start the class
                WriteTooltipComment(writer, numTabs, "This code is auto-generated. All changes will be overwritten!");
                numTabs = WriteStartEncapsulation(writer, numTabs, "public partial class GameSettings : Global.ISingletonScript");

                // Write out the array of versions
                WriteLine(writer, numTabs, "#region Private Arrays");
                WriteAllSettingsVersions(writer, numTabs, versionsArgs);
                writer.WriteLine();

                // Write out the array of single settings
                WriteAllSingleSettings(writer, numTabs, settingsArgs);
                WriteLine(writer, numTabs, "#endregion");
                writer.WriteLine();

                // Write AppVersion property
                WriteAppVersionProperty(writer, numTabs);

                // Write out the list of settings
                WriteAllSettingsProperties(writer, numTabs, settingsArgs);

                // End the class
                numTabs = WriteEndEncapsulation(writer, numTabs);

                // End the namespace
                numTabs = WriteEndEncapsulation(writer, numTabs);
            }
            AssetDatabase.ImportAsset(GameSettingsUnityPath, ImportAssetOptions.ForceUpdate);
        }

        private static void WriteAllSettingsVersions(TextWriter writer, int numTabs, VersionGeneratorArgs versionsArgs)
        {
            // Write the comment for AllSettingsVersions
            WriteTooltipComment(writer, numTabs, "Array of all the <see cref=\"ISettingsVersion\"/> detected in this project.", "Used as reference in the properties.");

            // Declare AllSettingsVersions array
            numTabs = WriteStartEncapsulation(writer, numTabs, "private readonly ISettingsVersion[] AllSettingsVersions = new ISettingsVersion[]");

            // Go through all the versions
            foreach (ISettingsVersionGenerator version in versionsArgs)
            {
                // Write out the version's class' default constructor
                version.WriteCodeForConstructor(writer, numTabs);
                writer.WriteLine(',');
            }

            // End array
            numTabs = WriteEndEncapsulation(writer, numTabs, true);
        }

        private static void WriteAllSingleSettings(TextWriter writer, int numTabs, SettingsGeneratorArgs settingsArgs)
        {
            // Write the comment for allSingleSettings
            WriteTooltipComment(writer, numTabs, "Array cache used by <see cref=\"AllSingleSettings\"/>.");

            // Declare allSingleSettings array member variable
            WriteLine(writer, numTabs, "private IStoredSetting[] allSingleSettings = null;");
            writer.WriteLine();

            // Write the comment for AllSingleSettings
            WriteTooltipComment(writer, numTabs, "Array of <see cref=\"IStoredSetting\"/> that has a Property in this class.", "Used for collective saving and retrieval of settings.");

            // Declare AllSingleSettings property
            numTabs = WriteStartEncapsulation(writer, numTabs, "private IStoredSetting[] AllSingleSettings");

            // Start get
            numTabs = WriteStartEncapsulation(writer, numTabs, "get");

            // Start if
            numTabs = WriteStartEncapsulation(writer, numTabs, "if(allSingleSettings == null)");

            // Start intializing array
            numTabs = WriteStartEncapsulation(writer, numTabs, "allSingleSettings = new IStoredSetting[]", false);

            // Go through each group
            foreach (KeyValuePair<int, ICollection<SettingsGeneratorArgs.SingleSettingsInfo>> groupOfSettings in settingsArgs)
            {
                // Start a region
                writer.WriteLine();
                WriteStartOfLine(writer, numTabs, "#region ISingleSettings from version ");
                writer.WriteLine(groupOfSettings.Key);

                // Go through all the settings
                foreach (SettingsGeneratorArgs.SingleSettingsInfo setting in groupOfSettings.Value)
                {
                    if (setting.isStoredSetting == true)
                    {
                        // Write the setting
                        WriteTabs(writer, numTabs);
                        setting.generator.WriteCodeToInstance(writer, setting.versionArrayIndex, false);
                        writer.WriteLine(',');
                    }
                }

                // End region
                WriteLine(writer, numTabs, "#endregion");
            }

            // End array
            numTabs = WriteEndEncapsulation(writer, numTabs, true);

            // End if
            numTabs = WriteEndEncapsulation(writer, numTabs);

            // Write return statement
            WriteLine(writer, numTabs, "return allSingleSettings;");

            // End get
            numTabs = WriteEndEncapsulation(writer, numTabs);

            // End AllSingleSettings property
            numTabs = WriteEndEncapsulation(writer, numTabs);
        }

        private static void WriteAppVersionProperty(TextWriter writer, int numTabs)
        {
            WriteTooltipComment(writer, numTabs, "The latest version number stored in settings.", "This is the size of <see cref=\"AllSettingsVersions\"/>");

            // Declare AppVersion property
            numTabs = WriteStartEncapsulation(writer, numTabs, "public int AppVersion");

            // Define "get"
            numTabs = WriteStartEncapsulation(writer, numTabs, "get");

            // Return array size
            WriteLine(writer, numTabs, "return AllSettingsVersions.Length;");

            // Close "get"
            numTabs = WriteEndEncapsulation(writer, numTabs);

            // Close property
            numTabs = WriteEndEncapsulation(writer, numTabs);
        }

        private static void WriteAllSettingsProperties(TextWriter writer, int numTabs, SettingsGeneratorArgs settingsArgs)
        {
            // Go through each group
            foreach (KeyValuePair<int, ICollection<SettingsGeneratorArgs.SingleSettingsInfo>> groupOfSettings in settingsArgs)
            {
                // Declare region
                writer.WriteLine();
                WriteStartOfLine(writer, numTabs, "#region Properties from AppVersion ");
                writer.Write(groupOfSettings.Key);

                // Go through all the settings
                foreach (SettingsGeneratorArgs.SingleSettingsInfo setting in groupOfSettings.Value)
                {
                    // Write a line
                    writer.WriteLine();

                    // Write the setting
                    setting.generator.WriteCodeToAccessSetting(writer, numTabs, setting.versionArrayIndex);
                }

                // End region
                WriteLine(writer, numTabs, "#endregion");
            }
        }

        private static void WriteAllUsings(TextWriter writer, int numTabs, NamespaceGeneratorArgs namespaceArgs)
        {
            bool appendNewline = false;
            foreach (string namespaceName in namespaceArgs)
            {
                // Write tabs
                WriteTabs(writer, numTabs);

                // Write the "using namespace;" lines
                writer.Write("using ");
                writer.Write(namespaceName);
                writer.WriteLine(';');
                appendNewline = true;
            }

            // Write a newline below all the usings
            if (appendNewline == true)
            {
                writer.WriteLine();
            }
        }

        public static void WriteTooltipComment(TextWriter writer, int numTabs, params string[] comments)
        {
            // Start summary comment
            WriteLine(writer, numTabs, "/// <summary>");

            // Write all the comments
            foreach (string line in comments)
            {
                // Start the line with a comment
                WriteStartOfLine(writer, numTabs, "/// ");

                // Print out the comment
                writer.WriteLine(line);
            }

            // End summary comment
            WriteLine(writer, numTabs, "/// </summary>");
        }

        public static int WriteStartEncapsulation(TextWriter writer, int numTabs, string declaration, bool addNewlineAtTheEnd = true)
        {
            // Write out the line and open curly braces
            WriteLine(writer, numTabs, declaration);
            if (addNewlineAtTheEnd == true)
            {
                WriteLine(writer, numTabs, '{');
            }
            else
            {
                WriteTabs(writer, numTabs);
                writer.Write('{');
            }

            // Calculate the next tab count
            if (numTabs < 0)
            {
                numTabs = 0;
            }
            ++numTabs;

            // Return the next tab count
            return numTabs;
        }

        public static int WriteEndEncapsulation(TextWriter writer, int numTabs, bool addSemicolonAtTheEnd = false)
        {
            // Calculate the next tab count
            --numTabs;
            if (numTabs < 0)
            {
                numTabs = 0;
            }

            // Write out the close curly braces
            if (addSemicolonAtTheEnd)
            {
                WriteLine(writer, numTabs, "};");
            }
            else
            {
                WriteLine(writer, numTabs, '}');
            }

            // Return the next tab count
            return numTabs;
        }
#endif
    }
}
