﻿//******************************************************************************************************
//  StatisticsExporter.cs - Gbtc
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
//  02/25/2008 - Pinal C. Patel
//       Original version of source code generated.
//  04/09/2008 - Pinal C. Patel
//       Allowed for filter the data to be used for calculating statistics using the FilterClause
//       setting that can be specified in the export definition file.
//  05/07/2008 - Pinal C. Patel
//       Added slope-based data elimination algorithm that can be used to eliminated spikes in data.
//  04/17/2009 - Pinal C. Patel
//       Converted to C#.
//  08/05/2009 - Josh L. Patterson
//       Edited Comments.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace GSF.Historian.Exporters;

/// <summary>
/// Represents an exporter that can export the <see cref="Statistics"/> in CSV or XML format to a file.
/// </summary>
/// <example>
/// Definition of a sample <see cref="Export"/> that can be processed by <see cref="StatisticsExporter"/>:
/// <code>
/// <![CDATA[
/// <?xml version="1.0" encoding="utf-16"?>
/// <Export xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
///   <Name>StatisticsExporte</Name>
///   <Type>Intervaled</Type>
///   <Interval>10</Interval>
///   <Exporter>StatisticsExporter</Exporter>
///   <Settings>
///     <ExportSetting>
///       <Name>OutputFile</Name>
///       <Value>\\trotibco\XML\EIRA.xml</Value>
///     </ExportSetting>
///     <ExportSetting>
///       <Name>OutputFormat</Name>
///       <Value>XML</Value>
///     </ExportSetting>    
///     <ExportSetting>
///       <Name>FilterClause</Name>
///       <Value>Value&gt;=59 And Value&lt;=61</Value>
///     </ExportSetting>
///     <ExportSetting>
///       <Name>SlopeThreshold</Name>
///       <Value>.9</Value>
///     </ExportSetting>
///   </Settings>
///   <Records>
///     <ExportRecord>
///       <Instance>P1</Instance>
///       <Identifier>1285</Identifier>
///     </ExportRecord>    
///     <ExportRecord>
///       <Instance>P2</Instance>
///       <Identifier>3173</Identifier>
///     </ExportRecord>    
///     <ExportRecord>
///       <Instance>P3</Instance>
///       <Identifier>1838</Identifier>
///     </ExportRecord>    
///  </Records>
/// </Export>
/// ]]>
/// </code>
/// <para>
/// Description of custom settings required by <see cref="StatisticsExporter"/> in an <see cref="Export"/>:
/// <list type="table">
///     <listheader>
///         <term>Setting Name</term>
///         <description>Setting Description</description>
///     </listheader>
///     <item>
///         <term>OutputFile</term>
///         <description>Name of the CSV or XML file (including path) where export data is to be written.</description>
///     </item>
///     <item>
///         <term>OutputFormat</term>
///         <description>Format (CSV or XML) in which export data is to be written to the output file.</description>
///     </item>
///     <item>
///         <term>FilterClause (Optional)</term>
///         <description>SQL-like expression to be used for limiting the data included in the calculation.</description>
///     </item>
///     <item>
///         <term>SlopeThreshold (Optional)</term>
///         <description>Floating point value to be used for eliminating data with a slope exceeding the specified threshold.</description>
///     </item>
/// </list>
/// </para>
/// </example>
/// <seealso cref="Export"/>
public class StatisticsExporter : RawDataExporter
{
    #region [ Members ]

    // Nested Types

    /// <summary>
    /// A class for calculating the MIN, MAX and AVG of time-series data over a period of time.
    /// </summary>
    protected class Statistics
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Statistics"/> class.
        /// </summary>
        public Statistics()
        {
            MinimumValue = double.NaN;
            MaximumValue = double.NaN;
            AverageValue = double.NaN;
        }

        /// <summary>
        /// The smallest value in the time-series data.
        /// </summary>
        public double MinimumValue;

        /// <summary>
        /// The largest value in the time-series data.
        /// </summary>
        public double MaximumValue;

        /// <summary>
        /// The average value in the time-series data.
        /// </summary>
        public double AverageValue;

