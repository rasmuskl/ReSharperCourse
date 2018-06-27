using System;
using System.Collections.Generic;
using System.Linq;
using Rebus;

namespace ReSharperCourse
{
    public class RebusHierarchyLesson
    {
        public void Lesson(IActivateHandlers activateHandlers)
        {
            var handlers = activateHandlers.GetHandlerInstancesFor<RebusHierarchyLesson>();

            IHandleMessages<RebusHierarchyLesson> firstHandler = handlers.First();
            
            firstHandler.Handle(new RebusHierarchyLesson());
        }
    }
}
