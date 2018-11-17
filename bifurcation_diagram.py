from math import fabs
from model import Model
from typing import List, Tuple

max_count, count, x_max, h = 10000, 2000, 1000, 0.000005


def build_bifurcation_diagram(model: Model, x01: float, x02: float) -> Tuple[List[float], List[float], List[float]]:
    model.d12 = 0
    d12n, x1tn, x2tn = [model.d12], [x01], [x02]

    while model.d12 < 0.0025:
        x1t, x2t = x01, x02
        for i in range(max_count):
            x1t_i, x2t_i = model.f(x1t, x2t), model.g(x1t, x2t)

            if fabs(x1t_i) > x_max or fabs(x2t_i) > x_max:
                print(f'(x01 = {x01}, x02 = {x02}) -> inf')
                break

            x1t, x2t = x1t_i, x2t_i

            if i < count:
                d12n.append(model.d12)
                x1tn.append(x1t)
                x2tn.append(x2t)

        model.d12 += h

    return d12n, x1tn, x2tn
