del /s /q ".\\results\\"
.venv\Scripts\activate && ^
mlagents-learn ./config/ShipControl.yaml --force & ^
deactivate