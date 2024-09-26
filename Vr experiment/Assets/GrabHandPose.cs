using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabHandPose : MonoBehaviour
{
    public HandData rightHandPose;
    public HandData leftHandPose;
    public Transform rightHandAttachPoint;
    public Transform leftHandAttachPoint;

    private XRGrabInteractable grabInteractable;
    private HandData leftHandData;
    private HandData rightHandData;

    private Vector3 leftHandStartPosition;
    private Quaternion leftHandStartRotation;
    private Vector3 rightHandStartPosition;
    private Quaternion rightHandStartRotation;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(SetupPose);
        grabInteractable.selectExited.AddListener(UnsetPose);
        
        rightHandPose.gameObject.SetActive(false);
        leftHandPose.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (grabInteractable.isSelected)
        {
            UpdateHandTransform(leftHandData, leftHandAttachPoint, leftHandPose);
            UpdateHandTransform(rightHandData, rightHandAttachPoint, rightHandPose);
        }
    }

    void UpdateHandTransform(HandData handData, Transform attachPoint, HandData posePrefab)
    {
        if (handData != null && attachPoint != null)
        {
            Vector3 positionOffset = posePrefab.root.localPosition;
            
            handData.root.position = attachPoint.position + attachPoint.rotation * positionOffset;
            
            handData.root.rotation = attachPoint.rotation * posePrefab.root.localRotation;
        }
    }

    public void SetupPose(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor directInteractor)
        {
            HandData handData = directInteractor.GetComponentInChildren<HandData>();
            handData.animator.enabled = false;

            if (handData.handType == HandData.HandModelType.Right)
            {
                rightHandData = handData;
                rightHandStartPosition = handData.root.localPosition;
                rightHandStartRotation = handData.root.localRotation;
                CopyHandData(rightHandPose, handData);
            }
            else
            {
                leftHandData = handData;
                leftHandStartPosition = handData.root.localPosition;
                leftHandStartRotation = handData.root.localRotation;
                CopyHandData(leftHandPose, handData);
            }
        }
    }

    public void UnsetPose(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor directInteractor)
        {
            HandData handData = directInteractor.GetComponentInChildren<HandData>();
            handData.animator.enabled = true;

            if (handData.handType == HandData.HandModelType.Right)
            {
                ResetHandTransform(handData, rightHandStartPosition, rightHandStartRotation);
                rightHandData = null;
            }
            else
            {
                ResetHandTransform(handData, leftHandStartPosition, leftHandStartRotation);
                leftHandData = null;
            }
        }
    }

    void CopyHandData(HandData source, HandData destination)
    {
        for (int i = 0; i < source.fingerBones.Length; i++)
        {
            destination.fingerBones[i].localRotation = source.fingerBones[i].localRotation;
        }
    }

    void ResetHandTransform(HandData handData, Vector3 startPosition, Quaternion startRotation)
    {
        handData.root.localPosition = startPosition;
        handData.root.localRotation = startRotation;
        
        for (int i = 0; i < handData.fingerBones.Length; i++)
        {
            handData.fingerBones[i].localRotation = Quaternion.identity;
        }
    }
}