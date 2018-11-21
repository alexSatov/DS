from math import inf
from copy import copy
from typing import Tuple, List, Dict
from multiprocessing import cpu_count
from concurrent.futures import ProcessPoolExecutor

from model import State, Model
from phase_trajectory import get_phase_trajectory

proc_count = cpu_count()


def get_bifurcation_diagram_xd(model: Model, state: State, d_end: float, h: float) -> List[Tuple[float, float, float]]:
    points = []
    model.d12 -= h

    while model.d12 < d_end:
        model.d12 += h
        local_points = get_phase_trajectory(model, state, 8000, 2000)

        if local_points[-1] == (inf, inf):
            continue

        local_points = [(model.d12,) + p for p in local_points]
        points.extend(local_points)

    return points


def get_bifurcation_diagram_xd_parallel(model: Model, state: State, d_end: float, h: float) -> List[
    Tuple[float, float, float]]:
    tasks = []

    with ProcessPoolExecutor(max_workers=proc_count) as executor:
        d_part = d_end / proc_count

        for i in range(proc_count):
            model_copy = copy(model)
            model_copy.d12 = d_part * i
            tasks.append(executor.submit(get_bifurcation_diagram_xd, model_copy, state, d_part * (i + 1), h))

    points = []

    for task in tasks:
        points.extend(task.result())

    return points


def get_bifurcation_diagram_dd(model: Model, state_0: State, d1_end: float, d2_end: float, h1: float, h2: float) -> \
        Tuple[List[Tuple[float, float]], List[Tuple[float, float]], Dict[int, List[Tuple[float, float]]]]:
    d21_0, equilibrium, infs = model.d21, [], []
    cycles: Dict[int, List[Tuple[float, float]]] = {2: [], 3: [], 4: [], 5: [], 6: [], 7: [], 8: [], 9: [], 10: [],
                                                    11: [], 12: [], 13: [], 14: [], 15: [], 16: []}

    while model.d12 < d1_end:
        model.d21 = d21_0 - h2
        while model.d21 < d2_end:
            model.d21 += h2
            x1, x2 = get_phase_trajectory(model, state_0, 1000, 1)[-1]

            if x1 == inf or x2 == inf:
                infs.append((x1, x2))
                continue

            state = State(x1, x2)
            states = [state]

            for i in range(15):
                states.append(model.next_state(state))

                if i == 0 and states[1].equals(state):
                    equilibrium.append(state.to_point())
                    break

                for j in range(len(states) - 2):
                    if states[0].equals(states[j + 2]):
                        cycles[j + 2].append(state.to_point())
                        break
        model.d12 += h1

    return equilibrium, infs, cycles


def get_bifurcation_diagram_dd_parallel(model: Model, state: State, d1_end: float, d2_end: float, h1: float,
                                        h2: float) -> Tuple[List[Tuple[float, float]], List[Tuple[float, float]],
                                                            Dict[int, List[Tuple[float, float]]]]:
    tasks = []

    with ProcessPoolExecutor(max_workers=proc_count) as executor:
        d_part = d1_end / proc_count

        for i in range(proc_count):
            model_copy = copy(model)
            model_copy.d12 = d_part * i
            tasks.append(
                executor.submit(get_bifurcation_diagram_dd, model_copy, state, d_part * (i + 1), d2_end, h1, h2))

    equilibrium, infs = [], []
    cycles: Dict[int, List[Tuple[float, float]]] = {2: [], 3: [], 4: [], 5: [], 6: [], 7: [], 8: [], 9: [], 10: [],
                                                    11: [], 12: [], 13: [], 14: [], 15: [], 16: []}

    for task in tasks:
        task_equilibrium, task_infs, task_cycles = task.result()
        equilibrium.extend(task_equilibrium)
        task_infs.extend(task_infs)

        for i in range(2, 17):
            cycles[i].extend(task_cycles[i])

    return equilibrium, infs, cycles
