@IF "%~1"=="" (set ModelName="Default") ELSE (set ModelName=%1)

del /s /q ".\\results\\%ModelName%

cd scripts
.venv\Scripts\activate && ^
:: mlagents-learn ../config/ShipAgent.yaml --force --run-id=%ModelName% --results-dir="../results" & ^
python main.py ../config/ShipAgent.yaml --force --run-id=%ModelName% --results-dir="../results" & ^
deactivate