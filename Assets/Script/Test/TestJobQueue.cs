using UnityEngine;
using System.Collections;

namespace Test
{
    class TestJobQueue : MonoBehaviour
    {
        JobQueue<CalcLimit2> queue;
    
        void OnEnable()
        {
            queue = new JobQueue<CalcLimit2>(2); // create new queue with 2 threads
            queue.AddJob(new CalcLimit2 { count = 200, CustomName = "200 iterations" });
            queue.AddJob(new CalcLimit2 { count = 20, CustomName = "Do 20 iterations" });
            queue.AddJob(new CalcLimit2 { count = 9001, CustomName = "over 9000" });
            queue.AddJob(new CalcLimit2 { count = 50000000, CustomName = "50M" });
        }
        
        void Update()
        {
            queue.Update();
        }
        void OnDisable()
        {
            queue.ShutdownQueue(); // important to terminate the threads inside the queue.
        }
        
        
        // The example job class:
        public class CalcLimit2 : JobItem
        {
            // some identifying name, not related to the actual job
            public string CustomName;
            // input variables. Should be set before the job is started
            public int count = 5;
            // output / result variable. This represents the data that this job produces.
            public float Result;
            protected override void DoWork()
            {
                // this is executed on a seperate thread
                float v = 0;
                for(int i = 0; i < count; i++)
                {
                    v += Mathf.Pow(0.5f, i);
                    // check every 100 iteration if the job should be aborted
                    if ((i % 100) == 0 && IsAborted)
                        return;
                }
                Result = v;
            }
            public override void OnFinished()
            {
                // this is executed on the main thread.
                Debug.Log("Job: " + CustomName + " has finished with the result: " + Result);
            }
        }
    }
}

