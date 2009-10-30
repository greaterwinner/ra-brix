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

namespace Ra.Brix.Loader
{
    /**
     * Class contains methods for raising events and other helpers, like for instance helpers
     * to load controls and such.
     */
    public sealed class ActiveEvents
    {
        private readonly Dictionary<string, List<Tuple<MethodInfo, object>>> _methods =
            new Dictionary<string, List<Tuple<MethodInfo, object>>>();
        private static ActiveEvents _instance;

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
            if (_methods.ContainsKey(name))
            {
                // We must run this in two operations since events clear controls out
                // and hence make "dead references" to Event Handlers and such...
                // Therefor we first iterate and find all event handlers interested in
                // this event before we start calling them one by one. But every time in
                // between calling the next one, we must verify that it still exists within
                // the collection...
                List<Tuple<MethodInfo, object>> tmp = new List<Tuple<MethodInfo, object>>();
                foreach (Tuple<MethodInfo, object> idx in _methods[name])
                {
                    tmp.Add(idx);
                }
                foreach (Tuple<MethodInfo, object> idx in tmp)
                {
                    // Since events might load and clear controls we need to check if the event handler
                    // still exists after *every* event handler we dispatch control to...
                    foreach (Tuple<MethodInfo, object> idx2 in _methods[name])
                    {
                        if (idx.Equals(idx2))
                        {
                            idx.Left.Invoke(idx.Right, new[] { sender, e });
                            break; // Then we break out of the inner loop giving control back to the outer
                        }
                    }
                }
            }
        }

        // TODO: Remove or make internal somehow...?
        public void RemoveListener(object context)
        {
            foreach (string idx in _methods.Keys)
            {
                List<Tuple<MethodInfo, object>> idxCur = _methods[idx];
                List<Tuple<MethodInfo, object>> toRemove = new List<Tuple<MethodInfo, object>>();
                foreach (Tuple<MethodInfo, object> idxObj in idxCur)
                {
                    if (idxObj.Right == context)
                        toRemove.Add(idxObj);
                }
                foreach (Tuple<MethodInfo, object> idxObj in toRemove)
                {
                    idxCur.Remove(idxObj);
                }
            }
        }

        internal void AddListener(object context, MethodInfo method, string name)
        {
            if (name == null)
            {
                throw new ArgumentException("Cannot have an event handler listening to 'null' event");
            }
            if (!_methods.ContainsKey(name))
                _methods[name] = new List<Tuple<MethodInfo, object>>();
            _methods[name].Add(new Tuple<MethodInfo, object>(method, context));
        }

        internal void ClearAllInstanceEventHandlers()
        {
            foreach (string idxKey in _methods.Keys)
            {
                List<Tuple<MethodInfo, object>> toBeRemoved = new List<Tuple<MethodInfo, object>>();
                foreach (Tuple<MethodInfo, object> idxTuple in _methods[idxKey])
                {
                    if (idxTuple.Right != null)
                        toBeRemoved.Add(idxTuple);
                }
                foreach (Tuple<MethodInfo, object> idxToBeRemoved in toBeRemoved)
                {
                    _methods[idxKey].Remove(idxToBeRemoved);
                }
            }
        }
    }
}