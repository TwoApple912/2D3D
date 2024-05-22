using Unity.VisualScripting;
using UnityEngine;

public class SyncPlayerLocation : MonoBehaviour
{
    private SnapDimensionToAxes _snapDimensionToAxes;
    
    private Character2DSpecificFunctions _character2DSpecificFunctions; // Use to access the standing-on object thru ---cast.
    
    public Transform threeDimensionCharacter;
    public Transform twoDimensionCharacter;

    void Start()
    {
        _snapDimensionToAxes = GameObject.Find("2D Dimension").GetComponent<SnapDimensionToAxes>();
        _character2DSpecificFunctions = twoDimensionCharacter.GetComponent<Character2DSpecificFunctions>();
    }

    public void SwitchTo2D()
    {
        twoDimensionCharacter.position = threeDimensionCharacter.position;
    }

    public void SwitchTo3D()
    {
        switch (_snapDimensionToAxes.numberOfAxes)
        {
            case 4:
                SwitchTo3DOn4Axes();
                break;
            case 360:
                // TODO: 360 degree camera snap
                break;
        }
        
        /*// Check if the object is above the colliding object's center to transition
        if ((_character2DSpecificFunctions.gameObject.GetComponent<Collider>().bounds.center.y / _character2DSpecificFunctions.standingPosition.transform.position.y) > 0.5f)
        {
            /* This part is very confusing so bear with me here.
               Due to the SphereCast takes in the first point collided (which is in the rear of the platform, I will
                   manually shift the position of the player closer to the center of the platform while maintaining the y
                   value.
               pointA : SphereCast hit.point.
               pointB : the colliding object's center.
            #1#
            Vector3 pointA = new Vector3(
                _character2DSpecificFunctions.standingPosition.point.x,
                _character2DSpecificFunctions.standingPosition.point.y + 0.5f,
                _character2DSpecificFunctions.standingPosition.point.z
            );
            Vector3 pointB = new Vector3(
                _character2DSpecificFunctions.standingPosition.transform.position.x,
                pointA.y,
                _character2DSpecificFunctions.standingPosition.transform.position.z
            );
            Vector3 newPosition = Vector3.Lerp(pointA, pointB, 0.25f);

            threeDimensionCharacter.position = newPosition;
        }
        else threeDimensionCharacter.position = twoDimensionCharacter.position;*/
    }

    private void SwitchTo3DOn4Axes()
    {
        if (_character2DSpecificFunctions.standingPosition.transform.gameObject.layer != LayerMask.NameToLayer("Level Element"))
        {
            threeDimensionCharacter.position = twoDimensionCharacter.transform.position;
        }
        else if (_character2DSpecificFunctions.gameObject.GetComponent<Collider>().bounds.center.y / _character2DSpecificFunctions.standingPosition.transform.position.y > 0.5f)
        {
            if (_snapDimensionToAxes.snappedCamAngle == 0 || _snapDimensionToAxes.snappedCamAngle == 180)
            {
                Vector3 newPosition = new Vector3(twoDimensionCharacter.position.x, twoDimensionCharacter.position.y, _character2DSpecificFunctions.standingPosition.transform.position.z);
                        
                threeDimensionCharacter.position = newPosition;
            }
            else if (_snapDimensionToAxes.snappedCamAngle == 90 || _snapDimensionToAxes.snappedCamAngle == 270)
            {
                Vector3 newPosition = new Vector3(_character2DSpecificFunctions.standingPosition.transform.position.x, twoDimensionCharacter.position.y, twoDimensionCharacter.position.z);
                        
                threeDimensionCharacter.position = newPosition;
            }
        }
    }
}