from math import fabs
from model import Model
from typing import List, Tuple, Any
from copy import copy
from multiprocessing import cpu_count
from concurrent.futures import ProcessPoolExecutor

max_count, count, x_max, h, proc_count = 10000, 2000, 1000, 0.000005, cpu_count()


def build_bifurcation_diagram(model: Model, x01: float, x02: float, d_end: float = 0.0025) -> List[Tuple[Any]]:
    points = {(model.d12, x01, x02)}

    while model.d12 < d_end:
        x1t, x2t = x01, x02
        local_points = set()

        for i in range(max_count):
            x1t_i, x2t_i = model.f(x1t, x2t), model.g(x1t, x2t)

            if fabs(x1t_i) > x_max or fabs(x2t_i) > x_max:
                print(f'(x01 = {x01}, x02 = {x02}) -> inf')
                local_points = set()
                break

            x1t, x2t = x1t_i, x2t_i

            if i < count:
                local_points.add((model.d12, x1t, x2t))

        points = points.union(local_points)
        model.d12 += h

    return list(zip(*points))


def build_bifurcation_diagram_parallel(model: Model, x01: float, x02: float, d_end: float = 0.0025) -> List[Tuple[Any]]:
    tasks = []

    with ProcessPoolExecutor(max_workers=proc_count) as executor:
        d_part = d_end / proc_count

        for i in range(proc_count):
            model_copy = copy(model)
            model_copy.d12 = d_part * i
            tasks.append(executor.submit(build_bifurcation_diagram, model_copy, x01, x02, d_part * (i + 1)))

    d, x1, x2 = (), (), ()

    for task in tasks:
        result = task.result()
        d += result[0]
        x1 += result[1]
        x2 += result[2]

    return [d, x1, x2]
