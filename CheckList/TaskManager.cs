using System;
using System.Collections.Generic;
using System.Threading;

namespace CheckList
{
    public static class TaskManager
    {
        private static readonly List<ActElement> ActList = new List<ActElement>();

        private static readonly object Locker = new object();

        static TaskManager()
        {
            var taskManagerThread = new Thread(ActCycle) { Name = "mythread", IsBackground = true };
            taskManagerThread.Start();
        }

        public static void Add(Action threadAct, Action mainDispAct = null)
        {
            lock (Locker)
            {
                ActList.Add(new ActElement(threadAct, mainDispAct));
            }
        }


        private static void ActCycle()
        {
            while (true)
            {
                if (ActList.Count > 0)
                {
                    lock (Locker)
                    {
                        var currentActElement = ActList[0];
                        ActList.RemoveAt(0);

                        if (currentActElement.ThreadAct != null) currentActElement.ThreadAct();
                        if (currentActElement.InMainAct != null) MainWindow.InMainDispatch(currentActElement.InMainAct);
                    }
                }
                Thread.Sleep(5);
            }
        }
    }

    public class ActElement
    {
        public readonly Action ThreadAct;
        public readonly Action InMainAct;

        public ActElement(Action threadAct, Action inMainAct)
        {
            ThreadAct = threadAct;
            InMainAct = inMainAct;
        }
    }
}
