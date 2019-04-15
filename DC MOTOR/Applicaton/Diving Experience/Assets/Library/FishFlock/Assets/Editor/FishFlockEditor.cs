using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FishFlock
{
    [CustomEditor(typeof(FishFlockController))]
    public class FishFlockEditor : Editor
    {
        FishFlockController controller;

        SerializedProperty frameSkipAmountProperty;
        SerializedProperty swimmingAreaWidthProperty;
        SerializedProperty swimmingAreaHeightProperty;
        SerializedProperty swimmingAreaDepthProperty;
        SerializedProperty groupAreaWidthProperty;
        SerializedProperty groupAreaHeightProperty;
        SerializedProperty groupAreaDepthProperty;
        SerializedProperty debugDrawProperty;
        SerializedProperty followTargetProperty;
        SerializedProperty targetProperty;
        SerializedProperty minTargetPointsProperty;
        SerializedProperty maxTargetPointsProperty;
        SerializedProperty recalculatePointsProperty;

        SerializedProperty separationScaleProperty;
        SerializedProperty alignmentScaleProperty;
        SerializedProperty cohesionScaleProperty;
        SerializedProperty forceGroupAreaDestinationProperty;

        SerializedProperty prefabProperty;
        SerializedProperty fishAmountProperty;

        SerializedProperty minAccelerationProperty;
        SerializedProperty maxAccelerationProperty;
        SerializedProperty minSpeedProperty;
        SerializedProperty maxSpeedProperty;
        SerializedProperty minForceProperty;
        SerializedProperty maxForceProperty;
        SerializedProperty minScaleProperty;
        SerializedProperty maxScaleProperty;
        SerializedProperty minTurnSpeedProperty;
        SerializedProperty maxTurnSpeedProperty;
        SerializedProperty lookAheadDistanceProperty;
        SerializedProperty lookSideDistanceProperty;
        SerializedProperty collisionAvoidAmountProperty;
        SerializedProperty collisionAvoidLayerProperty;
        SerializedProperty neighbourDistanceProperty;
        SerializedProperty fishSizeProperty;

        SerializedProperty attackTargetProperty;
        SerializedProperty distanceToAttackProperty;
        SerializedProperty secondsAttackingProperty;
        SerializedProperty keepAttackingProperty;
        SerializedProperty targetToAttackProperty;

        private void OnEnable()
        {
            controller = (FishFlockController)target;

            frameSkipAmountProperty = serializedObject.FindProperty("frameSkipAmount");
            swimmingAreaWidthProperty = serializedObject.FindProperty("swimmingAreaWidth");
            swimmingAreaHeightProperty = serializedObject.FindProperty("swimmingAreaHeight");
            swimmingAreaDepthProperty = serializedObject.FindProperty("swimmingAreaDepth");
            groupAreaWidthProperty = serializedObject.FindProperty("groupAreaWidth");
            groupAreaHeightProperty = serializedObject.FindProperty("groupAreaHeight");
            groupAreaDepthProperty = serializedObject.FindProperty("groupAreaDepth");
            debugDrawProperty = serializedObject.FindProperty("debugDraw");
            followTargetProperty = serializedObject.FindProperty("followTarget");
            targetProperty = serializedObject.FindProperty("target");
            minTargetPointsProperty = serializedObject.FindProperty("minTargetPoints");
            maxTargetPointsProperty = serializedObject.FindProperty("maxTargetPoints");
            recalculatePointsProperty = serializedObject.FindProperty("recalculatePoints");

            prefabProperty = serializedObject.FindProperty("prefab");
            fishAmountProperty = serializedObject.FindProperty("fishAmount");

            minAccelerationProperty = serializedObject.FindProperty("minAcceleration");
            maxAccelerationProperty = serializedObject.FindProperty("maxAcceleration");
            minSpeedProperty = serializedObject.FindProperty("minSpeed");
            maxSpeedProperty = serializedObject.FindProperty("maxSpeed");
            minForceProperty = serializedObject.FindProperty("minForce");
            maxForceProperty = serializedObject.FindProperty("maxForce");
            minScaleProperty = serializedObject.FindProperty("minScale");
            maxScaleProperty = serializedObject.FindProperty("maxScale");
            minTurnSpeedProperty = serializedObject.FindProperty("minTurnSpeed");
            maxTurnSpeedProperty = serializedObject.FindProperty("maxTurnSpeed");
            lookAheadDistanceProperty = serializedObject.FindProperty("lookAheadDistance");
            lookSideDistanceProperty = serializedObject.FindProperty("lookSideDistance");
            collisionAvoidAmountProperty = serializedObject.FindProperty("collisionAvoidAmount");
            collisionAvoidLayerProperty = serializedObject.FindProperty("collisionAvoidLayer");
            neighbourDistanceProperty = serializedObject.FindProperty("neighbourDistance");
            fishSizeProperty = serializedObject.FindProperty("fishSize");
            separationScaleProperty = serializedObject.FindProperty("separationScale");
            alignmentScaleProperty = serializedObject.FindProperty("alignmentScale");
            cohesionScaleProperty = serializedObject.FindProperty("cohesionScale");
            forceGroupAreaDestinationProperty = serializedObject.FindProperty("forceGroupAreaDestination");

            attackTargetProperty = serializedObject.FindProperty("attackTarget");
            distanceToAttackProperty = serializedObject.FindProperty("distanceToAttack");
            secondsAttackingProperty = serializedObject.FindProperty("secondsAttacking");
            keepAttackingProperty = serializedObject.FindProperty("keepAttacking");
            targetToAttackProperty = serializedObject.FindProperty("targetToAttack");
        }

        public override void OnInspectorGUI()
        {
            GUIStyle boxStyles = new GUIStyle(EditorStyles.label);
            boxStyles.fontSize = 11;
            boxStyles.fontStyle = FontStyle.Bold;

            serializedObject.Update();

            GUI.color = Color.grey;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Settings", boxStyles);
            EditorGUILayout.Space();

            GUI.color = Color.white;
            EditorGUILayout.PropertyField(frameSkipAmountProperty);
            EditorGUILayout.PropertyField(swimmingAreaWidthProperty);
            EditorGUILayout.PropertyField(swimmingAreaHeightProperty);
            EditorGUILayout.PropertyField(swimmingAreaDepthProperty);
            EditorGUILayout.PropertyField(groupAreaWidthProperty);
            EditorGUILayout.PropertyField(groupAreaHeightProperty);
            EditorGUILayout.PropertyField(groupAreaDepthProperty);
            EditorGUILayout.PropertyField(debugDrawProperty);
            GUI.color = Color.grey;

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Flocking", boxStyles);
            EditorGUILayout.Space();

            GUI.color = Color.white;
            EditorGUILayout.PropertyField(prefabProperty);
            EditorGUILayout.PropertyField(fishAmountProperty);
            EditorGUILayout.PropertyField(minAccelerationProperty);
            EditorGUILayout.PropertyField(maxAccelerationProperty);
            EditorGUILayout.PropertyField(minSpeedProperty);
            EditorGUILayout.PropertyField(maxSpeedProperty);
            EditorGUILayout.PropertyField(minForceProperty);
            EditorGUILayout.PropertyField(maxForceProperty);
            EditorGUILayout.PropertyField(minTurnSpeedProperty);
            EditorGUILayout.PropertyField(maxTurnSpeedProperty);
            EditorGUILayout.PropertyField(minScaleProperty);
            EditorGUILayout.PropertyField(maxScaleProperty);
            EditorGUILayout.PropertyField(neighbourDistanceProperty);
            EditorGUILayout.PropertyField(fishSizeProperty);
            EditorGUILayout.PropertyField(separationScaleProperty);
            EditorGUILayout.PropertyField(alignmentScaleProperty);
            EditorGUILayout.PropertyField(cohesionScaleProperty);
            EditorGUILayout.PropertyField(forceGroupAreaDestinationProperty);

            GUI.color = Color.grey;

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Collision Avoidance", boxStyles);
            EditorGUILayout.Space();

            GUI.color = Color.white;
            EditorGUILayout.PropertyField(lookAheadDistanceProperty);
            EditorGUILayout.PropertyField(lookSideDistanceProperty);
            EditorGUILayout.PropertyField(collisionAvoidAmountProperty);
            EditorGUILayout.PropertyField(collisionAvoidLayerProperty);
            GUI.color = Color.grey;

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Target Following", boxStyles);
            EditorGUILayout.Space();

            GUI.color = Color.white;
            EditorGUILayout.PropertyField(followTargetProperty);
            EditorGUILayout.PropertyField(targetProperty);
            GUI.color = Color.grey;

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Random Target Points Following", boxStyles);
            EditorGUILayout.Space();

            GUI.color = Color.white;
            EditorGUILayout.PropertyField(minTargetPointsProperty);
            EditorGUILayout.PropertyField(maxTargetPointsProperty);
            EditorGUILayout.PropertyField(recalculatePointsProperty);

            if (controller.followTarget)
            {
                EditorGUILayout.HelpBox("Follow target is enabled, target following overwrites random target following.", MessageType.Info);
            }

            GUI.color = Color.grey;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Attack", boxStyles);
            EditorGUILayout.Space();

            GUI.color = Color.white;

            EditorGUILayout.PropertyField(attackTargetProperty);
            EditorGUILayout.PropertyField(distanceToAttackProperty);
            EditorGUILayout.PropertyField(secondsAttackingProperty);
            EditorGUILayout.PropertyField(keepAttackingProperty);
            EditorGUILayout.PropertyField(targetToAttackProperty);

            GUI.color = Color.grey;
            EditorGUILayout.EndVertical();

            GUI.color = Color.white;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
