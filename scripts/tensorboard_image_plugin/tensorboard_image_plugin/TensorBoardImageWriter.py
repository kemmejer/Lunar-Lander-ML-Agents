
import io
from typing import Dict, List

import matplotlib.pyplot as plt
import numpy as np
from mlagents.trainers.settings import RunOptions
from mlagents.trainers.stats import (StatsSummary, StatsWriter,
                                     TensorboardWriter)
from PIL import Image

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
            self.image_visualization.set_image_data(graph_name, value.full_dist)
            image: plt.Figure = self.image_visualization.generate_image(graph_name)
            if image is not None:
                self.save_image(image, graph_name, category, step)
                plt.clf()

    def save_image(self, image: plt.Figure, graph_name: ImageGraphName, category: str, step: int) -> None:
        img_byte_io = io.BytesIO()
        image.savefig(img_byte_io, format="png", transparent=False)
        img: Image.Image = Image.open(img_byte_io)

        if img.mode != "RGB":
            img = img.convert("RGB")

        img_np = np.array(img)
        self.summary_writers[category].add_image("Images/" + graph_name.name, img_np, step, dataformats="HWC")
        self.summary_writers[category].flush()

    def save_data(self) -> None:
        pass


def get_tensor_board_image_writer(run_options: RunOptions) -> List[StatsWriter]:
    """
    Registration function. This is referenced in setup.py and will
    be called by mlagents-learn when it starts to determine the
    list of StatsWriters to use.

    It must return a list of StatsWriters.
    """

    print("Creating a new TensorBoardImageWriter")

    return [TensorBoardImageWriter(run_options)]
