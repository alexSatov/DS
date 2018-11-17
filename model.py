class Model:
    def __init__(self, d12=0, d21=0.0063, a1=0.0002, a2=0.00052, b1=10, b2=20, px=0.25, py=1):
        self.a1: float = a1
        self.a2: float = a2
        self.b1: float = b1
        self.b2: float = b2
        self.px: float = px
        self.py: float = py
        self.d12: float = d12
        self.d21: float = d21

    def f(self, x1: float, x2: float) -> float:
        return (self.b1 / (self.px * self.py)) * \
               (self.a1 * x1 * (self.b1 - self.px * x1) + self.d12 * x2 * (self.b2 - self.px * x2))

    def g(self, x1: float, x2: float) -> float:
        return (self.b2 / (self.px * self.py)) * \
               (self.a2 * x2 * (self.b2 - self.px * x2) + self.d21 * x1 * (self.b1 - self.px * x1))
