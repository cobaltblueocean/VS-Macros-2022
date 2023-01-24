﻿//-----------------------------------------------------------------------
// <copyright file="Engine.cs" company="Microsoft Corporation">
//     Copyright Microsoft Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using ExecutionEngine.Enums;
using ExecutionEngine.Helpers;
using ExecutionEngine.Interfaces;
using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using VSMacros.ExecutionEngine;

namespace ExecutionEngine
{
    public sealed class Engine : IDisposable
    {
        private IActiveScript engine;
        private Parser parser;
        private Site scriptSite;
        private object dispatch;

        public static object DteObject { get; private set; }
        public static object CommandHelper { get; private set; }

        private IMoniker GetItemMoniker(int pid, string version)
        {
            IMoniker moniker;
            ErrorHandler.ThrowOnFailure(NativeMethods.CreateItemMoniker("!", string.Format(CultureInfo.InvariantCulture, "VisualStudio.DTE.{0}:{1}", version, pid), out moniker));

            return moniker;
        }

        private IRunningObjectTable GetRunningObjectTable()
        {
            IRunningObjectTable rot;
            int hr = NativeMethods.GetRunningObjectTable(0, out rot);
            if (ErrorHandler.Failed(hr))
            {
                ErrorHandler.ThrowOnFailure(hr, null);
            }

            return rot;
        }

        private object GetDteObject(IRunningObjectTable rot, IMoniker moniker)
        {
            object dteObject;
            int hr = rot.GetObject(moniker, out dteObject);
            if (ErrorHandler.Failed(hr))
            {
                ErrorHandler.ThrowOnFailure(hr, null);
            }

            return dteObject;
        }

        private void InitializeDteObject(int pid, string version)
        {
            IMoniker moniker = this.GetItemMoniker(pid, version);
            IRunningObjectTable rot = this.GetRunningObjectTable();
            Engine.DteObject = this.GetDteObject(rot, moniker);

            if (Engine.DteObject == null)
            {
                throw new InvalidOperationException();
            }
        }

        private void InitializeCommandHelper()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var globalProvider = ServiceProvider.GlobalProvider;
            Validate.IsNotNull(globalProvider, "globalProvider");

            Engine.CommandHelper = new CommandHelper(globalProvider);
            Validate.IsNotNull(Engine.CommandHelper, "Engine.CommandHelper");
        }

        internal IActiveScript CreateEngine()
        {
            Type engine = Type.GetTypeFromProgID("jscript", true);
            return Activator.CreateInstance(engine) as IActiveScript;
        }

        public Engine(int pid, string version)
        {
            this.engine = this.CreateEngine();
            this.scriptSite = new Site();
            this.parser = new Parser(this.engine);
            this.engine.SetScriptSite(this.scriptSite);

            InformEngineOfNewObjects(pid, version);
        }

        private void InformEngineOfNewObjects(int pid, string version)
        {
            const string dte = "dte";
            this.InitializeDteObject(pid, version);
            this.engine.AddNamedItem(dte, ScriptItem.CodeOnly | ScriptItem.IsVisible);

            const string cmdHelper = "cmdHelper";
            this.InitializeCommandHelper();
            this.engine.AddNamedItem(cmdHelper, ScriptItem.CodeOnly | ScriptItem.IsVisible);
        }

        public void Dispose()
        {
            if (this.engine != null)
            {
                this.engine = null;
            }
        }

        public bool CallMethod(string methodName, params object[] arguments)
        {
            try
            {
                this.dispatch.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, this.dispatch, arguments);
                return true;
            }
            catch (Exception e)
            {
                var internalException = e.InnerException;
                if (!Site.RuntimeError)
                {
                    Site.InternalError = true;
                    Site.InternalVSException = new InternalVSException(e.Message, e.Source, e.StackTrace, e.TargetSite.ToString());
                }
                return false;
            }
        }

        internal void Parse(string unparsed)
        {
            try
            {
                this.engine.SetScriptState(ScriptState.Connected);
                this.parser.Parse(unparsed);

                IntPtr dispatch;
                this.engine.GetScriptDispatch(null, out dispatch);
                this.dispatch = Marshal.GetObjectForIUnknown(dispatch);
            }
            catch (Exception e)
            {
                Site.InternalError = true;
                Site.InternalVSException = new InternalVSException(e.Message, e.Source, e.StackTrace, e.TargetSite.ToString());
            }
        }
    }
}
