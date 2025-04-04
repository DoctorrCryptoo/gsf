﻿//******************************************************************************************************
//  Notifiers.cs - Gbtc
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
//  08/05/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using GSF.Adapters;

namespace GSF.Historian.Notifiers;

/// <summary>
/// A class that loads all <see cref="INotifier">Notifier</see> adapters.
/// </summary>
/// <seealso cref="INotifier"/>
public class Notifiers : AdapterLoader<INotifier>
{
    #region [ Members ]

    // Nested Types

    private class Notification
    {
        public Notification(string subject, string message, string details, NotificationTypes notificationType)
        {
            Subject = subject;
            Message = message;
            Details = details;
            NotificationType = notificationType;
        }

        public readonly string Subject;
        public readonly string Message;
        public readonly string Details;
        public readonly NotificationTypes NotificationType;
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Sends a notification.
    /// </summary>
    /// <param name="subject">>Subject-matter for the notification.</param>
    /// <param name="message">Brief message for the notification.</param>
    /// <param name="notificationType">One of the <see cref="NotificationTypes"/> values.</param>
    public void SendNotification(string subject, string message, NotificationTypes notificationType)
    {
        SendNotification(subject, message, "", notificationType);
    }

    /// <summary>
    /// Sends a notification.
    /// </summary>
    /// <param name="subject">>Subject-matter for the notification.</param>
    /// <param name="message">Brief message for the notification.</param>
    /// <param name="details">Detailed message for the notification.</param>
    /// <param name="notificationType">One of the <see cref="NotificationTypes"/> values.</param>
    public void SendNotification(string subject, string message, string details, NotificationTypes notificationType)
    {
        OperationQueue.Enqueue(new Notification(subject, message, details, notificationType));
    }

    /// <summary>
    /// Executes <see cref="INotifier.Notify"/> on the specified notifier <paramref name="adapter"/>.
    /// </summary>
    /// <param name="adapter">An <see cref="INotifier"/> object.</param>
    /// <param name="data">Data about the notification.</param>
    protected override void ExecuteAdapterOperation(INotifier adapter, object data)
    {
        Notification notification = (Notification)data;
        adapter.Notify(notification.Subject, notification.Message, notification.Details, notification.NotificationType);
    }

    #endregion
}