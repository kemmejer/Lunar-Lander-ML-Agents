
import io
import itertools
import os
from datetime import datetime
from typing import Dict, List
from mlagents.trainers.settings import RunOptions
from mlagents.trainers.stats import StatsWriter, StatsSummary

import matplotlib.pyplot as plt
import numpy as np
from packaging import version
from torch.utils.tensorboard import SummaryWriter
from mlagents.trainers.stats import TensorboardWriter


import Constants
from ImageVisualization import ImageGraphName, ImageVisualization


class TensorBoardImageWriter(TensorboardWriter):

    def __init__(self, run_options: RunOptions) -> None:
        checkpoint_settings = run_options.checkpoint_settings
        super().__init__(checkpoint_settings.write_path, clear_past_data=not checkpoint_settings.resume)
        self.image_graph_names: list[str] = [graph.name for graph in ImageGraphName]
        self.image_visualization = ImageVisualization()

    def write_stats(self, category: str, values: Dict[str, StatsSummary], step: int) -> None:
        self._maybe_create_summary_writer(category)
        for key, value in values.items():
            if key not in self.image_graph_names:
                continue

            graph_name: ImageGraphName = ImageGraphName[key]
            self.image_visualization.add_data(graph_name, value.full_dist)


def get_tensor_board_image_writer(run_options: RunOptions) -> List[StatsWriter]:
    """
    Registration function. This is referenced in setup.py and will
    be called by mlagents-learn when it starts to determine the
    list of StatsWriters to use.

    It must return a list of StatsWriters.
    """
    print("Creating a new stats writer! This is so exciting!")

    return [TensorBoardImageWriter(run_options)]
