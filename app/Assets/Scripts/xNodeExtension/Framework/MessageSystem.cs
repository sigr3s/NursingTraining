using System;
using UnityEngine;

namespace NT
{
    public class MessageSystem {
        public static Action<string> onMessageSent;    

        public static void SendMessage(string message){
            onMessageSent?.Invoke(message);
        }    
    }
}
