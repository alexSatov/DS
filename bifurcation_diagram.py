from copy import copy
from typing import Set, Tuple
from multiprocessing import cpu_count
from concurrent.futures import ProcessPoolExecutor

from model import State, Model
from phase_trajectory import get_phase_trajectory


proc_count = cpu_count()


def get_bifurcation_diagram(model: Model, state: State, d_end: float = 0.0025, h: float = 0.000005) -> \
        Set[Tuple[float, float, float]]:
    points = set()

    while model.d12 < d_end:
        local_points = map(lambda point: (model.d12,) + point, get_phase_trajectory(model, state, 2000, 8000, 2000))
        points = points.union(local_points)
        model.d12 += h

    return points


def get_bifurcation_diagram_parallel(model: Model, state: State, d_end: float = 0.0025, h: float = 0.000005) -> \
        Set[Tuple[float, float, float]]:
    tasks = []

    with ProcessPoolExecutor(max_workers=proc_count) as executor:
        d_part = d_end / proc_count

        for i in range(proc_count):
            model_copy = copy(model)
            model_copy.d12 = d_part * i
            tasks.append(executor.submit(get_bifurcation_diagram, model_copy, state, d_part * (i + 1), h))

    points = set()

    for task in tasks:
        points = points.union(task.result())

    return points
