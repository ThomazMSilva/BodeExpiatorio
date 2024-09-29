using UnityEditor;

[CustomEditor(typeof(MovimentoJogador))] //pedi pro chato gepeto fazer isso pq eh puramente escrever um por um
public class MovimentoJogadorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Start modifying the inspector
        serializedObject.Update();

        // Serialized properties
        SerializedProperty moveSpeed = serializedObject.FindProperty("moveSpeed");
        SerializedProperty clampVelocity = serializedObject.FindProperty("clampVelocity");
        SerializedProperty terminalVelocity = serializedObject.FindProperty("terminalVelocity");
        SerializedProperty instantAirVelocityChange = serializedObject.FindProperty("instantAirVelocityChange");
        SerializedProperty airAccelerationMultiplier = serializedObject.FindProperty("airAccelerationMultiplier");
        SerializedProperty instantGroundVelocityChange = serializedObject.FindProperty("instantGroundVelocityChange");
        SerializedProperty groundAccelerationMultiplier = serializedObject.FindProperty("groundAccelerationMultiplier");
        SerializedProperty allowRagdollMomentum = serializedObject.FindProperty("allowRagdollMomentum");
        SerializedProperty maxStunTime = serializedObject.FindProperty("maxStunTime");
        SerializedProperty fallGravityMultiplier = serializedObject.FindProperty("fallGravityMultiplier");
        SerializedProperty jumpGravityMultiplier = serializedObject.FindProperty("jumpGravityMultiplier");
        SerializedProperty ragdollGravityMultiplier = serializedObject.FindProperty("ragdollGravityMultiplier");
        SerializedProperty ragdollJumpGravityMultiplier = serializedObject.FindProperty("ragdollJumpGravityMultiplier");
        SerializedProperty ragdollKneelGravityMultiplier = serializedObject.FindProperty("ragdollKneelGravityMultiplier");
        SerializedProperty isRagdollJumpUnlocked = serializedObject.FindProperty("isRagdollJumpUnlocked");
        SerializedProperty raycastDistance = serializedObject.FindProperty("raycastDistance");
        SerializedProperty groundLayer = serializedObject.FindProperty("groundLayer");
        SerializedProperty jumpForce = serializedObject.FindProperty("jumpForce");
        SerializedProperty coyoteTimeMax = serializedObject.FindProperty("coyoteTimeMax");
        SerializedProperty jumpBufferTimeMax = serializedObject.FindProperty("jumpBufferTimeMax");
        SerializedProperty shortJumpDelta = serializedObject.FindProperty("shortJumpDelta");
        SerializedProperty kneelHeightMultiplier = serializedObject.FindProperty("kneelHeightMultiplier");
        SerializedProperty kneelSpeedMultiplier = serializedObject.FindProperty("kneelSpeedMultiplier");

        // Draw all properties manually, applying custom logic for conditional fields

        EditorGUILayout.PropertyField(moveSpeed);


        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(clampVelocity);
        if (clampVelocity.boolValue)
        {
            EditorGUILayout.PropertyField(terminalVelocity);
            EditorGUILayout.Space();
        }


        EditorGUILayout.PropertyField(instantAirVelocityChange);
        if (!instantAirVelocityChange.boolValue)
        {
            EditorGUILayout.PropertyField(airAccelerationMultiplier);
            EditorGUILayout.Space();
        }


        EditorGUILayout.PropertyField(instantGroundVelocityChange);
        if (!instantGroundVelocityChange.boolValue)
        {
            EditorGUILayout.PropertyField(groundAccelerationMultiplier);
            EditorGUILayout.Space();
        }


        EditorGUILayout.PropertyField(allowRagdollMomentum);
        if (allowRagdollMomentum.boolValue)
        {
            EditorGUILayout.PropertyField(maxStunTime);
            EditorGUILayout.Space();
        }

        //EditorGUILayout.Space();
        EditorGUILayout.PropertyField(fallGravityMultiplier);
        EditorGUILayout.PropertyField(jumpGravityMultiplier);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(ragdollGravityMultiplier);
        EditorGUILayout.PropertyField(ragdollJumpGravityMultiplier);
        EditorGUILayout.PropertyField(ragdollKneelGravityMultiplier);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(isRagdollJumpUnlocked);
        EditorGUILayout.PropertyField(raycastDistance);
        EditorGUILayout.PropertyField(groundLayer);

        EditorGUILayout.Space();
        
        EditorGUILayout.PropertyField(jumpForce);
        EditorGUILayout.PropertyField(coyoteTimeMax);
        EditorGUILayout.PropertyField(jumpBufferTimeMax);
        EditorGUILayout.PropertyField(shortJumpDelta);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(kneelHeightMultiplier);
        EditorGUILayout.PropertyField(kneelSpeedMultiplier);

        // Apply modified properties
        serializedObject.ApplyModifiedProperties();
    }
}
