python -m pip install --upgrade pip
pip install virtualenv

cd scripts
python -m venv .venv
.venv\Scripts\activate && ^
python -m pip install --upgrade pip && ^
pip install torch~=1.13.1 -f https://download.pytorch.org/whl/torch_stable.html && ^
pip install matplotlib~=3.8.2 && ^
pip install pandas~=2.1.4 && ^
pip install npy-append-array~=0.9.16 && ^
pip install -e "../Dependencies/ml-agents-envs" && ^
pip install -e "../Dependencies/ml-agents" && ^
pip install -e "tensorboard_image_plugin" && ^
deactivate
