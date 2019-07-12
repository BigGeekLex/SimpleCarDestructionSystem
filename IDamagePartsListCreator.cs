using System.Collections.Generic;

namespace DamageSystem
{
    public interface IDamagePartsListCreator
    {
        void SetParts(List<DamageParts> parts);
    }
}