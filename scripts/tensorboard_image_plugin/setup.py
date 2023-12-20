from setuptools import setup
from mlagents.plugins import ML_AGENTS_STATS_WRITER

setup(
    name="tensorboard_image_writer_plugin",
    version="0.0.1",
    # Example of how to add your own registration functions that will be called
    # by mlagents-learn.
    #
    # Here, the get_example_stats_writer() function in mlagents_plugin_examples/example_stats_writer.py
    # will get registered with the ML_AGENTS_STATS_WRITER plugin interface.
    entry_points={
        ML_AGENTS_STATS_WRITER: [
            "tensorboard_image_writer=tensorboard_image_plugin.TensorBoardImageWriter:get_tensor_board_image_writer"
        ]
    },
)