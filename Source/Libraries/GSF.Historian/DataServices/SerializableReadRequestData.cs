﻿//******************************************************************************************************
//  SerializableReadRequestData.cs - Gbtc
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
//  11/08/2011 - Ritchie
//       Generated original version of source code based on suggestion and code from end user.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace GSF.Historian.DataServices;

/// <summary>
/// Represents a container for JSON serialized time-series data read request.
/// </summary>
[XmlType("ReadRequestData")]
[DataContract(Name = "ReadRequestData")]
public class SerializableReadRequestData
{
    #region [ Properties ]

    /// <summary>
    /// XML array of integer historian ID's.
    /// </summary>
    [XmlArray]
    [DataMember(Order = 0)]
    public int[] idArray { get; set; }

    /// <summary>
    /// Start time.
    /// </summary>
    [XmlAttribute]
    [DataMember(Order = 1)]
    public string startTime { get; set; }

    /// <summary>
    /// End time.
    /// </summary>
    [XmlAttribute]
    [DataMember(Order = 2)]
    public string endTime { get; set; }

    #endregion
}