using System.Threading;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Test
{
    class TestThread : MonoBehaviour
    {
        Thread work_thread;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            work_thread = new Thread(WorkThread);
            work_thread.Start();

            //StartCoroutine(WorkCoroutine());
        }

        void WorkThread()
        {
            int loop = 100;
            while(true)
            {
                --loop;

                Thread.Sleep(100);
            }
        }

        IEnumerator WorkCoroutine()
        {
            int loop = 100;
            while(loop > 0)
            {
                Debug.Log("Work Coroutine Update!");
                --loop;

                yield return null;
            }

            yield break;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            Debug.Log("Main Thread Update");
        }
        
        /// <summary>
        /// Callback sent to all game objects before the application is quit.
        /// </summary>
        void OnApplicationQuit()
        {
            if(work_thread != null)
            {
                work_thread.Abort();

                work_thread = null;
            }
        }
    } 
}
