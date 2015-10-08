using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

public class Platform : MonoBehaviour
{
    public bool IsVertical;
    public Vector2 MinPosition;
    public Vector2 MaxPosition;

    protected void Start()
    {
        var target = transform;
        var dragDelta = target.UpdateAsObservable().Select(x => Camera.main.ScreenToWorldPoint(Input.mousePosition)).Buffer(2).Select(buffer =>
        {
            var delta = buffer.Last() - buffer.First();
            return delta;
        }).Where(delta => delta.magnitude > Mathf.Epsilon);

        var buttonDown = target.UpdateAsObservable()
            .Where(x => Input.GetMouseButtonDown(0))
            .Select(x=> Camera.main.ScreenToWorldPoint(Input.mousePosition))
            .Where(
                   x =>
                   {
                       var col = Physics2D.OverlapPoint(x);
                       if (col)
                       {
                           return col.transform == target;
                       }
                       return false;
                   });
        var buttonUp = target.UpdateAsObservable().Where(x => Input.GetMouseButtonUp(0));

        buttonDown.Subscribe(_ =>
        {
            dragDelta.TakeUntil(buttonUp).Subscribe(delta =>
            {
                if (IsVertical)
                {
                    delta.x = 0;
                }
                else
                {
                    delta.y = 0;
                }
                target.position = new Vector2(
                    Mathf.Clamp(target.position.x + delta.x, MinPosition.x, MaxPosition.x), 
                    Mathf.Clamp(target.position.y + delta.y, MinPosition.y, MaxPosition.y));
                
            });
        });
    }
}