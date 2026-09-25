#!/usr/bin/env python3
"""Exact-rational verifier for finite YM-CROSSOVER invariant-tube certificates.

This script does NOT prove any Yang--Mills RG estimate by itself. It checks the
finite induction once rigorous one-step enclosure formulae have been supplied.
All arithmetic uses fractions.Fraction, so there is no floating-point roundoff.

A certificate supplies, for each step j, polynomial lower/upper bounds for the
next beta-coordinate and remainder coordinate on the current box. The checker
uses interval arithmetic to prove that these enclosures lie in the proposed
next box. A final polynomial upper bound q(beta,r) can be checked against a
Kotecky--Preiss threshold alpha.
"""
from __future__ import annotations

import argparse
import json
from dataclasses import dataclass
from fractions import Fraction
from pathlib import Path
from typing import Dict, Iterable


def F(x) -> Fraction:
    if isinstance(x, int):
        return Fraction(x)
    if isinstance(x, str):
        return Fraction(x)
    raise TypeError(f"expected integer/rational string, got {x!r}")


@dataclass(frozen=True)
class I:
    lo: Fraction
    hi: Fraction

    def __post_init__(self):
        if self.lo > self.hi:
            raise ValueError(f"empty interval [{self.lo},{self.hi}]")

    def __add__(self, other: "I") -> "I":
        return I(self.lo + other.lo, self.hi + other.hi)

    def __mul__(self, other: "I") -> "I":
        xs = (
            self.lo * other.lo,
            self.lo * other.hi,
            self.hi * other.lo,
            self.hi * other.hi,
        )
        return I(min(xs), max(xs))

    def pow(self, n: int) -> "I":
        if n < 0:
            raise ValueError("negative powers are not supported")
        if n == 0:
            return I(Fraction(1), Fraction(1))
        out = I(Fraction(1), Fraction(1))
        base = self
        k = n
        while k:
            if k & 1:
                out = out * base
            base = base * base
            k >>= 1
        return out

    def subset(self, other: "I") -> bool:
        return other.lo <= self.lo and self.hi <= other.hi

    def __str__(self) -> str:
        return f"[{self.lo}, {self.hi}]"


def parse_interval(v) -> I:
    return I(F(v[0]), F(v[1]))


def eval_poly(terms: Iterable[dict], box: Dict[str, I]) -> I:
    total = I(Fraction(0), Fraction(0))
    for term in terms:
        coef = F(term["c"])
        t = I(coef, coef)
        for var, power in term.items():
            if var == "c":
                continue
            if var not in box:
                raise KeyError(f"unknown variable {var!r}")
            t = t * box[var].pow(int(power))
        total = total + t
    return total


def prove_step(j: int, step: dict, box: Dict[str, I]) -> Dict[str, I]:
    target = {k: parse_interval(v) for k, v in step["target"].items()}

    # Certificate semantics:
    # beta' >= beta_lower(beta,r), beta' <= beta_upper(beta,r),
    # r'    >= r_lower(beta,r),    r'    <= r_upper(beta,r).
    blo = eval_poly(step["bounds"]["beta_lower"], box).lo
    bhi = eval_poly(step["bounds"]["beta_upper"], box).hi
    rlo = eval_poly(
        step["bounds"].get("r_lower", [{"c": "0"}]), box
    ).lo
    rhi = eval_poly(step["bounds"]["r_upper"], box).hi
    image = {"beta": I(blo, bhi), "r": I(rlo, rhi)}

    failures = []
    for key in ("beta", "r"):
        if not image[key].subset(target[key]):
            failures.append(
                f"{key}: certified image {image[key]} not subset of target {target[key]}"
            )
    if failures:
        raise AssertionError(f"step {j} failed: " + "; ".join(failures))

    print(f"STEP {j}: CERTIFIED  beta {image['beta']}  r {image['r']}")
    return target


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("certificate", type=Path)
    args = ap.parse_args()
    data = json.loads(args.certificate.read_text())

    box = {k: parse_interval(v) for k, v in data["initial"].items()}
    print(f"K_0: beta {box['beta']}  r {box['r']}")
    for j, step in enumerate(data["steps"]):
        box = prove_step(j, step, box)

    kp = data.get("kp_final")
    if kp:
        q = eval_poly(kp["q_upper"], box).hi
        alpha = F(kp["alpha"])
        if not q < alpha:
            raise AssertionError(
                f"KP landing failed: q <= {q}, threshold alpha={alpha}"
            )
        print(f"KP LANDING: CERTIFIED  q <= {q} < alpha = {alpha}")

    print(
        "CERTIFICATE ACCEPTED "
        "(conditional on the supplied one-step RG bounds)."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
