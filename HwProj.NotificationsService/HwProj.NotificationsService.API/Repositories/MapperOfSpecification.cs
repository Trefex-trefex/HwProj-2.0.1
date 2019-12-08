﻿using HwProj.NotificationsService.API.Models;

namespace HwProj.NotificationsService.API.Repositories
{
    public class MapperOfSpecification
    {
        public Specification GetSpecification(NotificationFilter filter)
        {
            if (filter.HasSeen != null && filter.Important != null)
            {
                return new AndSpecification(new HasSeenNotificationSpecification(), new ImprotanceOfNotificationSpecification());
            }

            return new OrSpecification(new HasSeenNotificationSpecification(), new ImprotanceOfNotificationSpecification());
        }
    }
}
