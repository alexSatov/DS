from math import fabs
from model import Model
from typing import List, Tuple

x_max, t1, t2 = 100, 2000, 1000


def build_phase_trajectory(model: Model, x01: float, x02: float) -> Tuple[List[float], List[float]]:
    x1t, x2t = x01, x02
    x1tn, x2tn = [x1t], [x2t]

    for t in range(t1 + t2):
        x1t_i, x2t_i = model.f(x1t, x2t), model.g(x1t, x2t)

        if fabs(x1t_i) > x_max or fabs(x2t_i) > x_max:
            print(f'(x01 = {x01}, x02 = {x02}) -> inf')
            break

        if t >= t1:
            x1tn.append(x1t_i)
            x2tn.append(x2t_i)

        x1t, x2t = x1t_i, x2t_i

    return x1tn, x2tn
