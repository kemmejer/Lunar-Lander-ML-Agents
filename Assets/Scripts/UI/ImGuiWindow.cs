using ImGuiNET;
using UImGui;
using UnityEngine;

public class StaticSample : MonoBehaviour
{
    private ShipParameterSO _shipParameter;

    private void Start()
    {
        _shipParameter = ShipParameterSO.GetInstance();
    }

    private void Awake()
    {
        UImGuiUtility.Layout += OnLayout;
        UImGuiUtility.OnInitialize += OnInitialize;
        UImGuiUtility.OnDeinitialize += OnDeinitialize;
    }

    // Unity Update method. 
    // Your code belongs here! Like ImGui.Begin... etc.
    private void OnLayout(UImGui.UImGui uimgui)
    {
        PlayerShipHeader(uimgui);
        ShipParameterHeader(uimgui);
    }

    // runs after UImGui.OnEnable();
    private void OnInitialize(UImGui.UImGui uimgui)
    {
    }

    // runs after UImGui.OnDisable();
    private void OnDeinitialize(UImGui.UImGui uimgui)
    {
    }

    private void OnDisable()
    {
        UImGuiUtility.Layout -= OnLayout;
        UImGuiUtility.OnInitialize -= OnInitialize;
        UImGuiUtility.OnDeinitialize -= OnDeinitialize;
    }

    private void PlayerShipHeader(UImGui.UImGui uimgui)
    {
        if (ImGui.CollapsingHeader("Player Ship"))
        {
            if(ImGui.Button("Spawn Player Ship"))
            {
                PlayerSpawnerBehaviour.GetInstance().SpawnShip(true);
            }
        }
    }

    private void ShipParameterHeader(UImGui.UImGui uimgui)
    {
        if (ImGui.CollapsingHeader("Ship Parameter"))
        {
            ImGui.Text("Fuel");
            ImGui.DragFloat("Max Fuel", ref _shipParameter.fuel.maxFuel, 1.0f, 100.0f, 1000.0f);
            ImGui.DragFloat("Fuel Consumption", ref _shipParameter.fuel.fuelConsumption, 0.1f, 0.1f, 10.0f);
            ImGui.Separator();

            ImGui.Text("Controls");
            ImGui.DragFloat("Rotation Speed", ref _shipParameter.controlParameter.rotationSpeed, 0.1f, 0.1f, 10.0f);
            ImGui.DragFloat("Thrust Amount", ref _shipParameter.controlParameter.thrustAmount, 0.1f, 0.1f, 10.0f);
            ImGui.Separator();

            ImGui.Text("Landing");
            ImGui.DragFloat("Max Velocity", ref _shipParameter.landing.maxVelocity, 0.1f, 0.5f, 5.0f);
            ImGui.DragFloat("Max Angle", ref _shipParameter.landing.maxAngle, 1.0f, 1.0f, 10.0f);
            ImGui.Separator();

            ImGui.Text("Physics");
            ImGui.DragFloat("Mass", ref _shipParameter.physics.mass, 1.0f, 1.0f, 1000.0f);
            ImGui.DragFloat("Drag", ref _shipParameter.physics.drag, 0.1f, 0.0f, 10.0f);
            ImGui.DragFloat("Angular Drag", ref _shipParameter.physics.angularDrag, 0.1f, 0.0f, 10.0f);
            ImGui.DragFloat("Gracity Scale", ref _shipParameter.physics.gravityScale, 0.01f, 0.01f, 0.1f);
        }
    }
}