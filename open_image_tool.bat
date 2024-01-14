@cd /D "%~dp0"

cd scripts
.venv\Scripts\activate && ^
python ImageVisualizationUI.py & ^
deactivate