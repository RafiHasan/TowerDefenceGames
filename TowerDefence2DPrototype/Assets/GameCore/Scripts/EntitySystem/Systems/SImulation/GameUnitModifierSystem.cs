using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[UpdateInGroup(typeof(NormalSystemGroup))]
public partial class GameUnitModifierSystem : SystemBase
{
    protected override void OnUpdate()
    {

        if (!SystemAPI.HasSingleton<GameUnitListComponent>())
            return;

        GameUnitListComponent gameUnitListComponent = SystemAPI.GetSingleton<GameUnitListComponent>();
        EntityManager entityManager = World.EntityManager;
        if (UnitManager.Instance == null)
        {
            for (int i = 0; i < gameUnitListComponent.UnitList.Count; i++)
            {
                Entity entity = gameUnitListComponent.UnitList[i];

                if (!entityManager.HasComponent<StatsComponent>(entity))
                    return;

                StatsComponent statsComponent = entityManager.GetComponentData<StatsComponent>(entity);

                for (int j = 0; j < statsComponent.Stats.Count; j++)
                {
                    Stat stat = statsComponent.Stats[j];

                    float value = stat.value;
                    float offset = 0;
                    StatID statID = stat.ID;

                    if (stat.ID == StatID.DAMAGEMIN)
                    {
                        statID = StatID.DAMAGE;
                        value = (statsComponent.GetStatValue(StatID.DAMAGEMAX) + statsComponent.GetStatValue(StatID.DAMAGEMIN)) / 2;
                        offset = (statsComponent.GetStatValue(StatID.DAMAGEMAX) - statsComponent.GetStatValue(StatID.DAMAGEMIN));
                    }

                    else if (stat.ID == StatID.DEBUFFTIMEMIN)
                    {
                        statID = StatID.DEBUFFTIME;
                        value = (statsComponent.GetStatValue(StatID.DEBUFFTIMEMAX) + statsComponent.GetStatValue(StatID.DEBUFFTIMEMIN)) / 2;
                        offset = (statsComponent.GetStatValue(StatID.DEBUFFTIMEMAX) - statsComponent.GetStatValue(StatID.DEBUFFTIMEMIN));
                    }

                    else if (stat.ID == StatID.COOLDOWNMIN)
                    {
                        statID = StatID.COOLDOWN;
                        value = (statsComponent.GetStatValue(StatID.COOLDOWNMIN) + statsComponent.GetStatValue(StatID.COOLDOWNMAX)) / 2;
                        offset = (statsComponent.GetStatValue(StatID.COOLDOWNMAX) - statsComponent.GetStatValue(StatID.COOLDOWNMIN));
                    }

                    UpdateStats(entityManager, entity, statID, value, offset);
                }
            }

            return;
        }

        

        

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

            

            entityManager.SetComponentData(entity, card.stats);


            if(entityManager.HasComponent<SpriteRenderer>(entity))
            {
                SpriteRenderer spriterenderer=entityManager.GetComponentObject<SpriteRenderer>(entity);
                spriterenderer.color = card.color;
            }


            for(int j=0;j< card.stats.Stats.Count;j++)
            {
                Stat stat = card.stats.Stats[j];

                float value = stat.value;
                float offset = 0;
                StatID statID = stat.ID;
                
                if(stat.ID==StatID.DAMAGEMIN)
                {
                    statID = StatID.DAMAGE;
                    value = (card.stats.GetStatValue(StatID.DAMAGEMAX) + card.stats.GetStatValue(StatID.DAMAGEMIN)) / 2;
                    offset = (card.stats.GetStatValue(StatID.DAMAGEMAX) - card.stats.GetStatValue(StatID.DAMAGEMIN));
                }
                
                else if (stat.ID == StatID.DEBUFFTIMEMIN)
                {
                    statID = StatID.DEBUFFTIME;
                    value = (card.stats.GetStatValue(StatID.DEBUFFTIMEMAX) + card.stats.GetStatValue(StatID.DEBUFFTIMEMIN)) / 2;
                    offset = (card.stats.GetStatValue(StatID.DEBUFFTIMEMAX) - card.stats.GetStatValue(StatID.DEBUFFTIMEMIN));
                }
                
                else if (stat.ID == StatID.COOLDOWNMIN)
                {
                    statID = StatID.COOLDOWNMIN;
                    value = (card.stats.GetStatValue(StatID.COOLDOWNMIN) + card.stats.GetStatValue(StatID.COOLDOWNMAX)) / 2;
                    offset = (card.stats.GetStatValue(StatID.COOLDOWNMAX) - card.stats.GetStatValue(StatID.COOLDOWNMIN));     
                }

                UpdateStats(entityManager, entity, statID, value, offset);
            }
        }


    }


    protected void UpdateStats(EntityManager entityManager,Entity entity,StatID ID,float value,float offset)
    {
        switch(ID)
        {
            case StatID.NONE:
                break;
            case StatID.HEALTH:
                HealthComponent healthComponent = entityManager.GetComponentData<HealthComponent>(entity);
                healthComponent.Value = value;
                entityManager.SetComponentData(entity, healthComponent);
                break;
            case StatID.SPEED:
                MovementComponent movementComponent = entityManager.GetComponentData<MovementComponent>(entity);
                movementComponent.Speed = value;
                entityManager.SetComponentData(entity, movementComponent);
                break;
            case StatID.DAMAGE:
                DamageDealerComponent damageDealerComponent = entityManager.GetComponentData<DamageDealerComponent>(entity);
                damageDealerComponent.Damage = value;
                damageDealerComponent.DamageOffset = offset;
                entityManager.SetComponentData(entity, damageDealerComponent);
                break;
            case StatID.RANGE:
                SearchAbleComponent searchAbleComponent = entityManager.GetComponentData<SearchAbleComponent>(entity);
                searchAbleComponent.SearchRadious = value;
                entityManager.SetComponentData(entity, searchAbleComponent);
                break;
            case StatID.COOLDOWN:
                if (entityManager.HasComponent<DamageDealerComponent>(entity))
                {
                    DamageDealerComponent component = entityManager.GetComponentData<DamageDealerComponent>(entity);
                    component.CoolDown = value;
                    component.CoolDownOffset = offset;
                    entityManager.SetComponentData(entity, component);
                }
                else if (entityManager.HasComponent<BuffDebufferComponent>(entity))
                {
                    BuffDebufferComponent component = entityManager.GetComponentData<BuffDebufferComponent>(entity);
                    component.CoolDown = value;
                    component.CoolDownOffset = offset;
                    entityManager.SetComponentData(entity, component);
                }
                break;
            case StatID.DEBUFFTIME:
                BuffDebufferComponent debufferComponent = entityManager.GetComponentData<BuffDebufferComponent>(entity);
                debufferComponent.EffectTime = value;
                debufferComponent.EffectTimeOffset = offset;
                entityManager.SetComponentData(entity, debufferComponent);
                break;
            case StatID.SLOWPERCENTAGE:
                debufferComponent = entityManager.GetComponentData<BuffDebufferComponent>(entity);
                debufferComponent.EffectPower = value;
                entityManager.SetComponentData(entity, debufferComponent);
                break;
            case StatID.DAMAGEPERSECOND:
                debufferComponent = entityManager.GetComponentData<BuffDebufferComponent>(entity);
                debufferComponent.EffectPower = value;
                entityManager.SetComponentData(entity, debufferComponent);
                break;
        }
    }


}
