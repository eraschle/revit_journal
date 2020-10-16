namespace RevitJournalUI.Pages
{
    public interface IPageView 
    {
        APageModel ViewModel { get; }

        void SetModelData(object data);
    }
}
