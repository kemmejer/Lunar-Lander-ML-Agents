from typing import List

import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
import seaborn as sns

from ImageVisualizationChannel import ImageGraphName, ImageVisualizationChannel

POSITION_STEP_SIZE = 0.5


def round_array(array: np.ndarray[float]):
    return np.round(array / POSITION_STEP_SIZE) * POSITION_STEP_SIZE


class WorldBounds:
    def __init__(self, world_bounds: List[float]) -> None:
        world_bounds = round_array(world_bounds)
        self.min_x = world_bounds[0]
        self.min_y = world_bounds[1]
        self.max_x = world_bounds[2]
        self.max_y = world_bounds[3]
        self.width = self.max_x - self.min_x
        self.height = self.max_y - self.min_y
        self.width_count = int(self.width / POSITION_STEP_SIZE) + 1
        self.height_count = int(self.height / POSITION_STEP_SIZE) + 1


class ImageVisualization:

    def __init__(self) -> None:
        self.channel = ImageVisualizationChannel(self.add_data)
        self.data = [np.array(np.float) for _ in range(len(ImageGraphName))]

    def add_data(self, graph_name: ImageGraphName, data: List[float]) -> None:
        # self.data[graph_name] = np.append(self.data[graph_name], data)
        self.data[int(graph_name)] = np.asarray(data)
        if graph_name == ImageGraphName.WorldBounds:
            self.bounds = WorldBounds(self.data[ImageGraphName.WorldBounds])
        else:
            self.generate_image(graph_name)

    def generate_image(self, graph_name: ImageGraphName) -> None:
        match graph_name:
            case ImageGraphName.Position:
                self.generate_position_image()
            case ImageGraphName.Rotation:
                self.generate_rotation_image()
            case ImageGraphName.Velocity:
                self.generate_velocity_image()
            case ImageGraphName.Reward:
                self.generate_reward_image()
            case ImageGraphName.Thrust:
                self.generate_thrust_image()

    def generate_dataframe(self, values: np.ndarray[float]) -> pd.DataFrame:
        positions = self.data[int(ImageGraphName.Position)]
        positions = round_array(positions)

        # Fill the value and coordinate arrays with all possible positions. This ensures that there are no gaps / jumps in the dataframe
        x_fill = np.arange(self.bounds.min_x, self.bounds.max_x + POSITION_STEP_SIZE, POSITION_STEP_SIZE)
        y_fill = np.arange(self.bounds.min_y, self.bounds.max_y + POSITION_STEP_SIZE, POSITION_STEP_SIZE)

        # Pad the coordinate arrays to the same length
        if self.bounds.width > self.bounds.height:
            y_fill = np.pad(y_fill, (0, self.bounds.width_count - self.bounds.height_count), 'constant')
        else:
            x_fill = np.pad(x_fill, (0, self.bounds.height_count - self.bounds.width_count), 'constant')

        # Pad the value array to the same length as the coordinate arrays
        values = np.append(values, np.full(x_fill.size, np.nan))

        # Retrieve the coordinate arrays
        x_coords = np.append(positions[0::2], x_fill)
        y_coords = np.append(positions[1::2], y_fill)

        # Create a dataframe. The grouping calculates the mean over all duplicate positions. The pivot fills all missing gaps in the dataframe with NaN
        df = pd.DataFrame(data={"x": x_coords, "y": y_coords, "z": values})
        df = df.groupby(["x", "y"], as_index=False).mean()
        df = df.pivot(index="y", columns="x", values="z")

        return df

    def generate_position_image(self) -> None:
        pass

    def generate_rotation_image(self) -> None:
        pass

    def generate_velocity_image(self) -> None:
        pass

    def generate_reward_image(self) -> None:
        rewards = self.data[ImageGraphName.Reward]
        df = self.generate_dataframe(rewards)
        axes = sns.heatmap(df)
        axes.invert_yaxis()
        plt.show()

    def generate_thrust_image(self) -> None:
        pass
