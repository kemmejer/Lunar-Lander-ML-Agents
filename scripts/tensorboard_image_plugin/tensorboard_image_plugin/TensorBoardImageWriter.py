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
        self.image_visualization = ImageVisualization(self.base_dir)
        self.image_folder_name = "Images/"
        self.image_generation_interval = 4
        self.image_generation_interval_counter = 0
        self.data = [None] * len(ImageGraphName)

    def write_stats(self, category: str, values: Dict[str, StatsSummary], step: int) -> None:
        self._maybe_create_summary_writer(category)
        self.image_visualization.save_data("Step", [step])
        
        for key, value in values.items():
            if key not in self.image_graph_names:
                continue

            # Save the data to a file
            graph_name: ImageGraphName = ImageGraphName[key]
            self.image_visualization.save_data(graph_name.name, value.full_dist)

            # Collect multiple datasets for image generation
            if (self.data[graph_name.value] is None):
                self.data[graph_name.value] = np.asarray(value.full_dist)
            else:
                self.data[graph_name.value] = np.append(self.data[graph_name.value], value.full_dist)

            if key == ImageGraphName.WorldBounds.name:
                self.image_visualization.set_image_data(graph_name, self.data[graph_name.value])

            # Generate the image
            if self.should_generate_image():
                self.generate_image(graph_name, category, step)

        if self.should_generate_image():
            self.image_generation_interval_counter = 0
        else:
            self.image_generation_interval_counter += 1

    def should_generate_image(self) -> bool:
        return self.image_generation_interval_counter >= self.image_generation_interval - 1

    def generate_image(self, graph_name: ImageGraphName, category: str, step: int) -> None:
        if graph_name == ImageGraphName.WorldBounds:
            return

        self.image_visualization.set_image_data(graph_name, self.data[graph_name.value])
        image: plt.Figure = self.image_visualization.generate_image(graph_name)
        if image is not None:
            self.save_image(image, graph_name, category, step)
            plt.clf()

        self.data[graph_name.value] = None

    def save_image(self, image: plt.Figure, graph_name: ImageGraphName, category: str, step: int) -> None:
        img_byte_io = io.BytesIO()
        image.savefig(img_byte_io, format="png", transparent=False, bbox_inches="tight")
        img: Image.Image = Image.open(img_byte_io)

        if img.mode != "RGB":
            img = img.convert("RGB")

        img_np = np.array(img)
        self.summary_writers[category].add_image(self.image_folder_name + graph_name.name, img_np, step, dataformats="HWC")
        self.summary_writers[category].flush()


def get_tensor_board_image_writer(run_options: RunOptions) -> List[StatsWriter]:
    """
    Registration function. This is referenced in setup.py and will
    be called by mlagents-learn when it starts to determine the
    list of StatsWriters to use.

    It must return a list of StatsWriters.
    """

    print("Creating TensorBoardImageWriter")

    return [TensorBoardImageWriter(run_options)]
