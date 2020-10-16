using Utilities.UI;

namespace RevitJournalUI.Pages
{
    public abstract class APageModel : ANotifyPropertyChangedModel
    {
        public abstract void SetModelData(object data);
    }
}
