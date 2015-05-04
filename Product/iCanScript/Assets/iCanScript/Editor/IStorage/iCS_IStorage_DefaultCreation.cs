using UnityEngine;
using System.Collections;
using P=Prelude;

namespace iCanScript.Editor {
    
    public partial class iCS_IStorage {
        // ---------------------------------------------------------------------------------
        public void CreateDefaultObjectsForVisualScript() {
            // Create root behaviour object.
            CreateBehaviour(iCSMonoBehaviour.name);
            // Create Default message handlers.
            var messages= iCS_LibraryDatabase.GetMessages(typeof(MonoBehaviour));
            var update= P.filter(o=> o.DisplayName == "Update", messages);
            if(update.Length ==  1) {
                CreateMessageHandler(0, update[0]);            
            }
        }
    }

}
