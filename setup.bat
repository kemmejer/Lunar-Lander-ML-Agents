python -m pip install --upgrade pip
pip install virtualenv

cd scripts
python -m venv .venv
.venv\Scripts\activate && ^
python -m pip install --upgrade pip && ^
pip install torch~=1.13.1 -f https://download.pytorch.org/whl/torch_stable.html && ^
pip install -e "../Dependencies/ml-agents-envs" && ^
pip install -e "../Dependencies/ml-agents" && ^
deactivate
