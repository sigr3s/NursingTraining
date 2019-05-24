using UnityEngine;

public class CallbackHierarchyItem : NodeHierarchyItem {
    public void Execute(){
        SessionManager.Instance.StartExecutionWithMessage(data.key);
    }
}