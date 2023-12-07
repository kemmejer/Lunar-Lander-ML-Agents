start "" "http://localhost:6006/"
cd scripts
.venv\Scripts\activate && ^
tensorboard --logdir ../results --port 6006 --reload_interval=30 --reload_multifile=true & ^
deactivate