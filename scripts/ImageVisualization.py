from mlagents_envs.environment import UnityEnvironment
import numpy as np
from ImageVisualizationChannel import ImageVisualizationChannel


class ImageVisualization:
    channel: ImageVisualizationChannel

    def __init__(self) -> None:
        self.channel = ImageVisualizationChannel()
