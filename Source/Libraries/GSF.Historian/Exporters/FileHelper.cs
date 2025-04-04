﻿//******************************************************************************************************
//  FileHelper.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  02/26/2008 - Pinal C. Patel
//       Original version of source code generated.
//  04/17/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated new header and license agreement.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System;
using System.Data;
using System.IO;
using GSF.Data;
using GSF.IO;

namespace GSF.Historian.Exporters;

/// <summary>
/// A class with helper methods for file related operations.
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// Writes <paramref name="data"/> to the specified <paramref name="files"/> in the specified <paramref name="format"/>.
    /// </summary>
    /// <param name="files">Comma or semicolon delimited list of file names to which <paramref name="data"/> is to be written.</param>
    /// <param name="format">Format (CSV or XML) in which <paramref name="data"/> is to be written to the <paramref name="files"/>.</param>
    /// <param name="data"><see cref="DataSet"/> containing the data to be written to the <paramref name="files"/>.</param>
    public static void WriteToFile(string files, string format, DataSet data)
    {
        switch (format.ToLower())
        {
            case "xml":
                // Get the data in XML format and write it to the specified files.
                WriteToFile(files, data.GetXml());
                break;
            case "csv":
                // Get the data in CSV format and write it to the specified files.
                WriteToFile(files, data.Tables[0].ToDelimitedString(",", false, true));
                break;
            default:
                // Throw an exception if a non-supported file format is specified.
                throw new ArgumentException($"{format.ToUpper()} file format is not supported");
        }
    }

    /// <summary>
    /// Writes the specified <paramref name="text"/> to the specified <paramref name="files"/>.
    /// </summary>
    /// <param name="files">Comma or semicolon delimited list of file names to which the <paramref name="text"/> is to be written.</param>
    /// <param name="text">Text to be written to the <paramref name="files"/>.</param>
    public static void WriteToFile(string files, string text)
    {
        if (string.IsNullOrEmpty(files))
            throw new ArgumentNullException(nameof(files));

        // Since the text may need to be written to one or more files, the text is first written to a temporary
        // file and then copied to overwrite the specified files in order to speed up the write process.
        string tempFile = FilePath.GetAbsolutePath(Path.GetTempFileName());

        try
        {
            // Write the text to the temp file.
            File.WriteAllText(tempFile, text);

            foreach (string fileName in files.Split(';', ','))
            {
                // Make the filename absolute to the app.
                string prepFileName = FilePath.GetAbsolutePath(fileName.Trim());

                // Wait for a lock on the file if it exits.
                if (File.Exists(prepFileName))
                    FilePath.WaitForWriteLock(prepFileName, ExporterBase.FileLockWaitTime);

                // Copy the temp file to replace the file even if it exists.
                File.Copy(tempFile, prepFileName, true);
            }
        }
        finally
        {
            // Delete the temp file if we were able to create it to begin with.
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}