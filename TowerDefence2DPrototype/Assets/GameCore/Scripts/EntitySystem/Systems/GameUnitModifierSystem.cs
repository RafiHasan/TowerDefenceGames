using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class GameUnitModifierSystem : SystemBase
{
    protected override void OnUpdate()
    {

        if (UnitManager.Instance == null)
            return;

        if (!SystemAPI.HasSingleton<GameUnitListComponent>())
            return;

        GameUnitListComponent gameUnitListComponent = SystemAPI.GetSingleton<GameUnitListComponent>();

        if(UnitManager.Instance.unitCards.Count!= gameUnitListComponent.UnitList.Count)
        {
            UnitManager.Instance.Clear();
            for(int i= 0; i< gameUnitListComponent.UnitList.Count;i++)
            {

                if(World.EntityManager.HasComponent<StatsComponent>(gameUnitListComponent.UnitList[i]))
                {
                    
                    if (World.EntityManager.HasComponent<SpriteRenderer>(gameUnitListComponent.UnitList[i]))
                    {
                        SpriteRenderer spriterenderer = World.EntityManager.GetComponentObject<SpriteRenderer>(gameUnitListComponent.UnitList[i]);
                        UnitManager.Instance.AddUnit(World.EntityManager.GetComponentData<StatsComponent>(gameUnitListComponent.UnitList[i]), spriterenderer.color);
                    }
                    else
                    {
                        UnitManager.Instance.AddUnit(World.EntityManager.GetComponentData<StatsComponent>(gameUnitListComponent.UnitList[i]),Color.white);
                    }
                }
                
            }
            return;
        }



        for (int i = 0; i < gameUnitListComponent.UnitList.Count; i++)
        {

            UnitCard card = UnitManager.Instance.unitCards[i];

            Entity entity = gameUnitListComponent.UnitList[i];

            EntityManager entityManager = World.EntityManager;

            entityManager.SetComponentData(entity, card.stats);


            if(entityManager.HasComponent<SpriteRenderer>(entity))
            {
                SpriteRenderer spriterenderer=entityManager.GetComponentObject<SpriteRenderer>(entity);
                spriterenderer.color = card.color;
            }


            for(int j=0;j< card.stats.Stats.Count;j++)
            {
                Stat stat = card.stats.Stats[j];
                if (stat.ID==StatID.HEALTH)
                {
                    HealthComponent component = entityManager.GetComponentData<HealthComponent>(entity);
                    component.Value = stat.value;
                    entityManager.SetComponentData(entity, component);
                }
                else if (stat.ID == StatID.RANGE)
                {
                    SearchAbleComponent component = entityManager.GetComponentData<SearchAbleComponent>(entity);
                    component.SearchRadious = stat.value;
                    entityManager.SetComponentData(entity, component);
                }
                else if (stat.ID == StatID.DAMAGE)
                {
                    DamageDealerComponent component = entityManager.GetComponentData<DamageDealerComponent>(entity);
                    component.Damage = stat.value;
                    component.DamageOffset = 0;
                    entityManager.SetComponentData(entity, component);
                }
                else if (stat.ID == StatID.COOLDOWN)
                {
                    if (entityManager.HasComponent<DamageDealerComponent>(entity))
                    {
                        DamageDealerComponent component = entityManager.GetComponentData<DamageDealerComponent>(entity);
                        component.CoolDown = stat.value;
                        component.CoolDownOffset = 0;
                        entityManager.SetComponentData(entity, component);
                    }
                    else if (entityManager.HasComponent<BuffDebufferComponent>(entity))
                    {
                        BuffDebufferComponent component = entityManager.GetComponentData<BuffDebufferComponent>(entity);
                        component.CoolDown = stat.value;
                        component.CoolDownOffset = 0;
                        entityManager.SetComponentData(entity, component);
                    }
                }
                else if(stat.ID==StatID.DAMAGEMIN)
                {
                    DamageDealerComponent component = entityManager.GetComponentData<DamageDealerComponent>(entity);
                    component.Damage = (card.stats.GetStatValue(StatID.DAMAGEMIN) + card.stats.GetStatValue(StatID.DAMAGEMAX))/2;
                    component.DamageOffset = (card.stats.GetStatValue(StatID.DAMAGEMAX)-card.stats.GetStatValue(StatID.DAMAGEMIN));
                    entityManager.SetComponentData(entity, component);                   
                }
                else if (stat.ID == StatID.COOLDOWNMIN)
                {
                    if(entityManager.HasComponent<DamageDealerComponent>(entity))
                    {
                        DamageDealerComponent component = entityManager.GetComponentData<DamageDealerComponent>(entity);
                        component.CoolDown = (card.stats.GetStatValue(StatID.COOLDOWNMIN) + card.stats.GetStatValue(StatID.COOLDOWNMAX)) / 2;
                        component.CoolDownOffset = (card.stats.GetStatValue(StatID.COOLDOWNMAX) - card.stats.GetStatValue(StatID.COOLDOWNMIN));
                        entityManager.SetComponentData(entity, component);
                    }
                    else if(entityManager.HasComponent<BuffDebufferComponent>(entity))
                    {
                        BuffDebufferComponent component = entityManager.GetComponentData<BuffDebufferComponent>(entity);
                        component.CoolDown = (card.stats.GetStatValue(StatID.COOLDOWNMIN) + card.stats.GetStatValue(StatID.COOLDOWNMAX)) / 2;
                        component.CoolDownOffset = (card.stats.GetStatValue(StatID.COOLDOWNMAX) - card.stats.GetStatValue(StatID.COOLDOWNMIN));
                        entityManager.SetComponentData(entity, component);
                    }                   
                }
            }


            
        }


    }
}
