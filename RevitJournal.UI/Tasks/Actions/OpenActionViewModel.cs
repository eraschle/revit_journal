using RevitJournal.Revit.Command;
using System.ComponentModel;
using System.Linq;
using Utilities;

namespace RevitJournalUI.Tasks.Actions
{
    public class OpenActionViewModel : ActionViewModel
    {
        public override void UpdateParameters()
        {
            base.UpdateParameters();
            if(!(Action is DocumentOpenAction openAction)) { return; }

            var auditParameter = Parameters.FirstOrDefault(par => StringUtils.Equals(par.Name, openAction.Audit.Name));
            if(auditParameter is null) { return; }

            auditParameter.PropertyChanged += AuditParameter_PropertyChanged;
        }

        private void AuditParameter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Checked));
        }
    }
}
