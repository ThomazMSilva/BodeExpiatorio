using UnityEditor;

[CustomEditor(typeof(MovimentoJogador)), CanEditMultipleObjects] //pedi pro chato gepeto fazer isso pq eh puramente escrever um por um
public class MovimentoJogadorEditor : Editor
{
    // Serialized properties
    SerializedProperty moveSpeed;
    SerializedProperty climbSpeed;
    SerializedProperty clampVelocity;
    SerializedProperty terminalVelocity;
    SerializedProperty instantAirVelocityChange;
    SerializedProperty airAccelerationMultiplier;
    SerializedProperty instantGroundVelocityChange;
    SerializedProperty groundAccelerationMultiplier;
    SerializedProperty allowRagdollMomentum;
    SerializedProperty maxStunTime;
    SerializedProperty fallGravityMultiplier;
    SerializedProperty jumpGravityMultiplier;
    SerializedProperty ragdollGravityMultiplier;
    SerializedProperty ragdollJumpGravityMultiplier;
    SerializedProperty ragdollKneelGravityMultiplier;
    SerializedProperty isRagdollJumpUnlocked;
    SerializedProperty raycastDistance;
    SerializedProperty groundLayer;
    SerializedProperty jumpForce;
    SerializedProperty coyoteTimeMax;
    SerializedProperty jumpBufferTimeMax;
    SerializedProperty shortJumpDelta;
    SerializedProperty kneelHeightMultiplier;
    SerializedProperty kneelSpeedMultiplier;
    SerializedProperty timeToPlayIdle;
    SerializedProperty isGrounded;
    SerializedProperty isBind;
    SerializedProperty isRagdolling;
    SerializedProperty isLookingRight;
    SerializedProperty input;
    SerializedProperty playerCollider;
    SerializedProperty rb;
    SerializedProperty playerAnim;

    private void OnEnable()
    {
        // Serialized properties
        moveSpeed = serializedObject.FindProperty("moveSpeed");
        climbSpeed = serializedObject.FindProperty("climbSpeed");
        clampVelocity = serializedObject.FindProperty("clampVelocity");
        terminalVelocity = serializedObject.FindProperty("terminalVelocity");
        instantAirVelocityChange = serializedObject.FindProperty("instantAirVelocityChange");
        airAccelerationMultiplier = serializedObject.FindProperty("airAccelerationMultiplier");
        instantGroundVelocityChange = serializedObject.FindProperty("instantGroundVelocityChange");
        groundAccelerationMultiplier = serializedObject.FindProperty("groundAccelerationMultiplier");
        allowRagdollMomentum = serializedObject.FindProperty("allowRagdollMomentum");
        maxStunTime = serializedObject.FindProperty("maxStunTime");
        fallGravityMultiplier = serializedObject.FindProperty("fallGravityMultiplier");
        jumpGravityMultiplier = serializedObject.FindProperty("jumpGravityMultiplier");
        ragdollGravityMultiplier = serializedObject.FindProperty("ragdollGravityMultiplier");
        ragdollJumpGravityMultiplier = serializedObject.FindProperty("ragdollJumpGravityMultiplier");
        ragdollKneelGravityMultiplier = serializedObject.FindProperty("ragdollKneelGravityMultiplier");
        isRagdollJumpUnlocked = serializedObject.FindProperty("isRagdollJumpUnlocked");
        raycastDistance = serializedObject.FindProperty("raycastDistance");
        groundLayer = serializedObject.FindProperty("groundLayer");
        jumpForce = serializedObject.FindProperty("jumpForce");
        coyoteTimeMax = serializedObject.FindProperty("coyoteTimeMax");
        jumpBufferTimeMax = serializedObject.FindProperty("jumpBufferTimeMax");
        shortJumpDelta = serializedObject.FindProperty("shortJumpDelta");
        kneelHeightMultiplier = serializedObject.FindProperty("kneelHeightMultiplier");
        kneelSpeedMultiplier = serializedObject.FindProperty("kneelSpeedMultiplier");
        timeToPlayIdle = serializedObject.FindProperty("timeToPlayIdle");
        isGrounded = serializedObject.FindProperty("isGrounded");
        isBind = serializedObject.FindProperty("isBind");
        isRagdolling = serializedObject.FindProperty("inRagdoll");
        isLookingRight = serializedObject.FindProperty("isLookingRight");
        input = serializedObject.FindProperty("input");
        playerCollider = serializedObject.FindProperty("playerCollider");
        rb = serializedObject.FindProperty("rb");
        playerAnim = serializedObject.FindProperty("playerAnim");
    }
    public override void OnInspectorGUI()
    {
        // Start modifying the inspector
        serializedObject.Update();

        // Draw all properties manually, applying custom logic for conditional fields

        EditorGUILayout.PropertyField(moveSpeed);
        EditorGUILayout.PropertyField(climbSpeed);

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

        EditorGUILayout.PropertyField(timeToPlayIdle);

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

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(isGrounded);
        EditorGUILayout.PropertyField(isBind);
        EditorGUILayout.PropertyField(isRagdolling);
        EditorGUILayout.PropertyField(isLookingRight);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(input);
        EditorGUILayout.PropertyField(playerCollider);
        EditorGUILayout.PropertyField(rb);
        EditorGUILayout.PropertyField(playerAnim);

        // Apply modified properties
        serializedObject.ApplyModifiedProperties();
    }
}
