using System;
using UnityEngine;
using Firebase.Messaging;

namespace Splash.Firebase
{
    public class NotificationManager : MonoBehaviour
    {
        public void Start() {
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        public void OnTokenReceived(object sender, TokenReceivedEventArgs token) {
            UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
        }

        public void OnMessageReceived(object sender, MessageReceivedEventArgs e) {
            UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
        }
    }
}