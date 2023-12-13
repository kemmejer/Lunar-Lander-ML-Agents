from hashlib import sha1
from typing import List

import matplotlib.pyplot as plt
import matplotlib.cm as cm
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

        plt.show()

    def get_grid_positions(self) -> tuple[np.ndarray[float], np.ndarray[float]]:
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

        # Retrieve the coordinate arrays
        x_coords = np.append(positions[0::2], x_fill)
        y_coords = np.append(positions[1::2], y_fill)

        return x_coords, y_coords

    def generate_dataframe(self, values: np.ndarray[float]) -> pd.DataFrame:
        x_coords, y_coords = self.get_grid_positions()
        values = np.append(values, np.full(x_coords.size - values.size, np.nan))  # Pad the value array to the same length as the coordinate arrays
        df = pd.DataFrame(data={"x": x_coords, "y": y_coords, "z": values})

        return df

    def generate_dataframe_2d(self, values: np.ndarray[float]) -> pd.DataFrame:
        x_coords, y_coords = self.get_grid_positions()
        values = np.append(values, np.full(int(2 * (x_coords.size - values.size / 2)), np.nan)
                           )  # Pad the value array to the same length as the coordinate arrays
        u_values = values[0::2]
        v_values = values[1::2]
        df = pd.DataFrame(data={"x": x_coords, "y": y_coords, "u": u_values, "v": v_values})

        return df

    def generate_position_image(self) -> None:
        positions = self.data[int(ImageGraphName.Position)]
        values = np.ones(int(positions.size / 2))

        df = self.generate_dataframe(values)
        df = df.groupby(["x", "y"], as_index=False).sum(min_count=1)
        df = df.pivot(index="y", columns="x", values="z")

        axes = sns.heatmap(df, cmap="viridis")
        axes.invert_yaxis()
        axes.set_aspect('equal', adjustable='box')

    def generate_rotation_image(self) -> None:
        rotations = self.data[int(ImageGraphName.Rotation)]
        df = self.generate_dataframe(rotations)
        df = df.groupby(["x", "y"], as_index=False).mean()

        radians = np.radians(df["z"] + 90.0)
        u_values = np.cos(radians)
        v_values = np.sin(radians)

        quiver = plt.quiver(df["x"], df["y"], u_values, v_values, df["z"], pivot="mid", cmap="viridis")
        quiver.axes.set_aspect('equal', adjustable='box')

        colorbar = plt.colorbar(quiver)
        colorbar.set_label("Euler Angle")

    def generate_velocity_image(self) -> None:
        velocity = self.data[int(ImageGraphName.Velocity)]
        df = self.generate_dataframe_2d(velocity)
        df = df.groupby(["x", "y"], as_index=False).mean()

        magnitudes = np.sqrt(df["u"] ** 2 + df["v"] ** 2)
        quiver = plt.quiver(df["x"], df["y"], df["u"], df["v"], magnitudes, cmap="viridis")
        quiver.axes.set_aspect('equal', adjustable='box')

        colorbar = plt.colorbar(quiver)
        colorbar.set_label('Magnitude of Velocity')

    def generate_reward_image(self) -> None:
        rewards = self.data[ImageGraphName.Reward]

        # The grouping calculates the mean over all duplicate positions. The pivot fills all missing gaps in the dataframe with NaN
        df = self.generate_dataframe(rewards)
        df = df.groupby(["x", "y"], as_index=False).mean()
        df = df.pivot(index="y", columns="x", values="z")

        axes = sns.heatmap(df, cmap="viridis", center=0.0)
        axes.invert_yaxis()
        axes.set_aspect('equal', adjustable='box')

    def generate_thrust_image(self) -> None:
        pass
