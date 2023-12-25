using ImGuiNET;
using System;
using System.Linq;
using UImGui;
using UnityEngine;

public class ImGuiWindow : MonoBehaviour
{
    private ShipParameterSO _shipParameter;
    private PlayerSpawnerSO _playerSpawnerSO;
    private GroundGeneratorSO _groundGeneratorSO;
    private RayCasterSO _rayCasterSO;
    private TrainingSO _trainingSO;
    private TrainingManagerBehaviour _trainingManager;

    private int _configIndex = 0;
    private string _modalConfigName = string.Empty;
    private string[] _configs;

    private string CurrentConfigName => _configs[_configIndex];

    private const ImGuiWindowFlags ModalFlags = ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
    private const string CreateConfigModalName = "Create Config";
    private const string DeleteConfigModalName = "Delete Config";
    private bool _createConfigModalOpenable = true;
    private bool _deleteConfigModalOpenable = true;
    private bool _configSaveAndDeleteActive;
    private bool _loadedModel;

    private void Awake()
    {
        UImGuiUtility.Layout += OnLayout;
        UImGuiUtility.OnInitialize += OnInitialize;
        UImGuiUtility.OnDeinitialize += OnDeinitialize;

        _shipParameter = ShipParameterSO.GetInstance();
        _playerSpawnerSO = PlayerSpawnerSO.GetInstance();
        _groundGeneratorSO = GroundGeneratorSO.GetInstance();
        _rayCasterSO = RayCasterSO.GetInstance();
        _trainingSO = TrainingSO.GetInstance();
        _trainingManager = TrainingManagerBehaviour.GetInstance();
        UpdateConfigNames();
        LoadConfig();
    }

    // Unity Update method. 
    // Your code belongs here! Like ImGui.Begin... etc.
    private void OnLayout(UImGui.UImGui uimgui)
    {
        if (ImGui.Begin("Settings"))
        {
            ControlHeader();
            ShipParameterHeader();
            GroundGeneratorHeader();
            MachineLearningHeader();
            Logger.Draw();

            ImGui.End();
        }

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

    private void ControlHeader()
    {
        if (ImGui.CollapsingHeader("Controls", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.Text("Ship");
            if (ImGui.Button("Spawn Player Ship"))
                PlayerSpawnerBehaviour.GetInstance().InstantiateShip(PlayerSpawnerBehaviour.ShipType.Player);

            if (_loadedModel)
            {
                ImGui.SameLine();
                if (ImGui.Button("Spawn Agent Ship"))
                    PlayerSpawnerBehaviour.GetInstance().InstantiateShip(PlayerSpawnerBehaviour.ShipType.TrainedAgent);
            }

            ImGui.Separator();
            ImGui.Text("Training");
            if (_trainingManager.IsTraining)
            {
                if (ImGui.Button("Stop Training"))
                    _trainingManager.StopTraining();
            }
            else
            {
                if (ImGui.Button("Start Training"))
                    _trainingManager.StartTraining();
            }

            if(_trainingManager.IsStarting)
            {
                ImGui.SameLine();
                ImGui.Text("Starting Training...");
            }

            if (_trainingManager.IsStopping)
            {
                ImGui.SameLine();
                ImGui.Text("Stopping Training...");
            }

            ImGui.Separator();
            ImGui.Text("Scene");
            if (ImGui.Button("Destroy Ships"))
                PlayerSpawnerBehaviour.GetInstance().DestroyShips();

            ImGui.SameLine();
            if (ImGui.Button("Delete Trails"))
                TrailManager.GetInstance().DestoryTrails();

            ImGui.Separator();
            ImGui.Text("Config");
            if (ImGui.Combo(string.Empty, ref _configIndex, _configs, _configs.Length))
                LoadConfig();

            ImGui.SameLine();
            if (ImGui.Button("+"))
                ImGui.OpenPopup(CreateConfigModalName);

            CreateConfigModal();

            if (_configSaveAndDeleteActive)
            {
                ImGui.SameLine();
                if (ImGui.Button("-"))
                    ImGui.OpenPopup(DeleteConfigModalName);
            }

            DeleteConfigModal();

            if (_configSaveAndDeleteActive)
            {
                if (ImGui.Button("Save"))
                    SaveConfig();

                ImGui.SameLine();
            }

            ImGui.Text(string.Format("Loaded Model: {0}", ConfigManager.CurrentModel?.Name ?? "None"));
        }
    }

    private void ShipParameterHeader()
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
            ImGui.DragFloat2("Gravity Scale", ref _shipParameter.physics.gravityScale.parameter, 0.01f, 0.01f, 0.1f);
            ImGui.Separator();

            ImGui.Text("Spawning");
            ImGui.DragFloat2("Horizontal Velocity", ref _playerSpawnerSO.horizontalStartingVelocity.parameter, 1.0f, 0.0f, 100.0f);
        }
    }

