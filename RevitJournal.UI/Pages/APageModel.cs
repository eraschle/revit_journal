using Utilities.UI;

namespace RevitJournalUI.Pages
{
    public abstract class APageModel : ANotifyPropertyChangedModel
    {
        public abstract object ModelData { get; }

        public abstract void SetModelData(object data);
    }
}
