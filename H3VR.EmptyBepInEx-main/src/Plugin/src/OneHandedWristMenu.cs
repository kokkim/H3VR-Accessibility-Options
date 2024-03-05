using System;
using System.Collections;
using UnityEngine;
using BepInEx;
using FistVR;

namespace AccessibilityOptions
{
    class OneHandedWristMenu : MonoBehaviour
    {
        GameObject pointerPrefab;
        Material pointerMat;

        float verticalPointerOffset;
        Color pointerColor;
        float pointerScale;

        FVRViveHand curHand;    //the hand the wrist menu is attached to
        private IEnumerator pointerCoroutine;
        GameObject spawnedPointer;  //the "cursor" prefab cast on the surface of the wrist menu

        private FVRPointableButton curPointable;

        void Awake()
        {
            On.FistVR.FVRWristMenu2.ActivateOnHand += FVRWristMenu2_ActivateOnHand;
            On.FistVR.FVRWristMenu2.Deactivate += FVRWristMenu2_Deactivate;

            verticalPointerOffset = AccessibilityOptionsBase.verticalPointerOffset.Value;
            pointerColor = AccessibilityOptionsBase.pointerColor.Value;
            pointerScale = AccessibilityOptionsBase.pointerScale.Value;

            pointerPrefab = AccessibilityOptionsBase.pointerAssetBundle.LoadAsset<GameObject>("WristMenuPointer");
            pointerMat = pointerPrefab.GetComponent<MeshRenderer>().material;

            pointerCoroutine = PointWithHead();
        }

        #region activate/deactivate
        private void FVRWristMenu2_ActivateOnHand(On.FistVR.FVRWristMenu2.orig_ActivateOnHand orig, FVRWristMenu2 self, FVRViveHand hand)
        {
            if (hand.transform.localPosition.magnitude < 0.01f) return; //fix to prevent turned off controller from hijacking wrist menu
            orig(self, hand);
            curHand = hand;

            if (spawnedPointer == null)
            {
                spawnedPointer = Instantiate(pointerPrefab);
                spawnedPointer.transform.localScale = new Vector3(pointerScale, pointerScale, pointerScale);
                pointerMat.color = pointerColor;
            }
            StartCoroutine(pointerCoroutine);
        }

        private void FVRWristMenu2_Deactivate(On.FistVR.FVRWristMenu2.orig_Deactivate orig, FVRWristMenu2 self)
        {
            orig(self);
            StopCoroutine(pointerCoroutine);
            if (curPointable != null && curHand != null) curPointable.EndPoint(curHand);
            if (spawnedPointer != null) Destroy(spawnedPointer);
        }
        #endregion

        IEnumerator PointWithHead()
        {
            while (true)
            {
                Quaternion pointerRayAngle = GM.CurrentPlayerBody.headRotationFiltered; //initially kept as quaternion due to glitchy fuckery
                pointerRayAngle = Quaternion.AngleAxis(verticalPointerOffset * -1, GM.CurrentPlayerBody.Head.right);
                Vector3 pointerRayDirection = pointerRayAngle * GM.CurrentPlayerBody.Head.forward;  //converted into a vector and adjusted to look forwards

                //cast ray from player's face
                if (Physics.Raycast(GM.CurrentPlayerBody.headPositionFiltered, pointerRayDirection, out RaycastHit hit, 1f, curHand.PointingLayerMask, QueryTriggerInteraction.Collide))
                {
                    if (hit.collider.GetComponentInParent<FVRWristMenu2>() != null) //check if the pointable is a wrist menu, not any other menu
                    {
                        spawnedPointer.SetActive(true);
                        spawnedPointer.transform.position = hit.point;
                        if (hit.collider.GetComponent<FVRPointableButton>() != null)
                        {
                            FVRPointableButton newPointable = hit.collider.GetComponent<FVRPointableButton>();
                            if (curPointable != null && newPointable != curPointable)
                            {
                                curPointable.EndPoint(curHand);  //failsafe to prevent multiple buttons being highlighted simultaneously
                            }
                            curPointable = newPointable;
                            curPointable.OnPoint(curHand);  //button activation via trigger pull is handled here
                        }
                    }
                }
                else
                {
                    if (curPointable != null) curPointable.EndPoint(curHand);
                    spawnedPointer.SetActive(false);
                }
                yield return null;
            }
        }
    }
}
