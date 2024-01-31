using Enums;
using JsonFx.Json;

namespace Core.UserStuff
{
    [JsonOptIn]
    public class SkinChooseData
    {
        [JsonMember]
        public SkinPart SkinPart { get; private set; }

        [JsonMember]
        public SkinType SkinType { get; private set; }

        public SkinChooseData() { }
        
        public SkinChooseData(SkinPart skinPart)
        {
            SkinType = SkinType.Default;
            SkinPart = skinPart;
        }

        public void SetSkin(SkinType skinType)
        {
            SkinType = skinType;
        }
    }
}