using UnityEngine;
using System.Collections;

/// <summary>
/// This Script allows moving a game object using mouse input. The script requires a Collider for testing on MouseClick.
/// </summary>
[RequireComponent(typeof(Collider))]
public class DragObjects : RenBehaviour {

    private Vector3 ScreenSpace;

    protected override void Update()
    {
        base.Update();
        ScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
    }

    protected IEnumerator OnMouseDown()
    {
        Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, ScreenSpace.z));

        while (Input.GetMouseButton(0))
        {
            Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, ScreenSpace.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
            curPosition.z = transform.position.z;

            transform.position = curPosition;
            
            yield return null;

        }
    }
}