    private void GroundGeneratorHeader()
    {
        if (ImGui.CollapsingHeader("Ground Generator"))
        {
            if (ImGui.Button("Generate Ground"))
                GroundGeneratorBehaviour.GetInstance().GenerateGround();

            ImGui.Separator();
            ImGui.DragFloat2("Height", ref _groundGeneratorSO.noiseHeight.parameter, 0.1f, 0.0f, 10.0f);
            ImGui.DragFloat2("Base noiseHeight", ref _groundGeneratorSO.baseHeight.parameter, 0.1f, 0.0f, 10.0f);
            ImGui.DragFloat2("Noise scale", ref _groundGeneratorSO.noiseScale.parameter, 0.01f, 0.0f, 1.0f);
            ImGui.DragInt2("Resolution", ref _groundGeneratorSO.resolution.parameter[0], 1, 2, 100);
            ImGui.DragInt2("Seed", ref _groundGeneratorSO.seed.parameter[0], 1, 0, int.MaxValue);
        }
    }

    private void MachineLearningHeader()
    {
        if (ImGui.CollapsingHeader("Machine Learning"))
        {
            ImGui.Text("Ray Cast");
            ImGui.Checkbox("Draw Rays", ref RayCasterSO.drawRays);
            ImGui.DragInt("Rays per direction", ref _rayCasterSO.raysPerDirection, 1, 0, 5);
            ImGui.DragFloat("Angle", ref _rayCasterSO.angle, 1.0f, 0.0f, 120.0f);
            ImGui.DragFloat("Horizontal Distribution", ref _rayCasterSO.horizontalRayDistribution, 0.1f, 0.0f, 1.0f);
            ImGui.Separator();

            ImGui.Text("TrainingAgent");
            ImGui.DragInt("Ship Count", ref _trainingSO.shipCount, 1, 1, 100);
            ImGui.DragInt("Decision Interval", ref _trainingSO.decisionInterval, 1, 1, 10);
        }
    }

    private void UpdateConfigNames()
    {
        _configs = ConfigManager.Configs.Select(config => config.Name).ToArray();
        _configIndex = Array.IndexOf(_configs, ConfigManager.CurrentConfig.Name);
        _configSaveAndDeleteActive = !(CurrentConfigName == Constants.DefaultConfigName);
        _loadedModel = ConfigManager.CurrentModel != null;
    }


    private void LoadConfig()
    {
        ConfigManager.LoadConfig(CurrentConfigName);
        _loadedModel = ConfigManager.CurrentModel != null;
        _configSaveAndDeleteActive = !(CurrentConfigName == Constants.DefaultConfigName);
    }

    private void SaveConfig()
    {
        ConfigManager.SaveConfig(CurrentConfigName);
        _loadedModel = ConfigManager.CurrentModel != null;
    }

    private void CreateConfigModal()
    {
        _createConfigModalOpenable = true;

        if (ImGui.BeginPopupModal(CreateConfigModalName, ref _createConfigModalOpenable, ModalFlags))
        {
            ImGui.InputText("Name", ref _modalConfigName, 16);

            if (ImGui.Button("Create"))
            {
                ConfigManager.SaveConfig(_modalConfigName);
                ConfigManager.LoadConfig(_modalConfigName);
                UpdateConfigNames();
                ImGui.CloseCurrentPopup();
            }

            ImGui.SameLine();
            if (ImGui.Button("Cancel"))
                ImGui.CloseCurrentPopup();

            ImGui.EndPopup();
        }

    }

    private void DeleteConfigModal()
    {
        _deleteConfigModalOpenable = true;

        if (ImGui.BeginPopupModal(DeleteConfigModalName, ref _deleteConfigModalOpenable, ModalFlags))
        {
            ImGui.Text(string.Format("Delete Config: \"{0}\"?", CurrentConfigName));

            if (ImGui.Button("Delete"))
            {
                ConfigManager.DeleteConfig(CurrentConfigName);
                ConfigManager.LoadConfig(ConfigManager.Configs.FirstOrDefault()?.Name);
                UpdateConfigNames();
                ImGui.CloseCurrentPopup();
            }

            ImGui.SameLine();
            if (ImGui.Button("Cancel"))
                ImGui.CloseCurrentPopup();

            ImGui.EndPopup();
        }
    }
}