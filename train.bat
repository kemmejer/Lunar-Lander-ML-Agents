@IF "%~1"=="" (set ModelName="Default") ELSE (set ModelName=%1)

@rd /s /q ".\\results\\%ModelName%

@cd scripts
@.venv\Scripts\activate && ^
python main.py ../config/ShipAgent.yaml --force --run-id=%ModelName% --results-dir="../results" --max-lifetime-restarts=0 & ^
deactivate