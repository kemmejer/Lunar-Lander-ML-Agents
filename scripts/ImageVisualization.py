from ctypes import Array
from mlagents_envs.environment import UnityEnvironment
import numpy as np
from typing import List
from ImageVisualizationChannel import ImageVisualizationChannel
from ImageVisualizationChannel import ImageGraphName

class ImageVisualization:

    def __init__(self) -> None:
        self.channel = ImageVisualizationChannel(self.add_data)
        self.data = [np.array(np.float) for _ in range(len(ImageGraphName))]

    def add_data(self, graph_name: ImageGraphName, data: List[float]) -> None:
        self.data[graph_name] = np.append(self.data[graph_name], data)
