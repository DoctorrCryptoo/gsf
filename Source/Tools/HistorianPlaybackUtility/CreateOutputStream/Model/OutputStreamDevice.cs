﻿//******************************************************************************************************
//  OutputStreamDevice.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  03/29/2017 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;

namespace CreateOutputStream.Model
{
    public class OutputStreamDevice
    {
        public Guid NodeID { get; set; }

        public int AdapterID { get; set; }

        [PrimaryKey(true)]
        public int ID { get; set; }

        public int IDCode { get; set; }

        [Required]
        [StringLength(200)]
        public string Acronym { get; set; }

        [StringLength(4)]
        public string BpaAcronym { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(15)]
        public string PhasorDataFormat { get; set; }

        [StringLength(15)]
        public string FrequencyDataFormat { get; set; }

        [StringLength(15)]
        public string AnalogDataFormat { get; set; }

        [StringLength(15)]
        public string CoordinateFormat { get; set; }

        public int LoadOrder { get; set; }

        public bool Enabled { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(200)]
        public string CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        [Required]
        [StringLength(200)]
        public string UpdatedBy { get; set; }
    }
}
