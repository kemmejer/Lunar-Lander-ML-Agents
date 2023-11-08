using ImGuiNET;
using System;
using UImGui;
using UnityEngine;
using UnityEngine.Analytics;

public class StaticSample : MonoBehaviour
{
    private ShipParameterSO _shipParameter;
    private PlayerSpawnerSO _playerSpawnerSO;
    private GroundGeneratorSO _groundGeneratorSO;

    private void Start()
    {
        _shipParameter = ShipParameterSO.GetInstance();
        _playerSpawnerSO = PlayerSpawnerSO.GetInstance();
        _groundGeneratorSO = GroundGeneratorSO.GetInstance();
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
        ShipSpawnHeader(uimgui);
        ShipParameterHeader(uimgui);
        GroundGeneratorHeader(uimgui);
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
            if (ImGui.Button("Spawn Player Ship"))
            {
                PlayerSpawnerBehaviour.GetInstance().SpawnShip(true);
            }
        }
    }

    private void ShipSpawnHeader(UImGui.UImGui uimgui)
    {
        if (ImGui.CollapsingHeader("Ship Spawn Parameter"))
        {
            ImGui.DragFloat2("Horizontal Velocity", ref _playerSpawnerSO.horizontalStartingVelocity.parameter, 1.0f, 0.0f, 100.0f);
        }
    }

    private void ShipParameterHeader(UImGui.UImGui uimgui)
    {
        if (ImGui.CollapsingHeader("Ship Parameter"))
        {
            ImGui.Text("Fuel");
            ImGui.DragFloat2("Max Fuel", ref _shipParameter.fuel.maxFuel.parameter, 1.0f, 100.0f, 1000.0f);
            ImGui.DragFloat2("Fuel Consumption", ref _shipParameter.fuel.fuelConsumption.parameter, 0.1f, 0.1f, 10.0f);
            ImGui.Separator();

            ImGui.Text("Controls");
            ImGui.DragFloat2("Rotation Speed", ref _shipParameter.controlParameter.rotationSpeed.parameter, 0.1f, 0.1f, 10.0f);
            ImGui.DragFloat2("Thrust Amount", ref _shipParameter.controlParameter.thrustAmount.parameter, 0.1f, 0.1f, 10.0f);
            ImGui.Separator();

            ImGui.Text("Landing");
            ImGui.DragFloat2("Max Velocity", ref _shipParameter.landing.maxVelocity.parameter, 0.1f, 0.5f, 5.0f);
            ImGui.DragFloat2("Max Angle", ref _shipParameter.landing.maxAngle.parameter, 1.0f, 1.0f, 10.0f);
            ImGui.Separator();

            ImGui.Text("Physics");
            ImGui.DragFloat2("Mass", ref _shipParameter.physics.mass.parameter, 1.0f, 1.0f, 1000.0f);
            ImGui.DragFloat2("Drag", ref _shipParameter.physics.drag.parameter, 0.1f, 0.0f, 10.0f);
            ImGui.DragFloat2("Angular Drag", ref _shipParameter.physics.angularDrag.parameter, 0.1f, 0.0f, 10.0f);
            ImGui.DragFloat2("Gracity Scale", ref _shipParameter.physics.gravityScale.parameter, 0.01f, 0.01f, 0.1f);
        }
    }

    private void GroundGeneratorHeader(UImGui.UImGui uimgui)
    {
        if(ImGui.CollapsingHeader("Ground Generator"))
        {
            if(ImGui.Button("Generate Ground"))
            {
                GroundGeneratorBehaviour.GetInstance().GenerateGround();
            }

            ImGui.DragFloat2("Height", ref _groundGeneratorSO.noiseHeight.parameter, 0.1f, 0.0f, 10.0f);
            ImGui.DragFloat2("Base noiseHeight", ref _groundGeneratorSO.baseHeight.parameter, 0.1f, 0.0f, 10.0f);
            ImGui.DragFloat2("Noise scale", ref _groundGeneratorSO.noiseScale.parameter, 0.01f, 0.0f, 1.0f);
            ImGui.DragInt("Resolution", ref _groundGeneratorSO.resolution, 1, 2, 100);
            ImGui.DragInt("Seed", ref _groundGeneratorSO.seed, 1, 0, int.MaxValue);

        }
    }
}