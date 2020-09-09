using RevitAction.Action;
using System;
using System.Collections.Generic;

namespace RevitCommand.JournalCommand
{
    public class DocumentPurgeUnusedCommand : ATaskActionDialog
    {
        public DocumentPurgeUnusedCommand() : base("Purge unused") { }

        public override string DialogId => throw new NotImplementedException();

        public override IEnumerable<string> AllowedDialogs => throw new NotImplementedException();

        public override string DialogButton => throw new NotImplementedException();

        public override string AssemblyPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override string Namespace => throw new NotImplementedException();

        public override string FullClassName => throw new NotImplementedException();

        public override string VendorId => throw new NotImplementedException();

        public override Guid Id => throw new NotImplementedException();
    }
}
