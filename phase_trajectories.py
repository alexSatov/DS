from math import fabs
from model import Model
from typing import List, Tuple


def build_pt(model: Model, x10: float, x20: float, t1=2000, t2=1000) -> Tuple[List[float], List[float]]:
    x1tn, x2tn = [x10], [x20]
    x1t, x2t = x10, x20

    for t in range(t1 + t2):
        x1t_i, x2t_i = model.f(x1t, x2t), model.g(x1t, x2t)

        if fabs(x1t_i) > 100 or fabs(x2t_i) > 100:
            print('(x10 = %i, x20 = %i) -> inf')
            break

        if t >= t1:
            x1tn.append(x1t_i)
            x2tn.append(x2t_i)

        x1t, x2t = x1t_i, x2t_i

    return x1tn, x2tn
