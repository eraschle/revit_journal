using Newtonsoft.Json;
using System;

namespace RevitAction.Action
{
    public class DialogHandler
    {
        public const int OK = 1;
        public const int CANCEL = 2;
        public const int YES = 6;
        public const int NO = 7;

        [JsonIgnore]
        public Guid ActionId { get; set; }

        public string DialogId { get; set; }

        public int ButtonId { get; set; }

        public DialogHandler() : this(Guid.Empty, string.Empty, 0) { }

        public DialogHandler(Guid actionId, string dialogId, int buttonId)
        {
            ActionId = actionId;
            DialogId = dialogId ?? string.Empty;
            ButtonId = buttonId;
        }
    }
}
