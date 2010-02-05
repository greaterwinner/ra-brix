/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using Ra.Brix.Types;
using System.Web.UI;
using System.Web;
using System.Threading;

namespace Ra.Brix.Loader
{
    /**
     * Class contains methods for raising events and other helpers, like for instance helpers
     * to load controls and such.
     */
    public sealed class ActiveEvents
    {
        private readonly Dictionary<string, List<Tuple<MethodInfo, Tuple<object, bool>>>> _methods =
            new Dictionary<string, List<Tuple<MethodInfo, Tuple<object, bool>>>>();
        private static ActiveEvents _instance;

        private delegate void AsyncDelegate(object sender, ActiveEventArgs e);

        private ActiveEvents()
        { }

        /**
         * This is a "Page Life Cycle Singleton" - which means it will be created
         * upon the start of the request (or at first de-reference) and live until
         * the end of the page life cycle.
         */
        public static ActiveEvents Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(ActiveEvents))
                    {
                        if (_instance == null)
                            _instance = new ActiveEvents();
                    }
                }
                return _instance;
            }
        }

        /**
         * Loads a control with the given name (class name) into the given position (name of Ra.Dynamic in
         * the Viewport currently used). Use this method to load Modules. Notice
         * that there exists an overload of this method which takes an object parameter that will be 
         * passed into the InitialLoading method when control is loaded.
         */
        public void RaiseLoadControl(string name, string position)
        {
            RaiseLoadControl(name, position, null);
        }

        /**
         * Loads a control with the given name (class name) into the given position (name of Ra.Dynamic in
         * the Viewport currently used). Use this method to load Modules. This overload of the method
         * will pass the "initializingArgument" parameter into the InitialLoading method when control 
         * is loaded.
         */
        public void RaiseLoadControl(string name, string position, Node parameters)
        {
            Node tmpNode = new Node("LoadControl");
            tmpNode["Name"].Value = name;
            tmpNode["Position"].Value = position;
            if (parameters == null)
                tmpNode["Parameters"].Value = null;
            else
                tmpNode["Parameters"].AddRange(parameters);
            RaiseActiveEvent(this, "LoadControl", tmpNode);
        }

        /**
         * Clear all controls out of the position (Ra-Dynamic) of your Viewport.
         */
        public void RaiseClearControls(string position)
        {
            Node tmp = new Node("ClearControls");
            tmp["Position"].Value = position;
            RaiseActiveEvent(this, "ClearControls", tmp);
        }

        /**
         * Raises an event with null as the initialization parameter.
         * This will dispatch control to all the ActiveEvent that are marked with
         * the Name attribute matching the name parameter of this method call.
         */
        public void RaiseActiveEvent(object sender, string name)
        {
            RaiseActiveEvent(sender, name, null);
        }

        // TODO: Refactor. WAY too long...!
        /**
         * Raises an event. This will dispatch control to all the ActiveEvent that are marked with
         * the Name attribute matching the name parameter of this method call.
         */
        public void RaiseActiveEvent(
            object sender, 
            string name, 
            Node pars)
        {
            ActiveEventArgs e = new ActiveEventArgs(name, pars);
            if (_methods.ContainsKey(name) || InstanceMethod.ContainsKey(name))
            {
                // We must run this in two operations since events clear controls out
                // and hence make "dead references" to Event Handlers and such...
                // Therefor we first iterate and find all event handlers interested in
                // this event before we start calling them one by one. But every time in
                // between calling the next one, we must verify that it still exists within
                // the collection...
                List<Tuple<MethodInfo, Tuple<object, bool>>> tmp = 
                    new List<Tuple<MethodInfo, Tuple<object, bool>>>();

                // Adding static method (if any)
                if (_methods.ContainsKey(name))
                {
                    foreach (Tuple<MethodInfo, Tuple<object, bool>> idx in _methods[name])
                    {
                        tmp.Add(idx);
                    }
                }

                // Adding instance method (if any)
                if (InstanceMethod.ContainsKey(name))
                {
                    foreach (Tuple<MethodInfo, Tuple<object, bool>> idx in InstanceMethod[name])
                    {
                        tmp.Add(idx);
                    }
                }

                // Looping through all methods...
                foreach (Tuple<MethodInfo, Tuple<object, bool>> idx in tmp)
                {
                    // Since events might load and clear controls we need to check if the event handler
                    // still exists after *every* event handler we dispatch control to...
                    List<Tuple<MethodInfo, Tuple<object, bool>>> recheck = new List<Tuple<MethodInfo, Tuple<object, bool>>>();

                    // Adding static method (if any)
                    if (_methods.ContainsKey(name))
                    {
                        foreach (Tuple<MethodInfo, Tuple<object, bool>> idx2 in _methods[name])
                        {
                            recheck.Add(idx);
                        }
                    }

                    // Adding instance method (if any)
                    if (InstanceMethod.ContainsKey(name))
                    {
                        foreach (Tuple<MethodInfo, Tuple<object, bool>> idx2 in InstanceMethod[name])
                        {
                            recheck.Add(idx);
                        }
                    }

                    foreach (Tuple<MethodInfo, Tuple<object, bool>> idx2 in recheck)
                    {
                        if (idx.Equals(idx2))
                        {
                            MethodInfo method = idx.Left;
                            object context = idx.Right.Left;
                            bool async = idx.Right.Right;
                            if (async)
                            {
                                ThreadPool.QueueUserWorkItem(
                                    delegate
                                    {
                                        method.Invoke(context, new[]{ sender, e });
                                    });
                            }
                            else
                            {
                                method.Invoke(context, new[] { sender, e });
                            }
                            break; // Then we break out of the inner loop giving control back to the outer
                        }
                    }
                }
            }
        }

        // TODO: Remove or make internal somehow...?
        public void RemoveListener(object context)
        {
            // Removing all event handler with the given context (object instance)
            foreach (string idx in InstanceMethod.Keys)
            {
                List<Tuple<MethodInfo, Tuple<object, bool>>> idxCur = InstanceMethod[idx];
                List<Tuple<MethodInfo, Tuple<object, bool>>> toRemove = new List<Tuple<MethodInfo, Tuple<object, bool>>>();
                foreach (Tuple<MethodInfo, Tuple<object, bool>> idxObj in idxCur)
                {
                    if (idxObj.Right.Left == context)
                        toRemove.Add(idxObj);
                }
                foreach (Tuple<MethodInfo, Tuple<object, bool>> idxObj in toRemove)
                {
                    idxCur.Remove(idxObj);
                }
            }

            // Remving all list of event handlers that no longer have any events...
            List<string> toBeRemoved = new List<string>();
            foreach(string idx in InstanceMethod.Keys)
            {
                if(InstanceMethod[idx].Count == 0)
                    toBeRemoved.Add(idx);
            }
            foreach (string idx in toBeRemoved)
            {
                InstanceMethod.Remove(idx);
            }
        }

        private Dictionary<string, List<Tuple<MethodInfo, Tuple<object, bool>>>> _nonWeb = new Dictionary<string, List<Tuple<MethodInfo, Tuple<object, bool>>>>();
        private Dictionary<string, List<Tuple<MethodInfo, Tuple<object, bool>>>> InstanceMethod
        {
            get
            {
                // NON-web scenario...
                if (HttpContext.Current == null)
                    return _nonWeb;

                Page page = (Page)HttpContext.Current.Handler;
                if (!page.Items.Contains("__Ra.Brix.Loader.ActiveEvents._requestEventHandlers"))
                {
                    page.Items["__Ra.Brix.Loader.ActiveEvents._requestEventHandlers"] =
                        new Dictionary<string, List<Tuple<MethodInfo, Tuple<object, bool>>>>();
                }
                return (Dictionary<string, List<Tuple<MethodInfo, Tuple<object, bool>>>>)
                    page.Items["__Ra.Brix.Loader.ActiveEvents._requestEventHandlers"];
            }
        }

        internal void AddListener(object context, MethodInfo method, string name, bool async)
        {
            if (name == null)
            {
                throw new ArgumentException("Cannot have an event handler listening to 'null' event");
            }
            if (context == null)
            {
                // Static event handler, will *NEVER* be cleared until application
                // itself is restarted
                if (!_methods.ContainsKey(name))
                    _methods[name] = new List<Tuple<MethodInfo, Tuple<object, bool>>>();
                _methods[name].Add(new Tuple<MethodInfo, Tuple<object, bool>>(method, new Tuple<object, bool>(context, async)));
            }
            else
            {
                // Request "instance" event handler, will be tossed away when
                // request is over
                if (!InstanceMethod.ContainsKey(name))
                    InstanceMethod[name] = new List<Tuple<MethodInfo, Tuple<object, bool>>>();
                InstanceMethod[name].Add(new Tuple<MethodInfo, Tuple<object, bool>>(method, new Tuple<object, bool>(context, async)));
            }
        }
    }
}