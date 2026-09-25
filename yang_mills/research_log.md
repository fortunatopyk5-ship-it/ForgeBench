# Research log

## 2026-09-25 — Iteration 1

Idea:
Reduce the gap half of the problem to a physical-scale Euclidean-time clustering estimate and investigate whether functional inequalities can supply it.

Mathematical setup:
Wilson lattice measure on compact link variables with Haar measure; avoid gauge fixing for the main reduction. Use reflection positivity to obtain transfer evolution. Separate the existence/UV problem from the IR gap problem.

Verified:
1. Spectral theorem proves YM-BRIDGE-1: a common exponential Euclidean-time decay rate on a dense centered local algebra excludes spectrum below that rate.
2. Correct lattice scaling target is gamma(a)>=m_* a. Requiring a lattice-unit exponent bounded below by a positive constant independent of a would be much stronger than needed.
3. Strong-coupling SU(N) result of Shen–R. Zhu–X. Zhu gives LSI/PI and exponential correlation decay only for |beta|<1/[16(d-1)]. In d=4: |beta|<1/48.
4. Their proof shows PI alone is not the spatial-mixing mechanism: PI gives decay in auxiliary semigroup time, while commutator/locality bounds control propagation; optimizing the two yields spatial decay.
5. This suggests a scale-invariant target lambda_a/v_a >= c a Lambda_YM.
6. Direct Bakry–Emery continuation fails because K_S becomes negative as beta grows.
7. Pointwise finite-beta gap, analytic continuation, no finite-beta phase transition, area law, or Lambda_YM>0 alone do not imply the required continuum spectral gap.

Problem encountered:
No weak-coupling, volume-uniform estimate with correct a(beta) scaling was obtained.

Result:
STATUS: PROMISING ROUTE — NOT A PROOF.

Next step:
Prove an abstract PI+finite-propagation => covariance theorem with explicit constants, then search for an RG mechanism that gives lambda_a/v_a at the asymptotically-free trajectory without assuming Dobrushin mixing as an input.
