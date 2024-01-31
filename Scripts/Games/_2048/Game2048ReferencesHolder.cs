using Core;
using Games.View;
using UnityEngine;
using UnityEngine.UI;

namespace Games
{
    public class Game2048ReferencesHolder : GameReferencesHolder
    {
        [SerializeField]
        private TableMapView _tableMapView;
        [SerializeField]
        private SwipeReceiver _swipeReceiver;

        public TableMapView TableMapView => _tableMapView;
        public SwipeReceiver SwipeReceiver => _swipeReceiver;

        public override void CollectData()
        {
            base.CollectData();

            _tableMapView = FindObjectOfType<TableMapView>();
            _swipeReceiver = FindObjectOfType<SwipeReceiver>();
        }
    }
}