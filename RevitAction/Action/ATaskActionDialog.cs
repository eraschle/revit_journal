using DataSource.Model.FileSystem;
using RevitAction.Report;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitAction.Action
{
    public abstract class ATaskActionDialog : ATaskAction, ITaskActionDialog
    {
        protected ATaskActionDialog(string name) : base(name) { }

        public abstract string DialogId { get; }
     
        public abstract IEnumerable<string> AllowedDialogs { get; }
        
        public abstract string DialogButton { get; }
        
        public abstract string AssemblyPath { get; set; }
        
        public abstract string Namespace { get; }
        
        public abstract string FullClassName { get; }
        
        public abstract string VendorId { get; }
    }
}