using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneScaler : MonoBehaviour
{
    [SerializeField] private GameObject _backGround;
    [SerializeField] private GameObject _playerSpawner;
    [SerializeField] private GameObject _groundGenerator;

    private float _screenWidth;
    private float _screenHeight;

    private void Start()
    {
        ScaleScene();
    }

    private void FixedUpdate()
    {
        if (_screenWidth == Screen.width && _screenHeight == Screen.height)
            return;

        ScaleScene();
    }

    public void ScaleScene()
    {
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;

        var screenBounds = CameraHelper.GetScreenBounds();

        _backGround.transform.localScale = new Vector3(screenBounds.extents.x * 2, screenBounds.extents.y * 2, 1.0f);
        _groundGenerator.GetComponent<GroundGeneratorBehaviour>().GenerateGround(false);

        _playerSpawner.transform.position = new Vector3(_playerSpawner.transform.position.x, screenBounds.max.y, _playerSpawner.transform.position.z);
        _playerSpawner.transform.localScale = new Vector3(screenBounds.extents.x * 2, 1.0f, 1.0f);
    }
}
