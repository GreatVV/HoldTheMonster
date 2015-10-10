using UnityEngine;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class DynamicPlatform : MonoBehaviour
{

    public GameObject PlatformPrefab;
    public float Speed = 2f;
    public float MaxLength = 3;
    /*
    private void Start()
    {
        var target = transform;
        var dragDelta = target.UpdateAsObservable()
                              .Select(x => Camera.main.ScreenToWorldPoint(Input.mousePosition))
                              .Buffer(2)
                              .Select
            (
             buffer =>
             {
                 var delta = buffer.Last() - buffer.First();
                 return delta;
             }).Where(delta => delta.magnitude > Mathf.Epsilon);

        var buttonDown = target.UpdateAsObservable()
                               .Where(x => Input.GetMouseButtonDown(0))
                               .Select(x => Camera.main.ScreenToWorldPoint(Input.mousePosition));

        var buttonUp = target.UpdateAsObservable().Where(x => Input.GetMouseButtonUp(0));

        buttonDown.Subscribe
            (
             buttonDownPosition =>
             {
                 //Create a platform
                 var go = Instantiate(PlatformPrefab);
                 go.transform.position = (Vector2) buttonDownPosition;

                 dragDelta.TakeUntil(buttonUp).Subscribe
                     (
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
    }*/

    void Start ()
    {
            var target = transform;
            var endPoint = target.UpdateAsObservable().Select(x => Camera.main.ScreenToWorldPoint(Input.mousePosition)).Buffer(2).Select(buffer =>
            {
                var delta = buffer.Last() - buffer.First();
                return delta;
            }).Where(delta => delta.magnitude > Mathf.Epsilon).Select(x=> Camera.main.ScreenToWorldPoint(Input.mousePosition));

            var buttonDown = target.UpdateAsObservable()
                                   .Where(x => Input.GetMouseButtonDown(0))
                                   .Select(x => Camera.main.ScreenToWorldPoint(Input.mousePosition));

            var buttonUp = target.UpdateAsObservable().Where(x => Input.GetMouseButtonUp(0));

            buttonDown.Subscribe(buttonDownPosition =>
            {
                //Create a platform
                var go = Instantiate(PlatformPrefab);
                go.transform.position = (Vector2)buttonDownPosition;

                endPoint.TakeUntil(buttonUp).Subscribe(
                    point =>
                    {
                    //rotate it
                    //go.transform.Rotate(Vector3.forward, delta.x * Speed);
                        var length = Mathf.Clamp((buttonDownPosition - point).magnitude,0, MaxLength);
                        go.transform.localScale = new Vector3(length, go.transform.localScale.y, go.transform.localScale.z);

                        var start = buttonDownPosition;
                        var horizontal = new Vector3(length, 0, 0);
                        var end = point - start;

                        var angle = Vector3.Angle(end, horizontal);
                        if (point.y < buttonDownPosition.y)
                        {
                            angle = -angle;
                        }
                    
                        go.transform.localRotation = Quaternion.Euler(0,0,angle);
                    },
                    () =>
                    {
                    //remove platfrom
                    Destroy(go);
                    });
            });
        }
    }
