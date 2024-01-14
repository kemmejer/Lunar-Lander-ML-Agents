::Arguments: 1=Selected Config (string), 2=Is Unity Editor (true/false)

@cd /D "%~dp0"

@IF "%~1"=="" (set ModelName="Default") ELSE (set ModelName=%1)
@IF "%~2"=="" (set IsUnityEditor="true") ELSE (set IsUnityEditor=%2)

@rd /s /q ".\\results\\%ModelName%

@cd scripts
@.venv\Scripts\activate && ^
IF "%IsUnityEditor%"=="true" (
	python main.py ../config/ShipAgent.yaml --force --run-id=%ModelName% --results-dir="../results" --max-lifetime-restarts=0
) ELSE (
	python main.py ../config/ShipAgent.yaml --force --run-id=%ModelName% --results-dir="../results" --max-lifetime-restarts=0 --env="../Build/Lunar Lander ML-Agents.exe" --env-args="--selected-ml-config=%ModelName%"
) & ^
deactivate