using System.Linq;
using Core.UserStuff;
using DependencyInjector;
using Enums;
using Graphics;
using UnityEngine;

namespace Core
{
    public abstract class SkinChanger : MonoBehaviour, IGameStartable
    {
        private IIconsService _iconsService;
        protected IUser _user;
        
        protected abstract GameType GameType { get; }
        
        public SkinType GetSkinType(SkinPart skinPart)
            => _user.GetChooseSkin(GameType, skinPart);

        protected Sprite[] GetSkins(SkinPart skinPart)
        {
            SkinType backSkin = GetSkinType(skinPart);
            return _iconsService.GetSkin(GameType, backSkin, skinPart);
        }
        
        protected Sprite GetSkin(SkinPart skinPart)
            => GetSkins(skinPart).First();

        void IGameStartable.OnStart(GameData gameData)
        {
            _iconsService = AllServices.Container.Single<IIconsService>();
            _user = AllServices.Container.Single<IUserService>().User;

            OnStart(gameData);
        }

        protected abstract void OnStart(GameData gameData);
    }
}