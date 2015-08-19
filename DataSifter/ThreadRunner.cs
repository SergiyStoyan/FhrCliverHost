//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        17 February 2008
//Copyright: (C) 2009, Sergey Stoyan
//********************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Reflection;
//using System.Reflection.Emit;
using System.Windows.Forms;

namespace Cliver.DataSifter
{
    /// <summary>
    /// Run a function as another thread, polls if it is working too long and then asks to break it.
    /// Used in GUI when a potentially time consuming operation is invoked.
    /// </summary>
    public class ThreadRunner
    {
        /// <summary>
        /// ThreadRunner constructor
        /// </summary>
        /// <param name="check_period">time period in ms when ask to abort the thread</param>
        /// <param name="owner">control whose mouse cursor will be changed to busy during running. Can be null</param>
        /// <param name="message">Can be null</param>
        public ThreadRunner(int check_period, Control owner, string message)
        {
            this.check_period = check_period;
            this.owner = owner;
            if (string.IsNullOrWhiteSpace(message))
                message = "It is working still. Do you want to abort the operation?";
            this.message = message;
        }
        int check_period = 1000;
        Control owner = null;
        string message = null;

        const int POLL_PERIOD = 100;

        public void Abort()
        {
            if (thread == null || !thread.IsAlive)
                return;
            thread.Abort();
            abort = true;
        }
        bool abort = false;

        /// <summary>
        /// Run a function as another thread.
        /// (!)Function must be public.
        /// </summary>
        /// <param name="target">instance of class hosting the function</param>
        /// <param name="function_name">name of the function</param>
        /// <param name="parameters">list of function parameters</param>
        /// <returns>any return from the fuction. If it is void then = null</returns>
        public object Run(object target, string function_name, params object[] parameters)
        {
            try
            {
                List<Type> ts = new List<Type>();
                foreach (object p in parameters)
                    ts.Add(p.GetType());
                //MethodInfo mi = target.GetType().GetMethod(function_name, BindingFlags.NonPublic | BindingFlags.Public);
                MethodInfo mi = target.GetType().GetMethod(function_name, ts.ToArray());
                if (mi == null)
                    throw new Exception("Can't find method " + target.ToString() + "::" + function_name);

                List<object> ps = new List<object>();
                ps.Add(target);
                ps.Add(mi);
                foreach (object p in parameters)
                    ps.Add(p);

                return run_(ps);
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
            return null;
        }

        /// <summary>
        /// Run delegate of a function as another thread.
        /// (!)Delegate must be public.
        /// </summary>
        /// <param name="target_function">delegate of the function</param>
        /// <param name="parameters">list of function parameters</param>
        /// <returns>any return from the fuction. If it is void then = null</returns>
        public object Run(object target_function, params object[] parameters)
        {
            try
            {
                List<object> ps = new List<object>();
                Delegate tf = (Delegate)target_function;
                ps.Add(tf.Target);
                ps.Add(tf.Method);
                foreach (object p in parameters)
                    ps.Add(p);

                return run_(ps);
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
            return null;
        }

        object run_(List<object> parameters)
        {
            try
            {
                result = null;

                if (owner != null)
                {
                    old_cursor = owner.Cursor;
                    owner.Cursor = Cursors.WaitCursor;
                }

                ParameterizedThreadStart pts = new ParameterizedThreadStart(_Run);
                thread = new Thread(pts);
                thread.Start(parameters);
                DateTime start_time = DateTime.Now;

                for (DateTime next_check_time = DateTime.Now.AddMilliseconds(check_period); !abort; )
                {
                    if (!thread.IsAlive)
                        break;
                    Thread.Sleep(POLL_PERIOD);
                    Application.DoEvents();
                    if (DateTime.Now >= next_check_time)
                    {
                        if (Message.YesNo("Running time: " + (int)(DateTime.Now - start_time).TotalSeconds + " seconds.\r\n\r\n" + message))
                        {
                            thread.Abort();
                            abort = true;
                            break;
                        }
                        next_check_time = DateTime.Now.AddMilliseconds(check_period);
                    }
                }

                if (owner != null)
                    owner.Cursor = old_cursor;
                return result;
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                Message.Error(ex);
            }

            if (owner != null)
                owner.Cursor = old_cursor;
            return null;
        }
        Cursor old_cursor;

        Thread thread;
        object result = null;

        void _Run(object parameters)
        {
            List<object> ps = (List<object>)parameters;
            object target = (object)ps[0];
            ps.RemoveAt(0);
            MethodInfo mi = (MethodInfo)ps[0];
            ps.RemoveAt(0);
            result = mi.Invoke(target, ps.ToArray());
        }
    }
}
