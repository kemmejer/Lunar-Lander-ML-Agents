import os
import tkinter as tk
from tkinter import messagebox, ttk

import matplotlib.pyplot as plt
import numpy as np

from ImageVisualization import ImageGraphName, ImageVisualization


class ImageVisualizationUI():

    def __init__(self) -> None:
        self.data_base_dir = "../results"
        self.image_visualization = ImageVisualization(self.data_base_dir)
        self.data = [None] * len(ImageGraphName)

    def render_window(self) -> None:
        self.root = tk.Tk()
        self.root.wm_title("Image Visualization")

        self.render_step_input()
        tk.mainloop()

    def render_step_input(self) -> None:
        element_width = "32"

        datasets: list[str] = self.read_available_datasets()

        label_dataset = tk.Label(text="Dataset")
        self.selected_dataset = tk.StringVar()
        self.selected_dataset.trace_add("write", self.on_dataset_selected)
        self.combo_dataset = ttk.Combobox(values=datasets, textvariable=self.selected_dataset, state="readonly", width=element_width)

        is_digit_command = self.root.register(self.is_digit)
        self.step_from_label_var = tk.StringVar(value="Step from")
        label_step_from = tk.Label(textvariable=self.step_from_label_var,)
        self.entry_step_from = tk.Entry(validate="all", validatecommand=(is_digit_command, "%P"), width=element_width)

        self.step_to_label_var = tk.StringVar(value="Step to")
        label_step_to = tk.Label(textvariable=self.step_to_label_var)
        self.entry_step_to = tk.Entry(validate="all", validatecommand=(is_digit_command, "%P"), width=element_width
                                      )
        button_generate = tk.Button(text="Generate plots", command=self.generate_plots)

        label_dataset.pack()
        self.combo_dataset.pack()
        label_step_from.pack()
        self.entry_step_from.pack()
        label_step_to.pack()
        self.entry_step_to.pack()
        button_generate.pack()

        if len(datasets) > 0:
            self.combo_dataset.current(0)

    def generate_plots(self) -> None:
        if self.combo_dataset.get() == "" or self.entry_step_from.get() == "" or self.entry_step_to.get() == "":
            return

        plt.close("all")
        if not self.load_data():
            return

        for graph_name in ImageGraphName:
            if graph_name == ImageGraphName.WorldBounds:
                continue
            figure = self.image_visualization.generate_image(graph_name)
            figure.show()

    def read_available_datasets(self) -> list[str]:
        folders: list[str] = [folder for folder in os.listdir(self.data_base_dir) if os.path.isdir(os.path.join(self.data_base_dir, folder))]
        valid_folders: list = []
        for folder in folders:
            path: str = os.path.join(self.data_base_dir, folder)
            files: list[str] = [file for file in os.listdir(path) if os.path.join(path, file) and file.endswith(self.image_visualization.data_file_extension)]
            if len(files) > 0:
                valid_folders.append(folder)

        return valid_folders

    def load_data(self) -> bool:
        selected_dataset: str = self.combo_dataset.get()
        if (selected_dataset == ""):
            return False

        dataset_dir: str = os.path.join(self.data_base_dir, selected_dataset)
        for graph_name in ImageGraphName:
            file_path: str = os.path.join(dataset_dir, graph_name.name + self.image_visualization.data_file_extension)
            data = self.load_data_from_file(file_path)
            if data is None:
                return False
            data = self.concat_data(data, graph_name)
            if len(data) == 0:
                return False
            self.image_visualization.set_image_data(graph_name, data)

        return True

    def concat_data(self, data: np.ndarray[float], graph_name: ImageGraphName) -> np.ndarray[float]:
        if graph_name == ImageGraphName.WorldBounds:
            return data

        step_from = int(self.entry_step_from.get())
        step_to = int(self.entry_step_to.get())

        step_from = self.clamp(step_from, self.min_step, self.max_step)
        step_to = self.clamp(step_to, self.min_step, self.max_step)

        if graph_name == ImageGraphName.Position or graph_name == ImageGraphName.Velocity:
            return data[step_from*2:step_to*2]  # Vectors
        else:
            return data[step_from:step_to]  # Scalars

    def load_step_count(self) -> None:
        selected_dataset: str = self.selected_dataset.get()
        dataset_dir: str = os.path.join(self.data_base_dir, selected_dataset)
        file_path: str = os.path.join(dataset_dir, "Step" + self.image_visualization.data_file_extension)
        steps = self.load_data_from_file(file_path)
        if steps is None or len(steps) == 0:
            return

        self.min_step = 0
        self.max_step = np.max(steps)
        self.step_from_label_var.set(f"Step from (min step={self.min_step:_})")
        self.step_to_label_var.set(f"Step to (max step={self.max_step:_})")

    def on_dataset_selected(self, index, value, op) -> None:
        self.load_step_count()

    def is_digit(self, value) -> bool:
        return str.isdigit(value) or value == ""

    def clamp(self, n, min, max):
        if n < min:
            return min
        elif n > max:
            return max
        else:
            return n

    def load_data_from_file(self, file_path: str):
        try:
            return np.load(file_path, mmap_mode="r")
        except:
            self.show_error(f"Could not parse the file: {file_path}")
            return None

    def show_error(self, message: str) -> None:
        messagebox.showerror("An error occurred", message)


ui = ImageVisualizationUI()
ui.render_window()
