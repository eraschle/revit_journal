using DataSource.Models.FileSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.System;

namespace RevitAction.Action
{
    public class TaskDialogAction
    {
        public Guid ActionId { get; set; }

        public string Name { get; set; }

        public ICollection<DialogHandler> DialogHandlers { get; set; } = new List<DialogHandler>();

        [JsonIgnore]
        public bool HasDialogHandlers
        {
            get { return DialogHandlers is object && DialogHandlers.Count > 0; }
        }

        public bool HasDialogHandler(string dialogId, out DialogHandler dialogHandler)
        {
            dialogHandler = DialogHandlers.FirstOrDefault(dlg => StringUtils.Equals(dlg.DialogId, dialogId));
            return dialogHandler != null;
        }

        public TaskDialogAction() : this(Guid.Empty, string.Empty, new List<DialogHandler>()) { }

        public TaskDialogAction(Guid actionId, string name, ICollection<DialogHandler> dialogHandlers)
        {
            ActionId = actionId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DialogHandlers = dialogHandlers ?? throw new ArgumentNullException(nameof(dialogHandlers));
        }
    }
}