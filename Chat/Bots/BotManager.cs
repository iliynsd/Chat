using Chat.Models;
using System;
using System.Collections.Generic;

namespace Chat.Bots
{
    public sealed class BotManager : IObservable<ChatAction>
    {
        private readonly List<IObserver<ChatAction>> _observers;

        public BotManager()
        {
            _observers = new List<IObserver<ChatAction>>();
        }

        public IDisposable Subscribe(IObserver<ChatAction> observer)
        {
            _observers.Add(observer);
            return null;
        }

        public void Notify(ChatAction action)
        {
            foreach(IObserver<ChatAction> observer in _observers)
            {
                observer.OnNext(action);
            }
        }
    }
}
