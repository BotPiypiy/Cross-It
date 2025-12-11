
using System;

public class EnemyControllerPool : ObjectPool<EnemyController>
{
    public Action AllEnemiesDead;

    public override void ReturnObject(EnemyController obj)
    {
        base.ReturnObject(obj);

        if(ActiveCount == 0)
        {
            AllEnemiesDead?.Invoke();
        }
    }
}
