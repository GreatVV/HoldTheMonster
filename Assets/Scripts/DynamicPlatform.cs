using UnityEngine;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class DynamicPlatform : MonoBehaviour
{

    public GameObject PlatformPrefab;
    public float Speed = 2f;
    // Use this for initialization
    void Start ()
    {
        var target = transform;
        var dragDelta = target.UpdateAsObservable().Select(x => Camera.main.ScreenToWorldPoint(Input.mousePosition)).Buffer(2).Select(buffer =>
        {
            var delta = buffer.Last() - buffer.First();
            return delta;
        }).Where(delta => delta.magnitude > Mathf.Epsilon);

	    var buttonDown = target.UpdateAsObservable()
	                           .Where(x => Input.GetMouseButtonDown(0))
	                           .Select(x => Camera.main.ScreenToWorldPoint(Input.mousePosition));
         
        var buttonUp = target.UpdateAsObservable().Where(x => Input.GetMouseButtonUp(0));

        buttonDown.Subscribe(buttonDownPosition =>
        {
            //Create a platform
            var go = Instantiate(PlatformPrefab);
            go.transform.position = (Vector2)buttonDownPosition;

            dragDelta.TakeUntil(buttonUp).Subscribe(
                delta =>
                {
                    //rotate it
                    go.transform.Rotate(Vector3.forward, delta.x * Speed);
                },
                () =>
                {
                    //remove platfrom
                    Destroy(go);
                });
        });
    }
}
