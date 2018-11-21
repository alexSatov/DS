from math import fabs, inf
from typing import List, Tuple

from model import Model, State


def get_phase_trajectory(model: Model, state_0: State, t1: int, t2: int, x_max: int = 100) -> List[Tuple[float, float]]:
    state = state_0
    points = []

    for t in range(t1 + t2):
        next_state = model.next_state(state)

        if fabs(next_state.x1) > x_max or fabs(next_state.x2) > x_max:
            points.append((inf, inf))
            return points

        if t >= t1:
            point = next_state.to_point()

            if len(points) == 0 or points[-1] != point:
                points.append(point)

        state = next_state

    return points
