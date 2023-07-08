using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

public struct GameUnitListComponent : IComponentData
{
    public Entity Parent;
    public CustomFixedList16<Entity> UnitList;
}
