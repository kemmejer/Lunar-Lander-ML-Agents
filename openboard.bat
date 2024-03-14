@cd /D "%~dp0"

start "" /b cmd /c "timeout /nobreak 4 >nul & start "" http://localhost:6006/"

cd scripts
.venv\Scripts\activate && ^
tensorboard --logdir ../results --port 6006 --reload_interval=30 --reload_multifile=true --samples_per_plugin images=100 & ^
deactivate