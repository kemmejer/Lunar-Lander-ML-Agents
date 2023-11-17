start "" "http://localhost:6006/"
.venv\Scripts\activate && ^
tensorboard --logdir results --port 6006 & ^
deactivate