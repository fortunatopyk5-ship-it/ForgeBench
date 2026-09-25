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


## 2026-09-25 — Iteration 1b

New theorem:
YM-SEMIGROUP-1. Under a Poincare variance decay and a weighted gradient-propagation bound for a local reversible diffusion, covariance decays in lattice distance with rate at least
kappa lambda/[2(lambda+v_kappa)].
This makes explicit the hidden second ingredient behind the Shen–Zhu–Zhu strong-coupling argument.

Adversarial finding:
A global auxiliary Markov spectral gap is not obviously a minimal target. Global/topological bottlenecks may make it small without forcing long-range local physical correlations.

New reduced route:
YM-RG-LANDING-1. Run exact RG from the asymptotically-free UV and prove that at a_* comparable to Lambda_YM^{-1} the effective polymer activities obey a uniform Kotecky-Preiss exponential norm bound. Standard cluster expansion would then yield an O(Lambda_YM) correlation rate. Reflection positivity can remain entirely at the original Wilson lattice level; it need not be preserved by each blocking transformation.

Current best next attack:
Make YM-RG-LANDING-1 less black-box by splitting it into:
(a) UV Balaban-controlled steps up to a matching scale;
(b) a finite crossover lemma for the remaining O(1) range of effective coupling;
(c) entry into a rigorous strong-mixing/polymer domain.
The likely genuinely new content is (b).


## 2026-09-25 — Iteration 1c

Reduction sharpened:
Introduce YM-CROSSOVER-1. After the UV RG reaches a fixed matching coupling g_match, prove that a finite, cutoff-independent number M_G of exact RG steps sends the full compact matching set of effective interactions into a Kotecky-Preiss strong-mixing domain.

Reason this matters:
The number of UV steps diverges as a->0, but those are the asymptotically-free/small-coupling steps for which constructive RG is the natural tool. The genuinely nonperturbative crossover starts from a fixed g_match, so its required scale interval is fixed; if it can be controlled, it is a finite-step problem.

Adversarial warning:
A scalar running coupling is not enough. The inclusion must be in a norm controlling the entire generated effective action, including irrelevant operators and large-field/polymer terms.


## 2026-09-25 — Iteration 2: YM-CROSSOVER-1 only

Adversarial result 1:
The previous reduction was too optimistic if it tracked only a running coupling. Explicit toy RG maps show that a coupling can run monotonically to a nominal strong-coupling region while generated irrelevant/polymer coordinates grow or settle outside every small-activity domain.

Adversarial result 2:
Measure-only landing in a KP domain does not by itself control original microscopic correlations. The exact conditional-covariance decomposition exposes the missing term. A hidden-sector toy model gives a direct counterexample. Added YM-RG-SOURCE-1.

Concrete state space:
Defined B_{alpha,mu,p,rho}, a weighted Banach space of globally defined gauge-invariant interactions on connected 4D block polymers. Local norms are explicit C^p norms built from invariant Lie derivatives and supremized over collar/boundary link fields. See crossover_spec.md.

Concrete landing domain:
Defined exact scalar super-polymer activities by expanding the global interaction, grouping connected overlapping families and integrating against product Haar. Proved that
Q_{alpha,mu}<alpha
implies the Kotecky-Preiss condition and uniform cluster expansion.

Finite-step reduction:
Proved YM-TUBE-1. YM-CROSSOVER measure inclusion follows from a finite list of one-step interval enclosures plus a final KP inequality.
Proved YM-RG-DRIFT-1. A uniform drift delta>0 together with an invariant remainder tube implies a cutoff-independent finite number of crossover steps; finite M is therefore no longer assumed.

Matching correction:
Balaban's verified published claims provide a substantial UV RG package, but do not automatically give the specific compact K_match in the new global norm. Added YM-MATCH-EXTRACT-1 as a separate unresolved input.

Continuum bridge audit:
Strengthened the cutoff bridge to YM-CUTOFF-BRIDGE-2. Once each fixed-cutoff theory has a genuine transfer gap m_a, the semigroup inequality
C_a(t+s)<=e^{-m_a s}C_a(t)
passes to the continuum using positive-time correlator convergence; no equal-time norm convergence or uniform cluster prefactor is required.

Validated numerics:
Added scripts/verify_crossover_tube.py, an exact Fraction-based interval checker, and a toy certificate. It certifies only the finite induction conditional on genuine RG enclosure formulae.

Result:
STATUS: YM-CROSSOVER-1 REDUCED TO LEMMAS A/B/C — NOT PROVED.

A = YM-MATCH-EXTRACT-1.
B = Yang-Mills one-step invariant-tube/drift enclosures ending in Q_KP<alpha.
C = YM-RG-SOURCE-1.

Next action:
Do not open a new mass-gap route. Extract an actual one-step Balaban gauge-RG map/bounds into the variables beta,r of crossover_spec.md and determine whether the first nontrivial tube step can be proved analytically.