        /// <summary>
        /// Calculates <see cref="Statistics"/> from the provided <paramref name="data"/> and clear the <paramref name="data"/> when done.
        /// </summary>
        /// <param name="data">A <see cref="DataSet"/> containing buffered real-time time-series data.</param>
        /// <param name="filerClause">Filter clause to be applied for limiting the data included in the calculation.</param>
        public void Calculate(DataSet data, string filerClause)
        {
            object minimum;
            object maximum;
            object average;

            lock (data)
            {
                // Synchronize to ensure no new data is being added during the computation.
                minimum = data.Tables[0].Compute("Min(Value)", filerClause);
                maximum = data.Tables[0].Compute("Max(Value)", filerClause);
                average = data.Tables[0].Compute("Avg(Value)", filerClause);

                // Clear all the buffered data to make space for new data to be buffered.
                data.Tables[0].Clear();
            }

            // Expose the calculated statistical values.
            MinimumValue = Convert.ToDouble(minimum == DBNull.Value ? double.NaN : minimum);
            MaximumValue = Convert.ToDouble(maximum == DBNull.Value ? double.NaN : maximum);
            AverageValue = Convert.ToDouble(average == DBNull.Value ? double.NaN : average);
        }
    }

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsExporter"/> class.
    /// </summary>
    public StatisticsExporter()
        : this(nameof(StatisticsExporter))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsExporter"/> class.
    /// </summary>
    /// <param name="name"><see cref="ExporterBase.Name"/> of the exporter.</param>
    protected StatisticsExporter(string name)
        : base(name)
    {
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Processes the <paramref name="export"/> using the current <see cref="DataListener.Data"/>.
    /// </summary>
    /// <param name="export"><see cref="Export"/> to be processed.</param>
    /// <exception cref="ArgumentException"><b>OutputFile</b> or <b>OutputFormat</b> setting is missing from the <see cref="Export.Settings"/> of the <paramref name="export"/>.</exception>
    protected override void ProcessExport(Export export)
    {
        // Ensure that required settings are present.
        ExportSetting outputFileSetting = export.FindSetting("OutputFile");
        
        if (outputFileSetting is null)
            throw new ArgumentException("OutputFile setting is missing");
        ExportSetting outputFormatSetting = export.FindSetting("OutputFormat");
        
        if (outputFormatSetting is null)
            throw new ArgumentException("OutputFormat setting is missing");

        // Get the calculated statistics.
        Statistics stats = GetStatistics(export);

        // Get the dataset template we'll be outputting.
        DataSet output = DatasetTemplate(nameof(Statistics));

        // Modify the dataset template for statistical values.
        DataTable data = output.Tables[0];
        data.PrimaryKey = null!;
        data.Columns.Clear();
        data.Columns.Add("MinimumValue", typeof(double));
        data.Columns.Add("MaximumValue", typeof(double));
        data.Columns.Add("AverageValue", typeof(double));

        foreach (DataColumn column in data.Columns)
            column.ColumnMapping = MappingType.Attribute;

        // Add the calculated statistical value.
        data.Rows.Add(stats.MinimumValue, stats.MaximumValue, stats.AverageValue);

        // Update the export timestamp, row count and interval.
        output.Tables[1].Rows.Add(DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss tt"), output.Tables[0].Rows.Count, $"{export.Interval} seconds");

        // Write the statistical data for the export to the specified files in specified format.
        FileHelper.WriteToFile(outputFileSetting.Value, outputFormatSetting.Value, output);
    }

    /// <summary>
    /// Processes the <paramref name="export"/> using the real-time <paramref name="data"/>.
    /// </summary>
    /// <param name="export"><see cref="Export"/> to be processed.</param>
    /// <param name="listener"><see cref="DataListener"/> that provided the <paramref name="data"/>.</param>
    /// <param name="data">Real-time time-series data received by the <paramref name="listener"/>.</param>
    /// <exception cref="NotSupportedException">Always</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void ProcessRealTimeExport(Export export, DataListener listener, IList<IDataPoint> data)
    {
        throw new NotSupportedException();  // Real-Time exports not supported.
    }

    /// <summary>
    /// Returns the calculated <see cref="Statistics"/> from buffered data of the specified <paramref name="export"/>.
    /// </summary>
    /// <param name="export"><see cref="Export"/> for which statistical values are to be calculated.</param>
    /// <returns>The calculated <see cref="Statistics"/> from buffered data of the specified <paramref name="export"/>.</returns>
    protected virtual Statistics GetStatistics(Export export)
    {
        Statistics stats = new();
        DataSet rawData = GetBufferedData(export.Name);
        ExportSetting filterClauseSetting = export.FindSetting("FilterClause");
        ExportSetting slopeThresholdSetting = export.FindSetting("SlopeThreshold");

        if (rawData is null)
            return stats;
        
        // Buffered data is available for the export, so calculate the statistics.
        if (slopeThresholdSetting is not null)
        {
            // Slope-based data elimination is to be employed.
            double slopeThreshold = Convert.ToDouble(slopeThresholdSetting.Value);
        
            lock (rawData)
            {
                foreach (ExportRecord record in export.Records)
                {
                    // Run data for each record against the slope-based data elimination algorithm.
                    double slope = 0;
                    bool delete = false;
                    DataRow[] records = rawData.Tables[0].Select($"Instance=\'{record.Instance}\' And ID={record.Identifier}", nameof(TimeTag));

                    if (records.Length <= 0)
                        continue;
                    
                    for (int i = 0; i < records.Length - 1; i++)
                    {
                        // Calculate slope for the data using standard slope formula.
                        slope = Math.Abs(((float)records[i]["Value"] - (float)records[i + 1]["Value"]) / (Ticks.ToSeconds(Convert.ToDateTime(records[i][nameof(TimeTag)]).Ticks) - Ticks.ToSeconds(Convert.ToDateTime(records[i + 1][nameof(TimeTag)]).Ticks)));
                        
                        if (!(slope > slopeThreshold))
                            continue;
                        
                        // Data for the point has slope that exceeds the specified slope threshold.
                        delete = true;
                        break;
                    }

                    if (!delete)
                        continue;
                    
                    // Data for the record is to be excluded from the calculation.
                    for (int i = 0; i < records.Length; i++)
                        records[i].Delete();
                    
                    OnStatusUpdate($"{record.Instance}:{record.Identifier} data eliminated (Slope={slope}; Threshold={slopeThreshold})");
                }
            }
        }

        stats.Calculate(rawData, filterClauseSetting is null ? string.Empty : filterClauseSetting.Value);

        return stats;
    }

    #endregion
}