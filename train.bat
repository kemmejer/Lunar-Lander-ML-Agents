@IF "%~1"=="" (set ModelName="Default") ELSE (set ModelName=%1)

del /s /q ".\\results\\%ModelName%

.venv\Scripts\activate && ^
mlagents-learn ./config/ShipAgent.yaml --force --run-id=%ModelName% & ^
deactivate